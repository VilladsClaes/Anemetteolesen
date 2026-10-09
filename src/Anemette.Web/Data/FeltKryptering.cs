using System.Security.Cryptography;
using System.Text;

namespace Anemette.Web.Data;

/// <summary>
/// Krypterer kunders personoplysninger (navn, adresse, e-mail ...) med AES-256-GCM, før de gemmes i databasen.
/// Nøglen ligger i konfigurationen (Kryptering:Noegle) og aldrig i databasen eller i Git.
/// Format: "v1:" + base64(nonce | tag | krypteret tekst).
/// </summary>
public sealed class FeltKryptering
{
    private const string Prefiks = "v1:";
    private const int NonceLaengde = 12;
    private const int TagLaengde = 16;
    private readonly byte[] _noegle;

    public FeltKryptering(byte[] noegle)
    {
        if (noegle.Length != 32)
            throw new ArgumentException("Krypteringsnøglen skal være 32 bytes (256 bit).", nameof(noegle));
        _noegle = noegle;
    }

    public static FeltKryptering FraKonfiguration(IConfiguration config, IHostEnvironment miljoe)
    {
        var base64 = config["Kryptering:Noegle"];
        if (string.IsNullOrWhiteSpace(base64))
        {
            if (!miljoe.IsDevelopment())
                throw new InvalidOperationException("Kryptering:Noegle mangler i konfigurationen. Se README.md.");
            // Kun til udvikling på egen maskine
            base64 = Convert.ToBase64String(SHA256.HashData("anemette-udvikling"u8));
        }
        return new FeltKryptering(Convert.FromBase64String(base64));
    }

    public string Krypter(string klartekst)
    {
        var data = Encoding.UTF8.GetBytes(klartekst);
        var resultat = new byte[NonceLaengde + TagLaengde + data.Length];
        var nonce = resultat.AsSpan(0, NonceLaengde);
        var tag = resultat.AsSpan(NonceLaengde, TagLaengde);
        var krypteret = resultat.AsSpan(NonceLaengde + TagLaengde);
        RandomNumberGenerator.Fill(nonce);
        using var aes = new AesGcm(_noegle, TagLaengde);
        aes.Encrypt(nonce, data, krypteret, tag);
        return Prefiks + Convert.ToBase64String(resultat);
    }

    public string Dekrypter(string vaerdi)
    {
        // Værdier uden prefiks er ikke krypterede (fx ældre data) og returneres uændret
        if (!vaerdi.StartsWith(Prefiks, StringComparison.Ordinal))
            return vaerdi;
        var bytes = Convert.FromBase64String(vaerdi[Prefiks.Length..]);
        if (bytes.Length < NonceLaengde + TagLaengde)
            throw new CryptographicException("Ugyldig krypteret værdi.");
        var nonce = bytes.AsSpan(0, NonceLaengde);
        var tag = bytes.AsSpan(NonceLaengde, TagLaengde);
        var krypteret = bytes.AsSpan(NonceLaengde + TagLaengde);
        var data = new byte[krypteret.Length];
        using var aes = new AesGcm(_noegle, TagLaengde);
        aes.Decrypt(nonce, krypteret, tag, data);
        return Encoding.UTF8.GetString(data);
    }
}
