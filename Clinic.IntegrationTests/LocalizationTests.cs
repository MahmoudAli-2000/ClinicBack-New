using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TourSite.Repository.Data;
using TourSite.Repository.Data.Contexts;
using Xunit;
public class LocalizationTests
{
 private static async Task<JsonElement> Body(HttpResponseMessage response) => JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();
 [Fact]
 public async Task Negotiates_header_query_validation_and_domain_errors()
 {
  await using var factory=new ClinicFactory(); await factory.Initialize(); using var client=factory.CreateClient();
  client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar-EG,ar;q=0.9,en;q=0.8");
  var invalid=await client.PostAsJsonAsync("/api/Auth/login", new {email="wrong@clinic.local",password="Wrong@123"});
  Assert.Equal(HttpStatusCode.Unauthorized,invalid.StatusCode);
  Assert.Equal("البريد الإلكتروني أو كلمة المرور غير صحيحة.",(await Body(invalid)).GetProperty("message").GetString());
  var english=await client.PostAsJsonAsync("/api/Auth/login?culture=en",new {email="wrong@clinic.local",password="Wrong@123"});
  Assert.Equal("Invalid email or password.",(await Body(english)).GetProperty("message").GetString());
  var validation=await client.PostAsJsonAsync("/api/Auth/login",new {email="invalid",password=""});
  Assert.Equal(HttpStatusCode.BadRequest,validation.StatusCode);
  Assert.Contains("قيمة الحقل",(await Body(validation)).GetProperty("errors")[0].GetString());
  var missing=await client.GetAsync("/api/Specialties/999999");Assert.Equal(HttpStatusCode.NotFound,missing.StatusCode);
  Assert.Equal("التخصص غير موجود.",(await Body(missing)).GetProperty("message").GetString());
 }
 [Fact]
 public async Task Language_is_validated_and_isolated_and_seed_is_bilingual_repeatable()
 {
  await using var factory=new ClinicFactory();await factory.Initialize();using var admin=factory.CreateClient();using var patient=factory.CreateClient();
  await admin.PostAsJsonAsync("/api/Auth/login",new {email="admin@clinic.local",password="Admin@123"});
  await patient.PostAsJsonAsync("/api/Auth/register",new {fullName="Language Patient",email="language@clinic.test",password="Testing@123"});
  Assert.True((await admin.PutAsJsonAsync("/api/Profile/language",new{language="ar"})).IsSuccessStatusCode);
  Assert.Equal("ar",(await Body(await admin.GetAsync("/api/Profile/language"))).GetProperty("language").GetString());
  Assert.Equal("en",(await Body(await patient.GetAsync("/api/Profile/language"))).GetProperty("language").GetString());
  Assert.Equal(HttpStatusCode.BadRequest,(await admin.PutAsJsonAsync("/api/Profile/language",new{language="de"})).StatusCode);
  using var scope=factory.Services.CreateScope();var db=scope.ServiceProvider.GetRequiredService<TourDbContext>();
  var count=await db.Specialties.CountAsync();await ClinicDbContextSeed.SeedBilingualCatalogAsync(db);await ClinicDbContextSeed.SeedBilingualCatalogAsync(db);
  Assert.Equal(count,await db.Specialties.CountAsync());Assert.All(await db.Specialties.ToListAsync(),s=>Assert.False(string.IsNullOrWhiteSpace(s.SpecialtyNameAr)));
  Assert.All(await db.Medications.ToListAsync(),m=>Assert.False(string.IsNullOrWhiteSpace(m.GenericNameAr)));
  admin.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar");
  var catalog=await Body(await admin.GetAsync("/api/Specialties/1")); Assert.Equal(catalog.GetProperty("specialtyNameAr").GetString(),catalog.GetProperty("displayName").GetString());
 }
 [Fact]
 public async Task Own_photo_rejects_invalid_upload_and_returns_image_with_name()
 {
  await using var factory=new ClinicFactory();await factory.Initialize();using var admin=factory.CreateClient();
  using(var forbidden=new MultipartFormDataContent()) {forbidden.Add(new ByteArrayContent(new byte[]{1}),"image","test.png");Assert.Equal(HttpStatusCode.Unauthorized,(await admin.PostAsync("/api/Profile/photo",forbidden)).StatusCode);}
  await admin.PostAsJsonAsync("/api/Auth/login",new {email="admin@clinic.local",password="Admin@123"});
  using(var bad=new MultipartFormDataContent()) {var bytes=new ByteArrayContent(new byte[]{1,2,3});bytes.Headers.ContentType=new("image/png");bad.Add(bytes,"image","test.png");Assert.Equal(HttpStatusCode.BadRequest,(await admin.PostAsync("/api/Profile/photo",bad)).StatusCode);}
  using var form=new MultipartFormDataContent();
  using var image=new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(10,10);using var stream=new MemoryStream();await SixLabors.ImageSharp.ImageExtensions.SaveAsPngAsync(image,stream);
  var content=new ByteArrayContent(stream.ToArray());content.Headers.ContentType=new("image/png");form.Add(content,"image","profile.png");
  var result=await admin.PostAsync("/api/Profile/photo",form);Assert.True(result.IsSuccessStatusCode,await result.Content.ReadAsStringAsync());var me=await Body(result);
  Assert.StartsWith("/images/profiles/",me.GetProperty("imageCover").GetString());Assert.False(string.IsNullOrWhiteSpace(me.GetProperty("fullName").GetString()));
  var reread=await Body(await admin.GetAsync("/api/Auth/me"));Assert.Equal(me.GetProperty("imageCover").GetString(),reread.GetProperty("imageCover").GetString());
 }
}
