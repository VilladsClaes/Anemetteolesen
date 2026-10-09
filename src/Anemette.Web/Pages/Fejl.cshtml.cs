using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Anemette.Web.Pages;

[IgnoreAntiforgeryToken]
public class FejlModel : PageModel
{
    public int Kode { get; private set; }

    public void OnGet(int? kode) => Kode = kode ?? 500;
    public void OnPost(int? kode) => Kode = kode ?? 500;
}
