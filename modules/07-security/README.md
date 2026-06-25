# Module 7 — Sécurité & gouvernance

**Durée :** 1 semaine · **Référence planning :** [Semaine 7](../../docs/planning.md#semaine-7--sécurité--gouvernance)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Expliquer les flux OAuth 2.0 et OpenID Connect
- Intégrer Microsoft Entra ID pour l'authentification et l'autorisation
- Appliquer les principes Zero Trust à une architecture cloud
- Gérer les secrets avec Azure Key Vault et Managed Identity
- Modéliser IAM / RBAC pour utilisateurs, services et APIs
- Produire un modèle de sécurité et un plan de gouvernance exploitables

---

## Contenu

| Thème | Fichier |
| ----- | ------- |
| Authentification et autorisation | [`auth.md`](auth.md) |
| Zero Trust, secrets, IAM | [`governance.md`](governance.md) |
| Cas pratiques et livrables | [`exercises.md`](exercises.md) |

### Concepts couverts

- OAuth 2.0 / OpenID Connect / Entra ID
- Zero Trust
- Secrets management (Key Vault)
- IAM / RBAC / ABAC
- OWASP, conformité (RGPD)

---

## Parcours recommandé

```text
1. Lire auth.md (OAuth2, OIDC, JWT, Entra ID)
2. Lire governance.md (Zero Trust, secrets, RBAC)
3. Exercices 1 à 3 (flux, menaces, RBAC)
4. Cas pratique — sécuriser une architecture distribuée
5. Rédiger modèle de sécurité + plan de gouvernance
```

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Modèle de sécurité | `security-model.md` (menaces, contrôles, flux auth) |
| Plan de gouvernance | `governance-plan.md` (IAM, secrets, conformité) |
| Diagramme de séquence | Flux authentification / autorisation |

Les détails sont dans [`exercises.md`](exercises.md).

---

## Prérequis

- Modules 1 à 6 (architecture, Azure, APIM, observabilité)
- Notions HTTP (headers, cookies, HTTPS)
- Tenant Entra ID (Azure AD) pour tests optionnels

---

## Ressources

- [Microsoft Entra ID](https://learn.microsoft.com/entra/identity/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Ressources module 7](../../docs/ressources.md#module-7--sécurité--gouvernance)

---

## Modules adjacents

- Précédent : [Module 6 — Résilience](../06-resilience/README.md)
- Suivant : [Module 8 — Cas réels](../08-system-design-cases/README.md)
