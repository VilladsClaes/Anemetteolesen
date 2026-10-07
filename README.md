# Anemetteolesen

Kildekode til [www.anemetteolesen.dk](https://www.anemetteolesen.dk) – en hjemmeside bygget i ASP.NET MVC 5 (.NET Framework 4.7.2).

## Teknologi

- ASP.NET MVC 5 / Razor
- Entity Framework 6
- Bootstrap 4 / jQuery

## Udvikling

Åbn `Anemette.csproj` i Visual Studio, gendan NuGet-pakker, og kør projektet (IIS Express).

## Deployment

Ved push til `main` bygger en GitHub Actions-workflow (`.github/workflows/deploy.yml`) projektet
og uploader de publicerede filer til webhotellet via FTPS.

Følgende repository secrets skal være sat under **Settings → Secrets and variables → Actions**:

| Secret          | Beskrivelse                                  |
|-----------------|-----------------------------------------------|
| `FTP_SERVER`    | FTP-serverens adresse, fx `ftp.simply.com`     |
| `FTP_USERNAME`  | FTP-brugernavn                                |
| `FTP_PASSWORD`  | FTP-adgangskode                               |

`Uploads/`-mappen er ekskluderet fra deployment, så brugeruploadede filer på serveren ikke
overskrives/slettes ved hver deployment.
