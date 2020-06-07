using System;
using System.ComponentModel.DataAnnotations;

namespace Codeuctivity.Pages
{
  public class MesureValue
  {
    [Key]
    public DateTime MesuareDateTime { get; set; }

    public double Temperature { get; set; }
  }
}