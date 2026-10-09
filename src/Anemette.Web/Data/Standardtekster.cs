namespace Anemette.Web.Data;

/// <summary>
/// Teksterne siden starter med. De bygger på Anemettes egne tekster fra tidligere versioner af hjemmesiden
/// og kan alle rettes under Administration → Sidetekster.
/// </summary>
public static class Standardtekster
{
    public static readonly (string Noegle, string Titel, string Indhold)[] Sider =
    [
        ("forside", "Urter, natur og gamle haver", """
            Velkommen til Skarresøhus Forlag. Jeg hedder Anemette Olesen, og siden 1980'erne har jeg skrevet, fortalt og undervist om urter, lægeplanter, vilde spiselige planter og grøn mad.

            Her på siden kan du se mine bøger og bestille dem, finde mine foredrag og naturvandringer – og læse lidt om, hvad jeg går og arbejder med lige nu.
            """),

        ("om", "Om Anemette Olesen", """
            Jeg bor på Djursland, hvor forlaget har til huse i en nedlagt brugs i Skarresø. Haven er halvanden tønde land stor og fyldt med urter, frugttræer og bærbuske – og en hel del ukrudt, som jeg bestemt ikke fjerner, for meget af det kan spises.

            ## Forfatter
            Jeg har skrevet en lang række bøger om urter, lægeplanter, vilde spiselige planter og grøn mad. Min første kogebog, *Spis dit ukrudt*, blev til i 1984, da jeg var køkkenleder på Naturhøjskolen på Møn, og den er siden kommet i mange udgaver.

            ## Gartner og køkkenleder
            Min fortid som museumsgartner har skærpet min interesse for urternes medicinhistorie, og som tidligere køkkenleder og restauratør af sommerrestauranten Den Gamle Pavillon i Spøttrup har jeg altid haft fødderne solidt plantet i køkkenet.

            ## Foredragsholder og underviser
            I mange år har jeg arbejdet freelance med have og mad. Det er blevet til mange madkurser, naturvandringer og foredrag over hele landet.

            ## Fotograf
            Jeg fotograferer også selv – især planter, haver og natur.

            ## Andre erfaringer
            Som ulandsfrivillig i Kenya arbejdede jeg i to et halvt år med kvindegrupper og dyrkede grøntsager sammen med dem. Og som biavler skrev jeg *Den lille søde om honning*.
            """),

        ("foredrag", "Foredrag", """
            Jeg holder foredrag om mange forskellige emner, og jeg sammensætter gerne præcis det foredrag, I ønsker. Jeg kommer ud til jeres forening, højskole, bibliotek eller institution og viser billeder undervejs.

            Herunder er nogle af de emner, jeg oftest bliver spurgt om. Ring eller skriv, så finder vi en dato og en pris.

            ### Lægeplanter i natur og have
            Et billedforedrag om lægeplanternes historie. Hvad går signaturlæren ud på, og hvordan brugte man før i tiden de giftige planter? Hvilke planter var heksenes urter – og hvilke husråd er stadig brugbare i dag?

            ### De gamle danske køkkenurter
            For blot fyrre år siden dyrkede vi andre urter i køkkenhaven end i dag. Hør om havrerod, kinaskok, havemælde, natlys, syre og kokleare – og få frø med hjem af nogle af de glemte køkkenurter.

            ### Krydderurters historie og brug
            Alle vores krydderurter har en historie som lægeplanter. Hør hvordan de tørres, så de bevarer duften, og hvordan de bruges i maden. Med smagsprøver og opskrifter.

            ### Snapseurter
            Om brændevinens historie og de urter, man selv kan dyrke til at krydre snapsen med – fra valnød og pors til perikon og malurt.

            ### Blomster på menuen
            Mindst 60 spiselige blomster. Hvordan brugte vores bedstemødre violen, rosen, nelliken og liljen – og hvordan kandiserer man blomster?

            ### Mariaplanter
            Hvorfor hedder timian Vor Frues Sengehalm? Om de mange planter, der har fået Jomfru Marias navn, og legenderne bag.

            ### Den bivenlige have
            Hvilke planter foretrækker bierne, og hvordan kan haven indrettes, så den bliver mere bivenlig?

            ### Østens urter og frugter
            Mange af Østens krydderurter og grøntsager kan dyrkes i en dansk have. Mød fx perilla, mizuna, vandpeber og spaghettibønne.

            ### Kvinders liv i Kenya
            Om to et halvt år som ulandsfrivillig med kvindegrupper i Kenya – og om at dyrke te, kaffe, bananer, papaya og søde kartofler.
            """),

        ("naturvandringer", "Naturvandringer og kurser", """
            Tag med ud i naturen og lær de vilde planter at kende. På mine naturvandringer finder vi de spiselige planter, der vokser lige uden for døren, og jeg fortæller om, hvordan de er blevet brugt i mad og husråd gennem tiden.

            Jeg holder også kurser i vild mad, spiseblomster og krydderurter i have og køkken. Kurserne kan holdes i min have i Skarresø, hvor råvarerne kommer direkte fra jorden – eller jeg kommer ud til jer.

            Vil I have en naturvandring eller et kursus for jeres forening eller gruppe, så kontakt mig. Så finder vi et emne og en dato, der passer.
            """),

        ("forlaget", "Skarresøhus Forlag", """
            Skarresøhus Forlag udgiver først og fremmest bøger om urter – om ukrudt, lægeplanter, krydderurter og grøn mad.

            Jeg har oversat Hildegard af Bingens træbog, urtebog og dyrebog og bearbejdet Henrik Harpestrengs og Henrik Smids urtebøger til nutidigt dansk. Som gammel køkkenleder har jeg samlet opskrifter efter emne, så der er en kålkogebog, en bønnebog og bøger om krydderier, krydderurter og køkkenurter.

            Urternes historie handler også om de myter og legender, der knytter sig til planterne. Derfor finder du også bøger som *Marias planter* og *Plantelegender*.
            """),

        ("bestilling", "Sådan bestiller du", """
            Læg de bøger, du gerne vil have, i kurven, og udfyld dine oplysninger. Jeg pakker bøgerne og sender dem med posten sammen med en faktura. Porto lægges til prisen.

            Du kan også altid ringe eller skrive til mig, hvis du hellere vil bestille på den måde.
            """),

        ("privatliv", "Privatliv og cookies", """
            Skarresøhus Forlag v/ Anemette Olesen, Ballevej 27, 8550 Ryomgård, er dataansvarlig for de oplysninger, du giver på denne hjemmeside.

            ## Hvilke oplysninger gemmer jeg?
            - **Når du bestiller bøger:** navn, adresse, e-mail og telefonnummer, så jeg kan sende bøgerne og fakturaen og kontakte dig, hvis der er spørgsmål. Oplysningerne gemmes, så længe bogføringsloven kræver det (5 år efter regnskabsåret), og slettes derefter.
            - **Når du tilmelder dig et arrangement:** navn, e-mail og telefonnummer, så jeg kan kontakte dig om arrangementet. Oplysningerne slettes et år efter arrangementet.
            - **Når du skriver via kontaktformularen:** dit navn, din e-mail, eventuelt telefonnummer og din besked. Beskeden slettes senest to år efter.

            Oplysningerne gemmes krypteret og bruges kun til det, du har givet dem til. De bliver aldrig solgt eller givet videre til andre end dem, der er nødvendige for at sende dine bøger (fx PostNord).

            ## Dine rettigheder
            Du har ret til at få at vide, hvilke oplysninger jeg har om dig, og til at få dem rettet eller slettet. Skriv eller ring til mig, så hjælper jeg. Hvis du er utilfreds med, hvordan dine oplysninger behandles, kan du klage til Datatilsynet (datatilsynet.dk).

            ## Cookies
            Hjemmesiden bruger kun de cookies, der er nødvendige for, at den virker:
            - **Indkøbskurv** – husker hvilke bøger, du har lagt i kurven.
            - **Sikkerhed** – beskytter formularerne mod misbrug.
            - **Login** – bruges kun, når administratoren er logget ind.

            Der bruges ingen cookies til statistik, reklamer eller sporing, og siden henter ingen indhold fra andre firmaer. Derfor skal du ikke give samtykke til cookies.
            """),
    ];

