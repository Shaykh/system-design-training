# Théorie — Fondamentaux du System Design

Ce document couvre les concepts de base nécessaires pour concevoir des systèmes distribués. Chaque section se termine par des questions à se poser lors d'un design review.

---

## 1. Qu'est-ce que le System Design ?

Le **system design** consiste à définir l'architecture d'un système logiciel en répondant à des besoins fonctionnels et **non fonctionnels** (NFR) :

| Type | Exemples |
| ---- | -------- |
| Fonctionnel | « L'utilisateur peut commander un produit » |
| Non fonctionnel | Disponibilité 99,9 %, latence p95 < 200 ms, 10 000 req/s |

Un bon design ne cherche pas la solution parfaite, mais la solution **adaptée au contexte** : volume, budget, équipe, délais.

**Questions clés avant de dessiner :**

- Qui utilise le système ? Combien d'utilisateurs simultanés ?
- Quelles opérations sont critiques (lecture, écriture, temps réel) ?
- Quel niveau de cohérence des données est acceptable ?
- Quel budget infra et quelle taille d'équipe pour maintenir le système ?

---

## 2. Scalabilité

La **scalabilité** est la capacité d'un système à gérer une charge croissante en ajoutant des ressources.

### Scalabilité verticale (scale up)

Ajouter de la puissance à une machine existante : plus de CPU, RAM, disque.

| Avantages | Inconvénients |
| --------- | ------------- |
| Simple à mettre en œuvre | Plafond matériel (une machine a une limite) |
| Pas de complexité distribuée | Point de défaillance unique |
| Adapté aux petits systèmes | Coût non linéaire (machines très puissantes) |

**Exemple :** passer d'une base PostgreSQL 4 vCPU / 16 Go à 16 vCPU / 64 Go.

### Scalabilité horizontale (scale out)

Ajouter des machines (nœuds) et répartir la charge.

| Avantages | Inconvénients |
| --------- | ------------- |
| Pas de plafond théorique | Complexité : load balancing, état partagé |
| Meilleure résilience (redondance) | Cohérence des données plus difficile |
| Coût progressif | Nécessite une architecture stateless ou partagée |

**Exemple :** déployer 5 instances d'une API derrière un load balancer.

### Quand choisir quoi ?

```text
Charge faible, équipe petite     → vertical souvent suffisant
Charge élevée, haute dispo       → horizontal incontournable
Pic prévisible (Black Friday)    → horizontal + auto-scaling
```

### Scalabilité des différentes couches

| Couche | Stratégie courante |
| ------ | ----------------- |
| API / compute | Horizontal (stateless) |
| Cache | Horizontal (Redis cluster) |
| Base de données | Vertical d'abord, puis read replicas, sharding |
| Fichiers | Object storage (S3, Blob) — scale out natif |

---

## 3. Latence vs throughput

Ce sont deux métriques distinctes qu'il ne faut pas confondre.

### Latence

Temps pour **une** opération (requête → réponse).

- Mesurée en millisecondes (ms)
- On regarde souvent p50, p95, p99 (percentiles)
- **p99 = 500 ms** signifie que 99 % des requêtes répondent en moins de 500 ms

### Throughput

Nombre d'opérations traitées **par unité de temps**.

- Mesuré en requêtes/seconde (req/s), messages/seconde, Mo/s
- Un système peut avoir un bon throughput mais une latence élevée (batch processing)
- Un système peut avoir une faible latence mais un throughput limité (synchrone, séquentiel)

### Relation et compromis

```text
Throughput élevé + latence faible = objectif idéal, rarement gratuit

Techniques pour réduire la latence :
  - Cache
  - CDN (contenu statique proche de l'utilisateur)
  - Connexions persistantes
  - Parallélisation

Techniques pour augmenter le throughput :
  - Traitement asynchrone (queues)
  - Batch processing
  - Plus d'instances (scale out)
```

**Exemple concret :**

- Une API de recherche : latence p95 < 100 ms (expérience utilisateur)
- Un pipeline ETL nocturne : throughput de 10 millions d'enregistrements/heure (latence secondaire)

---

## 4. Théorème CAP

Formulé par Eric Brewer : dans un système distribué, on ne peut garantir simultanément que **deux** des trois propriétés suivantes :

