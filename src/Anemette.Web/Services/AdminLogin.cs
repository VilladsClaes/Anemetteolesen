using System.Security.Cryptography;
using System.Text;
using Anemette.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Services;

/// <summary>
/// Kontrol af administratorers adgangskoder. Nye adgangskoder gemmes med PBKDF2 (ASP.NET Core PasswordHasher).
/// Brugere fra den gamle side har en SHA-256-hash med salt; den opgraderes automatisk ved første login.
/// </summary>
public class AdminLogin(AppDbContext db)
{
    public const int MindsteLaengde = 10;
    private static readonly PasswordHasher<Administrator> Hasher = new();

    public async Task<Administrator?> KontrollerAsync(string brugernavn, string adgangskode)
    {
        var admin = await db.Administratorer.FirstOrDefaultAsync(a => a.Brugernavn == brugernavn.Trim());
        if (admin is null)
        {
            // Samme tidsforbrug som ved et rigtigt forsøg, så man ikke kan gætte brugernavne
            Hasher.HashPassword(new Administrator(), adgangskode);
            return null;
        }

        bool ok;
        if (admin.AdgangskodeHash is not null)
        {
            var resultat = Hasher.VerifyHashedPassword(admin, admin.AdgangskodeHash, adgangskode);
            ok = resultat != PasswordVerificationResult.Failed;
            if (resultat == PasswordVerificationResult.SuccessRehashNeeded)
                admin.AdgangskodeHash = Hasher.HashPassword(admin, adgangskode);
        }
        else
        {
            ok = admin.GammelHash is not null && GammelHashMatcher(adgangskode, admin.GammelSalt ?? "", admin.GammelHash);
            if (ok)
            {
                admin.AdgangskodeHash = Hasher.HashPassword(admin, adgangskode);
                admin.GammelHash = admin.GammelSalt = null;
            }
        }

        if (!ok) return null;
        admin.SidstLoggetInd = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return admin;
    }

    public static void SaetAdgangskode(Administrator admin, string adgangskode)
    {
        admin.AdgangskodeHash = Hasher.HashPassword(admin, adgangskode);
        admin.GammelHash = admin.GammelSalt = null;
    }

    /// <summary>Den gamle sides metode: Base64(SHA-256(adgangskode + salt)).</summary>
    internal static bool GammelHashMatcher(string adgangskode, string salt, string forventet)
    {
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(adgangskode + salt)));
        return CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(hash), Encoding.ASCII.GetBytes(forventet));
    }
}
