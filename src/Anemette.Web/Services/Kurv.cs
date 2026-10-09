namespace Anemette.Web.Services;

/// <summary>
/// Indkøbskurven gemmes i en nødvendig cookie som "bogId:antal,bogId:antal" – uden personoplysninger.
/// </summary>
public class Kurv(IHttpContextAccessor http)
{
    public const string CookieNavn = "kurv";
    public const int MaksAntal = 20;

    private HttpContext Ctx => http.HttpContext ?? throw new InvalidOperationException("Ingen HTTP-forespørgsel.");

    public Dictionary<int, int> Indhold()
    {
        var resultat = new Dictionary<int, int>();
        var vaerdi = Ctx.Request.Cookies[CookieNavn];
        if (string.IsNullOrEmpty(vaerdi)) return resultat;
        foreach (var del in vaerdi.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var dele = del.Split(':');
            if (dele.Length == 2 && int.TryParse(dele[0], out var id) && int.TryParse(dele[1], out var antal) && antal > 0)
                resultat[id] = Math.Min(antal, MaksAntal);
        }
        return resultat;
    }

    public int AntalVarer() => Indhold().Values.Sum();

    public void Tilfoej(int bogId, int antal = 1)
    {
        var kurv = Indhold();
        kurv[bogId] = Math.Min(kurv.GetValueOrDefault(bogId) + antal, MaksAntal);
        Gem(kurv);
    }

    public void Saet(int bogId, int antal)
    {
        var kurv = Indhold();
        if (antal <= 0) kurv.Remove(bogId);
        else kurv[bogId] = Math.Min(antal, MaksAntal);
        Gem(kurv);
    }

    public void Toem() => Ctx.Response.Cookies.Delete(CookieNavn);

    private void Gem(Dictionary<int, int> kurv)
    {
        if (kurv.Count == 0)
        {
            Toem();
            return;
        }
        Ctx.Response.Cookies.Append(CookieNavn, string.Join(',', kurv.Select(k => $"{k.Key}:{k.Value}")), new CookieOptions
        {
            HttpOnly = true,
            Secure = Ctx.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
        });
    }
}
