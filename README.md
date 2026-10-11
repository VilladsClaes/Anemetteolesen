# Anemette Olesen – Skarresøhus Forlag

Kildekode til [www.anemetteolesen.dk](https://www.anemetteolesen.dk): hjemmesiden for forfatter, urtekyndig, foredragsholder og fotograf Anemette Olesen og hendes forlag.

Siden viser bøger (med bestilling via kurv og formular), foredrag, naturvandringer og kurser (med tilmelding), nyheder og kontaktoplysninger. Bag siden ligger en administration med lager, ordrer, salgstal, tilmeldinger og redigerbare tekster.

## Teknik

- **ASP.NET Core 10** (Razor Pages), udgivet som *self-contained* til Simply.com's Windows-webhotel (IIS, out-of-process)
- **MySQL** hos Simply.com via Entity Framework Core 10 (`MySql.EntityFrameworkCore`). Databasen opdateres automatisk med migreringer ved opstart.
- Ingen eksterne scripts, skrifttyper eller sporing. Skrifttyperne (Libre Baskerville og Source Serif 4, SIL OFL) ligger på siden selv.
- Kunders personoplysninger (navn, adresse, e-mail, telefon, beskeder) gemmes **krypteret** med AES-256-GCM. Nøglen ligger kun i konfigurationen.
- Mail sendes fra Anemettes Microsoft 365-postkasse via Microsoft Graph (se **Mail** nedenfor).

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
| `GRAPH_TENANT_ID`      | Microsoft 365: *Directory (tenant) ID* (se **Mail**)                                          |
| `GRAPH_KLIENT_ID`      | Microsoft 365: *Application (client) ID* for app-registreringen                               |
| `GRAPH_HEMMELIGHED`    | Microsoft 365: *Client secret* (Value). Udløber – se **Mail**.                                 |

Workflowet skriver disse værdier i `appsettings.Production.json` på serveren. Filen er aldrig i Git.

Fejl ved opstart på serveren kan ses i `logs/stdout*.log` via FTP.

Den gamle ASP.NET MVC-side kan findes i Git under tagget `gammel-side`.

## Mail

Domænets mail ligger hos **Microsoft 365** (MX peger på Outlook), og SPF-posten (`v=spf1 include:spf.protection.outlook.com -all`) tillader kun Microsoft at sende fra @anemetteolesen.dk. Derfor sender siden mails fra `anemette@anemetteolesen.dk` via **Microsoft Graph** med en app-registrering – ikke via SMTP. Uden opsætningen gemmes bestillinger, tilmeldinger og beskeder stadig og kan ses i administrationen.

### A. Opret app-registreringen (ca. 10 minutter)

Log ind som administrator for Microsoft 365-kontoen bag anemetteolesen.dk.

1. Gå til <https://entra.microsoft.com> → **Applications** → **App registrations** → **+ New registration**.
   - *Name*: `Hjemmesiden anemetteolesen.dk`
   - *Supported account types*: **Single tenant** (*Accounts in this organizational directory only*)
   - *Redirect URI*: lad den være tom. Klik **Register**.
2. På oversigten kopieres
   - **Application (client) ID** → GitHub-secret `GRAPH_KLIENT_ID`
   - **Directory (tenant) ID** → GitHub-secret `GRAPH_TENANT_ID`
3. **Certificates & secrets** → **Client secrets** → **+ New client secret**. *Description*: `GitHub`, *Expires*: **24 months** → **Add**.
   Kopiér straks kolonnen **Value** (ikke *Secret ID*) → GitHub-secret `GRAPH_HEMMELIGHED`.
   Sæt en påmindelse i kalenderen om ca. 23 måneder – så skal der laves en ny hemmelighed.
4. **API permissions** → **+ Add a permission** → **Microsoft Graph** → **Application permissions** → søg `Mail.Send` → sæt flueben → **Add permissions**.
   Klik derefter **Grant admin consent for …** → **Yes**. Status skal vise et grønt flueben.
   (Den forvalgte *User.Read* kan fjernes – den bruges ikke.)

### B. Begræns appen til kun Anemettes postkasse (anbefalet)

*Mail.Send* giver appen lov til at sende fra **alle** postkasser i Microsoft 365-kontoen. Er `anemette@` den eneste postkasse, er risikoen lille, men det er bedst at begrænse den. Det gøres i Exchange Online PowerShell:

1. I Entra: **Applications** → **Enterprise applications** → åbn *Hjemmesiden anemetteolesen.dk* → kopiér **Object ID** (fra *Enterprise applications* – ikke fra *App registrations*).
2. Åbn PowerShell og kør (indsæt dine id'er):
   ```powershell
   Install-Module ExchangeOnlineManagement -Scope CurrentUser
   Connect-ExchangeOnline
   New-ServicePrincipal -AppId <KLIENT-ID> -ObjectId <OBJECT-ID> -DisplayName "Hjemmesiden anemetteolesen.dk"
   New-ManagementScope -Name "Kun anemette" -RecipientRestrictionFilter "PrimarySmtpAddress -eq 'anemette@anemetteolesen.dk'"
   New-ManagementRoleAssignment -App <KLIENT-ID> -Role "Application Mail.Send" -CustomResourceScope "Kun anemette"
   ```
3. Fjern derefter *Mail.Send* igen under **API permissions** i Entra (… → **Remove permission**). Tilladelser i Entra og Exchange lægges sammen, så den brede tilladelse skal væk, for at begrænsningen virker.
4. Kontrollér (kan tage op til et par timer at slå igennem):
   ```powershell
   Test-ServicePrincipalAuthorization -Identity <KLIENT-ID> -Resource anemette@anemetteolesen.dk
   ```
   *Application Mail.Send* skal stå med *InScope* `True`.

### C. Gem i GitHub og test

1. GitHub → **Settings** → **Secrets and variables** → **Actions** → **New repository secret**: `GRAPH_TENANT_ID`, `GRAPH_KLIENT_ID`, `GRAPH_HEMMELIGHED`.
2. GitHub → **Actions** → **Byg, test og udgiv** → **Run workflow**.
3. Log ind på `/admin` og klik **Send en testmail**. Fejler den, står Microsofts begrundelse i `logs/stdout*.log` på serveren.

## Udvikling

Kræver .NET 10 SDK og en MySQL 8-database.

1. Ret forbindelsen i `src/Anemette.Web/appsettings.Development.json`.
2. `dotnet run --project src/Anemette.Web`
3. `dotnet test Anemette.slnx`

Ændringer i datamodellen: `dotnet tool restore` og derefter
`dotnet ef migrations add Navn --project src/Anemette.Web -o Data/Migreringer`.

## Persondata

Se siden *Privatliv og cookies*. Ved opstart anonymiseres ordrer ældre end 5 regnskabsår (bogføringsloven). Tilmeldinger slettes et år efter arrangementet og kontaktbeskeder efter to år. Siden bruger kun nødvendige cookies (kurv, formularsikkerhed og admin-login).
