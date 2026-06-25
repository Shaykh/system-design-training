# Monitoring, logging et alerting

L'**observabilité** permet de comprendre le comportement interne d'un système à partir de ses sorties externes : métriques, logs et traces.

> « You can't fix what you can't see. »

---

## 1. Les trois piliers

| Pilier | Question | Exemple |
| ------ | -------- | ------- |
| **Métriques** | Combien ? Quelle latence ? | req/s, CPU, p95 |
| **Logs** | Qu'est-ce qui s'est passé ? | Erreur SQL, userId, orderId |
| **Traces** | Où est passée la requête ? | API → Service Bus → Function → SQL |

```diagram
         ┌──────────┐
Requête ─► Traces   (span tree, correlation ID)
         ├──────────┤
         │  Logs    (événements structurés par span)
         ├──────────┤
         │ Métriques│ (agrégations temps réel)
         └──────────┘
```

---

## 2. SLI, SLO et SLA

| Terme | Définition | Exemple |
| ----- | ---------- | ------- |
| **SLI** | Indicateur mesuré | Latence p95 API `/orders` |
| **SLO** | Objectif interne | p95 < 300 ms sur 30 jours |
| **SLA** | Engagement contractuel | 99,9 % dispo ou crédit client |
| **Error budget** | Marge d'erreur avant violation SLO | 0,1 % = 43 min/mois |

### Exemples de SLI

| SLI | Mesure |
| --- | ------ |
| Disponibilité | % requêtes HTTP 2xx / total |
| Latence | p95, p99 par endpoint |
| Fraîcheur | Délai ingestion pipeline |
| Taux d'erreur | % 5xx |

### SLO recommandés (API métier)

| SLO | Cible typique |
| --- | ------------- |
| Disponibilité | 99,9 % |
| Latence p95 | < 300 ms |
| Latence p99 | < 1 s |
| Taux erreur 5xx | < 0,1 % |

---

## 3. Azure Monitor et Application Insights

### Architecture

```diagram
Application (.NET)
      │
      ├── SDK App Insights (auto-instrumentation)
      │         │
      │         ▼
      │   Application Insights resource
      │         │
      └── OpenTelemetry ──► Azure Monitor
                │
                ▼
         Log Analytics workspace
                │
      ┌─────────┼─────────┐
      ▼         ▼         ▼
 Dashboards  Alerts   Workbooks
```

### Application Insights — données collectées

| Type | Contenu |
| ---- | ------- |
| **Requests** | HTTP entrantes, durée, code réponse |
| **Dependencies** | SQL, HTTP sortant, Redis, Service Bus |
| **Exceptions** | Stack traces, contexte |
| **Traces** | Logs `ILogger` envoyés à App Insights |
| **Custom metrics** | Compteurs métier |
| **Live Metrics** | Flux temps réel (debug) |

### Configuration ASP.NET Core

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();

// Ou OpenTelemetry (recommandé long terme)
builder.Services.AddOpenTelemetry()
    .UseAzureMonitor();
```

`appsettings.json` :

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=...;IngestionEndpoint=..."
  }
}
```

**Sur Azure :** activer Managed Identity + configuration sans secret en code.

---

## 4. Logging structuré

### Mauvais vs bon

```csharp
// ❌ Difficile à requêter
_logger.LogInformation($"Order {orderId} created for {customerId}");

// ✅ Structured logging (message templates)
_logger.LogInformation(
    "Order {OrderId} created for customer {CustomerId}",
    orderId, customerId);
```

### Niveaux de log

| Niveau | Usage |
| ------ | ----- |
| **Trace** | Debug très détaillé |
| **Debug** | Développement |
| **Information** | Flux nominal (démarrage, événements métier) |
| **Warning** | Anomalie récupérable (retry, fallback) |
| **Error** | Échec opération |
| **Critical** | Panne système |

**Production :** niveau `Information` ou `Warning` par défaut ; `Debug` activable par feature flag.

### Serilog (option courante)

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.ApplicationInsights(telemetryConfig, TelemetryConverter.Traces)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "OrderApi")
    .CreateLogger();
