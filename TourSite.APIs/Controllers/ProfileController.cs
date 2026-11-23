using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using TourSite.Core.Entities;
using TourSite.Core.Localization;
using TourSite.Core.Servicies.Contract;
namespace TourSite.APIs.Controllers;
public class LanguagePreferenceDto { [Required, RegularExpression("^(en|ar)$")] public string Language { get; set; } = "en"; }
[Authorize]
public class ProfileController(UserManager<User> users, IWebHostEnvironment environment, IAuthService auth) : BaseApiController
{
 private const string LanguageClaim = "carepoint:language";
 [HttpGet("language")]
 public async Task<IActionResult> GetLanguage() {
  var user = await users.GetUserAsync(User); if(user is null) return Unauthorized();
  var value = (await users.GetClaimsAsync(user)).LastOrDefault(c => c.Type == LanguageClaim)?.Value;
  return Ok(new {language = value is "ar" ? "ar" : "en"});
 }
 [HttpPut("language")]
 public async Task<IActionResult> SetLanguage(LanguagePreferenceDto dto) {
  var user = await users.GetUserAsync(User); if(user is null) return Unauthorized();
  var claims = (await users.GetClaimsAsync(user)).Where(c => c.Type == LanguageClaim).ToArray();
  var claim = new Claim(LanguageClaim, dto.Language);
  var result = claims.Length == 0 ? await users.AddClaimAsync(user,claim) : await users.ReplaceClaimAsync(user,claims[0],claim);
  return result.Succeeded ? Ok(new {language = dto.Language}) : BadRequest(new {message = Messages.Get("BadRequest")});
 }
 [HttpPost("photo")]
 [RequestSizeLimit(2_200_000)]
 public async Task<IActionResult> UploadPhoto(IFormFile image, CancellationToken ct) {
  var user = await users.GetUserAsync(User); if(user is null) return Unauthorized();
  if(image is null || image.Length == 0 || image.Length > 2*1024*1024 || !new[]{"image/jpeg","image/png","image/webp"}.Contains(image.ContentType)) return BadRequest(new {message = Messages.Get("InvalidPhoto")});
  var webroot = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath,"wwwroot");
  var folder = Path.Combine(webroot,"images","profiles"); Directory.CreateDirectory(folder);
  var filename = Guid.NewGuid().ToString("N") + ".webp"; var output = Path.Combine(folder,filename);
  try {
   await using var stream = image.OpenReadStream();
   var info = await Image.IdentifyAsync(stream,ct);
   if(info.Width > 6000 || info.Height > 6000 || (long)info.Width*info.Height > 20_000_000) return BadRequest(new {message = Messages.Get("InvalidPhoto")});
   stream.Position=0;
   using var decoded = await Image.LoadAsync(stream,ct);
   if(decoded.Metadata.DecodedImageFormat?.Name is not ("JPEG" or "PNG" or "Webp" or "WEBP")) return BadRequest(new {message = Messages.Get("InvalidPhoto")});
   decoded.Mutate(x=>x.AutoOrient().Resize(new ResizeOptions {Size=new Size(512,512),Mode=ResizeMode.Crop}));
   decoded.Metadata.ExifProfile=null; decoded.Metadata.XmpProfile=null; decoded.Metadata.IccProfile=null;
   await decoded.SaveAsWebpAsync(output,new WebpEncoder {Quality=85},ct);
  } catch(Exception ex) when(ex is UnknownImageFormatException or InvalidImageContentException or NotSupportedException) { return BadRequest(new {message=Messages.Get("InvalidPhoto")}); }
  var previous=user.ImageCover; user.ImageCover="images/profiles/"+filename;
  var result=await users.UpdateAsync(user);
  if(!result.Succeeded) { System.IO.File.Delete(output); return BadRequest(new {message=Messages.Get("BadRequest")}); }
  if(previous.StartsWith("images/profiles/") && Path.GetFileName(previous) != filename) {
   var old = Path.Combine(folder,Path.GetFileName(previous)); if(System.IO.File.Exists(old)) System.IO.File.Delete(old);
  }
  return Ok(await auth.GetCurrentUserAsync(user.Id,ct));
 }
}
