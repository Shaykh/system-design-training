# Dossier d'Architecture Technique (DAT) — ShopFlow

**Version :** 1.0  
**Auteur :** Équipe architecture ShopFlow  
**Date :** 2025-06-25  
**Classification :** Interne

---

## 1. Contexte

ShopFlow est une plateforme e-commerce B2C ciblant le marché européen. Le MVP doit supporter 50 000 utilisateurs actifs par jour, 3 000 commandes quotidiennes et des pics à 30 commandes par minute, avec un catalogue de 80 000 références.

Le présent document décrit l'architecture cible hébergée sur **Microsoft Azure** (région West Europe), alignée sur le [cahier des charges](requirements.md).

---

## 2. Objectifs

### 2.1 Objectifs métier

- Permettre l'achat en ligne de produits physiques avec expérience fluide
- Garantir la cohérence stock / commande / paiement
- Préparer l'extension UE et le trafic saisonnier (Black Friday)

### 2.2 Objectifs architecturaux

| Objectif | Mesure de succès |
| -------- | ---------------- |
| Scalabilité | Scale horizontal API sans refonte |
| Disponibilité | 99,9 % parcours achat |
| Sécurité | PCI scope réduit, RGPD UE |
| Maintenabilité | Monolithe modulaire (Clean Architecture) |
| Observabilité | SLO monitorés, MTTR < 1 h incidents P1 |
| Time-to-market | MVP 6 mois, déploiements hebdomadaires |

---

## 3. Architecture générale

### 3.1 Style architectural

**Monolithe modulaire** déployé sur App Service, avec traitement asynchrone via Service Bus. Choix motivé par la taille d'équipe et le volume modéré (pas de microservices au MVP).

```diagram
[Front Door + WAF] → [APIM] → [ShopFlow API — .NET 8]
                                    │
                    ┌───────────────┼───────────────┐
                    ▼               ▼               ▼
              [Azure SQL]    [Redis Cache]   [Service Bus]
                    │               │               │
                    │               │         [Functions]
                    ▼               ▼               ▼
              [Blob Storage]   [Search index]  [Email / Index]
```

### 3.2 Bounded contexts (modules)

| Contexte | Responsabilités |
| -------- | --------------- |
| **Catalog** | Produits, catégories, recherche |
| **Cart** | Panier invité et authentifié |
| **Ordering** | Commandes, statuts, stock |
| **Payment** | Intégration Stripe, ledger interne |
| **Identity** | Profils clients (référence Entra) |
| **Notification** | Emails transactionnels |

### 3.3 Flux critique — création commande

1. Client valide le panier → `POST /api/orders`
2. API vérifie stock (optimistic locking), crée commande `PENDING_PAYMENT`
3. Appel Payment Service → Stripe authorize (idempotency key)
4. Succès → commande `CONFIRMED`, message `OrderConfirmed` sur Service Bus
5. Worker : email confirmation, mise à jour index recherche stock
6. À l'expédition : capture Stripe, statut `SHIPPED`

---

## 4. Choix techniques

| Composant | Technologie | Alternative écartée | Justification |
| --------- | ----------- | ------------------- | ------------- |
| API | ASP.NET Core 8, App Service P1v3 | AKS | Simplicité ops, équipe .NET |
| Base transactionnelle | Azure SQL (S3) | PostgreSQL Flexible | Écosystème .NET, geo-replica |
| Cache | Azure Cache Redis C1 | Cache in-memory seul | Partagé entre instances |
| Recherche | Azure AI Search (Basic) | Elasticsearch self-hosted | Managé, intégration Azure |
| Files | Azure Service Bus Standard | RabbitMQ | Intégration native, DLQ |
| Workers | Azure Functions Premium | WebJobs | Scale événementiel |
| Frontend | React SPA (Static Web Apps) | Blazor WASM | Compétences équipe front |
| Paiement | Stripe | Adyen | Time-to-market, docs |
| Auth clients | Entra External ID | Auth maison | Standards OIDC |
| Auth admins | Entra ID | — | SSO corporate |
| Gateway | API Management Standard | Ingress seul | Rate limit partenaires |
| CDN / WAF | Azure Front Door | App Gateway seul | Global, WAF edge |
| Secrets | Key Vault | Config App Service | Rotation, audit |
| Monitoring | Application Insights | Datadog | Coût, intégration Azure |
| IaC | Bicep | Terraform | Équipe Azure-first |

