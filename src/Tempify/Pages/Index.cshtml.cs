using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Codeuctivity.Pages
{
  public class IndexModel : PageModel
  {
    private readonly ILogger<IndexModel> _logger;
    public string Temperature { get; private set; } = "Unknown Temperature";
    public string TemperatureValuesChratified { get; private set; } = string.Empty;
    public string MesuareDateChratified { get; private set; } = string.Empty;

    public IndexModel(ILogger<IndexModel> logger)
    {
      _logger = logger;
      using (var db = new TemperDbContext())
      {
        var nfi = new NumberFormatInfo
        {
          NumberDecimalSeparator = "."
        };
        foreach (var item in db.MesureValues.Where(_ => DateTime.Compare(_.MesuareDateTime.AddDays(7), DateTime.Now) > 0).ToList())
        {
          MesuareDateChratified = MesuareDateChratified + "'" + item.MesuareDateTime.ToShortTimeString() + "',";
          TemperatureValuesChratified = TemperatureValuesChratified + item.Temperature.ToString("0.##", nfi) + ",";
        }
        TemperatureValuesChratified = TemperatureValuesChratified.TrimEnd(',');
        MesuareDateChratified = MesuareDateChratified.TrimEnd(',');
        db.SaveChanges();
      }
    }

    public void OnGet() => Temperature = TemperReader.GetTemperature().StringifiedResult;
  }
}