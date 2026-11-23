using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TourSite.Core.Localization;
namespace TourSite.APIs.Helper;
// Only standard response envelopes are translated. User-authored notes/data are never rewritten.
public class LocalizedMessageFilter : IResultFilter
{
 public void OnResultExecuting(ResultExecutingContext context)
 {
  if (context.Result is ObjectResult { Value: not null } result) {
   var properties = result.Value.GetType().GetProperties();
   if (properties.Length == 1 && properties[0].Name == "message" && properties[0].GetValue(result.Value) is string message)
    result.Value = new { message = Messages.Translate(message) };
  }
 }
 public void OnResultExecuted(ResultExecutedContext context) { }
}
