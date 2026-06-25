# Patterns d'architecture applicative

Ce document présente les approches structurantes pour organiser le code d'une application métier complexe. L'objectif commun : **isoler le domaine** des détails techniques (base de données, UI, frameworks).

---

## 1. Pourquoi structurer l'architecture ?

Sans structure, une application tend vers le **big ball of mud** :

- Logique métier dispersée dans les controllers
- Dépendances directes vers EF Core, HTTP, fichiers
- Tests impossibles sans base de données réelle
- Changement de technologie = réécriture massive

Une architecture en couches (ou en anneaux) impose une règle simple :

> Les dépendances pointent **vers l'intérieur**, vers le domaine.

---

## 2. Couplage et cohésion

Avant de choisir un pattern, évaluez la qualité du code existant.

### Couplage

Degré d'interdépendance entre modules.

| Type | Description | Impact |
| ---- | ----------- | ------ |
| Couplage faible | Module A change peu si B change | Souhaitable |
| Couplage fort | Modification en cascade | Difficile à maintenir |
| Couplage temporel | A et B doivent être disponibles simultanément | Fragile en distribué |

**Signaux d'alerte :** imports transverses, classes « god object », DTO utilisés partout, tests qui nécessitent toute l'infra.

### Cohésion

Degré auquel les éléments d'un module appartiennent ensemble.

| Type | Description |
| ---- | ----------- |
| Cohésion forte | Une classe / module = une responsabilité claire |
| Cohésion faible | Module fourre-tout sans lien logique |

**Objectif :** haute cohésion à l'intérieur d'un module, faible couplage entre modules.

### Matrice de décision

```text
Couplage fort + cohésion faible  → refactoring prioritaire
Couplage faible + cohésion forte → base saine pour évoluer
```

---

## 3. Clean Architecture

Proposée par Robert C. Martin (Uncle Bob). Organisation en **anneaux concentriques** :

```diagram
        ┌─────────────────────────────────────┐
        │  Frameworks & Drivers (UI, DB)    │
        │  ┌───────────────────────────────┐  │
        │  │  Interface Adapters           │  │
        │  │  (Controllers, Gateways, DTO) │  │
        │  │  ┌─────────────────────────┐  │  │
        │  │  │  Application (Use Cases)│  │  │
        │  │  │  ┌───────────────────┐  │  │  │
        │  │  │  │  Entities (Domain)│  │  │  │
        │  │  │  └───────────────────┘  │  │  │
        │  │  └─────────────────────────┘  │  │
        │  └───────────────────────────────┘  │
        └─────────────────────────────────────┘

        Dépendances ──────────────────► vers le centre
```

### Couches

| Couche | Responsabilité | Exemple .NET |
| ------ | -------------- | ------------ |
| **Domain** | Entités, value objects, règles métier | `Order`, `Money`, `OrderCannotBeEmptyException` |
| **Application** | Cas d'usage, orchestration | `CreateOrderHandler`, `IOrderRepository` (interface) |
| **Infrastructure** | Implémentations techniques | `EfOrderRepository`, `EmailSender` |
| **Presentation** | API, UI | `OrdersController`, endpoints Minimal API |

### Règles

1. Le **domaine ne dépend de rien** (pas de référence à EF, ASP.NET, etc.)
2. L'**application** définit des interfaces (ports) implémentées par l'infrastructure
3. L'**infrastructure** référence application et domaine
4. La **présentation** appelle les cas d'usage, pas la base directement

### Structure de projet .NET typique

```text
src/
├── MyApp.Domain/           # Entités, value objects, exceptions domaine
├── MyApp.Application/      # Commands, queries, handlers, interfaces
├── MyApp.Infrastructure/   # EF Core, services externes
└── MyApp.Api/              # Controllers, DI, middleware
```

---

## 4. Architecture hexagonale (Ports & Adapters)

Coincée par Alistair Cockburn. Même philosophie que Clean Architecture, vocabulaire différent :

```diagram
                    ┌──────────────┐
    HTTP API ──────►│              │──────► Base de données
    (Adapter)       │    DOMAIN    │        (Adapter)
                    │   + Ports    │
    CLI ───────────►│              │──────► Message bus
                    └──────────────┘
```

- **Port** : interface définie par le domaine (`IOrderRepository`, `INotificationPort`)
- **Adapter primaire** (driving) : déclenche le domaine (API, UI, consumer de messages)
- **Adapter secondaire** (driven) : appelé par le domaine (BDD, email, stockage)

### Équivalence avec Clean Architecture

