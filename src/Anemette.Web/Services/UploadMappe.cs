namespace Anemette.Web.Services;

/// <summary>Mappen med bogforsider og billeder, som vises under /Uploads/.</summary>
public record UploadMappe(string Sti)
{
    private static readonly Dictionary<string, byte[][]> Signaturer = new()
    {
        [".jpg"] = [[0xFF, 0xD8, 0xFF]],
        [".jpeg"] = [[0xFF, 0xD8, 0xFF]],
        [".png"] = [[0x89, 0x50, 0x4E, 0x47]],
        [".gif"] = [[0x47, 0x49, 0x46, 0x38]],
        [".webp"] = [[0x52, 0x49, 0x46, 0x46]],
    };

    public static string Url(string? filnavn) =>
        string.IsNullOrEmpty(filnavn) ? "" : "/Uploads/" + Uri.EscapeDataString(filnavn);

    /// <summary>Gemmer et uploadet billede under et sikkert filnavn. Returnerer filnavnet eller en fejlbesked.</summary>
    public async Task<(string? Filnavn, string? Fejl)> GemBilledeAsync(IFormFile fil, string navnForslag)
    {
        var endelse = Path.GetExtension(fil.FileName).ToLowerInvariant();
        if (!Signaturer.TryGetValue(endelse, out var signaturer))
            return (null, "Billedet skal være en JPG-, PNG-, GIF- eller WebP-fil.");
        if (fil.Length is 0 or > 10 * 1024 * 1024)
            return (null, "Billedet må højst fylde 10 MB.");

        await using var strøm = fil.OpenReadStream();
        var start = new byte[8];
        var laest = await strøm.ReadAsync(start);
        if (!signaturer.Any(sig => laest >= sig.Length && start.AsSpan(0, sig.Length).SequenceEqual(sig)))
            return (null, "Filen ser ikke ud til at være et billede.");

        var filnavn = $"{Tekst.Slug(navnForslag)}-{Guid.NewGuid().ToString("N")[..8]}{endelse}";
        strøm.Position = 0;
        await using var ud = File.Create(Path.Combine(Sti, filnavn));
        await strøm.CopyToAsync(ud);
        return (filnavn, null);
    }
}
