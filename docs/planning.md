# Planning de formation

Planning type sur **10 semaines** (8 modules + projet final). Ajustable selon votre disponibilité : comptez **6 à 8 h par semaine** en autonomie, ou **2 sessions de 3 h** si vous suivez la formation en groupe.

Pour le contenu détaillé de chaque module, voir [programme.md](programme.md).

---

## Vue d'ensemble

| Semaine | Module | Focus principal | Livrable clé |
| ------- | ------ | --------------- | ------------ |
| 1 | [01 — Fondamentaux](../modules/01-fundamentals/) | Compromis techniques | API REST + diagramme |
| 2 | [02 — Architecture](../modules/02-architecture/) | Patterns applicatifs | Component + Sequence |
| 3 | [03 — Data](../modules/03-data/) | Persistance distribuée | Architecture data |
| 4 | [04 — Scalabilité](../modules/04-scalability/) | Performance à l'échelle | Plan de montée en charge |
| 5 | [05 — Cloud Azure](../modules/05-cloud-azure/) | Services managés | Chiffrage + PRA |
| 6 | [06 — Résilience](../modules/06-resilience/) | Observabilité | Stratégie de résilience |
| 7 | [07 — Sécurité](../modules/07-security/) | Gouvernance | Modèle de sécurité |
| 8 | [08 — Cas réels](../modules/08-system-design-cases/) | Méthode system design | 2 cas complets |
| 9–10 | [Projet final](../project/) | Synthèse | DAT + C4 + CI/CD |

---

## Semaine 1 — Fondamentaux

**Objectif :** poser le vocabulaire et les compromis du system design.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun | Lecture théorie : scalabilité, CAP, latence/throughput | 2 h | `01-fundamentals/theory.md` |
| Mar | Exercices guidés | 2 h | `01-fundamentals/exercises.md` |
| Jeu | Design d'une API REST (besoin, endpoints, contraintes) | 2 h | — |
| Ven | Diagramme + fiche synthèse des compromis | 2 h | Draw.io / Excalidraw |

**Checkpoint :** pouvoir expliquer pourquoi on choisit scalabilité horizontale vs verticale sur un cas concret.

---

## Semaine 2 — Architecture applicative

**Objectif :** structurer une application selon des patterns reconnus.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun | Clean Architecture, hexagonale, DDD (lecture) | 2 h | `02-architecture/patterns.md` |
| Mar | Atelier CQRS / MediatR | 3 h | `02-architecture/workshop.md` |
| Jeu | Diagrammes Component + Sequence | 2 h | — |
| Ven | Documentation d'architecture + design review | 1 h | checklist |

**Checkpoint :** diagramme Component cohérent avec les frontières de domaine identifiées.

---

## Semaine 3 — Data & persistance

**Objectif :** choisir et justifier une stratégie de données distribuées.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun | SQL vs NoSQL, OLTP vs OLAP | 2 h | `03-data/database-design.md` |
| Mar | Saga, transactions distribuées, concurrence | 2 h | `03-data/patterns.md` |
| Jeu | Atelier : SQL + Redis + Event store | 3 h | `03-data/use-cases.md` |
| Ven | Rédaction architecture data | 1 h | — |

**Checkpoint :** chaque choix de stockage est justifié par un besoin fonctionnel ou non fonctionnel.

---

## Semaine 4 — Scalabilité & performance

**Objectif :** concevoir pour la charge et identifier les goulots d'étranglement.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun | Load balancing, caching, CDN | 2 h | `04-scalability/caching.md` |
| Mar | Sharding, files, streaming | 2 h | `04-scalability/performance.md` |
| Jeu | Design système type (e-commerce ou streaming) | 3 h | `04-scalability/exercises.md` |
| Ven | Plan de montée en charge (ordres de grandeur) | 1 h | — |

**Checkpoint :** estimation chiffrée (req/s, stockage, cache hit ratio cible).

---

## Semaine 5 — Cloud Azure

**Objectif :** mapper le design sur des services Azure et estimer les coûts.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun | Patterns Azure, catalogue de services | 2 h | `05-cloud-azure/services.md` |
| Mar | Design cloud complet (atelier) | 3 h | `05-cloud-azure/architecture.md` |
| Jeu | Diagrammes C4 (Context + Container) | 2 h | template C4 |
| Ven | Chiffrage Azure + plan PRA / PCA | 1 h | `project/cost/` |

**Checkpoint :** chaque composant du diagramme est rattaché à un service Azure nommé.

---

## Semaine 6 — Résilience & observabilité

**Objectif :** rendre le système observable et tolérant aux pannes.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun | Retry, circuit breaker, bulkhead | 2 h | `06-resilience/patterns.md` |
| Mar | Monitoring, logging distribué, alerting | 2 h | `06-resilience/monitoring.md` |
| Jeu | Instrumentation (conception ou PoC) | 2 h | — |
| Ven | Stratégie de résilience documentée | 2 h | — |

