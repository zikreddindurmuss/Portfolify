# Portfolify — Dev Infrastructure

## PostgreSQL (Docker)

### Başlatmak

```bash
docker-compose up -d
```

### Durdurmak

```bash
docker-compose down
```

> `down` komutu container'ı siler ama `postgres_data` volume'u korur — veriler kaybolmaz.

### Logları izlemek

```bash
docker-compose logs -f
```

---

### Mevcut container ile çakışma varsa

Daha önce elle oluşturulmuş `portfolify-db` container'ı varsa önce kaldırın:

```bash
docker stop portfolify-db && docker rm portfolify-db
```

Ardından `docker-compose up -d` ile yeniden ayağa kaldırın.