```

### Données à inclure (sans PII excessive)

- `traceId`, `spanId` (corrélation automatique)
- `userId`, `tenantId` (si autorisé RGPD)
- `orderId`, `correlationId` métier
- Durée, résultat, code erreur

**Ne pas logger :** mots de passe, tokens, numéros de carte, données santé.

---

## 5. Tracing distribué

Une requête traverse plusieurs services ; le **trace ID** relie tous les spans.

```diagram
trace_id: abc123
├── span: POST /api/orders          (120 ms) — Order API
│   ├── span: SQL INSERT            (15 ms)
│   └── span: Service Bus publish   (8 ms)
└── span: ProcessOrder (Function)   (450 ms)
    ├── span: HTTP Payment API      (200 ms)
    └── span: SQL UPDATE            (10 ms)
```

### Propagation

- HTTP header `traceparent` (W3C standard)
- App Insights et OpenTelemetry propagent automatiquement

### Activité manuelle

```csharp
using var activity = ActivitySource.StartActivity("CalculatePricing");
activity?.SetTag("order.id", orderId);
// ... logique
```

---

## 6. Métriques custom

```csharp
private static readonly Counter<long> OrdersCreated =
    Meter.CreateCounter<long>("orders.created");

OrdersCreated.Add(1, new KeyValuePair<string, object?>("tenant", tenantId));
```

Métriques métier utiles :

- Commandes créées / minute
- Paiements échoués
- Profondeur file (custom gauge)
- Cache hit ratio

---

## 7. Requêtes KQL (Log Analytics)

### Latence p95 par endpoint (24 h)

```kql
requests
| where timestamp > ago(24h)
| summarize p95=percentile(duration, 95) by name
| order by p95 desc
```

### Taux d'erreur 5xx

```kql
requests
| where timestamp > ago(1h)
| summarize total=count(), errors=countif(toint(resultCode) >= 500)
| extend errorRate = 100.0 * errors / total
```

### Dépendances lentes

```kql
dependencies
| where timestamp > ago(1h)
| where duration > 3000
| project timestamp, name, target, duration, success
| order by duration desc
```

### Corréler logs et traces

```kql
let traceId = "abc123...";
union requests, dependencies, traces, exceptions
| where operation_Id == traceId
| order by timestamp asc
```

---

## 8. Alerting

### Principes

| Règle | Détail |
| ----- | ------ |
| **Actionnable** | Chaque alerte a un runbook |
| **Éviter le bruit** | Seuils + fenêtres, pas d'alerte sur un seul pic |
| **Sévérité** | Critical → réveil ; Warning → ticket |
| **SLO-based** | Alerter sur burn rate du error budget |

### Exemples d'alertes

| Alerte | Condition | Sévérité |
| ------ | --------- | -------- |
| API down | Availability < 99 % sur 5 min | Critical |
| Latence dégradée | p95 > 500 ms sur 15 min | Warning |
| Erreurs SQL | Exceptions SQL > 10 / 5 min | Critical |
| Queue profonde | Service Bus active messages > 10 000 | Warning |
| Circuit open | Custom metric `circuit.open` > 0 | Warning |

### Configuration Azure (concept)

```text
Metric alert / Log alert
  → Action group (email, SMS, webhook Teams/PagerDuty)
  → Runbook Azure Automation (optionnel)
```

### Burn rate alert (SLO)

Si vous consommez l'error budget trop vite sur une fenêtre glissante → alerte avant violation SLA.

---

## 9. Dashboard de supervision

### Structure recommandée

```diagram
┌─────────────────────────────────────────────────────┐
│ Vue exécutive (SLA, req/s, error rate, latence p95) │
├──────────────────────┬──────────────────────────────┤
│ Santé services       │ Infrastructure Azure         │
│ (golden signals)     │ (CPU, memory, instances)     │
├──────────────────────┴──────────────────────────────┤
│ Dépendances (SQL, Redis, HTTP externes)             │
├─────────────────────────────────────────────────────┤
│ Files / workers · Exceptions récentes · Déploiements│
└─────────────────────────────────────────────────────┘
```

### Golden signals (Google SRE)

| Signal | Métrique |
| ------ | -------- |
| **Latency** | p50, p95, p99 |
| **Traffic** | req/s |
| **Errors** | % 5xx, exceptions |
| **Saturation** | CPU, queue depth, connexions DB |

### Outils Azure

| Outil | Usage |
| ----- | ----- |
| **Azure Dashboard** | Tuiles personnalisées |
| **Workbooks** | Rapports interactifs KQL |
| **Grafana** | Alternative, datasource Azure Monitor |
| **App Insights Application Map** | Graphe dépendances auto |

---

## 10. Atelier — Instrumentation et monitoring

**Durée :** 4–6 h · Livrables obligatoires du module.

### Partie A — Instrumenter une API .NET

Sur une API existante (module 2 OrderApp ou nouveau projet) :

1. Ajouter **Application Insights** ou **OpenTelemetry + Azure Monitor**
2. Configurer **health checks** (`/health`, `/health/ready`)
3. Ajouter **Polly** sur au moins 2 clients HTTP :
   - Retry + circuit breaker sur service inventaire
   - Timeout strict sur service paiement
4. Logging structuré avec `OrderId` sur le flux création commande
5. Vérifier dans le portail : Requests, Dependencies, une trace complète

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
dotnet add package Microsoft.Extensions.Http.Polly
dotnet add package Polly.Extensions.Http
```

