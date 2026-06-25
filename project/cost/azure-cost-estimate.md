# Estimation coûts Azure — ShopFlow

**Région :** West Europe  
**Période :** mensuel  
**Date :** 2025-06-25  
**Devise :** EUR (estimation — vérifier avec [Pricing Calculator](https://azure.microsoft.com/pricing/calculator/))

---

## Hypothèses de charge

| Métrique | Valeur |
| -------- | ------ |
| DAU | 50 000 |
| Pic utilisateurs simultanés | 5 000 |
| Req API / s (pic) | ~800 |
| Commandes / jour | 3 000 |
| Stockage SQL | ~200 Go an 1 |
| Stockage Blob images | ~500 Go |
| Logs App Insights | ~30 Go / mois |

---

## Production

| Ressource | SKU / config | Qté | Coût unitaire (€) | Coût mensuel (€) |
| --------- | ------------ | --- | ----------------- | ---------------- |
| App Service API | P1v3 Linux | 2 | 140 | 280 |
| Static Web Apps | Standard | 1 | 80 | 80 |
| Azure SQL | S3 (100 DTU) | 1 | 450 | 450 |
| SQL geo-replica | S3 secondary | 1 | 450 | 450 |
| Redis Cache | C1 Standard | 1 | 120 | 120 |
| Azure AI Search | Basic | 1 | 220 | 220 |
| Service Bus | Standard | 1 namespace | 10 | 10 |
| Functions Premium | EP1 | 1 | 140 | 140 |
| API Management | Standard 1 unit | 1 | 600 | 600 |
| Front Door Premium | Base + WAF | 1 | 280 | 280 |
| Blob Storage | 500 Go Hot GRS | — | 0,02/Go | 10 |
| Key Vault | Standard ops | — | — | 5 |
| Application Insights | 30 Go ingestion | — | 2,5/Go | 75 |
| Bande passante sortante | ~2 To | — | 0,08/Go | 160 |
| **Sous-total Production** | | | | **~2 880** |

---

## Non-production

| Environnement | Ressources principales | Coût mensuel (€) |
| ------------- | ---------------------- | ---------------- |
| Staging | App P1v3 × 1, SQL S2, Redis C0, pas de replica | ~600 |
| Dev | App B2, SQL Basic, pas de Front Door/APIM dédié | ~300 |

**Total tous environnements : ~3 780 € / mois**

---

## Marge et optimisations

| Action | Économie estimée |
| ------ | ---------------- |
| Reserved Instances App Service (1 an) | −20 à 30 % compute |
| Arrêt dev/staging nuits et week-ends | −40 % env non-prod |
| Tier Cool Blob images peu consultées | −30 % stockage |
| Sampling logs DEBUG | −20 % App Insights |
| Budget alerte à 4 000 € | — |

**Budget recommandé avec marge pics (Black Friday) : 4 500 € / mois**

---

## PRA — coût de la reprise

| Composant DR | Configuration | Coût additionnel |
| ------------ | ------------- | ---------------- |
| SQL geo-replica North Europe | Lecture secondaire | Inclus ci-dessus |
| App Service standby | 1 instance passive (scale 0→1 failover) | ~140 € (activable à la demande) |
| Front Door | Failover priority routing | Inclus |
| RTO 4 h / RPO 1 h | Runbook manuel + scripts Bicep | Coût ops, pas infra fixe majeure |

### Runbook failover (résumé)

1. Confirmer incident région primaire (Azure Status + monitoring)
2. Promouvoir replica SQL (failover group) — RPO selon réplication
3. Front Door : basculer priorité backend vers North Europe
4. Vérifier health checks `/health/ready`
5. Communiquer statut clients

---

## Évolution des coûts (phases)

| Phase | DAU | Commandes/j | Infra estimée (€/mois) |
| ----- | --- | ----------- | ---------------------- |
| MVP launch | 5 000 | 300 | ~1 200 |
| Croissance | 50 000 | 3 000 | ~3 800 |
| Scale 3× | 150 000 | 10 000 | ~9 000 (+ AKS ?, sharding SQL) |

---

## Fichier Excel

Le fichier `azure-estimation.xlsx` peut être alimenté à partir de ce tableau pour partage avec la finance. Recalculer trimestriellement.

---

## Références

- [DAT](../dat/DAT.md)
- [Module 5 — Chiffrage](../../modules/05-cloud-azure/architecture.md#7-chiffrage-azure)
