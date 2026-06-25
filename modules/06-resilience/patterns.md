# Patterns de résilience

Un système distribué **tombe**. La résilience consiste à **limiter l'impact** des pannes, à **se rétablir** rapidement et à **dégrader gracieusement** plutôt que d'échouer brutalement.

---

## 1. Principes fondamentaux

| Principe | Description |
| -------- | ----------- |
| **Fail fast** | Détecter l'échec tôt, ne pas bloquer indéfiniment |
| **Graceful degradation** | Réponse partielle ou mode dégradé |
| **Isolation** | Une panne ne se propage pas (bulkhead) |
| **Redundancy** | Composants de secours (module 1, 5) |
| **Automation** | Retry, failover, scale sans intervention humaine |

### Cascade de pannes

```text
Service A lent → threads épuisés sur B qui appelle A
              → B tombe → C qui appelle B tombe
              → panne généralisée
```

La résilience vise à **casser la chaîne** à chaque maillon.

---

## 2. Timeout

Ne jamais attendre indéfiniment une dépendance.

```csharp
var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
```

| Bonne pratique | Détail |
| -------------- | ------ |
| Timeout < timeout client amont | Éviter que le LB coupe avant votre app |
| Timeout par dépendance | Payment API ≠ catalogue interne |
| Documenter les valeurs | Dans la stratégie de résilience |

**Règle empirique :** timeout client = p99 dépendance × 2–3 (avec plafond).

---

## 3. Retry

Réessayer une opération échouée, souvent pour des erreurs **transitoires**.

### Erreurs retriables vs non retriables

| Retriable | Non retriable |
| --------- | ------------- |
| HTTP 503, 502, 429 | HTTP 400, 401, 404 |
| Timeout réseau | Erreur métier validée |
| SQL deadlock transitoire | Violation contrainte unique (sans idempotence) |

### Backoff exponentiel

```
Tentative 1 : immédiat
Tentative 2 : attendre 2 s
Tentative 3 : attendre 4 s
Tentative 4 : attendre 8 s (+ jitter aléatoire)
```

Le **jitter** évite que tous les clients retentent simultanément (thundering herd).

### Polly — Retry

```csharp
using Polly;
using Polly.Extensions.Http;

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500)));

services.AddHttpClient<IPaymentClient, PaymentClient>()
    .AddPolicyHandler(retryPolicy);
```

### Idempotence obligatoire

Sans idempotence, un retry sur `POST /payments` peut **débiter deux fois**.

Solutions :
- `Idempotency-Key` header + table de déduplication
- Opérations naturellement idempotentes (`PUT`, `DELETE`)
- Token unique côté client

---

## 4. Circuit breaker

Couper temporairement les appels vers une dépendance défaillante pour lui laisser récupérer.

### États

```
        échecs seuil atteint
   ┌──────────────► OPEN (rejette immédiatement)
   │                    │
   │                    │ après break duration
   │                    ▼
CLOSED ◄──────── HALF-OPEN (quelques essais)
   ▲                    │
   │                    │ succès → CLOSED
   │                    │ échec → OPEN
   └────────────────────┘
```

| État | Comportement |
| ---- | ------------ |
| **Closed** | Trafic normal, compteur d'échecs |
| **Open** | Échec immédiat (ou fallback), pas d'appel réseau |
| **Half-Open** | Quelques requêtes test |

### Polly — Circuit breaker

```csharp
var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, breakDelay) =>
            Log.Warning("Circuit OPEN for {Delay}s", breakDelay.TotalSeconds),
        onReset: () => Log.Information("Circuit CLOSED"),
        onHalfOpen: () => Log.Information("Circuit HALF-OPEN"));

services.AddHttpClient<IInventoryClient, InventoryClient>()
    .AddPolicyHandler(circuitBreakerPolicy);
```

### Combinaison Retry + Circuit breaker

Ordre recommandé : **Retry à l'intérieur, Circuit breaker à l'extérieur**

```csharp
var pipeline = Policy.WrapAsync(circuitBreakerPolicy, retryPolicy);
```

Sinon les retries alimentent le compteur du breaker trop vite.

---

## 5. Bulkhead (cloisonnement)

Isoler les ressources pour qu'une dépendance lente n'épuise pas tout le pool.

