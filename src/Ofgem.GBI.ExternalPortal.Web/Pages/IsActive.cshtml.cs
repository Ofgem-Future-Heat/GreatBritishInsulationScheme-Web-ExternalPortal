using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ofgem.GBI.ExternalPortal.Web.Models;

namespace Ofgem.GBI.ExternalPortal.Web.Pages
{
    [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
    public class IsActiveModel : PageModel
    {
    }
}
