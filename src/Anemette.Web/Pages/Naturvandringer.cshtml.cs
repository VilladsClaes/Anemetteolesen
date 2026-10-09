using Anemette.Web.Data;
using Anemette.Web.Pages.Shared;

namespace Anemette.Web.Pages;

public class NaturvandringerModel(AppDbContext db) : IndholdssideModel(db)
{
    public async Task OnGetAsync()
    {
        await HentSideAsync("naturvandringer");
        await HentArrangementerAsync(ArrangementType.Naturvandring, ArrangementType.Kursus);
    }
}
