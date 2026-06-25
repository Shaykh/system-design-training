# Catalogue de services Azure

Guide de sélection des services Azure pour une architecture applicative .NET moderne. Chaque section indique **quand utiliser**, **alternatives** et **pièges courants**.

---

## 1. Vue d'ensemble par couche

```diagram
                    ┌─────────────────────────────────┐
                    │     Azure Front Door / CDN      │
                    └───────────────┬─────────────────┘
                                    │
                    ┌───────────────▼─────────────────┐
                    │      API Management (APIM)      │
                    └───────────────┬─────────────────┘
                                    │
          ┌─────────────────────────┼─────────────────────────┐
          ▼                         ▼                         ▼
   ┌─────────────┐          ┌─────────────┐          ┌─────────────┐
   │ App Service │          │     AKS     │          │  Functions  │
   │  (Web API)  │          │ (Kubernetes)│          │  (serverless)│
   └──────┬──────┘          └──────┬──────┘          └──────┬──────┘
          │                        │                        │
          └────────────────────────┼────────────────────────┘
                                   │
     ┌─────────────┬───────────────┼───────────────┬─────────────┐
     ▼             ▼               ▼               ▼             ▼
 Azure SQL    PostgreSQL      Redis Cache    Service Bus   Blob Storage
 (Flexible)   (Flexible)       (Azure)        Event Grid    Key Vault
```

---

## 2. Compute — où exécuter le code ?

### Azure App Service

PaaS managé pour applications web et API (.NET, Node, Java, etc.).

| Avantages | Inconvénients |
| --------- | ------------- |
| Déploiement simple (Git, ZIP, slot staging) | Moins de contrôle qu'un VM/AKS |
| Auto-scale intégré | Limites par plan (SKU) |
| Slots blue/green | Multi-conteneurs limité (pas pour tous les cas) |
| TLS, logs, intégration VNet | |

**Choisir quand :**

- API REST / MVC .NET classique
- Équipe petite à moyenne, time-to-market prioritaire
- Charge prévisible à modérée (scale vertical + horizontal App Service Plan)

**Plans courants :**

| Plan | Usage |
| ---- | ----- |
| B1–B3 (Basic) | Dev / test |
| S1–S3 (Standard) | Prod légère, slots staging |
| P1v3–P3v3 (Premium) | Prod, auto-scale, VNet integration |
| Isolated | Conformité, réseau dédié |

### Azure Kubernetes Service (AKS)

Orchestration de conteneurs Kubernetes managée.

| Avantages | Inconvénients |
| --------- | ------------- |
| Portabilité, écosystème K8s | Courbe d'apprentissage |
| Scale fin, service mesh | Opérations (upgrades, monitoring) |
| Microservices, workloads hétérogènes | Coût cluster même à faible charge |

**Choisir quand :**

- Microservices nombreux, équipe DevOps mature
- Besoin de contrôle (sidecars, custom networking)
- Multi-cloud ou conteneurs déjà standard

**Éviter quand :** monolithe simple, équipe sans compétence K8s.

### Azure Functions

Exécution serverless événementielle.

| Avantages | Inconvénients |
| --------- | ------------- |
| Facturation à l'exécution | Cold start (plan Consumption) |
| Intégration native événements Azure | Limites durée exécution |
| Pas de serveur à gérer | Debugging distribué |

**Choisir quand :**

- Traitements courts déclenchés par événements (blob upload, queue, timer)
- Charge intermittente ou imprévisible
- Workers async complémentaires à une API

**Plans :** Consumption (pay-per-use), Premium (pas de cold start), Dedicated (App Service Plan).

### Azure Container Apps

Alternative plus simple qu'AKS pour conteneurs + scale to zero + KEDA.

**Choisir quand :** microservices conteneurisés sans gérer le control plane Kubernetes.

### Grille de décision compute

