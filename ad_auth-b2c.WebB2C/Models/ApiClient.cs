using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace LogCornerAuth.WebB2C.Models
{
    public class ApiClient
    {
        private readonly string _resourceId;
        private readonly string _authority;
        private readonly string _appId;
        private readonly string _appSecret;
        private readonly HttpClient _client;
        private bool _tokenSet;

        public ApiClient(HttpClient client,
            IConfiguration configuration)
        {
            _resourceId = configuration["Api:ResourceId"];
            _authority =
                $"{configuration["AzureAdB2C:Instance"]}{configuration["AzureAdB2C:TenantId"]}";

            _appId = configuration["AzureAdB2C:ClientId"];
            _appSecret = configuration["AzureAdB2C:ClientSecret"];

            this._client = client;
            this._client.BaseAddress = new Uri(configuration["Api:BaseUrl"]);
        }

        private async Task SetToken()
        {
            if (!_tokenSet)
            {
                try
                {
                    var authContext = new AuthenticationContext(_authority);
                    var credential = new ClientCredential(_appId, _appSecret);
                    var authResult = await authContext.AcquireTokenAsync(_resourceId, credential);
                    var token = authResult.AccessToken;

                    _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

                    _tokenSet = true;
                }
                catch (Exception ex)
                {

                }
            }
        }

        public async Task<string[]> GetValues()
        {
            await SetToken();

            var response = await _client.GetAsync("/api/values");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string[]>(content);
        }
    }
}