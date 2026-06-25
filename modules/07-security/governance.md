# Gouvernance, Zero Trust et gestion des secrets

La **gouvernance** définit qui décide, qui contrôle et comment les règles de sécurité sont appliquées dans l'organisation et sur l'infrastructure cloud.

---

## 1. Security by design

La sécurité intégrée dès la conception coûte moins cher qu'un ajout tardif.

| Phase | Actions sécurité |
| ----- | -------------- |
| **Design** | Modèle de menaces, classification données |
| **Développement** | SAST, dépendances (Dependabot), secrets scan |
| **CI/CD** | Pipeline sécurisé, pas de secrets en clair |
| **Déploiement** | IaC review, policies Azure |
| **Opérations** | Monitoring sécurité, patching, incident response |

---

## 2. Zero Trust

Principe : **ne jamais faire confiance par défaut**, même à l'intérieur du périmètre réseau.

> « Never trust, always verify. »

### Piliers Microsoft Zero Trust

| Pilier | Principe |
| ------ | -------- |
| **Identités** | Vérifier explicitement chaque utilisateur et service |
| **Appareils** | Conformité device (Intune) |
| **Applications** | Contrôle d'accès, CASB |
| **Données** | Classification, chiffrement, DLP |
| **Infrastructure** | Micro-segmentation, monitoring |
| **Réseau** | Pas de confiance implicite au réseau interne |

### Implications architecture

```text
❌ Réseau interne = zone de confiance
   → API sans auth car « derrière le firewall »

✅ Chaque requête authentifiée et autorisée
   → mTLS service-to-service ou JWT
   → Private Endpoints + pas d'accès public aux données
```

### Matrice Zero Trust — avant / après

| Aspect | Traditionnel | Zero Trust |
| ------ | ------------ | ---------- |
| Accès réseau | VPN = accès total | Accès par ressource, conditionnel |
| Auth utilisateur | Une fois au login | Continue, MFA adaptatif |
| Service-to-service | IP whitelist | Managed Identity + RBAC |
| Données | Chiffrement optionnel | Chiffrement par défaut (repos + transit) |

---

## 3. Gestion des secrets

### Ce qu'il ne faut jamais faire

```text
❌ Connection string dans appsettings.json committé Git
❌ Client secret en variable d'environnement non chiffrée
❌ Clé API dans le code source
```

### Azure Key Vault

Stockage centralisé : secrets, clés, certificats.

```diagram
App Service / AKS / Function
        │
        │ Managed Identity (pas de credential)
        ▼
   Key Vault
   ├── sql-connection-string
   ├── redis-key
   ├── payment-api-key
   └── tls-certificate
```

### Configuration ASP.NET Core

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### Rotation des secrets

| Pratique | Détail |
| -------- | ------ |
| **Rotation automatique** | Key Vault + Azure Automation |
| **Double secret** | Nouveau secret actif, ancien valide 24 h |
| **Alerte expiration** | 30 jours avant expiration certificat |
| **Pas de rotation manuelle** | Processus documenté et automatisé |

### Scan des secrets dans le CI

- **GitHub Advanced Security**, **gitleaks**, **trufflehog**
- Bloquer le merge si secret détecté
- Révoquer immédiatement tout secret exposé

---

## 4. IAM — Identity and Access Management

### Niveaux IAM Azure

| Niveau | Exemple | Portée |
| ------ | ------- | ------ |
| **Management groups** | Production, Non-prod | Hiérarchie abonnements |
| **Subscriptions** | Sub-prod-eu | Facturation, quotas |
| **Resource groups** | rg-orders-prod | Groupe de ressources |
| **Resources** | sql-orders-prod | Ressource individuelle |

### RBAC Azure

```text
Principal (user, group, SP, MI)
  + Role (Owner, Contributor, Reader, custom)
  + Scope (/subscriptions/.../resourceGroups/rg-orders)
  = Permissions effectives
```

### Rôles courants

| Rôle | Usage |
| ---- | ----- |
| **Owner** | Contrôle total + assignation rôles |
| **Contributor** | Gérer ressources, pas IAM |
| **Reader** | Lecture seule |
| **Key Vault Secrets User** | Lire secrets (apps) |
| **SQL DB Contributor** | Gérer bases (ops, pas données) |

### Principe du moindre privilège

```text
❌ Contributor sur l'abonnement pour un développeur
✅ Reader sur prod + Contributor sur rg-dev uniquement
```

### Managed Identities

| Type | Usage |
| ---- | ----- |
| **System-assigned** | Liée à une ressource, cycle de vie lié |
| **User-assigned** | Partagée entre ressources |

```bicep
resource appService 'Microsoft.Web/sites@2022-03-01' = {
  identity: { type: 'SystemAssigned' }
}
```

Puis assigner le rôle `Key Vault Secrets User` à l'identité sur le Key Vault.

### PIM — Privileged Identity Management

Accès admin **just-in-time** : activation temporaire avec approbation et MFA.

---

## 5. Gouvernance cloud Azure

### Azure Policy

Règles d'conformité automatiques :

| Policy | Effet |
| ------ | ----- |
| Autoriser uniquement `West Europe` | Deny autres régions |
| Exiger tags `cost-center`, `env` | Deny si manquant |
| Interdire IP publiques sur SQL | Deny |
| Exiger TLS 1.2 minimum | Audit / Deny |

