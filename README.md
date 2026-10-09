# Anemette Olesen – Skarresøhus Forlag

Kildekode til [www.anemetteolesen.dk](https://www.anemetteolesen.dk): hjemmesiden for forfatter, urtekyndig, foredragsholder og fotograf Anemette Olesen og hendes forlag.

Siden viser bøger (med bestilling via kurv og formular), foredrag, naturvandringer og kurser (med tilmelding), nyheder og kontaktoplysninger. Bag siden ligger en administration med lager, ordrer, salgstal, tilmeldinger og redigerbare tekster.

## Teknik

- **ASP.NET Core 10** (Razor Pages), udgivet som *self-contained* til Simply.com's Windows-webhotel (IIS, out-of-process)
- **MySQL** hos Simply.com via Entity Framework Core 10 (`MySql.EntityFrameworkCore`). Databasen opdateres automatisk med migreringer ved opstart.
- Ingen eksterne scripts, skrifttyper eller sporing. Skrifttyperne (Libre Baskerville og Source Serif 4, SIL OFL) ligger på siden selv.
- Kunders personoplysninger (navn, adresse, e-mail, telefon, beskeder) gemmes **krypteret** med AES-256-GCM. Nøglen ligger kun i konfigurationen.
- Mail sendes via Simply.com's mailserver (`websmtp.simply.com`) med MailKit.

```
src/Anemette.Web/      Hjemmesiden
  Data/                Datamodel, kryptering, startdata og migreringer
  Services/            Kurv, bestilling, mail, login m.m.
  Pages/               Offentlige sider
  Pages/Admin/         Administration (kræver login)
  wwwroot/             CSS, skrifttyper og tegninger
tests/Anemette.Tests/  Enhedstests
Uploads/               Bogforsider og fotos (ligger også på serveren; uploades ikke ved udgivelse)
```

## Administration

Gå til **www.anemetteolesen.dk/admin** og log ind. Brugere fra den gamle side kan logge ind med deres gamle adgangskode, som så automatisk gemmes med sikker hashing.

- **Oversigt**: nye ordrer, salgstal pr. bog og måned, lavt lager og kommende arrangementer
- **Ordrer**: se bestillinger, markér som sendt eller annullér (bøgerne lægges tilbage på lageret)
- **Bøger og lager**: ret lagertal direkte i listen; ret pris, tekst, status og forsidebillede pr. bog
- **Arrangementer**: foredrag, naturvandringer og kurser med tilmeldingslister
- **Nyt**: artikler om ny viden og kommende udgivelser
- **Sidetekster**: teksterne på forsiden, Om Anemette, Foredrag osv.
- **Beskeder**, **Indstillinger** (kontaktoplysninger) og **Konto** (adgangskode og flere administratorer)

Er der ingen administratorer, kan den første oprettes direkte på login-siden.

## Udgivelse

Ved push til `main` tester, bygger og udgiver GitHub Actions (`.github/workflows/deploy.yml`) siden til `/anemetteolesen.dk/` på webhotellet via FTPS. Under upload sættes siden kortvarigt på pause med `app_offline.htm`. Pull requests bliver kun testet.

Repository secrets (**Settings → Secrets and variables → Actions**):

| Secret                 | Beskrivelse                                                                                   |
|------------------------|-----------------------------------------------------------------------------------------------|
| `FTP_SERVER`           | FTP-server, fx `ftp.simply.com`                                                               |
| `FTP_USERNAME`         | FTP-brugernavn                                                                                |
| `FTP_PASSWORD`         | FTP-adgangskode                                                                               |
| `DB_CONNECTION_STRING` | MySQL, fx `server=mysql94.unoeuro.com;database=DB;user id=BRUGER;password=KODE;CharSet=utf8mb4` |
| `KRYPTERING_NOEGLE`    | 32 tilfældige bytes i base64. **Mistes den, kan gemte kundeoplysninger ikke læses.**          |
| `SMTP_USER`            | Mailkonto hos Simply.com, fx `anemette@anemetteolesen.dk` (valgfri – uden den sendes ingen mails) |
| `SMTP_PASSWORD`        | Adgangskode til mailkontoen                                                                   |

Workflowet skriver disse værdier i `appsettings.Production.json` på serveren. Filen er aldrig i Git.

Fejl ved opstart på serveren kan ses i `logs/stdout*.log` via FTP.

Den gamle ASP.NET MVC-side kan findes i Git under tagget `gammel-side`.

## Udvikling

Kræver .NET 10 SDK og en MySQL 8-database.

1. Ret forbindelsen i `src/Anemette.Web/appsettings.Development.json`.
2. `dotnet run --project src/Anemette.Web`
3. `dotnet test Anemette.slnx`

Ændringer i datamodellen: `dotnet tool restore` og derefter
`dotnet ef migrations add Navn --project src/Anemette.Web -o Data/Migreringer`.

## Persondata

Se siden *Privatliv og cookies*. Ved opstart anonymiseres ordrer ældre end 5 regnskabsår (bogføringsloven). Tilmeldinger slettes et år efter arrangementet og kontaktbeskeder efter to år. Siden bruger kun nødvendige cookies (kurv, formularsikkerhed og admin-login).
