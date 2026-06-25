# Module 6 — Résilience & observabilité

**Durée :** 1 semaine · **Référence planning :** [Semaine 6](../../docs/planning.md#semaine-6--résilience--observabilité)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Appliquer les patterns de résilience (retry, circuit breaker, bulkhead, timeout)
- Implémenter des politiques Polly dans une application .NET
- Instrumenter une application avec Application Insights et OpenTelemetry
- Concevoir un logging structuré et une corrélation distribuée (trace ID)
- Définir SLI/SLO et des règles d'alerting pertinentes
- Produire un dashboard de supervision et une stratégie de résilience documentée

---

## Contenu

| Thème | Fichier |
| ----- | ------- |
| Patterns de résilience | [`patterns.md`](patterns.md) |
| Monitoring, logs et alerting | [`monitoring.md`](monitoring.md) |

### Concepts couverts

- Retry / circuit breaker (Polly)
- Azure Monitor / Application Insights
- Logging distribué
- Alerting et SLO
- Dashboards de supervision

---

## Parcours recommandé

```text
1. Lire patterns.md (résilience et Polly)
2. Lire monitoring.md (observabilité Azure)
3. Atelier — instrumentation d'une API .NET
4. Concevoir dashboard + règles d'alerte
5. Rédiger la stratégie de résilience (par dépendance)
```

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Stratégie de résilience | `resilience-strategy.md` (politiques par dépendance) |
| Dashboard de supervision | Capture ou export Azure Dashboard / workbook |
| Instrumentation (optionnel) | Code PoC avec Polly + App Insights |

Les détails sont dans [`monitoring.md`](monitoring.md#atelier).

---

## Prérequis

- Modules 1 à 5 (patterns distribués, Azure)
- Projet .NET / ASP.NET Core pour l'atelier pratique
- Abonnement Azure avec Application Insights (gratuit jusqu'à un certain quota)

---

## Ressources

- [Polly documentation](https://www.pollydocs.org/)
- [Application Insights](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Ressources module 6](../../docs/ressources.md#module-6--résilience--observabilité)

---

## Modules adjacents

- Précédent : [Module 5 — Cloud Azure](../05-cloud-azure/README.md)
- Suivant : [Module 7 — Sécurité](../07-security/README.md)
