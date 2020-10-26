private static readonly string StrTemplateId = ConfigurationManager.AppSettings["templateID"];
private static readonly string StrParentId = ConfigurationManager.AppSettings["ParentID"];

private static readonly string BaseAddress = ConfigurationManager.AppSettings["BaseAddress"];
private static readonly string Domain = ConfigurationManager.AppSettings["domain"];
private static readonly string Username = ConfigurationManager.AppSettings["username"];
private static readonly string Password = ConfigurationManager.AppSettings["password"];

namespace Sample
{
    public static class InsertUpdateItem
    {
        private static void InsertUpdate(CustomItem item)
        {
            var strNewItem = "";
            try
            {
                var baseAddress = new Uri(BaseAddress);
                using (var client = new HttpClient() {BaseAddress = baseAddress})
                {
                    strNewItem = "itemname".ToString();

                    var user = new SitecoreUser {domain = Domain, username = Username, password = Password};
                    var content = new StringContent(
                        JsonConvert.SerializeObject(user),
                        Encoding.UTF8,
                        "application/json");

                    var authResult = client.PostAsync("sitecore/api/ssc/auth/login", content).Result;

                    Console.WriteLine(authResult.StatusCode);

                    var parent = client.GetAsync("sitecore/api/ssc/item/"+StrParentId+"?database=master").Result;
                    var result = parent.Content.ReadAsStringAsync().Result;
                    var array = JObject.Parse(result);
                    var parentPath = array["ItemPath"].ToString();
                    var requestUri = "sitecore/api/ssc/item/?path=" + parentPath + "/" + strNewItem +
                                    "&database=master";
                    // get item by path
                    var itemResult = client.GetAsync(requestUri).Result;

                    Console.WriteLine(itemResult.StatusCode);

                    // does the exist
                    if (itemResult.StatusCode == HttpStatusCode.NotFound)
                    {
                        var jsonItem = JsonConvert.SerializeObject(CutomItem);
                        content = new StringContent(
                            jsonItem,
                            Encoding.UTF8,
                            "application/json");
                        var postUri = "sitecore/api/ssc/item/sitecore/content/FolderName?database=master";
                        var response = client.PostAsync(postUri, content).Result;
                    }
                    else if (itemResult.StatusCode == HttpStatusCode.OK)
                    {
                        var returnItem = itemResult.Content.ReadAsStringAsync().Result;
                        var returnObject = JObject.Parse(returnItem);
                        var returnId = returnObject["ItemID"].ToString();
                        // post changes to that ID
                        var sitecoreItem = BuildCustmItem(item); // just create a model to build the sitecore item based on template
                        sitecoreItem.ParentID = StrParentId; // must set parent id or it will not update
                        var jsonItem = JsonConvert.SerializeObject(item);
                        content = new StringContent(
                            jsonItem,
                            Encoding.UTF8,
                            "application/json");
                        var postUri = "sitecore/api/ssc/item/" + returnId + "?database=master";
                        var httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), postUri)
                        {
                            Content = content
                        };
                        
                        var response = client.SendAsync(httpRequestMessage).Result;
                        Console.WriteLine(response.StatusCode);
                    }
                }

            }
            catch (Exception ex)
            {
                // do something
            }
        }
    }
}