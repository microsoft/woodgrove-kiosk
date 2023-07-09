namespace woodgrovedemo_kiosk.Services;
public class AuthHelper
{
    public static bool ValidateCredentials(HttpRequest req)
    {
        return (!string.IsNullOrEmpty(PrincipalName(req)));
    }

    public static string PrincipalName(HttpRequest req)
    {
        if (req.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-NAME", out var xMsClientPrincipalName))
        {
            return xMsClientPrincipalName;
        }

        return null;
    }
}