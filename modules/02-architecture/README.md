# Module 2 — Architecture applicative avancée

**Durée :** 1 semaine · **Référence planning :** [Semaine 2](../../docs/planning.md#semaine-2--architecture-applicative)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Structurer une application selon Clean Architecture, hexagonale ou onion
- Identifier les bounded contexts et modéliser un domaine (DDD tactique)
- Séparer lectures et écritures avec CQRS
- Comprendre les bases de l'event sourcing et quand l'envisager
- Évaluer couplage et cohésion d'un codebase existant
- Produire des diagrammes Component et Sequence ainsi qu'une documentation d'architecture

---

## Contenu

| Thème | Fichier |
| -----  | -------  |
| Patterns et théorie | [`patterns.md`](patterns.md) |
| Ateliers et livrables | [`workshop.md`](workshop.md) |

### Concepts couverts

- Clean Architecture
- Architecture hexagonale / Onion
- Domain Driven Design (stratégique et tactique)
- CQRS
- Event sourcing (introduction)
- Couplage et cohésion

---

## Parcours recommandé

```text
1. Lire patterns.md (sections 1 à 8)
2. Atelier 1 — Analyse de couplage sur un code legacy
3. Atelier 2 — Refactoring vers Clean Architecture (.NET)
4. Atelier 3 — Implémentation CQRS avec MediatR
5. Atelier 4 — Diagrammes Component + Sequence
6. Design review + documentation d'architecture
```

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Documentation d'architecture | `architecture-doc.md` |
| Diagramme Component | C4 niveau Component ou équivalent |
| Diagramme Sequence | Flux commande critique (ex. création commande) |
| Code atelier (optionnel) | Projet [`OrderApp/`](OrderApp/) |

Les détails et gabarits sont dans [`workshop.md`](workshop.md).

### Squelette .NET

Le dossier [`OrderApp/`](OrderApp/) contient une solution Clean Architecture prête à l'emploi (MediatR, CQRS, EF InMemory). Utilisez-la pour les ateliers 2 à 4.

```bash
cd modules/02-architecture/OrderApp
dotnet build && dotnet test
dotnet run --project OrderApp.Api
```

---

## Prérequis

- Module 1 complété (notions de monolithe, compromis techniques)
- Confort avec C# et ASP.NET Core (API minimal ou MVC)
- Notions de dépendances NuGet et injection de dépendances

---

## Ressources

- [Ressources module 2](../../docs/ressources.md#module-2--architecture-applicative)
- [Programme complet](../../docs/programme.md)
- Robert C. Martin — *Clean Architecture*
- Eric Evans — *Domain-Driven Design*

---

## Modules adjacents

- Précédent : [Module 1 — Fondamentaux](../01-fundamentals/README.md)
- Suivant : [Module 3 — Data & persistance](../03-data/README.md)
