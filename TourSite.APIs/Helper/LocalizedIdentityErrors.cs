using Microsoft.AspNetCore.Identity;
using TourSite.Core.Localization;
namespace TourSite.APIs.Helper;
public class LocalizedIdentityErrors : IdentityErrorDescriber
{
 private IdentityError Local(IdentityError error) { if(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar") error.Description=Messages.Error(Messages.Get(error.Code),400); return error; }
 public override IdentityError PasswordTooShort(int length) => Local(base.PasswordTooShort(length));
 public override IdentityError PasswordRequiresDigit() => Local(base.PasswordRequiresDigit());
 public override IdentityError PasswordRequiresLower() => Local(base.PasswordRequiresLower());
 public override IdentityError PasswordRequiresUpper() => Local(base.PasswordRequiresUpper());
 public override IdentityError PasswordRequiresNonAlphanumeric() => Local(base.PasswordRequiresNonAlphanumeric());
 public override IdentityError PasswordMismatch() => Local(base.PasswordMismatch());
 public override IdentityError DuplicateEmail(string email) => Local(base.DuplicateEmail(email));
 public override IdentityError DuplicateUserName(string userName) => Local(base.DuplicateUserName(userName));
 public override IdentityError InvalidToken() => Local(base.InvalidToken());
}
