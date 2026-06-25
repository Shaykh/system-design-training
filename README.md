# System Design & Architecture Training

Formation complète pour concevoir des architectures **modernes, scalables et résilientes** sur Azure (.NET).

**Statut du dépôt :** contenu pédagogique complet — 8 modules, projet final de référence, documentation et templates.

---

## Objectif

Acquérir les compétences pour :

- Concevoir des systèmes distribués de bout en bout
- Justifier les trade-offs techniques et estimer la charge
- Produire des livrables entreprise (DAT, C4, chiffrage, PRA)
- Sécuriser et opérer en production (CI/CD, monitoring, Zero Trust)

---

## Démarrage rapide

| Étape | Action |
| ----- | ------ |
| 1 | Lire [docs/README.md](docs/README.md) et [docs/programme.md](docs/programme.md) |
| 2 | Planifier avec [docs/planning.md](docs/planning.md) (10 semaines) |
| 3 | Commencer [Module 1 — Fondamentaux](modules/01-fundamentals/README.md) |
| 4 | Enchaîner les modules 2 à 8 |
| 5 | Réaliser le [projet final ShopFlow](project/README.md) |
| 6 | Optionnel : [modules bonus](docs/bonus.md) |

---

## Structure du dépôt

```text
system-design-training/
├── docs/                 # Programme, planning, ressources, bonus
├── modules/
│   ├── 01-fundamentals/
│   ├── 02-architecture/
│   ├── 03-data/
│   ├── 04-scalability/
│   ├── 05-cloud-azure/
│   ├── 06-resilience/
│   ├── 07-security/
│   └── 08-system-design-cases/
├── project/              # Projet capstone ShopFlow (référence)
├── templates/            # DAT, C4, checklist review
└── summary.md            # Synthèse du programme
```

---

## Modules

| # | Module | Durée |
| - | ------ | ----- |
| 1 | [Fondamentaux](modules/01-fundamentals/README.md) | 1 sem. |
| 2 | [Architecture applicative](modules/02-architecture/README.md) | 1 sem. |
| 3 | [Data & persistance](modules/03-data/README.md) | 1 sem. |
| 4 | [Scalabilité & performance](modules/04-scalability/README.md) | 1 sem. |
| 5 | [Cloud Azure](modules/05-cloud-azure/README.md) | 1 sem. |
| 6 | [Résilience & observabilité](modules/06-resilience/README.md) | 1 sem. |
| 7 | [Sécurité & gouvernance](modules/07-security/README.md) | 1 sem. |
| 8 | [Cas réels](modules/08-system-design-cases/README.md) | 1 sem. |
| — | [Projet final](project/README.md) | 2 sem. |

---

## Projet final — ShopFlow

Plateforme e-commerce B2C sur Azure : [cahier des charges](project/requirements.md), [DAT](project/dat/DAT.md), [architecture C4](project/architecture/README.md), [CI/CD](project/ci-cd/pipeline.yml), [chiffrage](project/cost/azure-cost-estimate.md).

---

## Templates

- [DAT](templates/dat-template.md)
- [C4](templates/c4-model-template.md)
- [Design review](templates/design-review-checklist.md)

---

## Public visé

Développeur senior, tech lead ou architecte en devenir — confort C# / .NET recommandé.

---

## Licence et contribution

Matériel de formation interne. Adapter librement pour votre organisation.
