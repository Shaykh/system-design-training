# Dossier d'Architecture Technique (DAT)

**Projet :** _______________  
**Version :** 1.0  
**Auteur :** _______________  
**Date :** _______________  
**Classification :** Interne / Confidentiel

> Exemple complet : [project/dat/DAT.md](../project/dat/DAT.md) (ShopFlow)

---

## 1. Contexte

_Décrire le système, le métier, les utilisateurs, le contexte réglementaire._

- Problème adressé :
- Périmètre géographique :
- Contraintes connues :

---

## 2. Objectifs

### Objectifs métier

| Objectif | Indicateur |

### Objectifs architecturaux

| Objectif | Mesure de succès |

---

## 3. Architecture générale

### Style architectural

_Monolithe, microservices, event-driven…_

### Diagramme high-level

_Lien ou inclusion diagramme C4 Container_

### Bounded contexts / modules

| Contexte | Responsabilités |

### Flux métier principal

_Ex. : parcours commande en 5–8 étapes_

---

## 4. Choix techniques

| Composant | Technologie | Alternative écartée | Justification |
| --------- | ----------- | ------------------- | ------------- |

### Décisions d'architecture (ADR)

#### ADR-001 — [Titre]

- **Statut :** proposé / accepté
- **Contexte :**
- **Décision :**
- **Conséquences :**

---

## 5. Diagrammes

| Diagramme | Fichier / lien |
| --------- | -------------- |
| C4 Context | |
| C4 Container | |
| C4 Component | |
| Séquence (flux critique) | |

Template C4 : [c4-model-template.md](c4-model-template.md)

---

## 6. Sécurité

| Domaine | Mesure |
| ------- | ------ |
| Authentification | |
| Autorisation | |
| Réseau | |
| Données / chiffrement | |
| Secrets | |

_Détail : voir modèle de sécurité si document séparé_

---

## 7. Résilience

| Dépendance | Timeout | Retry | Circuit breaker | Fallback |
| ---------- | ------- | ----- | --------------- | -------- |

- RTO / RPO :
- Stratégie PRA :

---

## 8. Coûts

| Environnement | Coût mensuel estimé |
| ------------- | ------------------- |

_Lien vers chiffrage détaillé (Pricing Calculator, tableau)_

---

## 9. Risques

| ID | Risque | Probabilité | Impact | Mitigation |
| -- | ------ | ----------- | ------ | ---------- |

---

## 10. Historique et approbations

| Version | Date | Modifications |
| ------- | ---- | ------------- |

| Rôle | Nom | Date |
| ---- | --- | ---- |
| Architecte | | |
| Tech Lead | | |
| RSSI | | |