| Critère | App Service | AKS | Functions | Container Apps |
| ------- | ----------- | --- | --------- | -------------- |
| Simplicité | ★★★★★ | ★★ | ★★★★ | ★★★★ |
| Contrôle | ★★★ | ★★★★★ | ★★ | ★★★★ |
| Microservices | ★★ | ★★★★★ | ★★ | ★★★★ |
| Coût faible charge | ★★★ | ★★ | ★★★★★ | ★★★★ |
| Événements / batch court | ★★ | ★★★ | ★★★★★ | ★★★★ |

---

## 3. Données

### Azure SQL Database / SQL Managed Instance

| Service | Usage |
| ------- | ----- |
| **Azure SQL Database** | OLTP cloud-native, serverless ou provisioned |
| **SQL Managed Instance** | Lift-and-shift SQL Server quasi complet |
| **SQL on VM** | Contrôle total, licensing BYOL |

**Choisir Azure SQL Database pour :** nouvelles apps .NET + EF Core, besoin elastic pool multi-tenant.

### Azure Database for PostgreSQL (Flexible Server)

Alternative open source, compatible module 3 (PostgreSQL).

**Choisir pour :** stack PostgreSQL, extensions, coût, JSONB.

### Azure Cache for Redis

Service Redis managé (cache, session, pub/sub, structures).

| Tier | Usage |
| ---- | ----- |
| Basic | Dev |
| Standard | Prod, replica |
| Premium | Cluster, persistence, VNet |

### Azure Cosmos DB

NoSQL global distribué (document, key-value, graph, column).

**Choisir quand :** latence globale < 10 ms, scale horizontal massif, multi-région write.

**Attention :** coût RU/s — dimensionner avec le calculateur.

### Azure Storage

| Service | Usage |
| ------- | ----- |
| **Blob** | Fichiers, images, vidéo, backups |
| **Queue** | Files simples (moins de features que Service Bus) |
| **Table** | NoSQL clé-attribut léger |
| **Files** | Partages SMB |

**Tiers Blob :** Hot (fréquent), Cool (archives courtes), Archive (long terme).

---

## 4. Messaging et intégration

### Azure Service Bus

Broker de messages enterprise (queues, topics, sessions, transactions).

| Fonctionnalité | Usage |
| -------------- | ----- |
| **Queue** | Point-to-point, ordre FIFO optionnel |
| **Topic + subscriptions** | Pub/sub avec filtres |
| **Sessions** | Ordre garanti par session |
| **Dead letter** | Messages en échec |

**Choisir quand :** workflows métier, sagas, commandes entre microservices, garanties de livraison.

### Azure Event Grid

Routage d'événements **léger** et **push** (event-driven).

**Choisir quand :** réagir aux changements Azure (blob created, resource updated) ou custom events HTTP.

**Différence Service Bus :** Event Grid = notification ; Service Bus = traitement avec retry, DLQ, sessions.

### Azure Event Hubs

