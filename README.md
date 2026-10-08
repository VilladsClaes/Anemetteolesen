# Anemetteolesen

Kildekode til [www.anemetteolesen.dk](https://www.anemetteolesen.dk) – en hjemmeside bygget i ASP.NET MVC 5 (.NET Framework 4.7.2).

## Teknologi

- ASP.NET MVC 5 / Razor
- Entity Framework 6 (Code First mod MySQL via MySql.Data.EntityFramework)
- Bootstrap 4 / jQuery

## Udvikling

1. Kopiér `ConnectionStrings.config.example` til `ConnectionStrings.config` og udfyld databasens oplysninger.
2. (Valgfrit) Kopiér `Secrets.config.example` til `Secrets.config` og udfyld mail-login.
3. Åbn `Anemette.csproj` i Visual Studio, gendan NuGet-pakker, og kør projektet (IIS Express).

`ConnectionStrings.config` og `Secrets.config` indeholder adgangskoder og må aldrig committes (de står i `.gitignore`).

## Database

Sitet bruger en MySQL-database på webhotellet (webhotellets ene MSSQL-database bruges til andet).
Datamodellen er beskrevet i `Models/DatabaseEntities.cs`.

`Database/Anemette-mysql.sql` opretter alle tabeller. Kør det én gang på en tom MySQL-database,
fx i phpMyAdmin under fanen **SQL**.

Den første administrator oprettes ved at gå til `/Admin/OpretAdministrator`, mens der endnu ikke
findes nogen administratorer. Derefter kræver siden login (`/Home/Login`).

## Deployment

Ved push til `main` bygger en GitHub Actions-workflow (`.github/workflows/deploy.yml`) projektet
og uploader de publicerede filer til `/anemetteolesen.dk/` på webhotellet via FTPS.
Pull requests bliver kun bygget (ikke udgivet), så man kan se at koden kompilerer før merge.

Følgende repository secrets skal være sat under **Settings → Secrets and variables → Actions**:

| Secret                 | Beskrivelse                                                                 |
|------------------------|------------------------------------------------------------------------------|
| `FTP_SERVER`           | FTP-serverens adresse, fx `ftp.simply.com`                                   |
| `FTP_USERNAME`         | FTP-brugernavn                                                               |
| `FTP_PASSWORD`         | FTP-adgangskode                                                              |
| `DB_CONNECTION_STRING` | MySQL-forbindelse, fx `server=mysqlXX.unoeuro.com;database=DB;user id=BRUGER;password=KODE;CharSet=utf8mb4` |
| `SMTP_USER`            | Mailkonto som kontaktformular/bestillinger sendes fra (valgfri)              |
| `SMTP_PASSWORD`        | Adgangskode til mailkontoen (valgfri)                                        |

`Uploads/`-mappen er ekskluderet fra deployment, så brugeruploadede filer på serveren ikke
overskrives/slettes ved hver deployment.