### 4.1 ADR-001 — Monolithe modulaire vs microservices

- **Décision :** monolithe modulaire avec frontières par namespace/assembly
- **Contexte :** 8 devs, 3k commandes/jour, délai MVP
- **Conséquences :** déploiement unique ; extraction future du contexte Payment si besoin

### 4.2 ADR-002 — Authorize / Capture paiement

- **Décision :** autorisation à la commande, capture à l'expédition
- **Contexte :** stock réservé mais pas expédié immédiatement
- **Conséquences :** gestion des autorisations expirées (job annulation)

---

## 5. Diagrammes

| Diagramme | Fichier |
| --------- | ------- |
| C4 Context | [architecture/README.md#niveau-1--context](../architecture/README.md#niveau-1--context) |
| C4 Container | [architecture/README.md#niveau-2--container](../architecture/README.md#niveau-2--container) |
| C4 Component | [architecture/README.md#niveau-3--component](../architecture/README.md#niveau-3--component) |
| Séquence commande | [architecture/README.md#séquence--création-de-commande](../architecture/README.md#séquence--création-de-commande) |
| Déploiement Azure | [architecture/README.md#déploiement](../architecture/README.md#déploiement) |

---

## 6. Sécurité

Synthèse — détail dans [security/security-model.md](../security/security-model.md).

| Domaine | Mesure |
| ------- | ------ |
| Authentification | OIDC Entra External ID (clients), Entra ID (admins) |
| Autorisation | RBAC app roles + ownership commandes |
| Réseau | Private Endpoints SQL, Redis ; APIM frontal |
| Données | Chiffrement repos ; pas de PAN |
| Secrets | Key Vault + Managed Identity |
| WAF | Front Door OWASP 3.2 |

---

## 7. Résilience

Synthèse — détail dans [monitoring/strategy.md](../monitoring/strategy.md).

| Dépendance | Timeout | Retry | Circuit breaker | Fallback |
| ---------- | ------- | ----- | --------------- | -------- |
| Azure SQL | 5 s | 2 transient | Non | Erreur 503 |
| Redis | 1 s | 1 | Oui (5/30s) | Bypass → SQL |
| Stripe API | 10 s | 0 | Oui | Erreur paiement explicite |
| Service Bus | — | Natif | DLQ | — |
| Azure AI Search | 3 s | 2 | Oui | Catalogue SQL simplifié |

**PRA :** SQL geo-replication North Europe, failover Front Door, runbook [cost/azure-cost-estimate.md#pra](../cost/azure-cost-estimate.md).

---

## 8. Coûts

Estimation détaillée : [cost/azure-cost-estimate.md](../cost/azure-cost-estimate.md).

| Environnement | Coût mensuel indicatif |
| ------------- | ---------------------- |
| Production | ~2 800 € |
| Staging | ~600 € |
| Dev | ~300 € |
| **Total** | **~3 700 €** (marge pics incluse) |

---

## 9. Risques

| ID | Risque | Probabilité | Impact | Mitigation |
| -- | ------ | ----------- | ------ | ---------- |
| R1 | Pic Black Friday non anticipé | Moyenne | Élevé | Load test, auto-scale, pré-warm cache |
| R2 | Indisponibilité Stripe | Faible | Élevé | Message client, retry, monitoring PSP |
| R3 | Fuite données (OWASP) | Faible | Critique | WAF, pentest, BOLA checks |
| R4 | Dépassement budget logs | Moyenne | Moyen | Sampling, quotas Log Analytics |
| R5 | Dette monolithe | Moyenne | Moyen | Modules isolés, ADR extraction |
| R6 | Erreur stock (survente) | Moyenne | Élevé | Optimistic locking, tests charge |

---

## 10. Historique des versions

| Version | Date | Auteur | Modifications |
| ------- | ---- | ------ | ------------- |
| 1.0 | 2025-06-25 | Architecture | Version initiale référence formation |

---

## 11. Approbations

| Rôle | Nom | Date | Signature |
| ---- | --- | ---- | --------- |
| Architecte | | | |
| Tech Lead | | | |
| RSSI | | | |
| Product Owner | | | |
