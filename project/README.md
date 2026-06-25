# Projet final — Plateforme e-commerce ShopFlow

Projet capstone de la formation **System Design & Architecture**. Vous appliquez l'ensemble des modules (fondamentaux → cas réels) pour concevoir un système de **niveau entreprise**, prêt à être présenté en revue d'architecture.

**Durée estimée :** 2 à 3 semaines (semaines 9–10 du [planning](../docs/planning.md)).

---

## Sujet retenu (référence)

**ShopFlow** — plateforme e-commerce B2C multi-canal (web + mobile) avec catalogue, panier, commande, paiement et back-office admin.

Les documents de ce dossier constituent la **solution de référence**. En formation, vous pouvez :

- Suivre le même sujet et adapter les choix
- Choisir une variante ([CRM](#variantes-possibles), [API multi-tenant](#variantes-possibles))
- Utiliser ce repo comme gabarit et remplacer le contenu par votre propre design

---

## Livrables attendus

| Livrable | Emplacement | Statut référence |
| -------- | ----------- | ---------------- |
| Cahier des charges | [`requirements.md`](requirements.md) | ✅ |
| Dossier d'architecture (DAT) | [`dat/DAT.md`](dat/DAT.md) | ✅ |
| Diagrammes C4 | [`architecture/README.md`](architecture/README.md) | ✅ |
| Pipeline CI/CD | [`ci-cd/pipeline.yml`](ci-cd/pipeline.yml) | ✅ |
| Chiffrage Azure | [`cost/azure-cost-estimate.md`](cost/azure-cost-estimate.md) | ✅ |
| Monitoring & résilience | [`monitoring/strategy.md`](monitoring/strategy.md) | ✅ |
| Sécurité (synthèse) | [`security/security-model.md`](security/security-model.md) | ✅ |

Les détails et gabarits sont dans [`architecture/README.md`](architecture/README.md) et [`DRAWIO.md`](architecture/DRAWIO.md).

---

## Parcours recommandé

```text
1. Lire requirements.md — périmètre et NFR
2. Estimer charge (module 1) — valider ou ajuster les hypothèses
3. Produire diagrammes C4 Context → Container → Component
4. Rédiger / compléter le DAT
5. Mapper services Azure (module 5) + chiffrage
6. Définir CI/CD, monitoring, sécurité (modules 6–7)
7. Design review avec la checklist templates/
8. Soutenance 30–45 min
```

---

## Critères d'évaluation

| Critère | Poids |
| ------- | ----- |
| Cohérence fonctionnelle / technique | 25 % |
| Diagrammes et DAT exploitables | 25 % |
| Justification des trade-offs | 20 % |
| Azure, coûts, PRA réalistes | 15 % |
| CI/CD, observabilité, sécurité | 15 % |

---

## Variantes possibles

| Sujet | Adaptation |
| ----- | ---------- |
| **CRM B2B** | Remplacer catalogue/commande par leads, opportunités, pipeline |
| **API multi-tenant SaaS** | Accent sur isolation tenant, elastic pool, Entra B2B |
| **Marketplace** | Ajouter vendeurs tiers, split paiement, modération |

Conserver la même structure de dossiers et livrables.

---

## Documents et templates

- [Template DAT](../templates/dat-template.md)
- [Template C4](../templates/c4-model-template.md)
- [Checklist design review](../templates/design-review-checklist.md)
- [Programme formation](../docs/programme.md)

---

## Soutenance (guide)

1. **Contexte** (5 min) — besoin métier, contraintes
2. **Architecture** (15 min) — diagrammes, flux commande
3. **Deep dive** (10 min) — 2 composants au choix (ex. paiement, cache)
4. **Ops** (5 min) — CI/CD, monitoring, DR
5. **Trade-offs & évolutions** (5 min)
6. **Questions** (10 min)
