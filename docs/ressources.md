# Ressources

Références utiles pour accompagner la formation. Classées par thème ; priorisez les ressources marquées **Essentiel** pour chaque module.

---

## Outils du repository

| Outil | Emplacement | Usage |
| ----- | ----------- | ----- |
| Template DAT | [`templates/dat-template.md`](../templates/dat-template.md) | Dossier d'architecture technique |
| Template C4 | [`templates/c4-model-template.md`](../templates/c4-model-template.md) | Diagrammes Context / Container / Component |
| Checklist design review | [`templates/design-review-checklist.md`](../templates/design-review-checklist.md) | Auto-évaluation et revue par les pairs |

---

## Outils de conception

| Outil | Lien | Usage |
| ----- | ---- | ----- |
| Draw.io (diagrams.net) | <https://app.diagrams.net> | Diagrammes C4, séquence, déploiement |
| Excalidraw | <https://excalidraw.com> | Croquis rapides en design review |
| Mermaid | <https://mermaid.js.org> | Diagrammes en Markdown (versionnable Git) |
| Structurizr | <https://structurizr.com> | Modèle C4 as code (niveau avancé) |

---

## Références générales — System Design

| Ressource | Type | Lien / référence | Priorité |
| --------- | ---- | ---------------- | -------- |
| System Design Primer | Repo GitHub | <https://github.com/donnemartin/system-design-primer> | Essentiel |
| Designing Data-Intensive Applications | Livre (Kleppmann) | O'Reilly | Essentiel |
| Azure Architecture Center | Documentation | <https://learn.microsoft.com/azure/architecture/> | Essentiel |
| C4 Model | Site officiel | <https://c4model.com> | Essentiel |
| AWS Well-Architected Framework | Documentation | <https://docs.aws.amazon.com/wellarchitected/> | Recommandé (concepts transposables) |

---

## Par module

### Module 1 — Fondamentaux

