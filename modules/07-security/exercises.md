# Exercices — Module 7 Sécurité & gouvernance

Durée estimée : **8 à 10 heures** (semaine 7 du [planning](../../docs/planning.md)).

---

## Exercice 1 — Identifier les flux OAuth (45 min)

Pour chaque scénario, indiquez le **flux OAuth 2.0** approprié et justifiez.

| Scénario | Flux | Justification |
| -------- | ---- | ------------- |
| Application mobile native (utilisateur) | | |
| SPA React (utilisateur) | | |
| Service batch nocturne (sans utilisateur) | | |
| API qui appelle une autre API au nom de l'utilisateur | | |
| Intégration partenaire machine-to-machine | | |

<details>
<summary>Corrigé indicatif</summary>

| Scénario | Flux |
| -------- | ---- |
| Mobile native | Authorization Code + PKCE |
| SPA React | Authorization Code + PKCE (pas implicit flow) |
| Batch nocturne | Client Credentials |
| API → API (user context) | On-Behalf-Of |
| Partenaire M2M | Client Credentials + scopes application |

</details>

---

## Exercice 2 — Analyse de menaces STRIDE (1 h)

### Contexte

API e-commerce multi-tenant : création commande, consultation historique, admin catalogue.

### Travail

Pour chaque élément, identifiez une menace STRIDE et une mitigation :

| Élément | STRIDE | Menace | Mitigation |
| ------- | ------ | ------ | ---------- |
| `GET /api/orders/{id}` | Spoofing | | |
| `GET /api/orders/{id}` | Tampering | | |
| JWT access token | Repudiation | | |
| Réponse API commande | Information disclosure | | |
| `POST /api/orders` | Denial of service | | |
| Service → SQL | Elevation of privilege | | |

<details>
<summary>Pistes</summary>

- Spoofing : token forgé → validation JWT Entra
- Tampering : modifier orderId → BOLA check ownership + tenant
- Repudiation : nier commande → logs audit signés
- Info disclosure : données autre tenant → filtre tenant_id depuis token
- DoS : flood POST → rate limiting APIM
- Elevation : SQL injection → ORM paramétré, MI avec droits minimaux

</details>

---

## Exercice 3 — Modèle RBAC (45 min)

### Contexte

Plateforme SaaS avec rôles :

| Rôle | Permissions |
| ---- | ----------- |
| `TenantAdmin` | Gérer utilisateurs du tenant, toutes commandes du tenant |
| `Buyer` | Créer et voir ses commandes |
| `Viewer` | Lecture seule commandes du tenant |
| `SupportAgent` | Lecture cross-tenant (support uniquement) |

1. Définissez les **App Roles** Entra ID à créer.
2. Pour chaque endpoint, indiquez la policy ASP.NET Core :

| Endpoint | Policy / rôle |
| -------- | ------------- |
| `POST /api/orders` | |
| `GET /api/orders` | |
| `GET /api/orders/{id}` | |
| `GET /api/admin/tenants/{id}/orders` | |
| `DELETE /api/users/{id}` | |

3. Comment empêcher un `Buyer` de voir la commande d'un autre buyer **du même tenant** ?

---

## Exercice 4 — Secrets et Zero Trust (30 min)

Une équipe propose :

> « On met la connection string SQL dans App Service Configuration, c'est plus simple que Key Vault. »

1. Listez **3 risques** de cette approche.
2. Décrivez l'architecture cible avec Managed Identity + Key Vault.
3. Citez **2 contrôles Zero Trust** violés par l'approche initiale.

---

## Cas pratique principal — Sécuriser une architecture distribuée (3–4 h)

**Livrables obligatoires du module.**

### Contexte

Reprennez l'architecture du **module 5** (SaaS facturation B2B, 500 tenants) ou l'e-commerce des modules précédents.

Services :

- API principale (App Service)
- Azure Functions (génération PDF)
- Service Bus (événements)
- Azure SQL (multi-tenant)
- Blob Storage (PDF factures)
- Partenaires externes (webhooks sortants)

### Travail

#### 1. Modèle de sécurité — `security-model.md`

