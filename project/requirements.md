# Cahier des charges — ShopFlow

**Version :** 1.0  
**Date :** 2025-06-25  
**Statut :** Référence projet final

---

## 1. Présentation

### 1.1 Contexte

ShopFlow est une plateforme e-commerce B2C visant le marché européen (lancement France, extension UE). Elle permet à des consommateurs d'acheter des produits physiques en ligne et aux équipes internes de gérer le catalogue et les commandes.

### 1.2 Objectifs métier

| Objectif | Indicateur |
| -------- | ---------- |
| Lancer le MVP en 6 mois | Mise en prod catalogue + commande + paiement |
| Conversion | Taux panier → commande > 2,5 % |
| Satisfaction | NPS > 40 |
| Disponibilité | 99,9 % sur le parcours achat |

### 1.3 Périmètre MVP

**Inclus :**

- Catalogue produits (recherche, filtres, fiches)
- Compte client (inscription, connexion SSO entreprise optionnelle B2B futur)
- Panier et checkout
- Paiement carte (prestataire externe)
- Suivi commande et emails transactionnels
- Back-office admin (produits, commandes, stocks basiques)
- API publique partenaires (lecture catalogue — phase 1.1)

**Hors scope MVP :**

- Application mobile native (responsive web uniquement)
- Marketplace multi-vendeurs
- Programme fidélité
- Magasins physiques / click & collect

---

## 2. Acteurs

| Acteur | Description |
| ------ | ----------- |
| **Client** | Acheteur grand public |
| **Admin catalogue** | Gère produits, prix, stocks |
| **Admin support** | Consulte commandes, déclenche remboursements |
| **Partenaire API** | Intègre le catalogue (clé API) |
| **Système paiement** | Stripe (PSP externe) |
| **Système email** | SendGrid / Azure Communication Services |

---

## 3. Exigences fonctionnelles

### 3.1 Catalogue

| ID | Exigence | Priorité |
| -- | -------- | -------- |
| CAT-01 | Lister les produits par catégorie avec pagination | Must |
| CAT-02 | Rechercher par mot-clé et filtres (prix, marque) | Must |
| CAT-03 | Afficher fiche produit (images, description, stock) | Must |
| CAT-04 | Cache pour temps de réponse < 200 ms p95 | Must |

### 3.2 Panier et commande

| ID | Exigence | Priorité |
| -- | -------- | -------- |
| ORD-01 | Ajouter / modifier / supprimer lignes panier | Must |
| ORD-02 | Panier persistant (connecté ou session invité 7 j) | Must |
| ORD-03 | Créer commande avec adresse livraison | Must |
| ORD-04 | Réserver stock à la validation commande | Must |
| ORD-05 | Annuler commande avant expédition | Should |
| ORD-06 | Historique commandes client | Must |

### 3.3 Paiement

| ID | Exigence | Priorité |
| -- | -------- | -------- |
| PAY-01 | Paiement carte via token PSP (pas de PAN stocké) | Must |
| PAY-02 | Authorize à la commande, capture à l'expédition | Must |
| PAY-03 | Remboursement partiel / total (admin) | Must |
| PAY-04 | Idempotence sur création paiement | Must |

### 3.4 Administration

| ID | Exigence | Priorité |
| -- | -------- | -------- |
| ADM-01 | CRUD produits et catégories | Must |
| ADM-02 | Consultation et mise à jour statut commande | Must |
| ADM-03 | Rôles Admin / Support (RBAC) | Must |

### 3.5 Notifications

| ID | Exigence | Priorité |
| -- | -------- | -------- |
| NOT-01 | Email confirmation commande | Must |
| NOT-02 | Email expédition avec tracking | Should |

---

## 4. Exigences non fonctionnelles (NFR)

### 4.1 Charge (hypothèses de dimensionnement)

| Métrique | Valeur | Notes |
| -------- | ------ | ----- |
| Utilisateurs enregistrés | 500 000 (an 1) | |
| DAU | 50 000 | |
| Utilisateurs simultanés (pic) | 5 000 | Soldes, Black Friday |
| Pages vues / jour | 1 000 000 | |
| Commandes / jour | 3 000 | |
| Commandes / minute (pic) | 30 | |
| Catalogue SKU | 80 000 | |
| Ratio lecture / écriture | 100:1 | |

### 4.2 Performance

| Métrique | Cible |
| -------- | ----- |
| Latence API catalogue p95 | < 200 ms |
| Latence API checkout p95 | < 500 ms |
| Latence recherche p95 | < 300 ms |
| TTFB page web p95 | < 1 s |

### 4.3 Disponibilité et reprise

| Métrique | Cible |
| -------- | ----- |
| Disponibilité parcours achat | 99,9 % |
| RPO | 1 heure |
| RTO | 4 heures |
| Région primaire | West Europe |
| Région secondaire (DR) | North Europe |

### 4.4 Sécurité et conformité

- Authentification : Entra ID B2C ou External ID pour clients ; Entra ID workforce pour admins
- Données personnelles en UE (RGPD)
- PCI : scope réduit via Stripe Elements (SAQ A)
- Chiffrement transit (TLS 1.2+) et repos (Azure par défaut)
- Secrets dans Key Vault, Managed Identity

### 4.5 Observabilité

- Application Insights sur tous les services
- SLI : disponibilité, latence p95, taux erreur 5xx
- Alertes PagerDuty / Teams sur SLO
- Logs structurés JSON, rétention 90 j

---

## 5. Contraintes

| Type | Contrainte |
| ---- | ---------- |
| Technique | Stack .NET 8, Azure |
| Équipe | 8 développeurs, 2 ops, 1 architecte |
| Budget infra an 1 | ~3 000 € / mois (hors pics) |
| Délai MVP | 6 mois |
| Intégration | Stripe pour paiement |

---

## 6. User stories prioritaires (échantillon)

```text
En tant que client,
Je veux rechercher un produit et l'ajouter au panier,
Afin de préparer mon achat.

En tant que client,
Je veux payer par carte de manière sécurisée,
Afin de confirmer ma commande.

En tant qu'admin catalogue,
Je veux mettre à jour le stock d'un produit,
Afin que les clients ne commandent pas un article indisponible.

En tant que support,
Je veux rembourser une commande,
Afin de traiter une réclamation client.
```

---

## 7. Flux métier principal

```text
Parcours achat :
  Catalogue → Panier → Checkout → Paiement (authorize)
    → Commande confirmée → Email
    → Préparation → Capture paiement → Expédition → Email tracking

Parcours admin :
  Login Entra → CRUD produit → Mise à jour index recherche (async)
```

---

## 8. Hypothèses et dépendances

- Le PSP (Stripe) est disponible 99,99 % (dépendance externe)
- Un seul entrepôt au MVP (stock centralisé)
- Pas de gestion multi-devises (EUR uniquement)
- Livraison gérée hors système (statut manuel « expédié »)

---

## 9. Critères d'acceptation MVP

- [ ] Un client peut parcourir le catalogue et rechercher un produit
- [ ] Un client peut passer commande et payer par carte
- [ ] Le stock est décrémenté sans survente (concurrence gérée)
- [ ] Un admin peut créer un produit visible en < 5 min (async indexation)
- [ ] Monitoring et alertes opérationnels en production
- [ ] Pipeline CI/CD déploie en staging puis prod avec approbation
- [ ] DAT et diagrammes C4 validés en revue d'architecture

---

## 10. Références

- [DAT](dat/DAT.md)
- [Architecture](architecture/README.md)
- [Chiffrage](cost/azure-cost-estimate.md)
