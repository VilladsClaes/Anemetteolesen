using System.Security.Cryptography;
using System.Text;
using Anemette.Web.Data;
using Anemette.Web.Services;
using Xunit;

namespace Anemette.Tests;

public class FeltKrypteringTests
{
    private static FeltKryptering Ny() => new(RandomNumberGenerator.GetBytes(32));

    [Fact]
    public void Krypteret_tekst_kan_dekrypteres_og_indeholder_ikke_klarteksten()
    {
        var k = Ny();
        var krypteret = k.Krypter("Grethe Ærø, Kirkevej 3");
        Assert.StartsWith("v1:", krypteret);
        Assert.DoesNotContain("Grethe", krypteret);
        Assert.Equal("Grethe Ærø, Kirkevej 3", k.Dekrypter(krypteret));
    }

    [Fact]
    public void Samme_tekst_giver_forskellig_krypteret_vaerdi() =>
        Assert.NotEqual(Ny().Krypter("x"), Ny().Krypter("x"));

    [Fact]
    public void Aendret_krypteret_vaerdi_afvises()
    {
        var k = Ny();
        var bytes = Convert.FromBase64String(k.Krypter("hemmelig")[3..]);
        bytes[^1] ^= 1;
        Assert.ThrowsAny<CryptographicException>(() => k.Dekrypter("v1:" + Convert.ToBase64String(bytes)));
    }

    [Fact]
    public void Forkert_noegle_kan_ikke_dekryptere() =>
        Assert.ThrowsAny<CryptographicException>(() => Ny().Dekrypter(Ny().Krypter("hemmelig")));

    [Fact]
    public void Ukrypteret_vaerdi_returneres_uaendret() => Assert.Equal("gammel tekst", Ny().Dekrypter("gammel tekst"));
}

public class TekstTests
{
    [Theory]
    [InlineData("Æblekogebogen", "aeblekogebogen")]
    [InlineData("Mit frugt- og bærkøkken", "mit-frugt-og-baerkoekken")]
    [InlineData("Roser -ej blot til pynt", "roser-ej-blot-til-pynt")]
    [InlineData("Krydderurtehavens énere", "krydderurtehavens-enere")]
    [InlineData("Havens årskalender", "havens-aarskalender")]
    public void Slug_laver_laesbare_adresser(string titel, string forventet) => Assert.Equal(forventet, Tekst.Slug(titel));

    [Fact]
    public void Markdown_tillader_ikke_html()
    {
        var html = Tekst.TilHtml("<script>alert(1)</script> **fed**");
        Assert.DoesNotContain("<script>", html);
        Assert.Contains("<strong>fed</strong>", html);
    }

    [Fact]
    public void Kroner_vises_paa_dansk()
    {
        Assert.Equal("1.040 kr.", Tekst.Kr(1040m));
        Assert.Equal("99,95 kr.", Tekst.Kr(99.95m));
    }

    [Fact]
    public void Dansk_sortering_placerer_aeoeaa_til_sidst()
    {
        var titler = new[] { "Østens Urter", "Æblekogebogen", "Banankogebogen", "Zebra" }.Order(Tekst.DanskSortering).ToArray();
        Assert.Equal(["Banankogebogen", "Zebra", "Æblekogebogen", "Østens Urter"], titler);
    }
}

public class AdminLoginTests
{
    [Fact]
    public void Gammel_sha256_hash_fra_den_gamle_side_genkendes()
    {
        var salt = "abcSalt123";
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("GammelKode123" + salt)));
        Assert.True(AdminLogin.GammelHashMatcher("GammelKode123", salt, hash));
        Assert.False(AdminLogin.GammelHashMatcher("forkert", salt, hash));
    }
}