    public static readonly (string Noegle, string Vaerdi)[] Indstillinger =
    [
        ("Navn", "Anemette Olesen"),
        ("Forlag", "Skarresøhus Forlag"),
        ("Adresse", "Ballevej 27"),
        ("Postnummer og by", "8550 Ryomgård"),
        ("Telefon", "23 40 43 09"),
        ("Email", "anemette@anemetteolesen.dk"),
        ("Ordrer sendes til", "anemette@anemetteolesen.dk"),
    ];

    /// <summary>Emner til bøgerne, så de er lette at finde. Bøger der ikke står her, får "Andre bøger".</summary>
    public static readonly Dictionary<string, string> Emner = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Banankogebogen"] = "Kogebøger og grøn mad",
        ["Fra Haven til Maven"] = "Kogebøger og grøn mad",
        ["Fuldkorn"] = "Kogebøger og grøn mad",
        ["Heksekogebogen"] = "Kogebøger og grøn mad",
        ["Kålkogebogen"] = "Kogebøger og grøn mad",
        ["Køkkenhavekogebogen"] = "Kogebøger og grøn mad",
        ["Mad med korn og frø"] = "Kogebøger og grøn mad",
        ["Mit frugt- og bærkøkken"] = "Kogebøger og grøn mad",
        ["Naturkogebog"] = "Kogebøger og grøn mad",
        ["Noget om Nødder"] = "Kogebøger og grøn mad",
        ["Noget om pålæg"] = "Kogebøger og grøn mad",
        ["Spis dit ukrudt"] = "Kogebøger og grøn mad",
        ["Tangkogebogen"] = "Kogebøger og grøn mad",
        ["Tørt og Godt"] = "Kogebøger og grøn mad",
        ["Vegetariske bønneretter"] = "Kogebøger og grøn mad",
        ["Velkommen i det grønne"] = "Kogebøger og grøn mad",
        ["Æblekogebogen"] = "Kogebøger og grøn mad",
        ["Økologi i køkkenet"] = "Kogebøger og grøn mad",
        ["Naturens kryddersnaps"] = "Kogebøger og grøn mad",
        ["Den lille søde om Honning"] = "Kogebøger og grøn mad",
        ["Min have er den rene barnemad"] = "Kogebøger og grøn mad",

