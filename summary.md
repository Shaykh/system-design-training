# 📘 Programme de formation — System Design & Architecture

## 🎯 Objectif global

Former à :

- Concevoir des architectures robustes et scalables
- Maîtriser les patterns de system design modernes (cloud / distribué)
- Prendre des décisions d’architecture justifiées (trade-offs)
- Produire des artefacts exploitables en entreprise (DAT, diagrammes, chiffrage…)

---

## 🧱 Structure du programme

### 🟦 Module 1 — Fondamentaux du System Design

#### Objectifs

- Comprendre les bases du system design
- Identifier les principaux compromis techniques

#### Contenu

- Scalabilité (verticale / horizontale)
- Latence vs throughput
- CAP theorem
- BASE vs ACID
- Monolith vs microservices
- High availability / fault tolerance

#### Livrables

- Fiche synthèse des compromis
- Design d’une API REST simple + diagramme

---

### 🟦 Module 2 — Architecture applicative avancée

#### Contenu

- Clean Architecture
- Hexagonal / Onion Architecture
- Domain Driven Design (DDD)
- CQRS
- Event sourcing (introduction)
- Couplage & cohésion

#### Ateliers

- Refactoring d’une application .NET
- Implémentation CQRS (ex: MediatR)

#### Livrables

- Diagrammes (Component + Sequence)
- Documentation d’architecture

---

### 🟦 Module 3 — Data & persistance

#### Contenu

- SQL vs NoSQL
- OLTP vs OLAP
- Database per service
- Saga pattern
- Transactions distribuées
- Gestion de la concurrence
- Cache invalidation

#### Atelier

- Design d’un système multi-bases (SQL + Redis + Event store)

#### Livrables
- Architecture data
- Choix technologiques justifiés

---

### 🟦 Module 4 — Scalabilité & performance

### Contenu
- Load balancing
- Caching (Redis, CDN)
- Partitioning / sharding
- Async processing (queues, event streaming)

### Atelier
- Design système type (Netflix, e-commerce, etc.)

### Livrables
- Architecture scalable
- Plan de montée en charge

---

## 🟦 Module 5 — Architecture Cloud (Azure)

### Contenu
- Azure Architecture Patterns
- App Services / AKS / Functions
- Service Bus / Event Grid
- API Management
- Multi-region / Disaster Recovery

### Atelier
- Design d’une architecture cloud complète

### Livrables
- Diagrammes C4
- Chiffrage Azure
- Plan PRA / PCA

---

## 🟦 Module 6 — Résilience & observabilité

### Contenu
- Retry / circuit breaker (Polly)
- Monitoring (Azure Monitor, App Insights)
- Logging distribué
- Alerting

### Atelier
- Instrumentation d’une application
- Mise en place de monitoring

### Livrables
- Dashboard de supervision
- Stratégie de résilience

---

## 🟦 Module 7 — Sécurité & gouvernance

### Contenu
- OAuth2 / OpenID Connect / Entra ID
- Zero Trust
- Secrets management
- IAM / RBAC

### Cas pratique
- Sécurisation d’une architecture distribuée

### Livrables
- Modèle de sécurité
- Plan de gouvernance

---

## 🟦 Module 8 — System Design (cas réels)

### Objectifs
- Appliquer les connaissances sur des cas complexes

### Cas étudiés
- WhatsApp
- Uber
- Système de paiement
- Système de logs distribués

### Méthodologie
1. Clarification du besoin
2. Estimation du scale
3. High-level design
4. Deep dive
5. Analyse des trade-offs

---

# 📊 Méthodologie pédagogique

- ✅ 30% théorie
- ✅ 40% pratique (hands-on)
- ✅ 30% design review

---

# 🧾 Livrables finaux

- Dossier d’architecture (DAT)
- Diagrammes C4 :
  - Context
  - Container
  - Component
- Choix techniques argumentés
- Estimation des coûts (Azure)
- Stratégie de résilience

---

# 🛠️ Outils recommandés

- Draw.io / Excalidraw
- Azure Architecture Center
- Git (repo d’architecture)
- Markdown
- Terraform / Bicep

---

# 📅 Planning type (8 à 10 semaines)

| Semaine | Module |
|--------|--------|
| 1 | Fondamentaux |
| 2 | Architecture |
| 3 | Data |
| 4 | Scalabilité |
| 5 | Cloud Azure |
| 6 | Résilience |
| 7 | Sécurité |
| 8-10 | Projet final |

---

# 🚀 Projet final

## Objectif
Concevoir un système complet de niveau entreprise

### Exemples
- Plateforme e-commerce
- CRM
- API multi-tenant

### À produire
- Architecture complète
- CI/CD
- Monitoring
- Sécurité
- Chiffrage

---

# ⭐ Bonus (niveau avancé)

- Architecture multi-tenant SaaS
- FinOps
- Event-driven avancé
- Data streaming (Kafka)
- Migration monolithe → microservices

---

# 📌 Résultat attendu

À l’issue de la formation, les participants seront capables de :
- Concevoir une architecture complète
- Justifier leurs choix techniques
- Produire des documents exploitables en entreprise
- Anticiper les problématiques de production