### Exemple : pools HTTP séparés

```csharp
// Client paiement — pool limité, critique
services.AddHttpClient<IPaymentClient, PaymentClient>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler())
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));
// + Polly BulkheadPolicy

var bulkhead = Policy.BulkheadAsync<HttpResponseMessage>(
    maxParallelization: 10,
    maxQueuingActions: 20);
```

### Exemple : threads / workers

```
Pool 100 threads total
  ├── 60 réservés API critiques
  ├── 30 appels externes
  └── 10 tâches background
```

Sans bulkhead : un appel lent vers un service tiers bloque toutes les requêtes.

---

## 6. Fallback et dégradation gracieuse

Fournir une **réponse alternative** quand la dépendance échoue.

```csharp
var fallbackPolicy = Policy<HttpResponseMessage>
    .Handle<Exception>()
    .OrResult(r => !r.IsSuccessStatusCode)
    .FallbackAsync(
        fallbackAction: async (ct) =>
        {
            var cached = await _cache.GetRecommendationsAsync();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(cached)
            };
        },
        onFallbackAsync: async (outcome, context) =>
            Log.Warning("Recommendations fallback activated"));

var policy = Policy.WrapAsync(fallbackPolicy, circuitBreakerPolicy, retryPolicy);
```

### Exemples métier

| Fonctionnalité | Dégradation |
| -------------- | ----------- |
| Recommandations produits | Produits populaires statiques |
| Avatar utilisateur | Image par défaut |
| Commentaires | Masquer section, message « indisponible » |
| Paiement | **Pas de fallback** — échec explicite |

---

## 7. Hedging (avancé)

Envoyer une requête **dupliquée** si la première est lente (utile pour latence p99).

```
Requête 1 ──────────────────► réponse (lente)
Requête 2 ─────► réponse (rapide) → utilisée, req 1 annulée
```

**Attention :** double charge sur le backend. Réserver aux lectures idempotentes.

---

## 8. Health checks

ASP.NET Core :

```csharp
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "database")
    .AddRedis(redisConnection, name: "redis")
    .AddUrlGroup(new Uri("https://payment-api/health"), name: "payment-api");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

| Endpoint | Usage |
| -------- | ----- |
| `/health/live` | Processus vivant (Kubernetes liveness) |
| `/health/ready` | Prêt à recevoir du trafic (readiness) |

Le load balancer retire les instances **not ready**.

---

## 9. Stratégie par dépendance

Document obligatoire du module — une ligne par dépendance externe :

| Dépendance | Criticité | Timeout | Retry | Circuit breaker | Fallback |
| ---------- | --------- | ------- | ----- | --------------- | -------- |
| Azure SQL | Critique | 5 s | 2 (transient) | Non | Non — erreur 503 |
| Redis cache | Haute | 1 s | 1 | Oui (5/30s) | Bypass → DB |
| Payment API | Critique | 10 s | 0 | Oui | Non — erreur explicite |
| Email SMTP | Basse | 30 s | 3 | Oui | Queue pour retry async |
| Search API | Moyenne | 3 s | 2 | Oui | Résultats vides + message |

---

## 10. Chaos engineering (introduction)

Tester la résilience **avant** la production.

| Pratique | Outil |
| -------- | ----- |
| Injection latence / erreurs | Chaos Mesh, Azure Chaos Studio |
| Game day | Couper une dépendance en staging |
| Load + panne | k6 + kill pod |

**Objectif :** valider que les circuit breakers et alertes fonctionnent réellement.

---

## 11. Anti-patterns

| Anti-pattern | Problème |
| ------------ | -------- |
| Retry infini | Surcharge de la dépendance en panne |
| Retry sur POST non idempotent | Doublons, corruption |
| Circuit breaker sans fallback ni message clair | UX opaque |
| Timeout global unique | Services rapides pénalisés |
| Catch-all silencieux | Impossible à diagnostiquer |
| Résilience uniquement en prod | Découverte tardive des bugs |

---

## Pour aller plus loin

- [Monitoring et alerting](monitoring.md)
- [Microsoft — Resiliency patterns](https://learn.microsoft.com/azure/architecture/framework/resiliency/reliability-patterns)
- [Polly — Policy registry](https://www.pollydocs.org/strategies/retry.html)
