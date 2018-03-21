using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;





using Newtonsoft.Json;

namespace InComm.Gateway.BL.Services.CloudVoids
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IHttpClient _httpClient;
        private readonly ILogging _logger;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVoidRepository _voidRepository;
        private readonly string _allocationApiUrl;

        public ContactService(IContactRepository contactRepository, IHttpClient httpClient, ILogging logger, ICustomerRepository customerRepository, IVoidRepository voidRepository)
        {
            _contactRepository = contactRepository;
            _httpClient = httpClient;
            _logger = logger;
            _customerRepository = customerRepository;
            _voidRepository = voidRepository;
            _allocationApiUrl = WebConfigurationManager.AppSettings["AllocationApiUrl"];
        }

        [Obsolete]
        public ForwardingAddressDTO GetForwardingAddressByVoidId(int voidId)
        {
            var contact = _contactRepository.GetContacts(voidId).FirstOrDefault();

            return Mapper.Map<ForwardingAddressDTO>(contact);
        }

        [Obsolete]
        public bool SaveForwardingAddress(ForwardingAddressDTO forwardingAddress)
        {
            var currentContact = _contactRepository.GetContacts(forwardingAddress.VoidId).FirstOrDefault();

            if (currentContact != null)
            {
                currentContact = Mapper.Map(forwardingAddress, currentContact);

                return _contactRepository.SaveContact(currentContact);
            }

            var voidObj = _voidRepository.GetVoidDetailByVoidId(forwardingAddress.VoidId);
            var tenant = _customerRepository.
                GetCustomersByPropertyCode(voidObj?.PropertyCode)?
                .FirstOrDefault(c => c.MainTenantFlag == "Y");
            currentContact = Mapper.Map<Contact>(forwardingAddress);

            if (tenant != null)
            {
                currentContact.DOB = tenant.DateOfBirth;
                currentContact.Ethnicity = tenant.Ethnicity;
                currentContact.Gender = tenant.Gender;
                currentContact.Forename = tenant.FirstName;
                currentContact.Surname = tenant.LastName;
                currentContact.TenancyPropertyCode = voidObj?.PropertyCode;
                currentContact.Title = tenant.Title;
            }

            return _contactRepository.SaveContact(currentContact);
        }

        public IList<ContactAddressDto> GetContactAddresses(int voidId)
        {
            var contact = _contactRepository.GetContacts(voidId);

            return Mapper.Map<IList<ContactAddressDto>>(contact);
        }

        public ContactAddressDto GetContactAddress(string contactId)
        {
            var contact = _contactRepository.GetContact(contactId);

            return Mapper.Map<ContactAddressDto>(contact);
        }

        public bool SaveContactAddress(ContactAddressDto contactAddressDto)
        {
            var contactAddress = Mapper.Map<Contact>(contactAddressDto);
            var voidObj = _voidRepository.GetVoidDetailByVoidId(contactAddressDto.VoidId);
            contactAddress.TenancyPropertyCode = voidObj?.PropertyCode;

            if (contactAddress.ContactType.ToLower() == "forwarding")
            {
                var tenant = _customerRepository.
                    GetCustomersByPropertyCode(voidObj?.PropertyCode)?
                    .FirstOrDefault(c => c.MainTenantFlag == "Y" && c.TenancyReference == voidObj?.TenancyRef);

                contactAddress.Title = tenant?.Title;
                contactAddress.Forename = tenant?.FirstName;
                contactAddress.Surname = tenant?.LastName;
                contactAddress.DOB = tenant?.DateOfBirth;
                contactAddress.Gender = tenant?.Gender;
                contactAddress.Ethnicity = tenant?.Ethnicity;
            }

            return _contactRepository.SaveContact(contactAddress);
        }

        public bool DeleteContactAddress(string contactId)
        {
            return _contactRepository.DeleteContact(contactId);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool SaveSupportDisabilities(int id, VBLContact contact)
        {
            var saveSupportDetailsUrl = _allocationApiUrl + "/VBL/contacts/PutContact?id=" + id + "&contact=" + contact;

            bool success;

            using (var httpClient = _httpClient.CreateHttpClient())
            {
                var result = httpClient.PutAsJsonAsync(saveSupportDetailsUrl, contact).Result;

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    bool.TryParse(result.Content.ReadAsStringAsync().Result, out success);
                }
                else
                {
                    throw new Exception(HttpStatusCode.InternalServerError.ToString());
                }
            }

            return success;
        }

        /// <summary>
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public IList<ContactDto> GetContacts(int applicationId)
        {
            try
            {
                var getUrl = $"{_allocationApiUrl}/VBL/contacts?{ODataFilterConstant.Filter}ApplicationId eq {applicationId}";

                using (var httpClient = _httpClient.CreateHttpClient())
                {
                    var response = httpClient.GetAsync(getUrl).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = JsonConvert.DeserializeObject<PageResult<VBLContact>>(response.Content.ReadAsStringAsync().Result).Items;

                    return Mapper.Map<IList<ContactDto>>(result) ?? new List<ContactDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorMessage(ex, $"GetContacts applicationId {applicationId}");
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public IList<ContactContactTypeDTO> GetContactByDetails(int applicationId)
        {
            try
            {
                var getUrl = $"{_allocationApiUrl}/VBL/contacts?{ODataFilterConstant.Filter}ApplicationId eq {applicationId}";

                using (var httpClient = _httpClient.CreateHttpClient())
                {
                    var response = httpClient.GetAsync(getUrl).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = JsonConvert.DeserializeObject<PageResult<VBLContact>>(response.Content.ReadAsStringAsync().Result).Items;

                    var contactsBy = result.ToList();

                    List<ContactContactTypeDTO> contactContactTypes = new List<ContactContactTypeDTO>();
                    foreach (var contactBy in contactsBy)
                    {
                        foreach (var contactByDetail in contactBy.ContactByDetails)
                        {
                            contactContactTypes.Add(new ContactContactTypeDTO
                            {
                                ContactId = contactBy.ContactId,
                                Code = contactByDetail.ContactText,
                                ContactByText = contactByDetail.ContactText,
                                ContactByValue = contactByDetail.ContactValue,
                                ContactById = contactByDetail.ContactById,
                            });
                        }
                    }

                    return contactContactTypes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorMessage(ex, $"GetContactByDetails applicationId {applicationId}");
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public IList<ContactDto> GetApplications(int contactId)
        {
            try
            {
                var getUrl = $"{_allocationApiUrl}/VBL/contacts?{ODataFilterConstant.Filter}ContactId eq {contactId}";
                using (var httpClient = _httpClient.CreateHttpClient())
                {
                    var response = httpClient.GetAsync(getUrl).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result =
                        JsonConvert.DeserializeObject<PageResult<VBLContact>>(
                            response.Content.ReadAsStringAsync().Result).Items;

                    return Mapper.Map<IList<ContactDto>>(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorMessage(ex, $"GetApplications contactId {contactId}");
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public ContactDto GetContact(int contactId)
        {
            try
            {
                var getUrl = $"{_allocationApiUrl}/VBL/contacts?{ODataFilterConstant.Filter}ContactId eq {contactId}";

                using (var httpClient = _httpClient.CreateHttpClient())
                {
                    var response = httpClient.GetAsync(getUrl).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = JsonConvert.DeserializeObject<VBLContact>(response.Content.ReadAsStringAsync().Result);

                    return Mapper.Map<ContactDto>(result) ?? new ContactDto();
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorMessage(ex, $"GetContacts contactId {contactId}");
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IList<EthnicityDto> GetEthnicities()
        {
            try
            {
                var getUrl = $"{_allocationApiUrl}/VBL/Common/GetEthnicities";

                using (var httpClient = _httpClient.CreateHttpClient())
                {
                    var response = httpClient.GetAsync(getUrl).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = JsonConvert.DeserializeObject<IList<EthnicityDto>>(response.Content.ReadAsStringAsync().Result);

                    return result ?? new List<EthnicityDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorMessage(ex, "GetEthnicities");
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="contactViewModel"></param>
        /// <returns></returns>
        public ContactViewModel GetContactsForSearch(ContactViewModel contactViewModel)
        {

            var getUrl = $"{_allocationApiUrl}/VBL/Contacts/GetContactsForSearch?" + GetFilterString(contactViewModel);

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var re = JsonConvert.DeserializeObject<OdataResult>(httpClient.GetStringAsync(getUrl).Result);
                    int searchResultCount = 0;
                    if (re.Count != null)
                    {
                        int.TryParse(re.Count.ToString(), out searchResultCount);
                    }
                    contactViewModel.TotalRows = searchResultCount;
                    contactViewModel.ContactSearchResult.Clear();
                    contactViewModel.ContactSearchResult.AddRange(re.Items.Select(item => JsonConvert.DeserializeObject<VBLContact>(item.ToString())));
                    return contactViewModel;
                }
                catch (Exception ex)
                {
                    _logger.LogErrorMessage(ex, "GetContactsForSearch");
                    throw;
                }
            }
        }

        public IList<ContactDto> GetContacts(IList<int> applicationIds)
        {
            try
            {
                var getUrl = $"{_allocationApiUrl}/VBL/contacts/GetContacts";

                using (var httpClient = _httpClient.CreateHttpClient())
                {
                    var response = httpClient.PostAsJsonAsync(getUrl, applicationIds).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = JsonConvert.DeserializeObject<IList<VBLContact>>(response.Content.ReadAsStringAsync().Result);

                    return Mapper.Map<IList<ContactDto>>(result) ?? new List<ContactDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorMessage(ex, "GetContacts");
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="contactViewModel"></param>
        /// <returns></returns>
        private string GetFilterString(ContactViewModel contactViewModel)
        {
            string filterString = string.Empty;
            if (contactViewModel != null)
            {

                if (contactViewModel.ApplicationId > 0)
                {
                    filterString += ODataFilterConstant.Filter + $"ApplicationId eq {contactViewModel.ApplicationId}";
                }
                if (!string.IsNullOrWhiteSpace(contactViewModel.Forename))
                {
                    if (string.IsNullOrWhiteSpace(filterString))
                    {
                        filterString = ODataFilterConstant.Filter +
                                       $"substringof('{contactViewModel.Forename}',Forename) eq true";
                    }
                    else
                    {
                        filterString += $" and substringof('{contactViewModel.Forename}',Forename) eq true";
                    }
                }
                if (!string.IsNullOrWhiteSpace(contactViewModel.Surname))
                {
                    if (string.IsNullOrWhiteSpace(filterString))
                    {
                        filterString = ODataFilterConstant.Filter +
                                       $"substringof('{contactViewModel.Surname}',Surname) eq true";
                    }
                    else
                    {
                        filterString += $" and substringof('{contactViewModel.Surname}',Surname) eq true";
                    }
                }
                if (!string.IsNullOrWhiteSpace(contactViewModel.Address))
                {
                    if (string.IsNullOrWhiteSpace(filterString))
                    {
                        filterString = ODataFilterConstant.Filter +
                                       $" (substringof('{contactViewModel.Address}',Application/Address/AddressLine1)";
                    }
                    else
                    {
                        filterString +=
                            $" and (substringof('{contactViewModel.Address}',Application/Address/AddressLine1)";
                    }
                    filterString += $" or substringof('{contactViewModel.Address}',Application/Address/AddressLine2)";
                    //filterString += $" or substringof('{contactViewModel.Address}',Application/Address/AddressLine3)";
                    //filterString += $" or substringof('{contactViewModel.Address}',Application/Address/AddressLine4)";
                    filterString += $" or substringof('{contactViewModel.Address}',Application/Address/PostCode)";
                    filterString += " and Application/Address/IsActive eq true)";
                }
                if (contactViewModel.DateOfBirth.HasValue && contactViewModel.DateOfBirth != DateTime.MinValue)
                {
                    if (string.IsNullOrWhiteSpace(filterString))
                    {
                        filterString = ODataFilterConstant.Filter +
                                       $"DateOfBirth eq  datetime'{contactViewModel.DateOfBirth.Value.Date.ToString("yyyy-MM-dd")}'";
                    }
                    else
                    {
                        filterString +=
                            $" and DateOfBirth eq datetime'{contactViewModel.DateOfBirth.Value.Date.ToString("yyyy-MM-dd")}'";
                    }
                }
                if (!string.IsNullOrEmpty(contactViewModel.NationalInsuranceNumber))
                {
                    if (string.IsNullOrWhiteSpace(filterString))
                    {
                        filterString = ODataFilterConstant.Filter +
                                       $"substringof('{contactViewModel.NationalInsuranceNumber}',NationalInsuranceNumber) eq true";
                    }
                    else
                    {
                        filterString += $" and substringof('{contactViewModel.NationalInsuranceNumber}',NationalInsuranceNumber) eq true";
                    }
                }

                AddPageSizeNumberAndSortingInFilterString(contactViewModel, ref filterString);
            }
            return filterString;
        }

        /// <summary>
        /// </summary>
        /// <param name="baseFilterModel"></param>
        /// <param name="filterString"></param>
        protected void AddPageSizeNumberAndSortingInFilterString(BaseFilterModel baseFilterModel,
            ref string filterString)
        {
            if (baseFilterModel.PageSize > 0)
            {
                filterString += ODataFilterConstant.Inlinecount;
                filterString += string.Format("{0}{1}", ODataFilterConstant.Top, baseFilterModel.PageSize);
            }
            if (baseFilterModel.PageNumber > 0)
            {
                filterString += string.Format("{0}{1}", ODataFilterConstant.Skip, (baseFilterModel.PageNumber - 1) * baseFilterModel.PageSize);
            }

            if (string.IsNullOrWhiteSpace(baseFilterModel.SortColumn))
            {
                return;
            }
            filterString += string.Format("{0}{1}", ODataFilterConstant.Orderby, baseFilterModel.SortColumn);
            if (baseFilterModel.SortDirection.Contains("Desc"))
            {
                filterString += " desc";
            }
            else
            {
                filterString += " asc";
            }
        }

    }
}