| Hexagonal | Clean Architecture |
| --------- | ----------------- |
| Domain | Entities + Application (selon granularité) |
| Port | Interface dans Application |
| Adapter | Infrastructure + Presentation |

---

## 5. Onion Architecture

Variante proposée par Jeffrey Palermo. Couches concentriques explicites :

```text
         [ Infrastructure ]
       [ Application Services ]
     [ Domain Services ]
   [ Domain Model (Entities) ]
```

- **Domain Model** : cœur, zéro dépendance externe
- **Domain Services** : logique métier qui ne rentre pas dans une entité
- **Application Services** : coordination des cas d'usage
- **Infrastructure** : ORM, messaging, fichiers

En pratique, Onion et Clean Architecture sont souvent **fusionnées** dans les projets .NET.

---

## 6. Domain Driven Design (DDD)

Approche de modélisation centrée sur le **métier**, pas sur les tables de base de données.

### DDD stratégique

Découper un grand système en **bounded contexts** (périmètres cohérents) :

```diagram
┌─────────────────┐     ┌─────────────────┐
│  Ventes         │     │  Livraison      │
│  (Commande,     │     │  (Expédition,   │
│   Client)       │     │   Tracking)     │
└────────┬────────┘     └────────┬────────┘
         │    Context Mapping     │
         └──────────┬─────────────┘
                    │
         Relations : Shared Kernel, ACL,
         Customer/Supplier, Conformist...
```

| Concept | Description |
| ------- | ----------- |
| **Bounded Context** | Frontière linguistique et modèle autonome |
| **Ubiquitous Language** | Vocabulaire partagé métier ↔ code |
| **Context Mapping** | Comment les contextes communiquent |

### DDD tactique (building blocks)

| Pattern | Rôle | Exemple |
| ------- | ---- | ------- |
| **Entity** | Objet avec identité persistante | `Order` (Id unique) |
| **Value Object** | Objet sans identité, immuable | `Address`, `Money` |
| **Aggregate** | Cluster d'entités avec racine | `Order` + `OrderLine` (accès via `Order`) |
| **Repository** | Persistance d'un agrégat | `IOrderRepository` |
| **Domain Service** | Logique cross-entités | `PricingService` |
| **Domain Event** | Fait métier survenu | `OrderPlaced` |

### Règles des agrégats

1. Une transaction = un agrégat (cohérence interne)
2. Référencer d'autres agrégats par **identifiant**, pas par objet
3. Taille raisonnable (éviter les méga-agrégats)

### Exemple aggregate

```csharp
public class Order  // Aggregate Root
{
    private readonly List<OrderLine> _lines = new();

    public Guid Id { get; private set; }
    public OrderStatus Status { get; private set; }

    public void AddLine(ProductId productId, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify a confirmed order.");

        _lines.Add(new OrderLine(productId, quantity, unitPrice));
    }

    public void Confirm()
    {
        if (!_lines.Any())
            throw new DomainException("Order must have at least one line.");

        Status = OrderStatus.Confirmed;
        // DomainEvents.Raise(new OrderConfirmed(Id));
    }
}
```

---

## 7. CQRS (Command Query Responsibility Segregation)

Séparer les **opérations d'écriture** (commands) des **opérations de lecture** (queries).

```diagram
         ┌─────────────┐
         │   Client    │
         └──────┬──────┘
                │
       ┌────────┴────────┐
       ▼                 ▼
┌─────────────┐   ┌─────────────┐
│  Commands   │   │   Queries   │
│  (écriture) │   │  (lecture)  │
└──────┬──────┘   └──────┬──────┘
       │                 │
       ▼                 ▼
┌─────────────┐   ┌─────────────┐
│ Write Model │   │ Read Model  │
│ (normalisé) │   │ (dénormalisé│
│             │   │  ou cache)  │
└─────────────┘   └─────────────┘
```

### Pourquoi ?

| Problème | CQRS apporte |
| -------- | ------------ |
| Modèle unique trop complexe | Modèles optimisés par usage |
| Lectures >> écritures | Read model dénormalisé, rapide |
| Besoins de reporting différents | Projections dédiées |

### CQRS simple (même base)

Dans un monolithe, CQRS **léger** suffit souvent :

- Commands : `CreateOrderCommand` → `CreateOrderHandler`
- Queries : `GetOrderByIdQuery` → `GetOrderByIdHandler`
- Même base PostgreSQL, handlers séparés

### CQRS avancé (bases séparées)

