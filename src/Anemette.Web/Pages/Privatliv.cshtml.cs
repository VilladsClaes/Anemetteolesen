using Anemette.Web.Data;
using Anemette.Web.Pages.Shared;

namespace Anemette.Web.Pages;

public class PrivatlivModel(AppDbContext db) : IndholdssideModel(db)
{
    public Task OnGetAsync() => HentSideAsync("privatliv");
}
