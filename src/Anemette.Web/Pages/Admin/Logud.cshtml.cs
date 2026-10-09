using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Anemette.Web.Pages.Admin;

public class LogudModel : PageModel
{
    public IActionResult OnGet() => Redirect("/admin");

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/");
    }
}
