# Programme de formation

Ce document décrit le parcours pédagogique, les prérequis et les livrables attendus. Pour le détail complet des objectifs par module, voir aussi [`summary.md`](../summary.md).

---

## Objectif global

À l'issue de la formation, vous serez capable de :

- Concevoir une architecture distribuée de bout en bout
- Argumenter vos choix techniques (trade-offs, contraintes, coûts)
- Produire des artefacts exploitables en entreprise (DAT, diagrammes C4, chiffrage)
- Anticiper les problématiques de production (résilience, sécurité, observabilité)

---

## Prérequis

| Domaine | Niveau attendu |
| ------- | -------------- |
| Développement | Confortable avec au moins un langage (idéalement C# / .NET) |
| APIs | Notions de REST, HTTP, JSON |
| Bases de données | SQL de base, notions de clé primaire / index |
| Réseau | DNS, load balancer, HTTPS (niveau conceptuel) |
| Cloud | Aucun prérequis Azure ; les concepts seront introduits au module 5 |

**Profil visé :** développeur senior, tech lead ou architecte en devenir souhaitant structurer sa pratique du system design.

---

## Méthodologie

| Activité | Part du temps | Description |
| -------- | ------------- | ----------- |
| Théorie | 30 % | Concepts, patterns, compromis |
| Pratique | 40 % | Ateliers, exercices, diagrammes |
| Design review | 30 % | Revue de vos choix, feedback, itération |

Chaque module suit le même rythme :

1. Lire le `README.md` du module
2. Étudier la théorie (`theory.md`, `patterns.md`, etc.)
3. Réaliser les exercices ou ateliers
4. Produire le livrable du module
5. Auto-évaluer avec la [checklist de design review](../templates/design-review-checklist.md)

---

## Parcours — 8 modules

### Module 1 — Fondamentaux du System Design

**Dossier :** [`modules/01-fundamentals/`](../modules/01-fundamentals/)

**Thèmes :** scalabilité, latence vs throughput, CAP, BASE vs ACID, monolithe vs microservices, haute disponibilité.

**Livrables :**

- Fiche synthèse des compromis
- Design d'une API REST simple + diagramme

---

### Module 2 — Architecture applicative avancée

**Dossier :** [`modules/02-architecture/`](../modules/02-architecture/)

**Thèmes :** Clean Architecture, hexagonale, DDD, CQRS, event sourcing (intro), couplage et cohésion.

**Livrables :**

- Diagrammes Component + Sequence
- Documentation d'architecture

---

### Module 3 — Data & persistance

**Dossier :** [`modules/03-data/`](../modules/03-data/)

**Thèmes :** SQL vs NoSQL, OLTP vs OLAP, database per service, Saga, transactions distribuées, concurrence, invalidation de cache.

**Livrables :**

- Architecture data
- Choix technologiques justifiés

---

### Module 4 — Scalabilité & performance

**Dossier :** [`modules/04-scalability/`](../modules/04-scalability/)

**Thèmes :** load balancing, cache (Redis, CDN), partitioning / sharding, traitement asynchrone (queues, streaming).

**Livrables :**

- Architecture scalable
- Plan de montée en charge

---

### Module 5 — Architecture Cloud (Azure)

**Dossier :** [`modules/05-cloud-azure/`](../modules/05-cloud-azure/)

**Thèmes :** patterns Azure, App Service / AKS / Functions, Service Bus / Event Grid, API Management, multi-région, PRA / PCA.

**Livrables :**

- Diagrammes C4
- Chiffrage Azure
- Plan de reprise d'activité

---

### Module 6 — Résilience & observabilité

**Dossier :** [`modules/06-resilience/`](../modules/06-resilience/)

**Thèmes :** retry, circuit breaker (Polly), Azure Monitor, App Insights, logging distribué, alerting.

**Livrables :**

- Dashboard de supervision (conception)
- Stratégie de résilience documentée

---

### Module 7 — Sécurité & gouvernance

**Dossier :** [`modules/07-security/`](../modules/07-security/)

**Thèmes :** OAuth2, OpenID Connect, Entra ID, Zero Trust, gestion des secrets, IAM / RBAC.

**Livrables :**

- Modèle de sécurité
- Plan de gouvernance

---

### Module 8 — System Design (cas réels)

**Dossier :** [`modules/08-system-design-cases/`](../modules/08-system-design-cases/)

**Cas étudiés :** WhatsApp, Uber, système de paiement, système de logs distribués.

**Méthodologie :**

1. Clarification du besoin
2. Estimation du scale
3. High-level design
4. Deep dive
5. Analyse des trade-offs

---

## Projet final

**Dossier :** [`project/`](../project/) — solution de référence [**ShopFlow**](../project/README.md) (e-commerce Azure)

Concevoir un système complet de niveau entreprise. Exemples de sujets :

- Plateforme e-commerce *(référence fournie)*
- CRM
- API multi-tenant

**À produire :**

- Architecture complète (diagrammes C4) — voir [architecture ShopFlow](../project/architecture/README.md)
- Dossier d'architecture technique ([template DAT](../templates/dat-template.md) · [exemple](../project/dat/DAT.md))
- Pipeline CI/CD — voir [pipeline.yml](../project/ci-cd/pipeline.yml)
- Stratégie de monitoring et de sécurité
- Chiffrage Azure — voir [estimation](../project/cost/azure-cost-estimate.md)

---

## Livrables finaux (synthèse)

| Artefact | Template / emplacement |
| -------- | ---------------------- |
| DAT | [`templates/dat-template.md`](../templates/dat-template.md) |
| Diagrammes C4 | [`templates/c4-model-template.md`](../templates/c4-model-template.md) |
| Design review | [`templates/design-review-checklist.md`](../templates/design-review-checklist.md) |
| Projet | [`project/`](../project/) |

---

## Modules bonus (niveau avancé)

Détail et exercices : [bonus.md](bonus.md)

À aborder après le projet final ou en parallèle selon votre rythme :

- Architecture multi-tenant SaaS
- FinOps
- Event-driven avancé
- Data streaming (Kafka)
- Migration monolithe → microservices

---

## Critères de réussite

Vous avez validé la formation lorsque vous pouvez, pour un cas donné :

- [ ] Poser les bonnes questions fonctionnelles et non fonctionnelles
- [ ] Estimer ordre de grandeur charge, stockage, bande passante
- [ ] Proposer un high-level design lisible et cohérent
- [ ] Identifier les points de défaillance et les mitiger
- [ ] Justifier chaque choix technique majeur
- [ ] Produire un DAT et des diagrammes C4 exploitables par une équipe

---

## Documents associés

- [Index docs](README.md)
- [Planning détaillé](planning.md) — calendrier semaine par semaine
- [Ressources](ressources.md) — livres, liens, outils
- [Bonus](bonus.md) — sujets avancés
- [`summary.md`](../summary.md) — référence complète du programme
