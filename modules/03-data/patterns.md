# Patterns data distribués

Ce document traite des défis de cohérence, concurrence et cache lorsque les données sont réparties entre plusieurs services ou systèmes de stockage.

---

## 1. Le problème des transactions distribuées

Une transaction locale ACID :

```sql
BEGIN;
  UPDATE accounts SET balance = balance - 100 WHERE id = 1;
  UPDATE accounts SET balance = balance + 100 WHERE id = 2;
COMMIT;
```

En microservices, le débit et le crédit peuvent être dans **deux bases distinctes** → pas de transaction unique native.

### Two-Phase Commit (2PC)

Protocole coordonné : prepare → commit ou rollback sur tous les participants.

| Avantages | Inconvénients |
| --------- | ------------- |
| Cohérence forte | Bloquant, latence élevée |
| Modèle connu | Single point of failure (coordinateur) |
| | Peu adapté aux microservices cloud |

**En pratique :** évité dans les architectures cloud modernes. Préférer Saga ou cohérence éventuelle.

---

## 2. Saga pattern

Une **saga** est une séquence de **transactions locales**, chacune dans son service, avec des **compensations** en cas d'échec.

### Saga chorégraphiée (choreography)

Chaque service réagit aux événements et publie le sien. Pas d'orchestrateur central.

```diagram
Orders                Payments              Inventory
   │                      │                     │
   │── OrderCreated ─────►│                     │
   │                      │── PaymentDone ─────►│
   │                      │                     │── StockReserved
   │◄── OrderConfirmed ───┼─────────────────────┤
   │                      │                     │
   │ (si échec stock)     │                     │
   │◄── StockFailed ──────┼─────────────────────┤
   │── CancelPayment ────►│                     │
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Découplage, pas de SPOF | Flux difficile à suivre |
| Simple à démarrer | Risque de cycles d'événements |

### Saga orchestrée

Un **orchestrateur** central enchaîne les étapes et déclenche les compensations.

```diagram
┌─────────────────┐
│ Saga Orchestrator│
└────────┬────────┘
         │
    1. ReserveStock ──► Inventory
    2. ChargePayment ─► Payments
    3. ConfirmOrder ───► Orders

    (échec étape 2)
    → RefundPayment, ReleaseStock
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Flux explicite, testable | Orchestrateur = composant critique |
| Debugging plus simple | Couplage vers l'orchestrateur |

### Compensations vs rollback

| Rollback (ACID) | Compensation (Saga) |
| --------------- | ------------------- |
| Annule comme si rien n'avait eu lieu | Action inverse **métier** |
| Automatique | À concevoir explicitement |
| Exemple : ROLLBACK | Exemple : `RefundPayment`, `ReleaseInventory` |

**Important :** une compensation n'efface pas l'historique — elle crée un nouvel état (audit, idempotence requise).

### Implémentation .NET (pistes)

- **MassTransit** : state machine saga
- **NServiceBus** : saga native
- **Azure Durable Functions** : orchestration workflow
- **Custom** : table `saga_state` + messages Service Bus

---

## 3. Outbox pattern

Garantit que **la mise à jour de la base** et **la publication du message** ne divergent pas.

```diagram
┌─────────────────────────────────────┐
│  Transaction locale                 │
│  1. UPDATE orders SET status = ...  │
│  2. INSERT INTO outbox (event JSON) │
│  COMMIT                           │
└─────────────────────────────────────┘
              │
              ▼ (worker / CDC)
        Message bus → autres services
```

