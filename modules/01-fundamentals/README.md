# Module 1 — Fondamentaux du System Design

**Durée :** 1 semaine · **Référence planning :** [Semaine 1](../../docs/planning.md#semaine-1--fondamentaux)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Expliquer les compromis fondamentaux du design distribué
- Estimer un ordre de grandeur de charge (req/s, stockage)
- Choisir entre scalabilité verticale et horizontale selon le contexte
- Positionner un système sur les spectres CAP et ACID/BASE
- Argumenter monolithe vs microservices
- Concevoir une API REST simple et produire un diagramme d'architecture

---

## Contenu

| Thème | Fichier |
| -----  | -------  |
| Théorie complète       | [`theory.md`](theory.md)       |
| Exercices et livrables | [`exercises.md`](exercises.md) |

### Concepts couverts

- Scalabilité (verticale / horizontale)
- Latence vs throughput
- Théorème CAP
- ACID vs BASE
- Monolithe vs microservices
- Haute disponibilité et tolérance aux pannes
- Estimation rapide (ordre de grandeur)

---

## Parcours recommandé

```text
1. Lire theory.md (sections 1 à 9)
2. Exercices 1 à 4 (compréhension)
3. Exercice 5 — fiche synthèse des compromis
4. Exercice 6 — design API REST + diagramme
5. Exercice 7 — design review
```

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Fiche synthèse des compromis | `trade-offs-summary.md` |
| Spécification API REST | `api-design.md` |
| Diagramme d'architecture | Draw.io, PNG ou Mermaid |

Les détails et gabarits sont dans [`exercises.md`](exercises.md).

---

## Ressources

- [Ressources module 1](../../docs/ressources.md#module-1--fondamentaux)
- [Programme complet](../../docs/programme.md)
- Kleppmann — *Designing Data-Intensive Applications*, ch. 1–2

---

## Module suivant

[Module 2 — Architecture applicative avancée](../02-architecture/README.md)
