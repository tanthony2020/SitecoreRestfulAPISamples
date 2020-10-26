private static readonly string StrTemplateId = ConfigurationManager.AppSettings["templateID"];
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

            // get children 
            var itemResult = client.GetAsync("sitecore/api/ssc/item/" + StrParentId + "/children?database=master").Result;

            Console.WriteLine(itemResult.StatusCode);        

            var result = itemResult.Content.ReadAsStringAsync().Result;
            
            var array = JArray.Parse(result);

            foreach (var c in array.Children())
            {
                // get values from sitecore
                var itemName = c.Children<JProperty>().FirstOrDefault(x => x.Name == "ItemName")?.Value.ToString();
            }

        }
    }
}