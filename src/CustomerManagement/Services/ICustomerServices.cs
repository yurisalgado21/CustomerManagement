using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Utils;

namespace CustomerManagement.Services
{
    public interface ICustomerServices
    {
        public List<string> GetDuplicateEmails(IEnumerable<CustomerDto> customers);
        public IEnumerable<Customer> GetAll(PaginationFilter validFilter);
        public Customer GetById(int id);
        public Customer GetByEmail(string email);
        public IEnumerable<Customer> GetListCustomersByEmail(IEnumerable<CustomerDto> customers);
        public bool VerifyDateOfBirth(DateTime customerDateOfBirth);
        public ServiceResult<Customer> Add(CustomerDto customer);
        public ServiceResult<Customer> AddAddressInCustomer(int id, AddressDto addressDto);
        public ServiceResult<IEnumerable<Customer>> AddRange(IEnumerable<CustomerDto> customers);
        public ServiceResult<BatchImportResponse> AddRange2(IEnumerable<CustomerDto> customers);
        public ServiceResult<CustomerDtoResponse> Update(int id,  CustomerDto customerDto);
        public ServiceResult<Customer> UpdateAddress(int id, AddressDto addressDto, int addressId);
        public ServiceResult<Customer> UpdatePatchCustomer(int id, CustomerPatchDto customerPatchDto);
        public ServiceResult<Customer> UpdatePatchAddress(int id, AddressPatchDto addressPatchDto, int addressId);
        public bool CheckIfTheCustomerHasARepeatingAddressInList(IEnumerable<AddressDto> addresses);
        public ServiceResult<Customer> Delete(int id);
        public ServiceResult<Customer> DeleteAddress(int id, int addressId);
        public Address CheckWhichPropertiesToChangeAddressUpdatePatch(Address address, AddressPatchDto addressPatchDto);
        public CustomerDtoResponse GenerateCustomerDtoResponse(Customer customer);
        public Customer GenerateListAddressForCustomerAndReturnCustomer(CustomerDto customer);
        public IEnumerable<CustomerDtoResponse> GenerateListCustomerDtoResponses(List<Customer> customers);
        public void SaveChanges();
    }
}