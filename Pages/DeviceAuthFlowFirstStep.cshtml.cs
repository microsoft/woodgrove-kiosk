using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;
using woodgrovedemo_kiosk.Services;

namespace woodgrovedemo_kiosk.Pages
{
    public class DeviceAuthFlowFirstStepModel : PageModel
    {
        private readonly IConfiguration Configuration;

        public DeviceAuthFlowFirstStepModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!AuthHelper.ValidCredentials(this.Request))
            {
                return new UnauthorizedResult();
            }

            // Get the app settings
            string Authority = Configuration.GetSection("DeviceCodeFlow:Authority").Value!;
            string TenantId = Configuration.GetSection("DeviceCodeFlow:TenantId").Value!;
            string ClientId = Configuration.GetSection("DeviceCodeFlow:ClientId").Value!;

            

            string endpoint = $"{Authority}{TenantId}/oauth2/v2.0/devicecode";
            string userEndpoint = $"{Authority}common/oauth2/deviceauth";
            var client = new HttpClient();

            var data = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("scope", "openid profile")
            };

            var response = await client.PostAsync(endpoint, new FormUrlEncodedContent(data));

            var responseString = await response.Content.ReadAsStringAsync();
            responseString = responseString.Replace("https://microsoft.com/devicelogin", userEndpoint);
            // Temporary fix to user endpoint location
            return new OkObjectResult(responseString);
        }
    }
}
