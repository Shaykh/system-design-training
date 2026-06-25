# Conception des bases de données

Ce document couvre les fondamentaux du choix et de la modélisation des systèmes de persistance dans une architecture moderne.

---

## 1. Rôle de la couche data

La persistance n'est pas qu'un détail technique : elle conditionne **performance**, **cohérence**, **évolutivité** et **coût**.

Questions à se poser avant de choisir une technologie :

- Quel volume de données (aujourd'hui, dans 3 ans) ?
- Ratio lecture / écriture ?
- Besoin de jointures complexes ou de schéma flexible ?
- Cohérence forte obligatoire ou cohérence éventuelle acceptable ?
- Requêtes ad hoc (analytics) ou transactions courtes (métier) ?

---

## 2. SQL vs NoSQL

### Bases relationnelles (SQL)

Données structurées en tables avec schéma défini, relations via clés étrangères, requêtes SQL.

| Avantages | Inconvénients |
| --------- | ------------- |
| Transactions ACID | Scale horizontal plus difficile |
| Jointures puissantes | Schéma rigide (migrations) |
| Maturité, outillage (PostgreSQL, SQL Server) | Performance sur très gros volumes non structurés |
| Modèle bien compris par les équipes | |

**Exemples :** PostgreSQL, SQL Server, MySQL, Azure SQL.

**Cas d'usage typiques :** commandes, facturation, inventaire, tout ce qui nécessite des invariants transactionnels.

### NoSQL — familles principales

| Famille | Modèle | Exemples | Cas d'usage |
| ------- | ------ | -------- | ----------- |
| **Clé-valeur** | `key → value` | Redis, DynamoDB | Cache, sessions, compteurs |
| **Document** | JSON/BSON | MongoDB, Cosmos DB | Catalogues, profils flexibles |
| **Colonne** | Colonnes groupées | Cassandra, HBase | Séries temporelles, logs à scale |
| **Graphe** | Nœuds + arêtes | Neo4j, Cosmos Gremlin | Réseaux sociaux, recommandations |

### Arbre de décision simplifié

```text
Besoin de transactions multi-lignes ACID ?
  ├── Oui → SQL (PostgreSQL, SQL Server)
  └── Non
        ├── Schéma très variable, documents ?
        │     └── Document DB (MongoDB)
        ├── Scale massif write, tolérance partition ?
        │     └── Colonne (Cassandra)
        ├── Traversée de relations complexes ?
        │     └── Graphe (Neo4j)
        └── Cache / session / compteur rapide ?
              └── Clé-valeur (Redis)
```

### Polyglot persistence

Une même application peut utiliser **plusieurs** types de stockage :

```diagram
┌─────────────┐
│   Service   │
│   Orders    │
└──────┬──────┘
       │
   ┌───┴───┬──────────┐
   ▼       ▼          ▼
PostgreSQL Redis   Event Store
(commandes) (cache)  (audit)
```

Chaque stockage est choisi pour un **besoin précis**, pas par mode.

---

## 3. OLTP vs OLAP

Deux mondes distincts qu'il ne faut pas mélanger sur la même base en production.

### OLTP (Online Transaction Processing)

- Transactions courtes, petites quantités de lignes
- Lectures et écritures fréquentes
- Données **normalisées** (3NF)
- Exemple : créer une commande, débiter un stock

```text
Utilisateur → API → INSERT / UPDATE → Base OLTP (PostgreSQL)
```

### OLAP (Online Analytical Processing)

- Requêtes lourdes sur de gros volumes
- Principalement en **lecture**
- Données **dénormalisées** (star schema, data mart)
- Exemple : chiffre d'affaires par région sur 3 ans

```text
OLTP → ETL / CDC → Entrepôt (Snowflake, Synapse) → Dashboards
```

### Comparaison

| Critère | OLTP | OLAP |
| ------- | ---- | ---- |
| Requêtes | Pointues, indexées | Scans larges, agrégations |
| Schéma | Normalisé | Dénormalisé (étoile) |
| Latence | Millisecondes | Secondes à minutes |
| Utilisateurs | Applications | Analystes, BI |
| Techno courante | PostgreSQL, SQL Server | Snowflake, BigQuery, Synapse |

### Pattern CQRS + data warehouse

Le module 2 introduisait CQRS au niveau applicatif. Ici, on l'étend :

- **Write model** → base OLTP
- **Read model analytics** → entrepôt alimenté par événements ou ETL

Ne pas exécuter de rapports lourds sur la base OLTP de production : impact sur la latence des transactions métier.

---

## 4. Database per service

Dans une architecture microservices, chaque service possède **sa propre base de données**. Aucun accès direct à la BDD d'un autre service.

```diagram
┌──────────────┐     API      ┌──────────────┐
│   Orders     │◄────────────►│   Payments   │
│   Service    │              │   Service    │
└──────┬───────┘              └──────┬───────┘
       │                             │
   ┌───┴───┐                     ┌───┴───┐
   │Orders │                     │Payments│
   │  DB   │                     │  DB   │
   └───────┘                     └───────┘
```

### Avantages

- Découplage : changement de schéma sans impacter les autres
- Scale indépendant
- Choix technologique adapté par domaine

### Inconvénients

- Pas de `JOIN` cross-service
- Cohérence distribuée à gérer (Saga, events)
- Duplication de données (eventual consistency)

### Partage de données entre services

| Approche | Description |
| -------- | ----------- |
| **API** | Service A appelle l'API de B pour obtenir des données |
| **Réplication par événements** | B publie `CustomerUpdated`, A maintient une copie locale |
| **CDC** | Capture des changements OLTP → stream → consommateurs |

**Anti-pattern :** base de données partagée entre microservices (retour au monolithe distribué).

---

## 5. Modélisation relationnelle (rappels utiles)

### Normalisation

| Forme | Principe |
| ----- | -------- |
| 1NF | Pas de tableaux dans les colonnes |
| 2NF | Pas de dépendance partielle à la clé |
| 3NF | Pas de dépendance transitive |

En OLTP, viser la 3NF. Dénormaliser **volontairement** pour la performance (avec documentation du compromis).

### Index

Accélèrent les lectures, ralentissent les écritures (maintenance d'index).

```sql
-- Recherche fréquente par client
CREATE INDEX idx_orders_customer_id ON orders(customer_id);

-- Recherche composite
CREATE INDEX idx_orders_customer_date ON orders(customer_id, created_at DESC);
```

**Règles :**

- Indexer les colonnes des `WHERE`, `JOIN`, `ORDER BY` fréquents
- Éviter la sur-indexation sur tables très écrites
- Analyser les requêtes lentes (`EXPLAIN` / plans d'exécution)

### Clés et identifiants

| Type | Usage |
| ---- | ----- |
| **Surrogate key** (UUID, bigint auto) | Identifiant technique stable |
| **Natural key** (email, SIRET) | Peut changer → prudent comme PK |
| **UUID v4** | Décentralisé, pas de collision entre services |
| **Snowflake ID** | Tri chronologique, génération distribuée |

En microservices, préférer des IDs générés côté application (UUID) plutôt que des séquences partagées.

---

## 6. Modélisation NoSQL (documents)

Exemple catalogue produits (MongoDB) :

```json
{
  "_id": "prod-123",
  "name": "Laptop X",
  "category": "electronics",
  "variants": [
    { "sku": "LX-16-512", "ram": 16, "storage": 512, "price": 999 }
  ],
  "attributes": { "brand": "Acme", "warranty": 24 }
}
```

**Avantages :** schéma flexible, lecture d'un document complet sans jointure.

**Attention :**

- Pas de transactions multi-documents partout (selon moteur)
- Duplication si les données sont référencées dans plusieurs documents
- Définir quand même un **schéma applicatif** (validation)

---

## 7. Event store (introduction)

Base optimisée pour stocker des **événements immuables** plutôt que l'état courant.

```text
Stream order-456 :
  [OrderCreated] [LineAdded] [LineAdded] [OrderConfirmed]
```

| Technologie | Usage |
| ----------- | ----- |
| EventStoreDB | Event sourcing natif |
| Kafka | Log distribué, rétention configurable |
| Azure Event Hubs | Ingestion événements à scale |
| Table SQL append-only | Approche minimaliste |

**Rôles dans l'architecture data :**

1. **Source de vérité** (event sourcing pur)
2. **Journal d'intégration** entre services (outbox → bus)
3. **Alimentation read models** et analytics

Couplage typique avec le module 2 : domain events persistés → projections vers SQL ou cache.

---

## 8. Choix technologiques — grille de décision

| Besoin | Technologie recommandée | Alternative |
| ------ | ----------------------- | ----------- |
| Transactions commandes | PostgreSQL / SQL Server | — |
| Cache lecture | Redis | Memcached |
| Sessions utilisateur | Redis | Cookie + JWT stateless |
| Recherche full-text | Elasticsearch | PostgreSQL FTS |
| Analytics | Synapse / Snowflake | PostgreSQL + Citus (petit scale) |
| Files / blobs | Azure Blob Storage | S3 |
| File d'attente | Azure Service Bus | RabbitMQ, Kafka |
| Audit immuable | Event store / append log | Table `events` dédiée |

---

## 9. Synthèse

| Question | Piste |
| -------- | ----- |
| Une ou plusieurs bases ? | Polyglot si besoins différents ; une seule au début si monolithe |
| SQL ou NoSQL ? | SQL par défaut pour le métier transactionnel |
| OLTP mélangé avec analytics ? | Non — pipeline séparé |
| Comment partager des données entre services ? | API ou événements, jamais BDD partagée |
| Quand un event store ? | Intégration, audit, CQRS read side — pas pour un CRUD simple |

---

## Pour aller plus loin

- [Patterns data distribués](patterns.md)
- [Ateliers](use-cases.md)
- [Ressources module 3](../../docs/ressources.md#module-3--data--persistance)