- Write : base normalisée (transactions ACID)
- Read : base dénormalisée, Elasticsearch, cache
- Synchronisation : events, polling, CDC

**Attention :** complexité accrue (eventual consistency côté lecture). Justifier par un besoin réel de scale ou de modèles très différents.

### MediatR en .NET

Bibliothèque courante pour implémenter CQRS + mediator pattern :

```csharp
// Command
public record CreateOrderCommand(Guid CustomerId, List<OrderLineDto> Lines) : IRequest<Guid>;

// Handler
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var order = Order.Create(request.CustomerId);
        foreach (var line in request.Lines)
            order.AddLine(line.ProductId, line.Quantity, line.UnitPrice);

        await _repository.AddAsync(order, ct);
        return order.Id;
    }
}

// Controller
[HttpPost]
public async Task<IActionResult> Create(CreateOrderCommand command)
    => Ok(await _mediator.Send(command));
```

---

## 8. Event Sourcing (introduction)

Stocker l'état d'un agrégat comme **séquence d'événements** plutôt que comme ligne courante.

```text
Événements stockés :
  1. OrderCreated       { orderId, customerId }
  2. OrderLineAdded     { orderId, productId, qty }
  3. OrderConfirmed     { orderId }

État actuel = replay des événements 1 → 3
```

### Avantages

- Audit trail complet (qui a fait quoi, quand)
- Reconstruction d'état à n'importe quel moment
- Découplage via événements (projections, intégrations)

### Inconvénients

- Courbe d'apprentissage et complexité opérationnelle
- Gestion des versions de schéma d'événements
- Requêtes ad hoc difficiles (besoin de projections / read models)
- Suppression RGPD plus complexe

### Quand l'envisager ?

| Oui | Non |
| --- | --- |
| Audit réglementaire obligatoire | CRUD simple |
| Logique métier riche avec historique | Équipe junior, délai court |
| Event-driven architecture déjà en place | Besoin principal = rapports SQL |

**En pratique :** commencer par des **domain events** (sans event sourcing complet), puis évoluer si le besoin d'audit le justifie.

### Domain Events vs Event Sourcing

| | Domain Event | Event Sourcing |
| - | ------------ | -------------- |
| Stockage | État courant + events optionnels | Events = source de vérité |
| Complexité | Faible à modérée | Élevée |
| Usage | Notifications, CQRS read side | Audit complet, temporal queries |

---

## 9. Comparer les approches

| Critère | Clean / Hexagonal | DDD | CQRS | Event Sourcing |
| ------- | ----------------- | --- | ---- | -------------- |
| Objectif | Isoler le domaine | Modéliser le métier | Séparer read/write | Historiser l'état |
| Complexité | Modérée | Modérée à élevée | Modérée à élevée | Élevée |
| Taille équipe min. | 1+ | 3+ pour le stratégique | 2+ | 4+ |
| Couplage à la BDD | Faible | Faible | Variable | Très faible (write) |

### Combinaisons courantes en .NET

```text
Monolithe modulaire
  └── Clean Architecture
        └── DDD tactique (aggregates, repositories)
              └── CQRS léger (MediatR)
                    └── Domain Events (sans ES complet)
```

---

## 10. Anti-patterns à éviter

| Anti-pattern | Problème | Alternative |
| ------------ | -------- | ----------- |
| Anemic domain model | Entités sans comportement, logique dans les services | Rich domain model |
| God service | Une classe fait tout | Découper par cas d'usage |
| Leaky abstraction | `Order` expose `DbContext` | Repository derrière interface |
| CQRS partout sans besoin | Complexité gratuite | CQRS sur les bounded contexts chargés |
| Microservices trop tôt | Distribuer un mauvais monolithe | Monolithe modulaire d'abord |

---

## 11. Synthèse

| Question | Piste de réponse |
| -------- | ---------------- |
| Où mettre la logique métier ? | Domaine (entités + domain services) |
| Comment tester sans BDD ? | Tests unitaires sur le domaine |
| Comment changer de BDD ? | Remplacer l'adapter infrastructure |
| Comment scaler les lectures ? | CQRS + read model |
| Comment tracer l'historique ? | Domain events → event sourcing si besoin fort |

---

## Pour aller plus loin

- [Ateliers pratiques](workshop.md)
- [Ressources module 2](../../docs/ressources.md#module-2--architecture-applicative)
- [Planning semaine 2](../../docs/planning.md#semaine-2--architecture-applicative)

**Prochaine étape :** réaliser les ateliers dans [`workshop.md`](workshop.md).
