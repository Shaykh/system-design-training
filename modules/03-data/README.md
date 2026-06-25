# Module 3 — Data & persistance

**Durée :** 1 semaine · **Référence planning :** [Semaine 3](../../docs/planning.md#semaine-3--data--persistance)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Choisir entre SQL et NoSQL selon le cas d'usage
- Distinguer OLTP et OLAP et architecturer en conséquence
- Appliquer le pattern database per service dans un contexte microservices
- Gérer la cohérence distribuée (Saga, transactions)
- Traiter la concurrence (optimiste, pessimiste)
- Concevoir une stratégie de cache et d'invalidation
- Produire une architecture data documentée avec choix technologiques justifiés

---

## Contenu

| Thème | Fichier |
| ----- | ------- |
| Conception des bases de données | [`database-design.md`](database-design.md) |
| Patterns data distribués | [`patterns.md`](patterns.md) |
| Ateliers et livrables | [`use-cases.md`](use-cases.md) |

### Concepts couverts

- SQL vs NoSQL
- OLTP vs OLAP
- Database per service
- Saga pattern
- Transactions distribuées
- Gestion de la concurrence
- Invalidation de cache

---

## Parcours recommandé

```text
1. Lire database-design.md (sections 1 à 7)
2. Lire patterns.md (sections 1 à 6)
3. Exercices 1 à 3 (choix technologiques, concurrence)
4. Atelier principal — système multi-bases (SQL + Redis + Event store)
5. Rédaction architecture data + design review
```

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Architecture data | `data-architecture.md` |
| Choix technologiques | Tableau justifié par cas d'usage |
| Diagramme | Flux de données entre SQL, cache et event store |

Les détails sont dans [`use-cases.md`](use-cases.md).

---

## Prérequis

- Modules 1 et 2 (ACID/BASE, CQRS, bounded contexts)
- Notions SQL (tables, index, transactions)
- Familiarité avec Redis (clé-valeur, TTL) — optionnel

---

## Ressources

- [Ressources module 3](../../docs/ressources.md#module-3--data--persistance)
- Kleppmann — *Designing Data-Intensive Applications*, ch. 2–3, 7, 9

---

## Modules adjacents

- Précédent : [Module 2 — Architecture applicative](../02-architecture/README.md)
- Suivant : [Module 4 — Scalabilité & performance](../04-scalability/README.md)
