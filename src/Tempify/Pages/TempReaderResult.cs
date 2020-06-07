namespace Codeuctivity.Pages
{
  public class TempReaderResult
  {
    public string StringifiedResult { get; }
    public double? Temperature { get; }

    public TempReaderResult(string stringifiedResult, double? temperature)
    {
      StringifiedResult = stringifiedResult;
      Temperature = temperature;
    }
  }
}