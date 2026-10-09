using Anemette.Web.Data;
using Anemette.Web.Pages.Shared;

namespace Anemette.Web.Pages;

public class ForedragModel(AppDbContext db) : IndholdssideModel(db)
{
    public async Task OnGetAsync()
    {
        await HentSideAsync("foredrag");
        await HentArrangementerAsync(ArrangementType.Foredrag);
    }
}
