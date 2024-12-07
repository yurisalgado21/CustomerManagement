using System.ComponentModel.DataAnnotations;
using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Services;
using CustomerManagement.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerRepository _repository;
        private readonly ICustomerServices _services;
        private IAddressRepository _addressRepository;

        public CustomersController(ICustomerRepository repository, ICustomerServices services,
                                   ApplicationDbContext dbContext, IAddressRepository addressRepository)
        {
            _repository = repository;
            _services = services;
            _dbContext = dbContext;
            _addressRepository = addressRepository;
        }

        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 0 || pageSize < 0) return BadRequest(ResponseMessagesCustomers.CustomerPaginationError);

            var validFilter = new PaginationFilter(pageNumber: pageNumber, pageSize: pageSize);

            var allCustomers = _services.GetAll(validFilter);
            if (allCustomers.Count() == 0) return NoContent();

            var allCustomersDtoResponse = _services.GenerateListCustomerDtoResponses(allCustomers.ToList());
            return Ok(allCustomersDtoResponse);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var findCustomer = _services.GetById(id);

            if (findCustomer == null) return NotFound(ResponseMessagesCustomers.CustomerNotFoundMessage);

            var newCustomerResponse = _services.GenerateCustomerDtoResponse(findCustomer);

            return Ok(newCustomerResponse);
        }

        [HttpGet("{id}/Addresses")]
        public IActionResult GetCustomerAddresses(int id)
        {
            var findCustomer = _services.GetById(id);

            if (findCustomer == null) return NotFound(ResponseMessagesCustomers.CustomerNotFoundMessage);

            var customerResponse = _services.GenerateCustomerDtoResponse(findCustomer);

            return Ok(customerResponse.Addresses);
        }

        [HttpPost]
        public IActionResult Add([FromBody] CustomerDto customer)
        {
            var result = _services.Add(customer);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();

            var newCustomerResponse = _services.GenerateCustomerDtoResponse(result.Data);

            return CreatedAtAction(actionName: nameof(GetById), routeValues: new {id = result.Data.CustomerId}, value: newCustomerResponse);
        }

        [HttpPost("{id}/Addresses")]
        public IActionResult AddAddressInCustomer(int id, [FromBody] AddressDto addressDto)
        {
            var result = _services.AddAddressInCustomer(id, addressDto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();

            return Created("", addressDto);
        }

        [HttpPost("batch")]
        public async Task<IActionResult> AddListCustomers([FromBody] IEnumerable<CustomerDto> customers)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (customers.Count() == 0) return NoContent();

                var result = _services.AddRange(customers);

                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, result.Message);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                        
                var getlistCustomers = _services.GetListCustomersByEmail(customers);
                var listCustomersForResponse = new List<CustomerDtoResponse>();

                foreach (var customer in getlistCustomers)
                {
                   var customerDto = _services.GenerateCustomerDtoResponse(customer);
                   listCustomersForResponse.Add(customerDto);
                }
                return Created("", listCustomersForResponse);
            }
            catch (Exception err)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = err.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CustomerDto customerDto)
        {
            var result = _services.Update(id, customerDto);
            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();
        
            return Ok(result.Data);
        }

        [HttpPut("{id}/Addresses/{addressId}")]
        public IActionResult Update(int id, [FromBody] AddressDto addressDto, int addressId)
        {
            var result = _services.UpdateAddress(id, addressDto, addressId);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();

            return Ok(addressDto);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePatch(int id, [FromBody] CustomerPatchDto customerPatchDto)
        {
            var result = _services.UpdatePatchCustomer(id, customerPatchDto);
            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }
            
            _services.SaveChanges();
        
            var customerResponse = new CustomerPatchDtoResponse
            {
                CustomerId = result.Data.CustomerId,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName!,
                Email = result.Data.Email,
                DateOfBirth = result.Data.DateOfBirth
            };

            return Ok(customerResponse);
        }

        [HttpPatch("{id}/Addresses/{addressId}")]
        public IActionResult UpdatePatch(int id, int addressId, [FromBody] AddressPatchDto addressPatchDto)
        {
            var result = _services.UpdatePatchAddress(id, addressPatchDto, addressId);
            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }
            
            _services.SaveChanges();

            return Ok(addressPatchDto);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _services.Delete(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            _services.SaveChanges();
                       
            return NoContent();
        }

        [HttpDelete("{id}/Addresses/{addressId}")]
        public IActionResult Delete(int id, int addressId)
        {
            var result = _services.DeleteAddress(id, addressId);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }
            _addressRepository.SaveChanges();

            return NoContent();
        }
    }
}