# Architecture ShopFlow — Diagrammes C4

Diagrammes de référence pour le projet final. Reproduisez-les dans Draw.io (`context-diagram.drawio`, `container-diagram.drawio`, `component-diagram.drawio`) si besoin d'édition visuelle.

---

## Niveau 1 — Context

Acteurs et systèmes externes.

```mermaid
flowchart TB
    Customer([Client])
    Admin([Admin / Support])
    Partner([Partenaire API])

    ShopFlow[ShopFlow Platform<br/>Catalogue, commandes, paiement]

    Entra[Entra ID / External ID]
    Stripe[Stripe PSP]
    Email[Service Email]

    Customer -->|Achète en ligne| ShopFlow
    Customer -->|Se connecte| Entra
    Admin -->|Gère catalogue et commandes| ShopFlow
    Admin -->|SSO| Entra
    Partner -->|Consulte catalogue API| ShopFlow
    ShopFlow -->|Authorize / Capture| Stripe
    ShopFlow -->|Valide tokens| Entra
    ShopFlow -->|Emails transactionnels| Email
```

---

## Niveau 2 — Container

Conteneurs applicatifs et mapping Azure.

```mermaid
flowchart TB
    subgraph Users
        Browser[Navigateur / Mobile web]
    end

    subgraph Azure["Azure — West Europe"]
        SWA[Static Web Apps<br/>React SPA]
        FD[Azure Front Door + WAF]
        APIM[API Management]
        API[App Service<br/>ShopFlow API .NET 8]
        FN[Azure Functions<br/>Workers]
        SB[Service Bus]
        SQL[(Azure SQL<br/>Primary)]
        Redis[(Redis Cache)]
        Search[Azure AI Search]
        Blob[Blob Storage<br/>Images produits]
        KV[Key Vault]
        AI[Application Insights]
    end

    Stripe[Stripe]
    Entra[Entra ID]

    Browser --> SWA
    Browser --> FD
    SWA --> FD
    FD --> APIM
    APIM --> API
    API --> SQL
    API --> Redis
    API --> Search
    API --> Blob
    API --> SB
    API --> KV
    API --> AI
    API --> Stripe
    API --> Entra
    SB --> FN
    FN --> SQL
    FN --> Email[Email Service]
    FN --> Search
```

### Mapping Azure

| Conteneur | Service Azure | SKU indicatif |
| --------- | ------------- | ------------- |
| SPA | Static Web Apps Standard | 1 app |
| Edge | Front Door Premium | WAF + CDN |
| Gateway | API Management | Standard 1 unit |
| API | App Service Linux | P1v3 × 2 |
| Workers | Functions Premium | EP1 |
| Messaging | Service Bus | Standard namespace |
| OLTP | Azure SQL | S3 + geo-replica |
| Cache | Azure Cache Redis | C1 Standard |
| Search | Azure AI Search | Basic |
| Médias | Blob Storage | Hot GRS |
| Secrets | Key Vault | Standard |
| Observabilité | Application Insights | Pay-as-you-go |

---

## Niveau 3 — Component

Composants internes de l'API (monolithe modulaire).

```mermaid
flowchart TB
    subgraph Presentation["API — Presentation"]
        Controllers[API Controllers]
        Middleware[Auth / Exception middleware]
    end

    subgraph Application["Application — Use Cases"]
        CatHandlers[Catalog Queries]
        CartHandlers[Cart Commands]
        OrdHandlers[Order Commands]
        PayHandlers[Payment Commands]
        MediatR[MediatR Pipeline]
    end

    subgraph Domain["Domain"]
        Product[Product Aggregate]
        Order[Order Aggregate]
        Cart[Cart Aggregate]
        Payment[Payment Aggregate]
    end

    subgraph Infrastructure["Infrastructure"]
        EfRepo[EF Core Repositories]
        RedisCache[Redis Cache Service]
        StripeAdapter[Stripe Adapter]
        BusPublisher[Service Bus Publisher]
        SearchIndexer[Search Index Client]
    end

    Controllers --> MediatR
    MediatR --> CatHandlers
    MediatR --> CartHandlers
    MediatR --> OrdHandlers
    MediatR --> PayHandlers
    CatHandlers --> Product
    OrdHandlers --> Order
    CartHandlers --> Cart
    PayHandlers --> Payment
    CatHandlers --> EfRepo
    CatHandlers --> RedisCache
    OrdHandlers --> EfRepo
    PayHandlers --> StripeAdapter
    OrdHandlers --> BusPublisher
    CatHandlers --> SearchIndexer
```

---

## Séquence — Création de commande

```mermaid
sequenceDiagram
    participant C as Client SPA
    participant APIM as API Management
    participant API as ShopFlow API
    participant SQL as Azure SQL
    participant Stripe as Stripe
    participant SB as Service Bus
    participant FN as Functions

    C->>APIM: POST /api/orders (Bearer JWT)
    APIM->>APIM: Validate JWT, rate limit
    APIM->>API: Forward request
    API->>SQL: BEGIN — check stock (optimistic)
    API->>Stripe: Authorize (idempotency-key)
    Stripe-->>API: authorized
    API->>SQL: INSERT order, COMMIT
    API->>SB: Publish OrderConfirmed
    API-->>C: 201 Created

  par Async
    SB->>FN: Deliver message
    FN->>FN: Send confirmation email
    FN->>FN: Update search index stock
  end
```

---

## Déploiement

```mermaid
flowchart LR
    subgraph CI["Azure DevOps / GitHub"]
        Build[Build + Test]
        Deploy[Deploy Bicep + App]
    end

    subgraph Env_Dev[Dev]
        API_D[App Service B2]
        SQL_D[SQL Basic]
    end

    subgraph Env_Stg[Staging]
        API_S[App Service P1v3]
        SQL_S[SQL S2]
    end

    subgraph Env_Prod[Production]
        API_P[App Service P1v3 × 2]
        SQL_P[SQL S3 + Replica]
        FD_P[Front Door]
    end

    Build --> Deploy
    Deploy --> Env_Dev
    Deploy --> Env_Stg
    Deploy -->|Approval| Env_Prod
```

---

## Flux de données — Catalogue

```
Lecture produit :
  Client → Front Door → APIM → API
    → Redis (hit ? return)
    → miss : SQL + remplissage Redis
    → Recherche : Azure AI Search (full-text)

Écriture produit (admin) :
  API → SQL → event ProductUpdated → Function → index Search + invalidation Redis
```

---

## Fichiers Draw.io

| Fichier | Contenu à y reporter |
| ------- | -------------------- |
| `context-diagram.drawio` | Diagramme Context |
| `container-diagram.drawio` | Container + services Azure |
| `component-diagram.drawio` | Composants API |

Export PNG recommandé pour inclusion dans le DAT ou les slides de soutenance.
