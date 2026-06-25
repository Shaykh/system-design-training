# Performance et montée en charge

Ce document couvre les mécanismes pour faire évoluer un système sous charge croissante : load balancing, partitionnement des données et traitement asynchrone.

---

## 1. Performance vs scalabilité

| Terme | Question posée |
| ----- | -------------- |
| **Performance** | À charge fixe, le système est-il rapide ? |
| **Scalabilité** | Quand la charge augmente, le système suit-il ? |

Un système peut être **rapide** mais **non scalable** (machine unique très puissante), ou **scalable** mais nécessitant plusieurs nœuds pour atteindre la latence cible.

**Objectif typique :** latence stable (p95 < seuil) tout en augmentant le throughput par ajout de ressources.

---

## 2. Analyse des goulots d'étranglement

Avant d'ajouter des serveurs, identifier **où** le système bloque.

### Modèle en couches

```text
Client → CDN → LB → API (N instances) → Cache → DB / Queue → Workers
```

Chaque maillon a une **capacité maximale** (req/s, Mo/s, IOPS).

### Signaux courants

| Symptôme | Goulot probable | Piste |
| -------- | --------------- | ----- |
| CPU API à 100 % | Compute | Scale horizontal API |
| Connexions DB saturées | Pool / requêtes | Cache, read replicas, optimiser requêtes |
| Disque IOPS max | Stockage | SSD, sharding, cache |
| Latence réseau inter-régions | Géographie | CDN, région plus proche |
| Files qui grossissent | Traitement async lent | Plus de consumers |
| GC pauses (.NET) | Mémoire / allocations | Profiling, pooling |

### Loi de Amdahl

Si 20 % du traitement est séquentiel, le speedup max même avec ∞ processeurs est **5×**. Identifier la partie séquentielle avant de scaler.

### Métriques à surveiller

| Métrique | Outil |
| -------- | ----- |
| Latence p50/p95/p99 | APM (App Insights) |
| Throughput req/s | Load balancer, API gateway |
| CPU / RAM / disque | Monitoring infra |
| Connexions DB actives | PostgreSQL `pg_stat_activity` |
| Profondeur de file | Service Bus, RabbitMQ |
| Cache hit ratio | Redis INFO stats |

---

## 3. Load balancing

Répartir le trafic entrant sur **plusieurs instances** d'un même service.

```diagram
                    ┌─────────┐
Clients ──────────► │   LB    │
                    └────┬────┘
           ┌─────────────┼─────────────┐
           ▼             ▼             ▼
        [API-1]       [API-2]       [API-3]
```

### Niveaux de load balancing

| Couche | Exemple | Portée |
| ------ | ------- | ------ |
| DNS | Route 53, Azure Traffic Manager | Géographique, failover |
| L4 (transport) | Azure Load Balancer | IP + port, TCP/UDP |
| L7 (application) | Application Gateway, NGINX | URL, headers, cookies |
| Service mesh | Istio, Linkerd | Entre microservices |

### Algorithmes

| Algorithme | Comportement | Usage |
| ---------- | ------------ | ----- |
| **Round robin** | Tour à tour | Instances homogènes |
| **Least connections** | Envoie au moins chargé | Connexions longues (WebSocket) |
| **IP hash** | Même client → même serveur | Session sticky sans cookie |
| **Weighted** | Plus de trafic vers instances puissantes | Hétérogène |
| **Random** | Aléatoire | Simple, souvent suffisant |

### Health checks

Le LB retire les instances qui échouent :

```text
GET /health → 200 OK + { "status": "healthy", "db": "ok" }
```

- **Liveness** : le processus répond-il ?
- **Readiness** : peut-il traiter du trafic (DB accessible) ?

Sans health check : trafic envoyé vers des instances mortes.

### Sticky sessions (affinité)

Même utilisateur → même instance (session en mémoire).

| Avantages | Inconvénients |
| --------- | ------------- |
| Sessions locales simples | Déséquilibre de charge |
| | Perte session si instance tombe |

**Préférer :** sessions externalisées (Redis) ou JWT stateless → pas de sticky.

