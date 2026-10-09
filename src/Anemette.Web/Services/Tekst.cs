using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Markdig;

namespace Anemette.Web.Services;

public static partial class Tekst
{
    public static readonly CultureInfo Dansk = CultureInfo.GetCultureInfo("da-DK");
    public static readonly StringComparer DanskSortering = StringComparer.Create(Dansk, ignoreCase: true);

    /// <summary>Laver en læsbar web-adresse af en titel: "Æblekogebogen" → "aeblekogebogen".</summary>
    public static string Slug(string tekst)
    {
        var sb = new StringBuilder();
        foreach (var tegn in tekst.Trim().ToLower(Dansk))
        {
            sb.Append(tegn switch
            {
                'æ' => "ae",
                'ø' => "oe",
                'å' => "aa",
                'é' or 'è' or 'ê' => "e",
                'ü' => "u",
                'ö' => "o",
                'ä' => "a",
                _ => char.IsAsciiLetterOrDigit(tegn) ? tegn.ToString() : "-",
            });
        }
        var slug = BindestregRegex().Replace(sb.ToString(), "-").Trim('-');
        return slug.Length == 0 ? "side" : slug[..Math.Min(slug.Length, 200)];
    }

    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()          // ingen rå HTML fra admin-tekster
        .UseSoftlineBreakAsHardlineBreak()
        .UseAutoLinks()
        .Build();

    /// <summary>Gør admin-tekst (almindelig tekst med enkel markdown) til sikker HTML.</summary>
    public static string TilHtml(string? tekst) => string.IsNullOrWhiteSpace(tekst) ? "" : Markdown.ToHtml(tekst, Pipeline);

    private static readonly TimeZoneInfo DanskTidszone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

    /// <summary>Klokken i Danmark lige nu (serveren kan stå i en anden tidszone).</summary>
    public static DateTime DanskTidNu => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, DanskTidszone);

    /// <summary>Gør et tidspunkt gemt i UTC til dansk tid til visning.</summary>
    public static DateTime DanskTid(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), DanskTidszone);

    public static string Kr(decimal beloeb) => beloeb.ToString(beloeb % 1 == 0 ? "#,##0" : "#,##0.00", Dansk) + " kr.";

    public static string Dato(DateTime dato) => dato.ToString("dddd 'den' d. MMMM yyyy", Dansk);

    public static string DatoKort(DateTime dato) => dato.ToString("d. MMMM yyyy", Dansk);

    /// <summary>Det danske navn fra [Display(Name = ...)] på en enum-værdi.</summary>
    public static string Visningsnavn(this Enum vaerdi) =>
        vaerdi.GetType().GetField(vaerdi.ToString())?
            .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
            .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>().FirstOrDefault()?.Name ?? vaerdi.ToString();

    [GeneratedRegex("-{2,}")]
    private static partial Regex BindestregRegex();
}
