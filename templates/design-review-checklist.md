# Checklist — Design review architecture

Utilisez cette checklist pour la revue du **projet final** ou de tout module de la formation.

**Projet :** _______________  
**Date :** _______________  
**Revue par :** _______________

---

## Clarification et périmètre

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | Besoins fonctionnels documentés | |
| ☐ | NFR chiffrés (charge, latence, dispo) | |
| ☐ | Hors scope explicite | |
| ☐ | Hypothèses listées | |

## Architecture

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | Diagramme C4 Context lisible | |
| ☐ | Diagramme C4 Container complet | |
| ☐ | Composants nommés et cohérents | |
| ☐ | Flux principaux documentés (séquence) | |
| ☐ | Pas de sur-ingénierie pour la charge visée | |

## Données

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | Choix SQL/NoSQL justifié | |
| ☐ | Stratégie cache et invalidation | |
| ☐ | Concurrence / stock traitée | |
| ☐ | Cohérence distribuée si microservices | |

## Scalabilité

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | Estimation req/s ou messages/s | |
| ☐ | Plan de montée en charge (phases) | |
| ☐ | Goulots identifiés | |

## Cloud Azure (si applicable)

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | Chaque composant mappé à un service Azure | |
| ☐ | Chiffrage estimé | |
| ☐ | RTO/RPO documentés | |
| ☐ | Région et conformité UE si PII | |

## Résilience et observabilité

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | Politique par dépendance externe | |
| ☐ | Health checks définis | |
| ☐ | SLO / alertes documentés | |
| ☐ | Dashboard ou spec monitoring | |

## Sécurité

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | Flux auth documenté | |
| ☐ | RBAC / ownership sur ressources | |
| ☐ | Secrets hors Git (Key Vault) | |
| ☐ | Pas de PII dans les logs | |

## Livraison

| ✓ | Critère | Commentaire |
| - | ------- | ----------- |
| ☐ | CI/CD décrit ou implémenté | |
| ☐ | DAT complet | |
| ☐ | Risques et mitigations | |
| ☐ | ADR pour décisions majeures | |

---

## Verdict

| Option | Cocher |
| ------ | ------ |
| ✅ Validé — prêt pour soutenance / prod | ☐ |
| 🔄 Itérations mineures requises | ☐ |
| ❌ Revue majeure nécessaire | ☐ |

**Points d'action :**

1.
2.
3.