### Partie B — Dashboard

Créer un **Azure Workbook** ou Dashboard avec minimum :

| Tuile | Type |
| ----- | ---- |
| Req/s | Metric / KQL |
| p95 latence | KQL `requests` |
| Taux erreur 5xx | KQL |
| Top 5 exceptions | `exceptions` |
| Durée dépendances SQL | `dependencies` |
| Disponibilité (synthetic ou availability) | Metric |

**Livrable :** capture d'écran ou export JSON + description dans `dashboard.md`.

### Partie C — Alertes

Configurer **2 alertes** (même en seuil bas pour test) :

1. Taux d'erreur > 1 % sur 5 min
2. p95 latence > 500 ms sur 10 min

Documenter : seuil, action group, runbook (3 étapes de diagnostic).

### Partie D — Stratégie de résilience

**Livrable :** `resilience-strategy.md`

```markdown
# Stratégie de résilience — [Nom système]

## 1. Objectifs SLO
| SLI | SLO |
| --- | --- |

## 2. Dépendances et politiques

| Dépendance | Type | Timeout | Retry | CB | Fallback | Notes |
| ---------- | ---- | ------- | ----- | -- | -------- | ----- |

## 3. Health checks
| Endpoint | Checks inclus | Usage LB/K8s |

## 4. Dégradation gracieuse
| Feature | Comportement dégradé |

## 5. Runbooks
### RB-001 : Circuit breaker Payment API OPEN
1. Vérifier status page prestataire
2. ...

### RB-002 : Latence SQL élevée
1. ...

## 6. Tests de résilience
| Scénario | Fréquence | Dernier test |
| -------- | --------- | ------------ |
```

---

## 11. Exercices complémentaires

### Exercice 1 — SLI/SLO (30 min)

Pour une API de recherche produits, définissez 3 SLI, leurs SLO et l'error budget mensuel associé (99,9 %).

### Exercice 2 — Lire une trace (20 min)

À partir d'une trace App Insights (ou schéma fourni), identifiez :

- Le service le plus lent
- Si le problème est réseau ou base
- Quelle politique Polly aurait limité l'impact

### Exercice 3 — Logs et RGPD (15 min)

Liste 5 champs à ne **jamais** logger et 5 champs **utiles** pour le debug conformes RGPD.

---

## 12. Checklist observabilité

| ✓ | Critère |
| - | ------- |
| ☐ | App Insights / OTel configuré en prod |
| ☐ | Logs structurés (pas de concaténation) |
| ☐ | Correlation ID propagé |
| ☐ | Health checks exposés |
| ☐ | Dashboard golden signals |
| ☐ | Alertes actionnables avec runbook |
| ☐ | SLO documentés |
| ☐ | Pas de PII dans les logs |
| ☐ | Rétention logs définie (coût + conformité) |

---

## Livrables à rendre

| Fichier | Obligatoire |
| ------- | ----------- |
| `resilience-strategy.md` | Oui |
| `dashboard.md` (+ capture Workbook) | Oui |
| Code instrumenté ou `instrumentation-notes.md` | Recommandé |
| Config alertes documentée | Recommandé |

---

## Suite

Module suivant : [07 — Sécurité & gouvernance](../07-security/README.md)
