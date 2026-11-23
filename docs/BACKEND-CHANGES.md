# Backend corrections

The backend business model and layers are preserved. Changes address verified frontend integration/security problems:

1. **String enum JSON**: statuses and booking sources now use names over JSON. The prior numeric output broke status labels and UI state transitions. Numeric enum input remains accepted by the default converter.
2. **JWT role mapping**: the role claim type now matches ASP.NET's inbound mapped role claim, restoring role-authorized endpoints.
3. **Cookie refresh**: login/register set an HttpOnly refresh cookie. Refresh can use that cookie, rotates tokens and renews the access cookie. Logout revokes the associated refresh token and clears both cookies. Existing JSON refresh-token input remains supported. Cookies use SameSite=Lax, with Secure outside Development. No tokens are put in JavaScript local storage.
4. **Medication writes**: restricted to Admin/Manager/Doctor/Nurse. Previously any authenticated patient could write the catalog. Read access is unchanged.
5. **Email callback origin**: `FrontendUrl` now overrides the older BaseUrl for recovery/confirmation routes.
6. **Local HTTP**: HTTPS redirection is skipped only in Development, matching the documented HTTP local launch and Angular proxy. Production retains redirection.
7. **SPA fallback**: enables client-route refresh if frontend assets are deployed under API wwwroot.
8. **Middleware**: removed a duplicate exception-middleware registration.
9. **AutoMapper**: compatible patch update from 15.0.1 to 15.1.4, tested with the API host.
10. **Repeatable checks**: added an isolated API integration test project, and Docker build/Compose examples.

## Remaining production caveats

- SQL Server is the supported runtime database. No SQL Server service was available during this build. Actual migrations, database constraints, query behavior and concurrent appointment booking need SQL Server validation.
- The existing seeder inserts demo clinic/account data on startup. Disable/replace this flow for production and rotate all demo credentials.
- The original ImageSharp 3.1.12 dependency has known audit advisories. A compatible minor upgrade to 3.2 was evaluated but introduced a Release-build license requirement, so it was not imposed. Decide on an appropriately licensed patched image-processing library/version before exposing image upload to untrusted users.
- Existing AutoMapper licensing requirements still apply. No license was purchased or accepted.
- Some nullable/compiler warnings remain from existing code.
- External notification channels need providers. In-app delivery is implemented; the UI does not imply that SMTP/SMS/WhatsApp/push are configured merely because their preference can be saved.
- Full penetration testing, CSRF posture, HTTPS/reverse-proxy headers, data-access audit logs and regulatory compliance are outside the verification performed here.
