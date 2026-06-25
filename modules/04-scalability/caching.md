# Cache et CDN

Ce document approfondit les stratégies de mise en cache et de distribution de contenu à la périphérie du réseau, complément du module 3 (invalidation, cache-aside).

---

## 1. Pourquoi cacher à l'échelle ?

Sans cache, chaque requête frappe la base ou un service coûteux :

```text
10 000 req/s × 5 ms DB = saturation rapide
10 000 req/s × 0,5 ms Redis (hit) = marge confortable
```

Le cache est souvent le **levier le plus rentable** pour la scalabilité des lectures.

| Niveau | Latence typique | Portée |
| ------ | --------------- | ------ |
| CPU L1/L2 | ns | Processus |
| Cache applicatif (mémoire) | µs | Instance |
| Redis / Memcached | 1–5 ms | Cluster partagé |
| CDN edge | 10–50 ms | Géographique |
| Base de données | 5–50 ms | Source de vérité |

---

## 2. Architecture multi-niveaux

```diagram
Client
   │
   ▼
┌──────┐
│ CDN  │  ← assets statiques, vidéo, pages statiques
└──┬───┘
   ▼
┌──────┐
│ API  │
└──┬───┘
   ├──► Cache local (mémoire) — données ultra-fréquentes, TTL court
   │
   └──► Redis cluster — cache partagé entre instances
              │
              ▼ miss
         PostgreSQL
```

### Cache local (in-process)

```csharp
// IMemoryCache — une instance uniquement
_cache.Set("config", config, TimeSpan.FromMinutes(5));
```

| Avantages | Inconvénients |
| --------- | ------------- |
| Très rapide | Pas partagé entre instances |
| Pas de réseau | Invalidation complexe multi-instances |

**Usage :** configuration, données de référence rarement modifiées.

### Cache distribué (Redis)

Partagé par toutes les instances API.

| Avantages | Inconvénients |
| --------- | ------------- |
| Cohérence entre instances | Latence réseau |
| Structures riches (hash, sorted set) | Point de défaillance → cluster / replica |

**Usage :** sessions, catalogue, résultats de requêtes coûteuses.

---

## 3. Redis à l'échelle

### Déploiement

| Mode | Description |
| ---- | ----------- |
| **Standalone** | Dev, faible charge |
| **Primary + replica** | Haute dispo lectures |
| **Cluster** | Sharding automatique, scale horizontal |

### Structures utiles

| Structure | Cas d'usage |
| --------- | ----------- |
| `STRING` | Objet JSON sérialisé, compteur |
| `HASH` | Champs d'une entité |
| `LIST` | File légère |
| `SET` | Tags uniques |
| `SORTED SET` | Classements, leaderboards |
| `EXPIRE` | TTL automatique |

### Compteur haute fréquence

```redis
INCR views:product:123
EXPIRE views:product:123 86400
```

Flush périodique vers PostgreSQL pour persistance.

### Leaderboard (sorted set)

```redis
ZADD leaderboard score user_id
ZREVRANGE leaderboard 0 9 WITHSCORES  # top 10
```

### Redis Cluster et hot keys

Une clé très populaire peut saturer un nœud. Solutions :

- **Replica reads** pour les lectures
- **Local cache** devant Redis pour les hot keys
- **Dériver la clé** : `hot:product:123:shard-{0..9}` avec agrégation

---

## 4. CDN (Content Delivery Network)

Réseau de serveurs **edge** proches des utilisateurs qui mettent en cache le contenu statique et parfois dynamique.

```text
Utilisateur (Paris) ──► Edge Paris ──► (miss) ──► Origin (Azure Blob / API)
                              │
                           (hit) → réponse immédiate
```

### Contenu adapté au CDN

| Type | Exemple | TTL |
| ---- | ------- | --- |
| Assets statiques | JS, CSS, images, fonts | Long (jours/semaines) |
| Vidéo | Segments HLS/DASH | Long |
| Pages HTML statiques | Landing marketing | Moyen |
| API dynamique | Données utilisateur | Court ou pas de CDN |

### Cache-Control HTTP

```text
# Asset versionné (hash dans le nom de fichier)
Cache-Control: public, max-age=31536000, immutable

# API ne doit pas être cachée par défaut
Cache-Control: private, no-store

# Catalogue peu volatile
Cache-Control: public, max-age=300, stale-while-revalidate=60
```

