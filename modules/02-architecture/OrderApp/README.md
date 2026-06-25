# OrderApp — Squelette Module 2

Application de référence pour les ateliers **Clean Architecture**, **CQRS** et **MediatR** (module 2).

## Structure

```text
OrderApp/
├── OrderApp.Domain/           # Entités, value objects, events (aucune dépendance infra)
├── OrderApp.Application/      # Commands, queries, handlers, ports
├── OrderApp.Infrastructure/   # EF Core InMemory, adapters
├── OrderApp.Api/              # Controllers REST fins
└── OrderApp.Domain.Tests/     # Tests unitaires du domaine
```

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Démarrage

```bash
cd modules/02-architecture/OrderApp
dotnet restore
dotnet build
dotnet test
dotnet run --project OrderApp.Api
```

Swagger : https://localhost:7xxx/swagger (voir port dans la console)

## Données de démo (seed)

| Ressource | Id |
|-----------|-----|
| Client | `11111111-1111-1111-1111-111111111111` |
| Produit Laptop | `22222222-2222-2222-2222-222222222222` |
| Produit Mouse | `33333333-3333-3333-3333-333333333333` |

## Exemples d'appels API

### Créer une commande

```http
POST /api/orders
Content-Type: application/json

{
  "customerId": "11111111-1111-1111-1111-111111111111",
  "lines": [
    { "productId": "22222222-2222-2222-2222-222222222222", "quantity": 1 }
  ]
}
```

### Confirmer une commande (déclenche domain event + email loggé)

```http
POST /api/orders/{orderId}/confirm
```

### Lire une commande

```http
GET /api/orders/{orderId}
```

## Ateliers associés

| Atelier | Fichier |
|---------|---------|
| Audit legacy | [workshop.md](../workshop.md#atelier-1--audit-de-couplage-et-cohésion-1-h) |
| Refactoring Clean Arch | Ce squelette |
| CQRS MediatR | Commands / Queries dans `OrderApp.Application` |
| Domain events | `OrderConfirmed` + `OrderConfirmedNotificationHandler` |

## Extensions suggérées

- [ ] Ajouter FluentValidation sur les commands
- [ ] Extraire `OrderApp.Legacy` avec le controller de l'atelier 1
- [ ] Remplacer InMemory par SQL Server
- [ ] Ajouter Polly sur un client HTTP fictif (module 6)

## Références

- [Module 2 — patterns](../patterns.md)
- [Module 2 — workshop](../workshop.md)