### Resource locks

- **CanNotDelete** : empêche suppression accidentelle
- **ReadOnly** : empêche modification

### Blueprints / Landing zones

Templates d'abonnement conforme (réseau, policies, logs centralisés).

### Tags et FinOps

```json
{
  "env": "prod",
  "project": "ecommerce",
  "cost-center": "CC-1234",
  "owner": "team-platform"
}
```

---

## 6. Classification des données

| Niveau | Exemple | Contrôles |
| ------ | ------- | --------- |
| **Public** | Documentation marketing | Aucun |
| **Interne** | Process internes | Auth employé |
| **Confidentiel** | Données clients, contrats | Chiffrement, RBAC strict |
| **Restreint** | Paiement, santé, secrets | Chiffrement, audit, accès minimal |

### RGPD (rappel architecture)

| Exigence | Implication technique |
| -------- | -------------------- |
| Minimisation | Ne stocker que le nécessaire |
| Droit à l'effacement | Procédure delete / anonymisation |
| Portabilité | Export structuré |
| Sécurité | Chiffrement, logs d'accès |
| DPA | Sous-traitants documentés (Azure) |

**Localisation :** données personnelles EU → régions EU (France Central, West Europe).

---

## 7. Sécurité réseau (complément module 5)

```diagram
Internet
   │
   ▼
[Front Door + WAF]
   │
   ▼
[APIM] ── Private Endpoint ──► [App Service]
                                    │
                    Private Endpoint ├─► SQL
                                    ├─► Key Vault
                                    └─► Redis
```

| Contrôle | Objectif |
| -------- | -------- |
| **NSG** | Filtrer trafic subnet |
| **Private Endpoints** | Pas d'exposition internet PaaS |
| **WAF** | OWASP Top 10, bot protection |
| **DDoS Protection** | Standard sur VNet prod |

---

## 8. Sécurité CI/CD

### Pipeline sécurisé

```yaml
# Exemple conceptuel
stages:
  - stage: Build
    steps:
      - task: DotNetCoreCLI@2
      - task: CredScan@3          # scan secrets
      - task: SonarQubeAnalyze    # SAST (optionnel)

  - stage: Deploy
    steps:
      - task: AzureKeyVault@2     # secrets au runtime
      - task: AzureWebApp@1
```

| Pratique | Détail |
| -------- | ------ |
| **OIDC federated credentials** | GitHub → Azure sans secret longue durée |
| **Environnements protégés** | Approbation manuelle prod |
| **Images signées** | ACR content trust |
| **SBOM** | Traçabilité dépendances |

---

## 9. Plan de gouvernance — structure

Gabarit pour le livrable du module :

```markdown
# Plan de gouvernance — [Organisation / Projet]

## 1. Périmètre et responsabilités
| Rôle | Responsable | Missions |
| ---- | ----------- | -------- |
| RSSI | | Politique sécurité |
| Tech Lead | | Architecture sécurisée |
| DPO | | Conformité RGPD |
| Ops | | IAM, patching |

## 2. Politique IAM
- Modèle RBAC Azure (rôles par environnement)
- PIM pour accès admin
- Revue des accès : trimestrielle

## 3. Gestion des secrets
- Key Vault par environnement
- Rotation : automatique / manuelle
- Scan CI : outil, fréquence

## 4. Politiques Azure
| Policy | Scope | Effet |
| ------ | ----- | ----- |

## 5. Classification données
| Type | Niveau | Stockage autorisé | Chiffrement |

## 6. Conformité
- RGPD : registre traitements, DPA Azure
- Audits : fréquence, périmètre

## 7. Gestion des incidents
- Détection : Defender for Cloud, SIEM
- Escalade : contacts, SLA
- Post-mortem : template blameless

## 8. Formation et sensibilisation
- Onboarding sécurité développeurs
- Phishing simulation

## 9. Indicateurs (KPI)
| KPI | Cible |
| --- | ----- |
| Vulnérabilités critiques ouvertes | 0 > 7 jours |
| % ressources avec tags obligatoires | 100 % |
| Revue accès à jour | 100 % / trimestre |
```

---

## 10. Microsoft Defender for Cloud

Posture de sécurité centralisée :

| Fonction | Description |
| -------- | ----------- |
| **Secure Score** | Recommandations priorisées |
| **Regulatory compliance** | ISO, SOC, RGPD dashboards |
| **Workload protection** | Plans Defender par ressource |
| **Alerts** | Menaces détectées |

Intégrer dans le processus : revue hebdomadaire des recommandations Critical/High.

---

## 11. Anti-patterns gouvernance

| Anti-pattern | Risque |
| ------------ | ------ |
| Compte Owner partagé | Pas de traçabilité |
| Secrets dans Git | Fuite, compromission |
| RBAC identique dev/prod | Erreur humaine en prod |
| Pas de revue d'accès | Accumulation privilèges |
| Policies sans exception documentée | Contournement informel |
| Logs sécurité non centralisés | Incident invisible |

---

## Pour aller plus loin

- [Authentification OAuth/OIDC](auth.md)
- [Exercices et modèle de sécurité](exercises.md)
- [Microsoft — Zero Trust](https://learn.microsoft.com/security/zero-trust/)
