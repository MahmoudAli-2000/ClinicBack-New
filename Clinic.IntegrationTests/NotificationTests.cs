using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TourSite.Repository.Data.Contexts;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using Xunit;
public class NotificationTests {
 private static async Task<JsonElement> Json(HttpResponseMessage r){Assert.True(r.IsSuccessStatusCode,await r.Content.ReadAsStringAsync());return JsonDocument.Parse(await r.Content.ReadAsStringAsync()).RootElement.Clone();}
 [Fact] public async Task Notifications_filter_hide_restore_delete_and_enforce_ownership(){
  await using var factory=new ClinicFactory();await factory.Initialize();using var admin=factory.CreateClient();using var other=factory.CreateClient();
  await admin.PostAsJsonAsync("/api/Auth/login",new{email="admin@clinic.local",password="Admin@123"});
  await other.PostAsJsonAsync("/api/Auth/register",new{fullName="Other Patient",email="notify@clinic.test",password="Testing@123"});
  var me=await Json(await admin.GetAsync("/api/Auth/me"));var uid=me.GetProperty("id").GetString()!;
  int id;
  using(var scope=factory.Services.CreateScope()) {var db=scope.ServiceProvider.GetRequiredService<TourDbContext>();var n=new Notification{UserId=uid,NotificationType="AppointmentReminder",Title="Reminder",Message="Test"};db.Notifications.Add(n);db.Notifications.Add(new Notification{UserId=uid,NotificationType="GeneralUpdate",Title="General",Message="Test"});await db.SaveChangesAsync();id=n.Id;}
  var reminders=await Json(await admin.GetAsync("/api/Notifications?category=reminders"));Assert.All(reminders.GetProperty("data").EnumerateArray(),n=>Assert.Equal("AppointmentReminder",n.GetProperty("notificationType").GetString()));
  Assert.Equal(HttpStatusCode.NotFound,(await other.DeleteAsync($"/api/Notifications/{id}")).StatusCode);
  Assert.Equal(HttpStatusCode.NotFound,(await other.PutAsync($"/api/Notifications/{id}/visibility?hidden=true",null)).StatusCode);
  await Json(await admin.PutAsync($"/api/Notifications/{id}/visibility?hidden=true",null));
  var hidden=await Json(await admin.GetAsync("/api/Notifications?hidden=true&category=reminders"));Assert.Contains(hidden.GetProperty("data").EnumerateArray(),n=>n.GetProperty("id").GetInt32()==id);
  var inbox=await Json(await admin.GetAsync("/api/Notifications?category=reminders"));Assert.DoesNotContain(inbox.GetProperty("data").EnumerateArray(),n=>n.GetProperty("id").GetInt32()==id);
  await Json(await admin.PutAsync($"/api/Notifications/{id}/visibility?hidden=false",null));
  await Json(await admin.DeleteAsync($"/api/Notifications/{id}"));Assert.Equal(HttpStatusCode.NotFound,(await admin.DeleteAsync($"/api/Notifications/{id}")).StatusCode);
  Assert.Equal(HttpStatusCode.BadRequest,(await admin.GetAsync("/api/Notifications?category=unknown")).StatusCode);
 }
 [Fact] public async Task Category_preferences_stop_new_notifications(){
  await using var factory=new ClinicFactory();await factory.Initialize();using var admin=factory.CreateClient();
  await admin.PostAsJsonAsync("/api/Auth/login",new{email="admin@clinic.local",password="Admin@123"});var me=await Json(await admin.GetAsync("/api/Auth/me"));var uid=me.GetProperty("id").GetString()!;
  await Json(await admin.PutAsJsonAsync("/api/Notifications/preferences",new {allowInApp=true,appointmentReminders=false,appointmentUpdates=false,generalUpdates=false}));
  using var scope=factory.Services.CreateScope();var db=scope.ServiceProvider.GetRequiredService<TourDbContext>();var before=await db.Notifications.CountAsync(n=>n.UserId==uid);var service=scope.ServiceProvider.GetRequiredService<INotificationService>();
  await service.PublishAsync(uid,"AppointmentReminder","Skipped","Test");await service.PublishAsync(uid,"AppointmentCreated","Skipped","Test");await service.PublishAsync(uid,"GeneralUpdate","Skipped","Test");
  Assert.Equal(before,await db.Notifications.CountAsync(n=>n.UserId==uid));
 }
}
