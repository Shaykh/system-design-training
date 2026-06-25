# Stratégie monitoring & résilience — ShopFlow

---

## 1. Objectifs SLO

| SLI | SLO (30 jours) | Error budget |
| --- | -------------- | ------------ |
| Disponibilité API parcours achat | 99,9 % | ~43 min downtime |
| Latence catalogue p95 | < 200 ms | — |
| Latence checkout p95 | < 500 ms | — |
| Taux erreur 5xx | < 0,1 % | — |

---

## 2. Stack observabilité

| Composant | Service |
| --------- | ------- |
| APM / traces | Application Insights |
| Logs | App Insights + Log Analytics |
| Métriques infra | Azure Monitor |
| Dashboards | Azure Workbook + Dashboard |
| Alertes | Azure Monitor Alerts → Teams / PagerDuty |
| Synthetic | Application Insights Availability tests |

---

## 3. Dashboard de supervision

### Vue exécutive (tuiles)

| Tuile | Source KQL / métrique |
| ----- | -------------------- |
| Req/s | `requests \| summarize count() by bin(timestamp, 1m)` |
| Disponibilité 24 h | `availabilityResults` |
| p95 latence globale | `requests \| percentile(duration, 95)` |
| Taux 5xx | `requests \| where resultCode startswith "5"` |
| Commandes / h | Custom metric `orders.created` |

### Vue technique

| Tuile | Contenu |
| ----- | ------- |
| Dépendances lentes | SQL, Redis, Stripe p95 |
| Exceptions top 10 | `exceptions \| summarize count() by problemId` |
| Service Bus | Active messages, DLQ depth |
| Redis | Hit ratio (custom), connected clients |
| App Service | CPU, memory, instances count |

### Exemple requête — erreurs paiement

```kql
exceptions
| where timestamp > ago(1h)
| where cloud_RoleName == "ShopFlow.Api"
| where outerMessage contains "Stripe" or outerMessage contains "Payment"
| project timestamp, outerMessage, operation_Id
| order by timestamp desc
```

---

## 4. Alertes

| Nom | Condition | Sévérité | Action |
| --- | --------- | -------- | ------ |
| API-Availability-Critical | Availability < 99 % sur 5 min | P1 | PagerDuty |
| API-Latency-Warning | p95 > 500 ms sur 15 min | P2 | Teams |
| API-5xx-Critical | Error rate > 1 % sur 5 min | P1 | PagerDuty |
| SQL-DTU-High | DTU > 85 % sur 10 min | P2 | Teams + scale review |
| ServiceBus-DLQ | DLQ messages > 50 | P2 | Teams |
| Stripe-Errors | Payment failures > 20 / 10 min | P1 | Teams + runbook |

Chaque alerte référence un **runbook** (voir module 6).

---

## 5. Stratégie de résilience par dépendance

| Dépendance | Timeout | Retry | Circuit breaker | Fallback |
| ---------- | ------- | ----- | --------------- | -------- |
| Azure SQL | 5 s | 2 (transient) | Non | 503 + retry client |
| Redis | 1 s | 1 | 5 failures / 30 s | Lecture SQL directe |
| Stripe | 10 s | 0 | 5 / 60 s | Message erreur paiement |
| Azure AI Search | 3 s | 2 | 5 / 30 s | Recherche SQL LIKE limitée |
| Service Bus publish | 5 s | 3 | — | Outbox table + worker |
| SendGrid / Email | 30 s | 3 async | — | DLQ + retry |

### Polly (API)

Enregistrer les policies HttpClient pour `IStripeClient` et `ISearchClient` — voir [module 6](../../modules/06-resilience/patterns.md).

---

## 6. Health checks

| Endpoint | Checks | Usage |
| -------- | ------ | ----- |
| `/health/live` | Processus | Liveness K8s / App Service |
| `/health/ready` | SQL, Redis, Service Bus | Load balancer, swap slot |

Le pipeline CI appelle `/health/ready` après chaque déploiement.

---

## 7. Logging

### Format standard

```json
{
  "timestamp": "2025-06-25T14:00:00Z",
  "level": "Information",
  "message": "Order created",
  "orderId": "ord-123",
  "customerId": "cust-456",
  "traceId": "abc...",
  "service": "ShopFlow.Api"
}
```

### Règles

- Pas de PAN, CVV, tokens Stripe complets
- `customerId` autorisé ; email masqué en INFO, complet en DEBUG désactivé prod
- Rétention Log Analytics : 90 jours

---

## 8. Tests de résilience

| Scénario | Fréquence | Dernier test |
| -------- | --------- | ------------ |
| Failover slot staging | Chaque release | — |
| SQL failover drill (staging) | Trimestriel | — |
| Charge pic (k6 2× traffic) | Avant Black Friday | — |
| Stripe webhook indisponible | Semestriel | — |

---

## 9. Livrable dashboard

**Action formation :** créer le Workbook Azure à partir de ce spec et joindre une capture dans `dashboard-screenshot.png`.

---

## Références

- [DAT](../dat/DAT.md)
- [Module 6](../../modules/06-resilience/monitoring.md)
