using System.Globalization;
using System.Resources;
namespace TourSite.Core.Localization;
public static class Messages
{
 private static readonly ResourceManager Resources = new("TourSite.Core.Localization.Messages", typeof(Messages).Assembly);
 public static string Get(string key) => Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;
 public static string Translate(string text) => Resources.GetString(text, CultureInfo.CurrentUICulture) ?? text;
 public static string Error(string text, int status)
 {
  if (text.Any(c => c >= 0x0600 && c <= 0x06ff)) return text;
  var translated = Translate(text);
  if (translated != text || CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "ar") return translated;
  return Get(status switch {400 => "BadRequest",401 => "Unauthorized",403 => "Forbidden",404 => "NotFound",409 => "OperationConflict",_ => "UnexpectedError"});
 }
}
