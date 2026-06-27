# Portfolify — Mimari Dokümantasyon

## Proje Amacı

Portfolify, yazılımcıların GitHub, LinkedIn, blog ve sosyal medya hesaplarını tek bir dijital kartvizit sayfasında toplayan **multi-tenant SaaS platformudur**. Her kullanıcı `portfolify.app/{slug}` adresinde kendi profiline sahip olur.

**Uzun vadeli vizyon:** LinkedIn benzeri sosyal ağ — takip sistemi, yetenek onaylama (skill endorsement), post/makale paylaşımı.

---

## Katman Sorumlulukları

### Domain (`Portfolify.Domain`)
İş kurallarının yaşadığı katman. Framework bağımlılığı yoktur.

- **Entities/** — `User`, ileride `Post`, `Skill`, `Endorsement`
- **Interfaces/** — `IUserRepository` (persistence soyutlaması)
- **Events/** — `IDomainEvent` marker interface, ileride `UserRegisteredEvent`, `SkillEndorsedEvent`
- **Common/** — `Entity<TId>` base sınıfı (Id, CreatedAt, UpdatedAt, DomainEvents)

### Application (`Portfolify.Application`)
Uygulama iş akışları. Domain'i orkestre eder, altyapıyı interface üzerinden kullanır.

- **Features/Auth/Commands/** — `RegisterCommand`, `LoginCommand`
- **Features/Auth/Queries/** — `GetProfileQuery`
- **Common/** — `Result<T>`, `Error` (exception yerine explicit hata yönetimi), `DependencyInjection`

### Infrastructure (`Portfolify.Infrastructure`)
Teknik detaylar: veritabanı, JWT, şifreleme.

- **Persistence/** — `AppDbContext` (EF Core + PostgreSQL/Npgsql)
- **Repositories/** — `UserRepository` (`IUserRepository` implementasyonu)
- `DependencyInjection.cs` — servis kayıtları

### API (`Portfolify.API`)
HTTP katmanı. İnce tutulur; iş mantığı içermez.

- **Controllers/** — `AuthController`
- **Middleware/** — `GlobalExceptionMiddleware`
- `Program.cs` — DI kompozisyon kökü

---

## Klasör Yapısı

```
portfolify/
├── backend/
│   ├── Portfolify.sln
│   ├── src/
│   │   ├── Portfolify.Domain/
│   │   │   ├── Common/Entity.cs
│   │   │   ├── Entities/User.cs
│   │   │   ├── Events/IDomainEvent.cs
│   │   │   └── Interfaces/IUserRepository.cs
│   │   ├── Portfolify.Application/
│   │   │   ├── Common/
│   │   │   │   ├── Result.cs
│   │   │   │   ├── Error.cs
│   │   │   │   └── DependencyInjection.cs
│   │   │   └── Features/Auth/
│   │   │       ├── Commands/RegisterCommand.cs
│   │   │       ├── Commands/LoginCommand.cs
│   │   │       └── Queries/GetProfileQuery.cs
│   │   ├── Portfolify.Infrastructure/
│   │   │   ├── Persistence/AppDbContext.cs
│   │   │   ├── Repositories/UserRepository.cs
│   │   │   └── DependencyInjection.cs
│   │   └── Portfolify.API/
│   │       ├── Controllers/AuthController.cs
│   │       ├── Middleware/GlobalExceptionMiddleware.cs
│   │       ├── Program.cs
│   │       └── appsettings.json
│   └── tests/
│       └── Portfolify.UnitTests/
├── frontend/           ← Next.js 14 (Faz 2)
└── docs/
    └── architecture.md
```

---

## Kullanılan Pattern'ler

### Clean Architecture
Bağımlılık akışı tek yönlüdür: `API → Infrastructure → Application → Domain`

Domain hiçbir dış katmana bağımlı değildir.

### CQRS (MediatR)
Her use case ya `IRequest<Result<T>>` (Command veya Query) olarak modellenir:

```
POST /api/auth/register  →  RegisterCommand  →  RegisterCommandHandler
POST /api/auth/login     →  LoginCommand     →  LoginCommandHandler
GET  /api/profile/{slug} →  GetProfileQuery  →  GetProfileQueryHandler
```

### Result<T> Pattern
Exception tabanlı akış kontrol yerine explicit hata yönetimi:

```csharp
// Handler:
if (await _repo.ExistsByEmailAsync(command.Email))
    return Error.EmailAlreadyInUse;  // implicit conversion

// Controller:
var result = await _mediator.Send(command);
if (result.IsFailure)
    return BadRequest(result.Error);
return Ok(result.Value);
```

### Multi-Tenant Slug
Her kullanıcının benzersiz `slug`'ı public URL kimliğidir:
- `portfolify.app/john-doe` → John Doe'nun profili
- DB'de `UNIQUE INDEX ON users(slug)`
- Slug küçük harf, URL-safe

### Domain Events
`Entity<TId>` base sınıfı `_domainEvents` listesi taşır. Şimdilik kullanılmıyor; Faz 2'de MediatR `INotification` ile publish edilecek.

---

## Proje Referans Grafiği

```
Portfolify.API
    ├── Portfolify.Application
    │       └── Portfolify.Domain
    └── Portfolify.Infrastructure
            └── Portfolify.Application
                    └── Portfolify.Domain
```

---

## Sosyal Ağ Genişlemesi — Mimari Notlar (Faz 2+)

Mevcut iskelet bu genişlemeye hazır olacak şekilde tasarlandı:

| Özellik | Domain Eklemesi | Application Eklemesi |
|---|---|---|
| Takip sistemi | `Follow` entity, `UserFollowedEvent` | `FollowUserCommand`, `GetFollowersQuery` |
| Yetenek onaylama | `Skill`, `Endorsement` entity'leri | `EndorseSkillCommand` |
| Post/Makale | `Post` entity, `PostPublishedEvent` | `CreatePostCommand`, `GetFeedQuery` |
| Bildirimler | `Notification` entity | `GetNotificationsQuery` |

**Önemli:** Multi-tenant yapı şimdi slug üzerinden kurulduğu için, ileride her slug'ın kendi içeriği (feed, takipçiler) kolayca izole edilebilir.

---

## Geliştirme Ortamı Kurulumu

```bash
# PostgreSQL bağlantısını ayarla (appsettings.json)
# İlk migration:
dotnet ef migrations add InitialCreate \
  --project src/Portfolify.Infrastructure \
  --startup-project src/Portfolify.API

# Veritabanını güncelle:
dotnet ef database update \
  --project src/Portfolify.Infrastructure \
  --startup-project src/Portfolify.API

# API'yi başlat:
dotnet run --project src/Portfolify.API

# Swagger: https://localhost:5001/swagger
```

---

## Tech Stack

| Katman | Teknoloji |
|---|---|
| Backend framework | .NET 9, ASP.NET Core |
| ORM | Entity Framework Core 9 |
| Veritabanı | PostgreSQL (Npgsql) |
| Mesajlaşma | MediatR 12 (CQRS) |
| Kimlik doğrulama | JWT Bearer, BCrypt.Net-Next |
| API dokümantasyon | Swagger / Swashbuckle |
| Frontend | Next.js 14, TypeScript, Tailwind CSS, shadcn/ui |
| Depolama | Huawei Cloud OBS |
