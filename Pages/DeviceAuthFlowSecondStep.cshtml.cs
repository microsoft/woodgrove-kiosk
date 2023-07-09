using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using woodgrovedemo_kiosk.Services;

namespace woodgrovedemo_kiosk.Pages
{
    public class DeviceAuthFlowSecondStepModel : PageModel
    {
        private readonly IConfiguration Configuration;

        public DeviceAuthFlowSecondStepModel(IConfiguration configuration)
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
            string TenantId = Configuration.GetSection("AzureAd:TenantId").Value!;
            string ClientId = Configuration.GetSection("DeviceCodeFlow:ClientId").Value!;

            string endpoint = $"{Authority}/{TenantId}/oauth2/v2.0/token";
            string device_code = Request.Query["device_code"]!;
            var client = new HttpClient();

            var data = new[]
            {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                new KeyValuePair<string, string>("device_code", device_code),

            };

            var response = await client.PostAsync(endpoint, new FormUrlEncodedContent(data));
            var responseString = await response.Content.ReadAsStringAsync();

            // Check for errors
            if (!response.IsSuccessStatusCode)
            {
                return new OkObjectResult(responseString);
            }

            DeviceAuthFlowResponse? deviceAuthFlowResponse =
                JsonSerializer.Deserialize<DeviceAuthFlowResponse>(responseString);

            var token = new JwtSecurityToken(deviceAuthFlowResponse!.id_token);
            string nameClaim = token.Claims.First(c => c.Type == "name").Value;

            return new OkObjectResult("{\"name\" : \"" + nameClaim + "\"}");
        }

    }

    public class DeviceAuthFlowResponse
    {
        public string token_type { get; set; } = "";
        public string scope { get; set; } = "";
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; } = "";
        public string id_token { get; set; } = "";
    }

}