### SSL termination

Le LB déchiffre HTTPS et forward en HTTP interne (réseau privé). Réduit la charge CPU sur les instances API.

---

## 4. Scaling horizontal des applications

### Stateless services

Chaque requête est indépendante ; aucun état en mémoire locale.

```text
✓ JWT dans Authorization header
✓ Session dans Redis
✗ Panier uniquement en RAM du serveur
```

### Auto-scaling

Règles basées sur des métriques :

```yaml
# Exemple conceptuel
scale_up_when:   cpu > 70% pendant 5 min
scale_down_when: cpu < 30% pendant 10 min
min_instances:   2
max_instances:   20
```

**Bonnes pratiques :**

- Scale **progressif** (éviter oscillations)
- **Cooldown** entre scale up/down
- Tester le **cold start** (nouvelle instance prête en combien de secondes ?)
- Pré-scaling avant pics connus (événements, soldes)

### Connection pooling

Chaque instance API ouvre des connexions DB. 50 instances × 20 connexions = 1000 connexions PostgreSQL.

```text
Limiter pool par instance
Utiliser PgBouncer / proxy SQL
Read replicas pour les lectures
```

---

## 5. Partitioning et sharding

Quand une seule base ne suffit plus (stockage, IOPS, throughput écriture).

### Partitioning (sur une machine)

Découper une table en **partitions** logiques (souvent par date ou plage de clé).

```sql
-- PostgreSQL : partition par mois
CREATE TABLE orders_2025_01 PARTITION OF orders
  FOR VALUES FROM ('2025-01-01') TO ('2025-02-01');
```

Requêtes filtrées par date → scan d'une seule partition.

### Sharding (multi-machines)

Données réparties sur **plusieurs bases** ; chaque shard est autonome.

```diagram
                    ┌──────────────┐
                    │ Shard router │
                    │ (app / proxy)│
                    └──────┬───────┘
           ┌───────────────┼───────────────┐
           ▼               ▼               ▼
      [Shard A]       [Shard B]       [Shard C]
      user_id         user_id         user_id
      0 – 33M         33M – 66M      66M – 100M
```

### Stratégies de clé de shard

| Stratégie | Description | Problème |
| --------- | ----------- | -------- |
| **Hash** | `hash(user_id) % N` | Rééquilibrage si N change |
| **Range** | Plages de clés | Hot spots si clés récentes concentrées |
| **Directory** | Table de mapping clé → shard | SPOF sur l'annuaire |
| **Géographique** | Shard par région | Conformité, latence |

### Hot spots

Un shard reçoit disproportionnément de trafic (célébrités, tenants enterprise).

**Mitigations :**

- Shard key composite (`tenant_id + entity_id`)
- Split d'un shard chaud en sous-shards
- Cache agressif sur les clés chaudes

### Requêtes cross-shard

Évitées autant que possible. Si nécessaires :

- **Scatter-gather** : interroger tous les shards, agréger
- **Index secondaire global** (Elasticsearch, service dédié)

**Coût :** latence × nombre de shards.

### Rééquilibrage

Ajouter un shard nécessite de **migrer** des données. Outils : Vitess, Citus, ou scripts maison avec double-écriture temporaire.

---

## 6. Read replicas

Scale des **lectures** sans sharding complet.

```diagram
         ┌─────────────┐
Écriture │   Primary   │
    ────►│  PostgreSQL │
         └──────┬──────┘
                │ réplication
         ┌──────┴──────┐
         ▼             ▼
    [Replica 1]   [Replica 2]  ◄── lectures
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Simple vs sharding | Réplication asynchrone → lag |
| Lectures scale-out | Écritures toujours sur primary |

**Usage :** rapports, listes, dashboards. Pas pour « lire sa propre écriture » immédiatement sans routage primary.

---

## 7. Traitement asynchrone

Découpler la **réponse utilisateur** du **traitement long**.

### Pattern request → queue → worker

```diagram
Client ──POST /orders──► API ──► [Queue] ──► Workers
              │                      │
              └── 202 Accepted       ├── envoi email
                  { orderId }        ├── génération PDF
                                     └── indexation search