        ["De gamle køkkenurter"] = "Urter og lægeplanter",
        ["Dufte"] = "Urter og lægeplanter",
        ["Duftende planter"] = "Urter og lægeplanter",
        ["Gamle Lægeurter"] = "Urter og lægeplanter",
        ["Havens køkkenapotek"] = "Urter og lægeplanter",
        ["Hekseurter"] = "Urter og lægeplanter",
        ["Klosterurter"] = "Urter og lægeplanter",
        ["Krydderurtehavens énere"] = "Urter og lægeplanter",
        ["Krydderurter i have og køkken"] = "Urter og lægeplanter",
        ["Lægeplanter i natur og have"] = "Urter og lægeplanter",
        ["Lægeurtehaven ved Borgen Spøttrup"] = "Urter og lægeplanter",
        ["Mit Grønne Apotek"] = "Urter og lægeplanter",
        ["Politikens bog om krydderurter"] = "Urter og lægeplanter",
        ["Urtete - til daglig glæde"] = "Urter og lægeplanter",
        ["Vitskøl Klosterhave"] = "Urter og lægeplanter",
        ["Østens Urter"] = "Urter og lægeplanter",

        ["Fabeldyr"] = "Historie og legender",
        ["Henrik Harpestreng"] = "Historie og legender",
        ["Henrik Smid"] = "Historie og legender",
        ["Hildegards dyrebog"] = "Historie og legender",
        ["Hildegards urtebog"] = "Historie og legender",
        ["Historier om bibelplanter"] = "Historie og legender",
        ["Marias planter"] = "Historie og legender",
        ["Physica"] = "Historie og legender",
        ["Plantelegender"] = "Historie og legender",

        ["Blomsterguide"] = "Have og natur",
        ["De Salttålende Planter"] = "Have og natur",
        ["Den giftige have"] = "Have og natur",
        ["Gamle potteplanter"] = "Have og natur",
        ["Havens årskalender"] = "Have og natur",
        ["Mælkebøtten"] = "Have og natur",
        ["Noget om høns"] = "Have og natur",
        ["Roser -ej blot til pynt"] = "Have og natur",
        ["Spiseblomster"] = "Have og natur",
        ["Spiselige Blomster"] = "Have og natur",
    };
}
