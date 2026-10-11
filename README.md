# salon.fur

Website and booking system for salon.fur, a dog grooming salon in Oosterzele.

- **Public site:** services, before & after photos, Google reviews, contact details, opening hours and a map.
- **Online booking:** customers choose a service, a day and a free time slot. Slots follow the opening hours (Thu 9–18, Fri 9–18, Sat 9–12), the length of the service, existing bookings and closed days.
- **Admin** (`/admin`, one account): calendar of all bookings, booking details and deletion, editing services and prices, and closed days (holidays).

Stack: ASP.NET Core 9 MVC, EF Core with PostgreSQL (Npgsql), ASP.NET Core Identity. Local database in Docker, production database on Neon, hosting on Render.

## Project structure

| Project | Contents |
|---|---|
| `Salonfur.Web` | Startup project: controllers, view models (`ViewModels/`), views, static files |
| `Salonfur.Core` | Entities, `AppDbContext`, `Seeder`, EF Core migrations, booking logic (`Scheduling/`) |

Opening hours are in `Salonfur.Core/Scheduling/OpeningHours.cs`. Address, phone numbers and links are in `Salonfur.Web/SalonInfo.cs`.

## Run locally

Requirements: .NET 9 SDK, Docker Desktop, and the EF Core tool (`dotnet tool install --global dotnet-ef`).

1. Start the database:

   ```bash
   docker compose up -d
   ```

2. Set the secrets (once). Run these in the `Salonfur.Web` folder:

   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=salonfur;Username=postgres;Password=salonfur_dev"
   dotnet user-secrets set "Admin:Email" "you@example.com"
   dotnet user-secrets set "Admin:Password" "CHANGE_ME"
   ```

   The admin password needs at least 10 characters with an uppercase letter, a lowercase letter, a digit and a symbol.

3. Run the app (from the repo root), or open `Salonfur.sln` in Visual Studio and press F5:

   ```bash
   dotnet run --project Salonfur.Web
   ```

   Migrations run and the admin account is created automatically at startup. The site runs at http://localhost:5117, the admin at http://localhost:5117/admin.

The admin account is only created when it doesn't exist yet. Changing `Admin:Password` later does not change the password of an existing account.

## Migrations

Run these from the repo root. `Salonfur.Core` holds the migrations; `Salonfur.Web` is the startup project.

```bash
dotnet ef migrations add <Name> --project Salonfur.Core --startup-project Salonfur.Web --output-dir Data/Migrations
dotnet ef database update --project Salonfur.Core --startup-project Salonfur.Web
```

`database update` is optional, because the app applies pending migrations at startup.

## Deploy to Render

1. Create a project on [Neon](https://neon.tech). Copy the connection details and write them in Npgsql format (Neon shows a `postgresql://` URL that must be rewritten):

   ```
   Host=<endpoint>.neon.tech;Database=<db>;Username=<user>;Password=CHANGE_ME;SSL Mode=Require
   ```

2. In Render, choose **New → Blueprint** and select this repository. `render.yaml` defines a Docker web service with health check `/healthz`.
3. Fill in the environment variables:
   - `ConnectionStrings__DefaultConnection`: the Neon connection string from step 1
   - `Admin__Email`
   - `Admin__Password`
4. Deploy. The app listens on the `PORT` Render provides, applies migrations and creates the admin account at startup.

## Notes

- Google reviews are linked, not embedded. Showing review text on the page needs the Google Places API and an API key.
- The map is an OpenStreetMap embed, so no API key or tracking cookies are needed.
- Only strictly necessary cookies are used (form protection and admin login), so there is no cookie banner.
