# Module 8 — System Design (cas réels)

**Durée :** 1 semaine · **Référence planning :** [Semaine 8](../../docs/planning.md#semaine-8--cas-réels-system-design)

---

## Objectifs

À la fin de ce module, vous serez capable de :

- Mener un entretien ou atelier de system design de bout en bout
- Poser les bonnes questions de clarification avant de dessiner
- Estimer charge, stockage et bande passante (ordres de grandeur)
- Produire un high-level design puis approfondir 2–3 composants critiques
- Argumenter les trade-offs et proposer des évolutions

---

## Méthodologie en 5 étapes

Chaque cas de ce module suit le même cadre — celui des entretiens system design et des revues d'architecture en entreprise.

```diagram
┌─────────────────┐
│ 1. Clarification│  Questions fonctionnelles + NFR
└────────┬────────┘
         ▼
┌─────────────────┐
│ 2. Estimation   │  Utilisateurs, QPS, stockage, bande passante
└────────┬────────┘
         ▼
┌─────────────────┐
│ 3. High-level   │  Diagramme composants, flux principaux
└────────┬────────┘
         ▼
┌─────────────────┐
│ 4. Deep dive    │  2–3 composants critiques en détail
└────────┬────────┘
         ▼
┌─────────────────┐
│ 5. Trade-offs   │  Alternatives, évolutions, risques
└─────────────────┘
```

### Grille de clarification (à réutiliser)

| Dimension | Questions types |
| --------- | --------------- |
| **Utilisateurs** | Combien ? Où ? Mobile / web ? |
| **Fonctionnel** | MVP vs complet ? Quelles features hors scope ? |
| **Lecture / écriture** | Ratio ? Temps réel obligatoire ? |
| **Latence** | Cible p95 ? |
| **Disponibilité** | SLA ? Tolérance aux partitions ? |
| **Cohérence** | Forte ou éventuelle acceptable ? |
| **Durée de rétention** | Messages, logs, transactions ? |
| **Sécurité** | Chiffrement E2E ? PCI-DSS ? |

### Grille d'estimation rapide

```text
DAU → requêtes/jour → requêtes/s (÷ 86400, × pic 3–5×)
Messages/jour × taille → stockage/jour × rétention
Utilisateurs simultanés × bande passante unitaire → Gbps
```

---

## Cas étudiés

| Cas | Fichier | Concepts clés |
| --- | ------- | ------------- |
| Messagerie instantanée | [`whatsapp.md`](whatsapp.md) | WebSocket, fan-out, E2E, présence |
| VTC / géolocalisation | [`uber.md`](uber.md) | Matching, géospatial, temps réel |
| Paiement | [`payment.md`](payment.md) | ACID, idempotence, ledger, PCI |
| Logs distribués | [`logging.md`](logging.md) | Ingestion massive, indexation, rétention |

---

## Parcours recommandé

```text
Semaine 8 — Lun/Mar : whatsapp.md OU uber.md (travail + corrigé)
Semaine 8 — Jeu/Ven : payment.md OU logging.md
Pour chaque cas :
  1. Travailler sans lire le corrigé (45–60 min)
  2. Comparer avec la solution de référence
  3. Présentation 15 min (oral ou écrit)
```

**Objectif :** traiter **2 cas en profondeur** minimum pendant la semaine.

---

## Livrables

| Livrable | Description |
| -------- | ----------- |
| Design document | `design-[cas].md` par cas traité |
| Diagrammes | High-level + deep dive (Mermaid ou Draw.io) |
| Présentation | 15 min : contexte → design → trade-offs |

### Gabarit `design-[cas].md`

```markdown
# System Design — [Nom du cas]

## 1. Clarification
### Hypothèses retenues
### Hors scope

## 2. Estimation
| Métrique | Calcul | Résultat |

## 3. High-level design
[Diagramme + description]

## 4. Deep dive
### Composant A
### Composant B

## 5. Trade-offs
| Décision | Choix | Alternative | Pourquoi |

## 6. Évolutions
[Scale 10×, nouvelles features]
```

---

## Prérequis

- Modules 1 à 7 complétés (ou équivalent)
- Capacité à produire des diagrammes rapidement (Excalidraw, Mermaid)

---

## Préparation entretien

| Phase | Durée | Contenu |
| ----- | ----- | ------- |
| Clarification | 5–10 min | Questions, hypothèses écrites |
| Estimation | 5 min | Chiffres au tableau |
| HLD | 10–15 min | Diagramme principal |
| Deep dive | 15–20 min | Selon questions interviewer |
| Trade-offs | 5 min | « Et si on avait 10× la charge ? » |

---

## Modules adjacents

- Précédent : [Module 7 — Sécurité](../07-security/README.md)
- Suite : [Projet final](../../project/README.md)