| Sujet | Ressource |
| ----- | --------- |
| Scalabilité | [Azure — Patterns de scalabilité](https://learn.microsoft.com/azure/architecture/guide/design-principles/scale) |
| CAP theorem | Kleppmann, ch. 7–9 ; article original Brewer |
| Latence / throughput | [Google SRE — Monitoring distributed systems](https://sre.google/sre-book/monitoring-distributed-systems/) |
| REST API design | [Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines) |

### Module 2 — Architecture applicative

| Sujet | Ressource |
| ----- | --------- |
| Clean Architecture | Robert C. Martin — *Clean Architecture* |
| Hexagonal / Ports & Adapters | Article Alistair Cockburn |
| DDD | Eric Evans — *Domain-Driven Design* ; Vaughn Vernon — *Implementing DDD* |
| CQRS | [Microsoft — CQRS pattern](https://learn.microsoft.com/azure/architecture/patterns/cqrs) |
| MediatR (.NET) | <https://github.com/jbogard/MediatR> |
| Event Sourcing (intro) | [Microsoft — Event Sourcing pattern](https://learn.microsoft.com/azure/architecture/patterns/event-sourcing) |

### Module 3 — Data & persistance

| Sujet | Ressource |
| ----- | --------- |
| SQL vs NoSQL | Kleppmann, ch. 2–3 |
| Database per service | [Microsoft — Database per service](https://learn.microsoft.com/azure/architecture/microservices/design/data-consistency) |
| Saga pattern | [Microsoft — Saga pattern](https://learn.microsoft.com/azure/architecture/reference-architectures/saga/saga) |
| Concurrence | Kleppmann, ch. 7 (transactions) |
| Cache invalidation | [Microsoft — Cache aside pattern](https://learn.microsoft.com/azure/architecture/patterns/cache-aside) |

### Module 4 — Scalabilité & performance

| Sujet | Ressource |
| ----- | --------- |
| Load balancing | [Azure — Load balancing](https://learn.microsoft.com/azure/architecture/guide/technology-choices/load-balancing-overview) |
| Caching | [Redis documentation](https://redis.io/docs/) |
| Sharding | Kleppmann, ch. 6 |
| Files & messaging | [Azure Service Bus](https://learn.microsoft.com/azure/service-bus-messaging/) |
| Event streaming | [Apache Kafka docs](https://kafka.apache.org/documentation/) (bonus) |

### Module 5 — Cloud Azure

| Sujet | Ressource |
| ----- | --------- |
| Patterns Azure | [Azure Architecture Center — Patterns](https://learn.microsoft.com/azure/architecture/patterns/) |
| App Service | <https://learn.microsoft.com/azure/app-service/> |
| AKS | <https://learn.microsoft.com/azure/aks/> |
| Azure Functions | <https://learn.microsoft.com/azure/azure-functions/> |
| Service Bus / Event Grid | Docs Microsoft (liens ci-dessus module 4) |
| API Management | <https://learn.microsoft.com/azure/api-management/> |
| Chiffrage | [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/) |
| PRA / PCA | [Azure — Disaster recovery](https://learn.microsoft.com/azure/architecture/framework/resiliency/disaster-recovery) |

### Module 6 — Résilience & observabilité

| Sujet | Ressource |
| ----- | --------- |
| Resilience patterns | [Azure — Resiliency patterns](https://learn.microsoft.com/azure/architecture/framework/resiliency/reliability-patterns) |
| Polly (.NET) | <https://www.pollydocs.org/> |
| Azure Monitor | <https://learn.microsoft.com/azure/azure-monitor/> |
| Application Insights | <https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview> |
| OpenTelemetry | <https://opentelemetry.io/docs/> |
| Google SRE Book | <https://sre.google/books/> |

### Module 7 — Sécurité & gouvernance

| Sujet | Ressource |
| ----- | --------- |
| OAuth 2.0 / OIDC | <https://oauth.net/2/> ; <https://openid.net/connect/> |
| Microsoft Entra ID | <https://learn.microsoft.com/entra/identity/> |
| Zero Trust | [Microsoft — Zero Trust](https://learn.microsoft.com/security/zero-trust/) |
| Secrets (Azure Key Vault) | <https://learn.microsoft.com/azure/key-vault/> |
| OWASP Top 10 | <https://owasp.org/www-project-top-ten/> |

### Module 8 — Cas réels

| Cas | Ressources suggérées |
| --- | ------------------- |
| WhatsApp | Articles engineering Meta ; System Design Primer (messaging) |
| Uber | Uber Engineering Blog — dispatch, geospatial |
| Paiement | Kleppmann ch. 11 ; PCI-DSS (niveau conceptuel) |
| Logs distribués | ELK stack, Azure Log Analytics, Loki (Grafana) |

**Méthode d'entretien system design :**

- Excalidraw ou tableau blanc pour le high-level design
- Toujours commencer par clarifier : utilisateurs, volume, latence, cohérence, budget

---

## Infrastructure as Code

| Outil | Lien | Usage |
| ----- | ---- | ----- |
| Bicep | <https://learn.microsoft.com/azure/azure-resource-manager/bicep/> | Déploiement Azure déclaratif |
| Terraform (Azure) | <https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs> | Alternative multi-cloud |
| Azure DevOps Pipelines | <https://learn.microsoft.com/azure/devops/pipelines/> | CI/CD (voir `project/ci-cd/`) |

---

## Communautés et veille

| Canal | Lien |
| ----- | ---- |
| Azure Architecture Blog | <https://azure.microsoft.com/blog/topics/architecture/> |
| .NET Blog (architecture) | <https://devblogs.microsoft.com/dotnet/> |
| High Scalability | <http://highscalability.com/> |
| r/softwarearchitecture | <https://www.reddit.com/r/softwarearchitecture/> |

---

## Lectures recommandées (ordre suggéré)

1. **Designing Data-Intensive Applications** — Martin Kleppmann (fondation data / distribué)
2. **Clean Architecture** — Robert C. Martin (structure applicative)
3. **Domain-Driven Design** — Eric Evans (modélisation métier)
4. **Site Reliability Engineering** — Google (ops, observabilité, résilience)
5. **Fundamentals of Software Architecture** — Richards & Ford (vue d'ensemble architecte)

---

## Comptes et environnements

Pour les ateliers Azure (modules 5 à 7 et projet final) :

- Créer un compte [Azure gratuit](https://azure.microsoft.com/free/) ou utiliser un abonnement entreprise
- Activer les alertes de budget pour éviter les surprises
- Préférer des ressources **dev/test** et les détruire après chaque atelier

Pour les ateliers .NET (module 2) :

- [.NET SDK](https://dotnet.microsoft.com/download)
- Visual Studio ou VS Code + extension C#

---

## Documents associés

- [Programme](programme.md) — parcours et livrables
- [Planning](planning.md) — calendrier semaine par semaine
- [`summary.md`](../summary.md) — référence du programme
