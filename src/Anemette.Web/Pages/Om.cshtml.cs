using Anemette.Web.Data;
using Anemette.Web.Pages.Shared;
using Microsoft.EntityFrameworkCore;

namespace Anemette.Web.Pages;

public class OmModel(AppDbContext db) : IndholdssideModel(db)
{
    public Side Forlaget { get; private set; } = new();

    public async Task OnGetAsync()
    {
        await HentSideAsync("om");
        Forlaget = await Db.Sider.AsNoTracking().FirstOrDefaultAsync(s => s.Noegle == "forlaget") ?? new Side();
    }
}
