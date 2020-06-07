using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Codeuctivity.Pages
{
  public class HistoryModel : PageModel
  {
    private readonly Codeuctivity.Pages.TemperDbContext _context;

    public HistoryModel(Codeuctivity.Pages.TemperDbContext context)
    {
      _context = context;
    }

    public IList<MesureValue> MesureValue { get; set; }

    public async Task OnGetAsync() => MesureValue = await _context.MesureValues.ToListAsync();
  }
}