| Propriété | Signification |
| --------- | ------------- |
| **C** — Consistency | Tous les nœuds voient les mêmes données au même moment |
| **A** — Availability | Chaque requête reçoit une réponse (pas d'erreur système) |
| **P** — Partition tolerance | Le système continue malgré une coupure réseau entre nœuds |

### Pourquoi P est non négociable

Dans un système distribué réel, les partitions réseau **arrivent**. On ne choisit donc pas « P ou pas P », mais **CP ou AP** lors d'une partition :

```text
Partition réseau détectée
        │
        ├── Choix CP : refuser certaines requêtes pour garantir la cohérence
        │              (ex. base relationnelle synchrone, quorum strict)
        │
        └── Choix AP : répondre avec des données potentiellement obsolètes
                       (ex. DynamoDB, Cassandra en mode eventuel)
```

### Exemples

| Système | Tendance | Justification |
| ------- | -------- | ------------- |
| PostgreSQL (single node) | CA (pas distribué par défaut) | Pas de partition entre nœuds |
| Redis Cluster | CP ou AP selon config | Dépend de la tolérance aux lectures stale |
| Cassandra | AP | Disponibilité et partition tolerance prioritaires |
| ZooKeeper / etcd | CP | Cohérence pour coordination (élection leader) |

### En pratique : cohérence éventuelle

La plupart des systèmes « AP » convergent vers la cohérence après un délai (**eventual consistency**). C'est acceptable pour :

- Compteurs de likes
- Fil d'actualité
- Inventaire avec réservation compensée

C'est **inacceptable** pour :

- Solde bancaire
- Double dépense
- Attribution de numéro de commande unique

---

## 5. ACID vs BASE

Modèles de garanties pour la persistance des données.

### ACID (bases relationnelles classiques)

| Lettre | Signification |
| ------ | ------------- |
| **A** — Atomicity | Tout ou rien (transaction) |
| **C** — Consistency | Contraintes respectées après chaque transaction |
| **I** — Isolation | Transactions concurrentes ne s'interfèrent pas |
| **D** — Durability | Données persistées survivent à un crash |

**Idéal pour :** commandes, paiements, inventaire critique.

### BASE (souvent NoSQL / distribué)

| Lettre | Signification |
| ------ | ------------- |
| **BA** — Basically Available | Le système répond la plupart du temps |
| **S** — Soft state | L'état peut changer sans nouvelle écriture (réplication) |
| **E** — Eventually consistent | Cohérence atteinte après un délai |

**Idéal pour :** logs, analytics, cache, données à très grand volume avec tolérance au délai.

### Comparaison

| Critère | ACID | BASE |
| ------- | ---- | ---- |
| Cohérence | Forte, immédiate | Éventuelle |
| Performance écriture | Limitée (verrous) | Souvent plus élevée |
| Scalabilité horizontale | Difficile | Naturelle |
| Complexité applicative | Plus faible | Compensation côté app (Saga, retry) |

**Règle pratique :** utiliser ACID là où une incohérence coûte cher ; BASE là où le volume et la disponibilité priment.

---

## 6. Monolithe vs microservices

### Monolithe

Une seule application déployable contenant toutes les fonctionnalités.

```diagram
┌─────────────────────────────────┐
│           MONOLITHE             │
│  UI │ API │ Métier │ Data Access │
└─────────────────────────────────┘
              │
         ┌────┴────┐
         │   BDD   │
         └─────────┘
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Simple à développer et déployer | Scale global (tout ou rien) |
| Transactions locales faciles | Couplage fort à long terme |
| Debugging et tests plus simples | Une panne peut tout impacter |
| Adapté aux petites équipes | Déploiements risqués à grande échelle |

### Microservices

Application découpée en services autonomes, chacun avec sa responsabilité et souvent sa base de données.

```diagram
┌──────────┐  ┌──────────┐  ┌──────────┐
│ Service  │  │ Service  │  │ Service  │
│  Users   │  │  Orders  │  │ Payments │
└────┬─────┘  └────┬─────┘  └────┬─────┘
     │             │             │
   ┌─┴─┐         ┌─┴─┐         ┌─┴─┐
   │DB │         │DB │         │DB │
   └───┘         └───┘         └───┘
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Scale indépendant par service | Complexité opérationnelle |
| Déploiements isolés | Transactions distribuées (Saga) |
| Équipes autonomes par domaine | Observabilité, réseau, latence inter-services |
| Résilience partielle | Coût infra et gouvernance |

### Quand migrer vers les microservices ?

Ne pas commencer par les microservices. Envisager le découpage quand :

- L'équipe dépasse ~10 développeurs sur le même codebase
- Des parties du monolithe ont des besoins de scale très différents
- Les cycles de déploiement se bloquent mutuellement
- Les domaines métier sont clairement séparables (DDD)

**Pattern courant :** *monolithe modulaire* d'abord, extraction progressive des services les plus contraints.

---

## 7. Haute disponibilité et tolérance aux pannes

### Définitions

| Terme | Signification |
| ----- | ------------- |
| **Disponibilité** | Proportion de temps où le système est opérationnel |
| **Fault tolerance** | Capacité à continuer malgré la défaillance d'un composant |
| **Résilience** | Capacité à se rétablir après une défaillance |
| **MTBF** | Mean Time Between Failures — temps moyen entre pannes |
| **MTTR** | Mean Time To Repair — temps moyen de rétablissement |

### SLA et « nines »

| Disponibilité | Downtime / an (approx.) |
| ------------- | ----------------------- |
| 99 % (2 nines) | ~3,65 jours |
| 99,9 % (3 nines) | ~8,76 heures |
| 99,99 % (4 nines) | ~52 minutes |
| 99,999 % (5 nines) | ~5 minutes |

Chaque nine supplémentaire coûte significativement plus cher en infra et en complexité.

### Techniques fondamentales

#### Redondance

- Plusieurs instances derrière un load balancer
- Réplicas de base de données (primary + standby)
- Multi-zone ou multi-région

#### Éliminer les SPOF (Single Point of Failure)

- Identifier chaque composant dont la panne arrête tout le système
- Ajouter redondance ou bascule automatique (failover)

#### Health checks

- Le load balancer retire les instances défaillantes
- Kubernetes redémarre les pods unhealthy

#### Dégradation gracieuse

- Si le service de recommandations tombe, la page produit s'affiche sans recommandations
- Circuit breaker pour éviter les cascades de pannes

```text
Sans redondance :
  Client → [API] → DB     ← panne API = système down

Avec redondance :
  Client → [LB] → [API-1]
              → [API-2] → [DB Primary]
                        → [DB Replica]
```

---

## 8. Estimation rapide (ordre de grandeur)

Compétence essentielle en system design : chiffrer avant de sur-architecturer.

### Exemple : API de lecture

**Hypothèses :**

- 1 million d'utilisateurs actifs / jour
- Chaque utilisateur fait 20 requêtes / jour
- Pic : 3× la moyenne

```text
Requêtes / jour   = 1M × 20 = 20M
Requêtes / sec    = 20M / 86400 ≈ 230 req/s (moyenne)
Pic               ≈ 700 req/s
```

**Taille des données (ex. profils utilisateur) :**

```text
1M utilisateurs × 2 Ko / profil = 2 Go de stockage
```

Ces chiffres orientent le choix : un monolithe + PostgreSQL + cache Redis peut suffire ; pas besoin de Kafka pour 700 req/s.

---

## 9. Synthèse des compromis

| Décision | Option A | Option B | Facteur décisif |
| -------- | -------- | -------- | --------------- |
| Scale | Vertical | Horizontal | Volume, budget, complexité acceptable |
| Données | ACID (SQL) | BASE (NoSQL) | Criticité de la cohérence |
| Architecture | Monolithe | Microservices | Taille équipe, maturité domaine |
| CAP (partition) | CP | AP | Métier tolère lecture stale ? |
| Perf | Latence faible | Throughput élevé | UX temps réel vs batch |
| Disponibilité | 99,9 % | 99,99 % | Coût vs impact business |

---

## 10. Pour aller plus loin

- [Ressources module 1](../../docs/ressources.md#module-1--fondamentaux)
- [Planning semaine 1](../../docs/planning.md#semaine-1--fondamentaux)
- Kleppmann — *Designing Data-Intensive Applications*, ch. 1–2

**Prochaine étape :** réaliser les exercices dans [`exercises.md`](exercises.md).
