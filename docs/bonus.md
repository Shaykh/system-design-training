# Modules bonus — Niveau avancé

Contenus à aborder **après le projet final** ou en parallèle si votre rythme le permet. Ils ne sont pas requis pour valider la formation de base.

---

## 1. Architecture multi-tenant SaaS

### Concepts

- Modèles d'isolation : base par tenant, schéma par tenant, `tenant_id` + RLS
- Elastic pool Azure SQL pour densité
- Configuration par tenant (feature flags, branding)
- Onboarding tenant automatisé (Bicep / Terraform par client enterprise)

### Ressources

- [Module 5 — atelier SaaS facturation](../modules/05-cloud-azure/architecture.md#8-atelier--architecture-cloud-complète-4-5-h)
- [Module 7 — multi-tenant](../modules/07-security/auth.md#multi-tenant-saas)
- Microsoft — [Multitenancy solutions](https://learn.microsoft.com/azure/architecture/guide/multitenant/overview)

### Exercice

Concevoir l'isolation data pour 1 000 tenants B2B avec 10 utilisateurs / tenant en moyenne. Comparer les 3 modèles (coût, sécurité, ops).

---

## 2. FinOps

### Concepts

- Tags obligatoires (`env`, `project`, `cost-center`)
- Budgets et alertes Azure Cost Management
- Reserved Instances / Savings Plans
- Unit economics (coût par commande, par utilisateur)
- Éteindre les environnements non-prod hors heures ouvrées

### Ressources

- [Chiffrage ShopFlow](../project/cost/azure-cost-estimate.md)
- [Module 5 — FinOps](../modules/05-cloud-azure/services.md#11-estimation-de-coûts--principes)
- [FinOps Foundation](https://www.finops.org/)

### Exercice

Produire un tableau « coût par transaction » pour ShopFlow à 3 000 et 30 000 commandes / jour.

---

## 3. Event-driven avancé

### Concepts

- Event choreography vs orchestration à l'échelle
- Event sourcing complet (pas seulement domain events)
- CQRS avec projections multiples
- Outbox + CDC (Debezium)
- Schema registry (Avro / Protobuf)

### Ressources

- [Module 2 — event sourcing](../modules/02-architecture/patterns.md#8-event-sourcing-introduction)
- [Module 3 — outbox, saga](../modules/03-data/patterns.md)
- [Module 4 — Kafka](../modules/04-scalability/performance.md#8-event-streaming-à-grande-échelle)

### Exercice

Repenser le flux commande ShopFlow en event sourcing pur : quels agrégats, quels events, quelles projections ?

---

## 4. Data streaming (Kafka)

### Concepts

- Topics, partitions, consumer groups
- Exactly-once semantics (idempotence + transactions Kafka)
- Kafka Connect, stream processing (Flink, Spark Streaming)
- Azure Event Hubs comme alternative managée

### Ressources

- [Kafka documentation](https://kafka.apache.org/documentation/)
- [Module 8 — logs distribués](../modules/08-system-design-cases/logging.md)

### Exercice

Remplacer Service Bus par Event Hubs dans ShopFlow : impact architecture, ordre des messages, coût.

---

## 5. Migration monolithe → microservices

### Concepts

- Strangler fig pattern
- Découpage par bounded context (DDD)
- Anti-pattern « distributed monolith »
- Database decomposition
- Tests de contrat (Pact)

### Ressources

- [Module 1 — monolithe vs microservices](../modules/01-fundamentals/theory.md#6-monolithe-vs-microservices)
- [Module 2 — bounded contexts](../modules/02-architecture/patterns.md#6-domain-driven-design-ddd)
- Microsoft — [Microservices migration](https://learn.microsoft.com/azure/architecture/guide/migration/microservices)

### Exercice

Plan de migration en 4 phases pour extraire le contexte **Payment** de ShopFlow monolithe.

---

## Retour au parcours principal

- [Programme](programme.md)
- [Projet final](../project/README.md)
