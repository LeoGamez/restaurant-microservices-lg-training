using Mango.Web.Model.DTOs;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        public ResponseDto responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new ResponseDto();
            this.httpClient = httpClient;
        }


        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MangoAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri= new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Clear();   
                if(apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),Encoding.UTF8,"application/json");
                }

                if (!String.IsNullOrEmpty(apiRequest.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization= new AuthenticationHeaderValue("Bearer",apiRequest.AccessToken);
                }

                var apiResponse = new HttpResponseMessage();
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                }

                apiResponse = await client.SendAsync(message);
                
                var apiContent= await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);

                return apiResponseDto;
                
            }
            catch (Exception e)
            {
                var serializedDto= JsonConvert.SerializeObject(new ResponseDto
                {
                    DisplayMessage="Error",
                    ErrorMessages = new List<string> { Convert.ToString(e.Message)},
                    IsSuccess=false
                });

                return  JsonConvert.DeserializeObject<T>(serializedDto);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }

      
    }
}