Ingestion de flux à très haut débit (millions d'événements/s).

**Choisir quand :** télémétrie, logs, streaming Kafka-like.

### Logic Apps

Orchestration low-code / connecteurs SaaS.

**Choisir quand :** intégrations B2B, workflows visuels, connecteurs Office 365 / Salesforce.

---

## 5. API et réseau

### Azure API Management (APIM)

Gateway API : routage, auth, rate limiting, transformation, developer portal.

```text
Clients → APIM → App Service / AKS / Functions
         │
         ├── OAuth / JWT validation
         ├── Quotas par abonnement
         ├── Cache réponses
         └── Logging vers App Insights
```

**Tiers :** Developer (test), Basic/Standard/ Premium (prod, VNet).

**Choisir quand :** API publique ou multi-partenaires, gouvernance centralisée.

### Azure Front Door

CDN global + load balancing L7 + WAF + routage multi-région.

**Choisir quand :** utilisateurs mondiaux, failover entre régions, protection DDoS/WAF edge.

### Application Gateway

Load balancer L7 régional + WAF (dans une région).

**Choisir quand :** trafic régional, terminaison SSL, routage path-based dans un VNet.

### Azure Load Balancer

Load balancing L4 (IP + port).

**Choisir quand :** TCP/UDP, interne ou public, simple.

---

## 6. Identité et sécurité

| Service | Rôle |
| ------- | ---- |
| **Microsoft Entra ID** | IAM, OAuth2/OIDC, B2B/B2C |
| **Key Vault** | Secrets, certificats, clés HSM |
| **Managed Identity** | Auth service-to-service sans secret en code |
| **Defender for Cloud** | Posture sécurité, recommandations |
| **Private Link** | Accès privé aux services PaaS (pas d'internet public) |

**Bonne pratique :** chaque App Service / Function utilise une **Managed Identity** pour accéder à SQL, Key Vault, Storage.

---

## 7. Observabilité (aperçu — détail module 6)

| Service | Rôle |
| ------- | ---- |
| **Azure Monitor** | Plateforme centralisée métriques + logs |
| **Application Insights** | APM, dépendances, traces distribuées |
| **Log Analytics** | Requêtes KQL sur les logs |
| **Azure Dashboards** | Visualisation |

---

## 8. DevOps et IaC

| Service | Rôle |
| ------- | ---- |
| **Azure DevOps** | Pipelines CI/CD, repos, boards |
| **GitHub Actions** | CI/CD (intégration Azure native) |
| **Bicep** | IaC déclaratif Azure (recommandé) |
| **Terraform** | IaC multi-cloud |
| **Container Registry (ACR)** | Images Docker privées |

---

## 9. Mapping rapide — besoin → service

| Besoin | Service Azure |
| ------ | ------------- |
| API .NET hébergée | App Service ou AKS |
| Worker async | Functions ou Container Apps |
| File de messages | Service Bus |
| Événements infrastructure | Event Grid |
| Streaming haut débit | Event Hubs |
| Cache | Azure Cache for Redis |
| Fichiers / médias | Blob Storage + CDN / Front Door |
| API publique gouvernée | APIM |
| Secrets | Key Vault |
| Auth utilisateurs | Entra ID / Entra External ID |
| SQL transactionnel | Azure SQL ou PostgreSQL Flexible |
| NoSQL global | Cosmos DB |
| Multi-région failover | Front Door + paired regions |
| Monitoring | App Insights + Monitor |

---

## 10. Régions et disponibilité

### Paires de régions

Azure associe des régions en **paires** pour la reprise (ex. West Europe ↔ North Europe).

### Options de redondance

| Option | SLA indicatif | Description |
| ------ | ------------- | ----------- |
| Zone redondante (ZRS) | 99,99 % | Réplication sur 3 zones de disponibilité |
| Géo-redondante (GRS) | 99,99 %+ | Copie vers région paire |
| Actif-passif multi-région | Variable | Primary + secondary avec failover |

### Choisir une région

- **Latence** : proche des utilisateurs
- **Souveraineté** : RGPD → EU (France Central, West Europe)
- **Disponibilité des services** : tous les SKUs ne sont pas partout
- **Coût** : varie par région

---

## 11. Estimation de coûts — principes

1. Utiliser le [Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
2. Inclure : compute, data, transfert sortant, APIM, monitoring
3. Prévoir **30 % de marge** pour pics et croissance
4. Activer **Budgets et alertes** dans Cost Management
5. Tags (`env`, `project`, `cost-center`) pour FinOps

### Postes de coût souvent sous-estimés

- Transfert de données sortant (egress)
- APIM Premium / Front Door
- Cosmos DB RU provisionnées
- Logs ingérés (Log Analytics GB/jour)
- Environnements non-prod laissés allumés

---

## Pour aller plus loin

- [Patterns et atelier cloud](architecture.md)
- [Azure Architecture Center — Technology choices](https://learn.microsoft.com/azure/architecture/guide/technology-choices/)
- [Well-Architected Framework](https://learn.microsoft.com/azure/well-architected/)
