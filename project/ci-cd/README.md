# Pipeline CI/CD — ShopFlow

Pipeline **GitHub Actions** de référence pour l'API .NET et le déploiement infrastructure Bicep.

Adaptable à Azure DevOps (équivalent : stages Build → Deploy Staging → Deploy Production).

---

## Fichier principal

Voir [`pipeline.yml`](pipeline.yml).

---

## Vue d'ensemble

```diagram
┌─────────┐    ┌─────────┐    ┌──────────────┐    ┌─────────────┐
│  Push   │───►│  Build  │───►│   Staging    │───►│ Production  │
│ main/PR │    │ Test    │    │  (auto)      │    │ (approval)  │
└─────────┘    └─────────┘    └──────────────┘    └─────────────┘
```

| Stage | Déclencheur | Actions |
| ----- | ----------- | ------- |
| **Build** | PR + push `main` | Restore, build, unit tests, SAST |
| **Package** | Build OK | Publish API, upload artifact |
| **Deploy Staging** | Push `main` | Deploy App Service staging slot, smoke tests |
| **Deploy Production** | Manuel / approval | Swap slot ou deploy prod, health check |

---

## Prérequis Azure

| Élément | Description |
| ------- | ----------- |
| Service Principal ou OIDC | Federated credential GitHub → Azure |
| Resource groups | `rg-shopflow-dev`, `-stg`, `-prod` |
| App Service | Slots `staging` et `production` |
| Key Vault | Secrets référencés au runtime (pas dans le pipeline) |

### Configuration OIDC (recommandé)

```bash
# Créer identity fédérée GitHub — voir documentation Microsoft
# Secrets GitHub :
#   AZURE_CLIENT_ID
#   AZURE_TENANT_ID
#   AZURE_SUBSCRIPTION_ID
```

---

## Qualité et sécurité dans le pipeline

| Étape | Outil |
| ----- | ----- |
| Unit tests | `dotnet test` |
| Couverture | Coverlet (seuil 70 % domaine) |
| Scan secrets | Gitleaks / GitHub secret scanning |
| Scan dépendances | `dotnet list package --vulnerable` |
| Analyse statique | SonarCloud (optionnel) |
| Scan image | Defender for Containers (si Docker) |

---

## Stratégie de déploiement

| Environnement | Stratégie |
| ------------- | --------- |
| Dev | Déploiement direct branche `develop` |
| Staging | Slot staging, tests E2E automatisés |
| Production | **Blue/green** via swap slot après validation staging |

### Rollback

```bash
# Swap inverse si régression détectée (< 5 min)
az webapp deployment slot swap --slot staging --target-slot production --reverse
```

---

## Smoke tests post-déploiement

```bash
curl -f https://api-staging.shopflow.com/health/ready
curl -f https://api-staging.shopflow.com/api/v1/products?page=1&size=1
```

---

## Variables par environnement

| Variable | Dev | Staging | Prod |
| -------- | --- | ------- | ---- |
| `ASPNETCORE_ENVIRONMENT` | Development | Staging | Production |
| App Insights | dev-insights | stg-insights | prod-insights |
| SQL | Basic | S2 | S3 + replica |

---

## Références

- [Monitoring](../monitoring/strategy.md)
- [DAT — résilience](../dat/DAT.md#7-résilience)