**Checkpoint :** pour chaque dépendance externe, une politique de résilience est définie.

---

## Semaine 7 — Sécurité & gouvernance

**Objectif :** intégrer sécurité et gouvernance dès la conception.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun | OAuth2, OIDC, Entra ID | 2 h | `07-security/auth.md` |
| Mar | Zero Trust, secrets, IAM / RBAC | 2 h | `07-security/governance.md` |
| Jeu | Cas pratique : sécuriser une archi distribuée | 2 h | `07-security/exercises.md` |
| Ven | Modèle de sécurité + plan de gouvernance | 2 h | — |

**Checkpoint :** flux d'authentification et d'autorisation documentés sur un diagramme de séquence.

---

## Semaine 8 — Cas réels (system design)

**Objectif :** appliquer la méthode complète sur des systèmes à grande échelle.

| Jour | Activité | Durée | Fichiers |
| ---- | -------- | ----- | -------- |
| Lun–Mar | Cas WhatsApp ou Uber (méthode en 5 étapes) | 4 h | `08-system-design-cases/` |
| Jeu–Ven | Cas paiement ou logs distribués | 4 h | idem |

**Méthode à suivre pour chaque cas :**

1. Clarification du besoin (fonctionnel + NFR)
2. Estimation du scale (utilisateurs, messages, stockage)
3. High-level design
4. Deep dive (2–3 composants critiques)
5. Trade-offs et évolutions possibles

**Checkpoint :** présentation orale ou écrite de 15 min par cas, avec questions / réponses.

---

## Semaines 9–10 — Projet final

**Objectif :** livrer une architecture de niveau entreprise, prête à être partagée avec une équipe.

### Semaine 9 — Conception

| Activité | Durée |
| -------- | ----- |
| Choix du sujet et lecture du cahier des charges | 2 h |
| Requirements + contraintes NFR | 3 h |
| Diagrammes C4 (Context, Container, Component) | 4 h |
| Rédaction DAT (sections 1 à 5) | 3 h |

### Semaine 10 — Finalisation

| Activité | Durée |
| -------- | ----- |
| DAT : sécurité, résilience, coûts, risques | 4 h |
| Pipeline CI/CD (conception + `pipeline.yml`) | 2 h |
| Design review finale (checklist) | 2 h |
| Présentation / soutenance | 2 h |

**Livrables attendus dans [`project/`](../project/) :**

- `requirements.md` complété — [référence ShopFlow](../project/requirements.md)
- Diagrammes dans `architecture/` — [Mermaid + guide Draw.io](../project/architecture/README.md)
- DAT dans `dat/` — [exemple DAT.md](../project/dat/DAT.md)
- Estimation dans `cost/` — [azure-cost-estimate.md](../project/cost/azure-cost-estimate.md)
- Pipeline dans `ci-cd/` — [pipeline.yml](../project/ci-cd/pipeline.yml)

---

## Jalons (milestones)

| Fin de semaine | Jalon |
| -------------- | ----- |
| 1 | Première API REST designée et documentée |
| 4 | Architecture scalable avec chiffres |
| 5 | Première version C4 + chiffrage Azure |
| 7 | Modèle de sécurité validé |
| 8 | 2 cas system design traités |
| 10 | Projet final livré |

---

## Rythmes alternatifs

### Format accéléré (6 semaines)

Fusionner les semaines 1–2, 3–4, 5–6, 7–8, puis 2 semaines de projet.

### Format étendu (16 semaines)

2 semaines par module, 2 semaines pour le projet final. Recommandé en parallèle d'une activité professionnelle à temps plein.

### Format entreprise (ateliers)

- **1 journée / module** : matinée théorie, après-midi atelier + design review en groupe
- **2 journées / projet final** : hackathon d'architecture avec soutenance

---

## Suivi de progression

Utilisez ce tableau pour suivre votre avancement :

| Module | Théorie | Exercices | Livrable | Review |
| ------ | ------- | --------- | -------- | ------ |
| 01 Fondamentaux | ☐ | ☐ | ☐ | ☐ |
| 02 Architecture | ☐ | ☐ | ☐ | ☐ |
| 03 Data | ☐ | ☐ | ☐ | ☐ |
| 04 Scalabilité | ☐ | ☐ | ☐ | ☐ |
| 05 Cloud Azure | ☐ | ☐ | ☐ | ☐ |
| 06 Résilience | ☐ | ☐ | ☐ | ☐ |
| 07 Sécurité | ☐ | ☐ | ☐ | ☐ |
| 08 Cas réels | ☐ | ☐ | ☐ | ☐ |
| Projet final | — | — | ☐ | ☐ |

---

## Documents associés

- [Index documentation](README.md)
- [Programme](programme.md) — contenu et livrables par module
- [Ressources](ressources.md) — outils et références
- [Bonus](bonus.md) — sujets avancés
- [`summary.md`](../summary.md) — référence synthétique
