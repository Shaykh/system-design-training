# Module 5 — Architecture Cloud (Azure)

**Durée :** 1 semaine · **Référence planning :** [Semaine 5](../../docs/planning.md#semaine-5--cloud-azure)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Mapper une architecture logique sur des services Azure concrets
- Choisir entre App Service, AKS, Functions et containers selon le contexte
- Intégrer messaging (Service Bus, Event Grid) et API Management
- Concevoir une architecture multi-région avec plan de reprise (PRA/PCA)
- Produire des diagrammes C4 (Context, Container) alignés Azure
- Estimer un chiffrage mensuel avec le Pricing Calculator

---

## Contenu

| Thème | Fichier |
| ----- | ------- |
| Catalogue de services Azure | [`services.md`](services.md) |
| Patterns, atelier et livrables | [`architecture.md`](architecture.md) |

### Concepts couverts

- Azure Architecture Patterns
- App Service / AKS / Azure Functions
- Service Bus / Event Grid
- API Management
- Multi-région / Disaster Recovery
- Well-Architected Framework

---

## Parcours recommandé

```text
1. Lire services.md (catalogue et grilles de choix)
2. Lire architecture.md (patterns, PRA/PCA, Well-Architected)
3. Atelier — design cloud complet d'un système e-commerce ou SaaS
4. Produire diagrammes C4 Context + Container
5. Chiffrage Azure + plan PRA/PCA
```

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Diagrammes C4 | Context + Container (Draw.io ou Mermaid) |
| Mapping Azure | Tableau composant logique → service Azure |
| Chiffrage | `azure-cost-estimate.md` ou feuille dans `project/cost/` |
| Plan PRA / PCA | `dr-plan.md` (RTO, RPO, procédures) |

Les détails sont dans [`architecture.md`](architecture.md).

---

## Prérequis

- Modules 1 à 4 (architecture, data, scalabilité)
- Compte [Azure gratuit](https://azure.microsoft.com/free/) recommandé pour explorer le portail
- Notions de réseau cloud (VNet, subnet, NSG) — introduites dans ce module

---

## Ressources

- [Azure Architecture Center](https://learn.microsoft.com/azure/architecture/)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
- [Template C4](../../templates/c4-model-template.md)
- [Ressources module 5](../../docs/ressources.md#module-5--architecture-cloud-azure)

---

## Modules adjacents

- Précédent : [Module 4 — Scalabilité](../04-scalability/README.md)
- Suivant : [Module 6 — Résilience](../06-resilience/README.md)
