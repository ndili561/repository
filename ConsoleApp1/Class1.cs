
using Newtonsoft.Json;

namespace InComm.Gateway.BL.Services.CloudVoids
{
    public class CustomerApplicationService : BaseService, ICustomerApplicationService
    {
        private readonly IHttpClient _httpClient;

        /// <summary>
        /// </summary>
        /// <param name="httpClient"></param>
        public CustomerApplicationService(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// </summary>
        /// <param name="customerApplicationId"></param>
        /// <returns></returns>
        public VblApplicationDto GetVblApplication(int customerApplicationId)
        {
            var url = $"{AllocationApiBaseUrl}/VBL/applications/GetApplication?applicationId={customerApplicationId}";

            using (var httpClient = _httpClient.CreateHttpClient())
            {
                var response = httpClient.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    LogApiMessage(url);

                    return null;
                }

                var jsonString = response.Content.ReadAsStringAsync().Result;
                var customerApplicationDetail = JsonConvert.DeserializeObject<VblApplicationDto>(jsonString);

                return customerApplicationDetail;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="customerApplication"></param>
        /// <returns></returns>
        public bool Save(VblApplicationDto customerApplication)
        {

            var url = $"{AllocationApiBaseUrl}/VBL/applications/UpdateApplicationStatus";
            var application = Mapper.Map<VBLApplication>(customerApplication);

            using (var httpClient = _httpClient.CreateHttpClient())
            {
                var response = httpClient.PostAsJsonAsync(url, application).Result;

                if (!response.IsSuccessStatusCode)
                {
                    LogApiMessage(url);

                    return false;
                }

                return true;
            }
        }
    }
}