```

| Avantages | Inconvénients |
| --------- | ------------- |
| API répond vite | Cohérence éventuelle |
| Absorbe les pics (buffer) | Complexité opérationnelle |
| Scale workers indépendamment | Debugging plus difficile |

### Technologies de messaging

| Type | Exemples | Usage |
| ---- | -------- | ----- |
| **Queue** | Azure Service Bus, RabbitMQ, SQS | Tâche = un consumer |
| **Log / stream** | Kafka, Event Hubs | Plusieurs consumers, replay |
| **Pub/Sub** | Redis Pub/Sub, Event Grid | Notifications légères |

### Queue vs stream

| | Queue | Stream (Kafka) |
| - | ----- | -------------- |
| Message consommé | Une fois (ack) | Offset par consumer group |
| Ordre | Par queue | Par partition |
| Rétention | Jusqu'à traitement | Configurable (jours) |
| Replay | Non (classique) | Oui |

### Backpressure

Si les workers ne suivent pas, la file grossit → latence traitement augmente.

**Mitigations :**

- Scale consumers (Kubernetes HPA sur profondeur queue)
- **Dead letter queue** (DLQ) pour messages en échec répété
- Rate limiting côté producteur
- Alertes sur `queue_depth > seuil`

### Idempotence

Les workers doivent supporter le **retraitement** (at-least-once delivery). Voir module 3.

---

## 8. Event streaming à grande échelle

Pour très haut débit et multiples consommateurs (analytics, notifications, search).

```diagram
Producers → Kafka Topic (partitions) → Consumer Group A (search)
                                    → Consumer Group B (analytics)
                                    → Consumer Group C (audit)
```

### Partitions Kafka

- Ordre garanti **dans une partition**
- Clé de partition = `order_id` → tous les événements d'une commande ordonnés
- N partitions = max parallélisme N consumers dans un groupe

### Cas d'usage

| Système | Rôle du stream |
| ------- | -------------- |
| Netflix | Événements visionnage → recommandations |
| Uber | Positions chauffeurs → dispatch |
| E-commerce | `OrderPlaced` → inventory, shipping, BI |

---

## 9. Capacity planning (introduction)

Estimer les ressources **avant** la mise en production et planifier les phases de croissance.

### Étapes

1. **Estimer la charge** (req/s, Mo/jour, événements/s)
2. **Identifier le goulot** théorique (souvent DB ou API)
3. **Dimensionner** chaque couche avec marge (30–50 %)
4. **Définir des seuils** d'alerte et règles d'auto-scale
5. **Planifier les phases** (MVP → croissance → scale)

### Exemple simplifié

```text
Pic : 5 000 req/s lecture catalogue
Cache hit ratio cible : 90 %
→ DB : 500 req/s lecture

1 replica PostgreSQL ≈ 2 000 req/s simple SELECT
→ 1 primary + 1 replica suffit avec marge

API : 5 000 req/s, 1 instance ≈ 500 req/s
→ 10 instances + LB
```

### Load testing

Valider les estimations :

- **k6**, **JMeter**, **Azure Load Testing**
- Scénarios : rampe progressive, pic brutal, endurance
- Mesurer p95, taux d'erreur, saturation

---

## 10. Checklist architecture scalable

| ✓ | Question |
| - | -------- |
| ☐ | Les services API sont-ils stateless ? |
| ☐ | Health checks configurés sur le LB ? |
| ☐ | Cache sur les lectures dominantes ? |
| ☐ | Files pour les opérations lentes ? |
| ☐ | Pool de connexions DB dimensionné ? |
| ☐ | Auto-scaling testé ? |
| ☐ | Plan de sharding si dépassement prévu ? |
| ☐ | DLQ et alerting sur les files ? |
| ☐ | CDN pour assets statiques ? |
| ☐ | Load test avant mise en prod ? |

---

## Pour aller plus loin

- [Cache et CDN](caching.md)
- [Exercices et ateliers](exercises.md)
- [Planning semaine 4](../../docs/planning.md#semaine-4--scalabilité--performance)
