private static readonly string StrTemplateId = ConfigurationManager.AppSettings["TemplateID"];
private static readonly string StrParentId = ConfigurationManager.AppSettings["ParentID"];

private static readonly string BaseAddress = ConfigurationManager.AppSettings["BaseAddress"];
private static readonly string Domain = ConfigurationManager.AppSettings["domain"];
private static readonly string Username = ConfigurationManager.AppSettings["username"];
private static readonly string Password = ConfigurationManager.AppSettings["password"];

namespace Sample
{
    public static class Authenticate
    {
        using (var client = new HttpClient() { BaseAddress = baseAddress })
        {
            var user = new SitecoreUser { domain = Domain, username = Username, password = Password };
            var content = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var authResult = client.PostAsync("sitecore/api/ssc/auth/login", content).Result;

            Console.WriteLine(authResult.StatusCode);

        }
    }
}