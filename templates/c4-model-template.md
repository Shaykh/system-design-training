# Template — Modèle C4

Guide pour produire les diagrammes C4 du projet final ou des modules.

---

## Niveaux C4

| Niveau | Audience | Contenu |
| ------ | -------- | ------- |
| **1 — Context** | Tous | Système + acteurs + systèmes externes |
| **2 — Container** | Tech / archi | Applications, BDD, files, mapping infra |
| **3 — Component** | Développeurs | Modules internes d'un container |
| **4 — Code** | Dev (optionnel) | Classes — rarement en revue archi |

**Règle :** un diagramme = un niveau de zoom. Ne pas mélanger Context et Container.

---

## Niveau 1 — Context (gabarit)

```mermaid
flowchart TB
    Actor1([Acteur 1])
    Actor2([Acteur 2])
    System[Votre système<br/>Description courte]
    Ext1[Système externe 1]
    Ext2[Système externe 2]

    Actor1 -->|Interaction| System
    Actor2 --> System
    System --> Ext1
    System --> Ext2
```

**À documenter :**

- Qui utilise le système ?
- De quoi dépend-il (paiement, auth, email) ?

---

## Niveau 2 — Container (gabarit)

```mermaid
flowchart TB
    User([Utilisateur])
    SPA[Web App / SPA]
    API[API Backend]
    DB[(Base de données)]
    Cache[(Cache)]
    Queue[Message Queue]
    Worker[Worker]

    User --> SPA
    User --> API
    SPA --> API
    API --> DB
    API --> Cache
    API --> Queue
    Queue --> Worker
    Worker --> DB
```

**À documenter :**

- Technologie par container (ex. App Service, Azure SQL)
- Protocoles (HTTPS, AMQP)

---

## Niveau 3 — Component (gabarit)

Zoom sur **un** container (souvent l'API).

```mermaid
flowchart TB
    subgraph API["API Backend"]
        Controller[Controllers]
        AppLayer[Application / Handlers]
        Domain[Domain]
        Infra[Infrastructure]
    end

    Controller --> AppLayer
    AppLayer --> Domain
    AppLayer --> Infra
```

---

## Bonnes pratiques

| Pratique | Détail |
| -------- | ------ |
| Légende | Couleurs : interne vs externe |
| Nommage | Noms métier, pas que techniques |
| Flèches | Verbes sur les relations (« commande », « authentifie ») |
| Cohérence | Mêmes noms entre Context et Container |
| Export | Draw.io, Mermaid, ou Structurizr |

---

## Exemple complet

Projet final ShopFlow : [project/architecture/README.md](../project/architecture/README.md)

---

## Outils

| Outil | Lien |
| ----- | ---- |
| Draw.io | <https://app.diagrams.net> |
| Mermaid | <https://mermaid.js.org> |
| C4 model | <https://c4model.com> |
| Structurizr | <https://structurizr.com> |
