using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using TourSite.Core.Entities;
using TourSite.Repository.Data;
using TourSite.Repository.Data.Contexts;
using Xunit;

public class ClinicFactory : WebApplicationFactory<TourSite.APIs.Program>
{
 protected override void ConfigureWebHost(IWebHostBuilder builder)
 {
  builder.UseEnvironment("Development");
  builder.ConfigureServices(services => {
   services.RemoveAll<DbContextOptions<TourDbContext>>();
   services.RemoveAll<IDbContextOptionsConfiguration<TourDbContext>>();
   services.AddDbContext<TourDbContext>(o => o.UseInMemoryDatabase("clinic-tests-"+GetHashCode()).ConfigureWarnings(w=>w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
  });
 }
 public async Task Initialize()
 {
  using var scope=Services.CreateScope();var s=scope.ServiceProvider;
  var db=s.GetRequiredService<TourDbContext>();await db.Database.EnsureCreatedAsync();
  await ClinicDbContextSeed.SeedAsync(db,s.GetRequiredService<RoleManager<IdentityRole>>(),s.GetRequiredService<UserManager<User>>(),s.GetRequiredService<IConfiguration>(),s.GetRequiredService<ILogger<ClinicFactory>>());
 }
}
public class ApiTests
{
 private static async Task<JsonElement> Json(HttpResponseMessage response)
 {
  var text=await response.Content.ReadAsStringAsync();Assert.True(response.IsSuccessStatusCode,$"{response.StatusCode}: {text}");return JsonDocument.Parse(text).RootElement.Clone();
 }
 [Theory]
 [InlineData("admin@clinic.local","Admin@123","Admin")]
 [InlineData("manager@clinic.local","Password@123","Manager")]
 [InlineData("frontdesk@clinic.local","Password@123","Receptionist")]
 [InlineData("nurse@clinic.local","Password@123","Nurse")]
 [InlineData("ahmed.doctor@clinic.local","Password@123","Doctor")]
 public async Task Roles_can_login_and_access_authorized_resources(string email,string password,string role)
 {
  await using var factory=new ClinicFactory();await factory.Initialize();using var client=factory.CreateClient();
  await Json(await client.PostAsJsonAsync("/api/Auth/login",new {email,password}));
  var user=await Json(await client.GetAsync("/api/Auth/me"));Assert.Contains(role,user.GetProperty("roles").EnumerateArray().Select(r=>r.GetString()));
  await Json(await client.GetAsync("/api/Appointments"));await Json(await client.GetAsync("/api/Notifications/preferences"));
  if(role is "Admin" or "Manager") await Json(await client.GetAsync("/api/Users"));
  else Assert.Equal(HttpStatusCode.Forbidden,(await client.GetAsync("/api/Users")).StatusCode);
 }
 [Fact]
 public async Task Patient_registration_cookie_and_permissions()
 {
  await using var factory=new ClinicFactory();await factory.Initialize();using var client=factory.CreateClient();
  await Json(await client.PostAsJsonAsync("/api/Auth/register",new {fullName="Test Patient",email="test.patient@clinic.test",password="Testing@123"}));
  var me=await Json(await client.GetAsync("/api/Auth/me"));Assert.NotEqual(JsonValueKind.Null,me.GetProperty("patientId").ValueKind);
  Assert.Equal(HttpStatusCode.Forbidden,(await client.GetAsync("/api/Users")).StatusCode);
  await Json(await client.GetAsync("/api/Appointments"));await Json(await client.GetAsync("/api/Doctors"));
  using(var form=new MultipartFormDataContent()){form.Add(new StringContent("Forbidden"),"GenericName");Assert.Equal(HttpStatusCode.Forbidden,(await client.PostAsync("/api/Medications",form)).StatusCode);}
  await Json(await client.PostAsync("/api/Auth/refresh",null));
  await Json(await client.PostAsJsonAsync("/api/Auth/logout",new {}));Assert.Equal(HttpStatusCode.Unauthorized,(await client.GetAsync("/api/Auth/me")).StatusCode);
 }
 [Fact]
 public async Task Catalog_crud_and_enum_contract()
 {
  await using var factory=new ClinicFactory();await factory.Initialize();using var client=factory.CreateClient();
  await Json(await client.PostAsJsonAsync("/api/Auth/login",new {email="admin@clinic.local",password="Admin@123"}));
  var patients=await Json(await client.GetAsync("/api/Patients"));Assert.Equal(JsonValueKind.String,patients.GetProperty("data")[0].GetProperty("status").ValueKind);
  using var form=new MultipartFormDataContent();form.Add(new StringContent("Integration Test"),"GenericName");form.Add(new StringContent("10 mg"),"Strength");
  var med=await Json(await client.PostAsync("/api/Medications",form));var id=med.GetProperty("id").GetInt32();Assert.True(id>0);
  await Json(await client.PutAsync($"/api/Medications/{id}/status?isActive=false",null));
  var read=await Json(await client.GetAsync($"/api/Medications/{id}"));Assert.False(read.GetProperty("isActive").GetBoolean());
 }
}
