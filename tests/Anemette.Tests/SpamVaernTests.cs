using System.Globalization;
using Anemette.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Anemette.Tests;

public class SpamVaernTests
{
    private readonly EphemeralDataProtectionProvider _dp = new();
    private SpamVaern Ny() => new(_dp, NullLogger<SpamVaern>.Instance);

    /// <summary>Et tidsstempel, der ser ud til at være lavet for <paramref name="siden"/> siden.</summary>
    private string Stempel(TimeSpan siden) => _dp.CreateProtector("Anemette.Formularstempel")
        .Protect((DateTime.UtcNow - siden).Ticks.ToString(CultureInfo.InvariantCulture));

    private static FormCollection Form(string? stempel, string? faelde = null)
    {
        var felter = new Dictionary<string, StringValues>();
        if (stempel != null) felter[SpamVaern.FeltTid] = stempel;
        if (faelde != null) felter[SpamVaern.FeltFaelde] = faelde;
        return new FormCollection(felter);
    }

    [Theory]
    [InlineData("Grethe Hansen", "Foredrag i Ebeltoft", "Kære Anemette. Kan du holde et foredrag om lægeplanter i vores forening i marts? Vi er ca. 40 medlemmer. Venlig hilsen Grethe")]
    [InlineData("Bent Ø. Ærø", "Spørgsmål til bog", "Jeg har købt Spis dit ukrudt og vil gerne vide, om skvalderkål kan fryses. Se evt. www.haveselskabet.dk")]
    [InlineData("Inge", "Naturvandring", "Hej, er der plads til to mere på turen den 12.? Mvh Inge, tlf 20 30 40 50")]
    public void Almindelige_danske_beskeder_godkendes(string navn, string emne, string besked) =>
        Assert.False(SpamVaern.LignerSpam(navn, emne, besked));

    [Theory]
    [InlineData("Best SEO Agency", "Grow your business", "We can get your website on the first page of google")]
    [InlineData("http://cheap-pills.example", "Hello", "Nice site")]
    [InlineData("John", "Offer", "Visit https://a.example and https://b.example for great deals")]
    [InlineData("Мария", "Привет", "Здравствуйте, предлагаем продвижение вашего сайта в поисковых системах")]
    [InlineData("Lisa", "Hi", "Earn money with crypto trading today")]
    public void Typisk_spam_genkendes(string navn, string emne, string besked) =>
        Assert.True(SpamVaern.LignerSpam(navn, emne, besked));

    [Fact]
    public void Udfyldt_faeldefelt_er_robot() =>
        Assert.Equal(SpamVurdering.Robot, Ny().Vurder(Form(Stempel(TimeSpan.FromMinutes(1)), faelde: "http://spam.example"), "Ole", "Hej"));

    [Fact]
    public void Manglende_eller_forfalsket_stempel_er_robot()
    {
        Assert.Equal(SpamVurdering.Robot, Ny().Vurder(Form(null), "Ole", "Hej"));
        Assert.Equal(SpamVurdering.Robot, Ny().Vurder(Form("forfalsket"), "Ole", "Hej"));
    }

    [Fact]
    public void For_hurtig_indsendelse_er_robot() =>
        Assert.Equal(SpamVurdering.Robot, Ny().Vurder(Form(Ny().NytStempel()), "Ole", "Hej"));

    [Fact]
    public void Meget_gammel_side_skal_sendes_igen() =>
        Assert.Equal(SpamVurdering.ForGammel, Ny().Vurder(Form(Stempel(TimeSpan.FromHours(13))), "Ole", "Hej"));

    [Fact]
    public void Menneske_der_har_brugt_lidt_tid_godkendes() =>
        Assert.Equal(SpamVurdering.Ok, Ny().Vurder(Form(Stempel(TimeSpan.FromSeconds(40))), "Ole Jensen", "Bestilling", "Har I flere af Mit Grønne Apotek?"));

    [Fact]
    public void Spamindhold_afvises_selv_med_gyldigt_stempel() =>
        Assert.Equal(SpamVurdering.Indhold, Ny().Vurder(Form(Stempel(TimeSpan.FromMinutes(2))), "SEO expert", "Backlinks", "Cheap backlinks for your site"));
}
