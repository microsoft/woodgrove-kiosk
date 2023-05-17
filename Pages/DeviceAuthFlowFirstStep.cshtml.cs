using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace woodgrovedemo_kiosk.Pages
{
    [Authorize]
    public class DeviceAuthFlowFirstStepModel : PageModel
    {
        private readonly IConfiguration Configuration;

        public DeviceAuthFlowFirstStepModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the app settings
            string TenantId = Configuration.GetSection("DeviceCodeFlow:TenantId").Value!;
            string ClientId = Configuration.GetSection("DeviceCodeFlow:ClientId").Value!;

            string endPoint = $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/devicecode";
            var client = new HttpClient();

            var data = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("scope", "openid profile")
            };

            var response = await client.PostAsync(endPoint, new FormUrlEncodedContent(data));

            var responseString = await response.Content.ReadAsStringAsync();

            return new OkObjectResult(responseString);
        }
    }
}