```markdown
# Modèle de sécurité — [Système]

## 1. Actifs et classification
| Actif | Classification | Stockage |

## 2. Acteurs
| Acteur | Type | AuthN |

## 3. Surface d'attaque
[Diagramme ou liste : internet, partenaires, admin]

## 4. Modèle de menaces (résumé STRIDE)
| Composant | Menaces principales | Contrôles |

## 5. Authentification
### Utilisateurs (SPA + API)
[Flux OIDC — référence diagramme séquence]

### Services internes
[Managed Identity, Client Credentials]

### Partenaires webhooks
[Signature HMAC, mTLS, ou subscription key APIM]

## 6. Autorisation
| Ressource | Modèle | Règle |
| --------- | ------ | ----- |

## 7. Protection des données
| Donnée | Transit | Repos | Rétention |

## 8. Contrôles réseau
[WAF, Private Endpoints, NSG]

## 9. Logging et audit
| Événement | Loggé | Rétention |
| --------- | ----- | --------- |

## 10. Conformité RGPD
[Base légale, droits, localisation EU]
```

#### 2. Diagramme de séquence — authentification

Produisez un diagramme Mermaid (login utilisateur → appel API protégée → accès SQL avec MI).

#### 3. Plan de gouvernance — `governance-plan.md`

Utilisez le gabarit de [`governance.md`](governance.md#9-plan-de-gouvernance--structure), adapté au projet.

Minimum :

- Politique IAM (rôles dev/staging/prod)
- Gestion secrets (Key Vault, rotation)
- 3 Azure Policies proposées
- Processus revue d'accès
- KPI sécurité (3 indicateurs)

#### 4. Checklist OWASP API Top 10

| # | Risque | Statut projet | Action si gap |
| - | ------ | ------------- | ------------- |
| API1 | Broken Object Level Authorization | | |
| API2 | Broken Authentication | | |
| API3 | Broken Object Property Level Authorization | | |
| ... | | | |

---

## Exercice 5 — Webhook sortant sécurisé (45 min)

Lorsqu'une facture est créée, l'API notifie le système du client via `POST https://client.com/webhook`.

Concevez la sécurité :

1. Comment le client **authentifie** que la requête vient bien de vous ?
2. Comment gérer la **rejouabilité** (retry) sans double traitement côté client ?
3. Que mettre dans les headers ?
4. Que faire si le endpoint client est down pendant 24 h ?

<details>
<summary>Piste</summary>

- Signature HMAC-SHA256 du body avec secret partagé (ou clé par tenant dans Key Vault)
- Header `X-Signature`, `X-Timestamp` (anti-replay), `X-Idempotency-Key`
- File Service Bus + retries exponentiels + DLQ
- Secret rotation via portail partenaire

</details>

---

## Exercice 6 — Revue de configuration (30 min)

Identifiez ce qui ne va pas :

```json
// appsettings.Production.json (committé dans Git)
{
  "ConnectionStrings": {
    "Default": "Server=sql.database.windows.net;User=sa;Password=SuperSecret123!"
  },
  "AzureAd": {
    "ClientSecret": "abc~secret-from-git"
  },
  "Cors": {
    "AllowedOrigins": ["*"]
  },
  "Logging": {
    "LogLevel": { "Default": "Trace" }
  }
}
```

Listez **au moins 6 problèmes** et la correction pour chacun.

---

## Exercice 7 — Design review sécurité (30 min)

| Question | ✓ / ✗ | Action si non |
| -------- | ----- | ------------- |
| Flux auth documenté (séquence) ? | | |
| Chaque API vérifie tenant depuis token ? | | |
| Secrets dans Key Vault ? | | |
| Managed Identity pour accès SQL/Storage ? | | |
| RBAC moindre privilège sur Azure ? | | |
| WAF / APIM en frontal ? | | |
| Logs sans PII ? | | |
| Plan incident défini ? | | |

---

## Livrables à rendre

| Fichier | Obligatoire |
| ------- | ----------- |
| `security-model.md` | Oui |
| `governance-plan.md` | Oui |
| Diagramme séquence auth | Oui |
| Checklist OWASP API | Recommandé |
| Réponses exercices 1–3 | Recommandé |

---

## Critères d'évaluation

| Critère | Attendu |
| ------- | ------- |
| AuthN/AuthZ | Flux OIDC clair, RBAC par rôle |
| Multi-tenant | Isolation tenant depuis token, pas paramètre client |
| Secrets | Key Vault + MI, rien en Git |
| Menaces | STRIDE ou équivalent sur composants clés |
| Gouvernance | IAM, policies, processus réalistes |
| Pragmatisme | Contrôles proportionnés au risque |

---

## Suite

Module suivant : [08 — System Design (cas réels)](../08-system-design-cases/README.md)