Sans outbox : risque de base commitée mais message jamais publié (ou l'inverse).

| Variante | Mécanisme |
| -------- | --------- |
| **Polling outbox** | Worker lit la table `outbox` et publie |
| **CDC (Debezium)** | Capture les inserts outbox → Kafka |
| **Transactional outbox** | Framework intégré (MassTransit, EF outbox) |

---

## 4. Gestion de la concurrence

Plusieurs requêtes modifient la même ressource simultanément.

### Lost update (problème)

```diagram
A lit stock = 10
B lit stock = 10
A écrit stock = 9  (vend 1)
B écrit stock = 9  (vend 1)  ← devrait être 8
```

### Verrouillage pessimiste

Bloquer la ligne jusqu'à la fin de la transaction.

```sql
SELECT * FROM products WHERE id = 1 FOR UPDATE;
-- personne d'autre ne modifie jusqu'au COMMIT
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Garantie forte | Risque de contention, deadlocks |
| Simple à raisonner | Ne scale pas bien sous forte charge |

### Concurrence optimiste

Pas de verrou ; on détecte le conflit à l'écriture via une **version**.

```csharp
public class Product
{
    public Guid Id { get; set; }
    public int Stock { get; set; }
    public int Version { get; set; }  // ou rowversion en SQL
}

// UPDATE products
// SET stock = @newStock, version = version + 1
// WHERE id = @id AND version = @expectedVersion
// → si 0 rows affected : ConcurrencyException → retry ou erreur 409
```

EF Core :

```csharp
entity.Property(e => e.Version).IsRowVersion(); // SQL Server timestamp
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Pas de blocage lecture | Conflits à gérer côté client |
| Bon pour lectures >> écritures | Retries nécessaires |

### Quand choisir ?

| Situation | Approche |
| --------- | -------- |
| Forte contention sur peu de lignes (compte bancaire) | Pessimiste ou sérialisation |
| Catalogues, profils, paniers | Optimiste |
| Compteurs agrégés (likes) | Opérations atomiques Redis `INCR` |
| Réservation stock e-commerce | Optimiste + file d'attente ou saga |

---

## 5. Cache — stratégies

Le cache réduit la latence et la charge sur la base OLTP.

```text
Client → Cache (Redis) → hit ? retour
                      → miss ? DB → remplir cache → retour
```

### Patterns de lecture

| Pattern | Description |
| ------- | ----------- |
| **Cache-aside** | App lit cache, sinon DB puis écrit cache |
| **Read-through** | Cache gère le chargement depuis DB |
| **Write-through** | Écriture synchrone cache + DB |
| **Write-behind** | Écriture cache d'abord, DB asynchrone (risqué) |

**Le plus courant :** cache-aside (contrôle explicite côté application).

### Exemple cache-aside (.NET pseudo-code)

```csharp
public async Task<ProductDto?> GetProductAsync(Guid id)
{
  var cached = await _cache.GetStringAsync($"product:{id}");
  if (cached != null)
    return JsonSerializer.Deserialize<ProductDto>(cached);

  var product = await _db.Products.FindAsync(id);
  if (product == null) return null;

  await _cache.SetStringAsync(
    $"product:{id}",
    JsonSerializer.Serialize(product),
    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

  return product;
}
```

---

## 6. Invalidation de cache

> « Il n'y a que deux choses difficiles en informatique : l'invalidation de cache et nommer les choses. »

### Stratégies

| Stratégie | Description | Usage |
| --------- | ----------- | ----- |
| **TTL** | Expiration automatique | Données tolérant un délai stale |
| **Invalidation explicite** | Supprimer la clé à l'écriture | Données sensibles à la fraîcheur |
| **Invalidation par événement** | `ProductUpdated` → purge cache | Microservices, découplage |
| **Write-invalidate** | Invalider avant ou après write DB | Cache-aside classique |

### Cache-aside + invalidation à l'écriture

```csharp
public async Task UpdateProductAsync(Product product)
{
  await _db.SaveChangesAsync(product);
  await _cache.RemoveAsync($"product:{product.Id}");
  await _cache.RemoveAsync("products:list"); // clé liste si existante
}
```

### Problème de la consistance cache / DB

```text
Thread A : UPDATE DB
Thread B : READ cache (stale) avant invalidation
Thread A : INVALIDATE cache
```

**Mitigations :**

- TTL court en complément de l'invalidation
- Version dans la clé (`product:123:v5`)
- Event-driven invalidation avec versioning
- Accepter stale reads si le métier le permet (module 1 — CAP/AP)

### Thundering herd (cache stampede)

Cache expire → 1000 requêtes simultanées frappent la DB.

**Solutions :**

- **Single-flight** : un seul thread recharge, les autres attendent
- **Probabilistic early expiration** : refresh avant expiration
- **Lock distribué** (Redis `SETNX`) pendant le rechargement

---

## 7. Idempotence et exactly-once (illusion)

En distribué, les messages peuvent être **dupliqués**. Les handlers doivent être **idempotents**.

```text
PaymentCharged event reçu 2 fois
  → vérifier si payment_id déjà traité
  → si oui, ignorer (200 OK)
  → si non, traiter et enregistrer l'id
```

Table `processed_messages(idempotency_key)` ou contrainte unique métier.

**Exactly-once** end-to-end est une illusion ; viser **at-least-once delivery + idempotent consumers**.

---

## 8. Lecture de données cross-service

Un service Orders a besoin du nom du client (service Customers).

| Approche | Fraîcheur | Complexité |
| -------- | --------- | ---------- |
| Appel API synchrone | Temps réel | Couplage, latence |
| Copie locale (event `CustomerUpdated`) | Éventuelle | Duplication, sync |
| Cache du résultat API | Configurable | Invalidation |

**Recommandation :** API pour besoins ponctuels ; réplication par événements pour lectures fréquentes.

---

## 9. Matrice de patterns

| Problème | Pattern |
| -------- | ------- |
| Transaction cross-service | Saga (orchestrée ou chorégraphiée) |
| DB + message cohérents | Outbox |
| Conflits d'écriture | Optimistic locking (défaut) ou pessimiste |
| Lenteur lectures | Cache-aside (Redis) |
| Données stale en cache | TTL + invalidation explicite |
| Message dupliqué | Idempotency key |
| Analytics sans impacter OLTP | CDC / ETL → entrepôt |

---

## 10. Anti-patterns data

| Anti-pattern | Pourquoi éviter |
| ------------ | --------------- |
| Distributed monolith (BDD partagée) | Couplage, pas de scale indépendant |
| 2PC entre microservices | Fragile, lent |
| Cache sans stratégie d'invalidation | Données incohérentes, bugs subtils |
| SELECT * sur tables énormes | Mémoire, réseau, latence |
| N+1 queries (ORM) | Charge DB multipliée |
| Event sourcing partout | Complexité sans besoin d'audit |

---

## Pour aller plus loin

- [Ateliers et cas pratiques](use-cases.md)
- Kleppmann — ch. 7 (transactions), ch. 9 (consistency)
- [Microsoft — Saga pattern](https://learn.microsoft.com/azure/architecture/reference-architectures/saga/saga)