### Invalidation CDN

| Méthode | Description |
| ------- | ----------- |
| **TTL** | Expiration naturelle |
| **Purge API** | Azure CDN, CloudFront purge par URL ou pattern |
| **Versioning** | `app.v2.js` au lieu d'invalider `app.js` |
| **Cache busting** | Query string `?v=build-123` |

**Bonne pratique :** nommer les fichiers avec un hash de contenu (`main.a3f2b1.js`) → pas de purge nécessaire au déploiement.

### CDN dynamique / edge computing

- **Azure Front Door** : routage, WAF, cache dynamique
- **Cloudflare Workers** : logique à l'edge
- Utile pour personnalisation légère sans round-trip origin

---

## 5. Stratégies de cache par cas d'usage

### Catalogue e-commerce

```text
CDN (images) + Redis (fiche produit JSON) + PostgreSQL (source)
Hit ratio cible : 85–95 % sur Redis
```

### Fil d'actualité

```text
Timeline pré-calculée en Redis (ZSET par user)
Invalidation : push nouvel item + trim
```

### Page d'accueil

```text
Cache full-page Redis ou CDN avec TTL 30–60 s
Edge Side Includes (ESI) pour blocs personnalisés (rare aujourd'hui)
```

### API avec paramètres variables

Clé de cache = hash des paramètres pertinents :

```text
cache:search:sha256(category=shoes&page=1&sort=price)
```

Éviter de cacher les réponses utilisateur-spécifiques sans inclure `user_id` dans la clé.

---

## 6. Cache hit ratio et dimensionnement

### Hit ratio

```text
Hit ratio = hits / (hits + misses)
```

| Ratio | Interprétation |
| ----- | -------------- |
| < 70 % | TTL trop court, clés mal choisies, ou données trop volatiles |
| 80–90 % | Bon pour la plupart des APIs |
| > 95 % | Excellent ; vérifier que les misses ne tuent pas la DB |

### Dimensionner Redis

```text
Mémoire = nombre_clés × taille_moyenne_valeur × overhead (1,2–1,5)
```

Exemple : 100 000 produits × 5 Ko × 1,3 ≈ **650 Mo** → instance 1 Go.

### Surveiller

- `used_memory`, `evicted_keys` (si politique `allkeys-lru`)
- Latence `redis-cli --latency`
- Taux de hit côté application (métrique custom)

---

## 7. Patterns avancés

### Stale-while-revalidate

Servir une valeur **légèrement périmée** pendant le rechargement en arrière-plan.

```text
Cache-Control: max-age=60, stale-while-revalidate=300
```

Améliore la latence perçue sous charge.

### Read-through / write-through (rappel)

Voir module 3. À l'échelle, **cache-aside** reste le plus contrôlable.

### Materialized views

Pré-calculer des agrégations en base ou Redis :

```sql
-- Rafraîchie toutes les 5 min par un worker
CREATE MATERIALIZED VIEW sales_daily AS ...
```

Évite des scans lourds à la volée.

### Pre-warming

Avant un pic (lancement produit), charger le cache proactivement :

```text
Script → charge top 10 000 SKU dans Redis
```

---

## 8. CDN + API — architecture type

```diagram
                    ┌─────────────────┐
                    │ Azure Front Door│
                    │ (CDN + WAF + LB)│
                    └────────┬────────┘
              ┌──────────────┼──────────────┐
              ▼              ▼              ▼
        /static/*        /api/*         /media/*
        Blob + CDN      App Service     Blob vidéo
                          │
                          ▼
                    Redis Cache
                          │
                          ▼
                    PostgreSQL
```

---

## 9. Erreurs courantes

| Erreur | Conséquence | Correction |
| ------ | ----------- | ---------- |
| Cacher sans TTL | Mémoire saturée | Toujours définir TTL |
| Clé trop granulaire | Millions de clés, faible hit | Agréger (liste paginée) |
| Cacher erreurs 500 | Propagation de panne | Ne pas cacher les erreurs |
| Ignorer cache stampede | DB submergée | Single-flight, early expiration |
| CDN sur API privée | Fuite de données | `Cache-Control: private` |

---

## Pour aller plus loin

- [Performance et load balancing](performance.md)
- [Exercices](exercises.md)
- [Redis documentation](https://redis.io/docs/)
