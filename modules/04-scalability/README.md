# Module 4 — Scalabilité & performance

**Durée :** 1 semaine · **Référence planning :** [Semaine 4](../../docs/planning.md#semaine-4--scalabilité--performance)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Concevoir une architecture supportant une montée en charge progressive
- Placer load balancers, cache et CDN aux bons endroits
- Partitionner des données (sharding) quand une seule base ne suffit plus
- Découpler les traitements lourds via files et streaming
- Identifier les goulots d'étranglement et proposer des mitigations
- Produire un plan de montée en charge chiffré

---

## Contenu

| Thème | Fichier |
| ----- | ------- |
| Performance et scaling horizontal | [`performance.md`](performance.md) |
| Cache, CDN et edge | [`caching.md`](caching.md) |
| Exercices et livrables | [`exercises.md`](exercises.md) |

### Concepts couverts

- Load balancing
- Caching (Redis, CDN)
- Partitioning / sharding
- Traitement asynchrone (queues, event streaming)
- Capacity planning

---

## Parcours recommandé

```text
1. Lire performance.md (sections 1 à 8)
2. Lire caching.md (sections 1 à 6)
3. Exercices 1 à 4 (estimations, bottlenecks)
4. Atelier — design système type (e-commerce ou streaming)
5. Rédaction plan de montée en charge + design review
```

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Architecture scalable | `scalable-architecture.md` + diagramme |
| Plan de montée en charge | `capacity-plan.md` (phases, chiffres, seuils) |

Les détails sont dans [`exercises.md`](exercises.md).

---

## Prérequis

- Modules 1 à 3 (latence/throughput, cache, SQL, patterns distribués)
- Capacité à estimer req/s et volumes de stockage

---

## Ressources

- [Ressources module 4](../../docs/ressources.md#module-4--scalabilité--performance)
- Kleppmann — *Designing Data-Intensive Applications*, ch. 6, 11

---

## Modules adjacents

- Précédent : [Module 3 — Data & persistance](../03-data/README.md)
- Suivant : [Module 5 — Cloud Azure](../05-cloud-azure/README.md)
