using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.UserAuth;
using TourSite.Core.Entities;
namespace TourSite.APIs.Controllers;
[Authorize]
public class AppearanceController(UserManager<User> users) : BaseApiController
{
 private const string ClaimName = "carepoint:appearance";
 [HttpGet]
 public async Task<ActionResult<AppearanceDto>> Get()
 {
  var user = await users.GetUserAsync(User);
  if (user is null) return Unauthorized();
  var claim = (await users.GetClaimsAsync(user)).LastOrDefault(c => c.Type == ClaimName);
  if (claim is null) return Ok(new AppearanceDto());
  try { return Ok(JsonSerializer.Deserialize<AppearanceDto>(claim.Value) ?? new AppearanceDto()); }
  catch (JsonException) { return Ok(new AppearanceDto()); }
 }
 [HttpPut]
 public async Task<ActionResult<AppearanceDto>> Save([FromBody] AppearanceDto dto)
 {
  var user = await users.GetUserAsync(User);
  if (user is null) return Unauthorized();
  var claims = (await users.GetClaimsAsync(user)).Where(c => c.Type == ClaimName).ToArray();
  var claim = new Claim(ClaimName, JsonSerializer.Serialize(dto));
  var result = claims.Length == 0 ? await users.AddClaimAsync(user, claim) : await users.ReplaceClaimAsync(user, claims[0], claim);
  if (!result.Succeeded) return BadRequest(new {message = "Could not save appearance settings."});
  return Ok(dto);
 }
}
