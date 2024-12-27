using Azure;
using CustomerManagement.Data;
using CustomerManagement.DTO;
using CustomerManagement.Models;
using CustomerManagement.Repository;
using CustomerManagement.Utils;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductServices _productServices;
        public OrderServices
        (
            IOrderRepository orderRepository,
            ApplicationDbContext dbContext,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IProductServices productServices
        )
        {
            _orderRepository = orderRepository;
            _dbContext = dbContext;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _productServices = productServices;
        }

        public ServiceResult<Order> Add(OrderDtoRequest orderDtoRequest)
        {
            List<Item> listItens = new List<Item>();

            var customer = _customerRepository.GetById(orderDtoRequest.CustomerId);

            if (customer == null)
            {
                return ServiceResult<Order>.ErrorResult(ResponseMessagesCustomers.CustomerNotFoundMessage, 404);
            }

            foreach (var item in orderDtoRequest.Itens)
            {
                var findProduct = _productServices.GetByCode(item.Product.Code);

                if (findProduct == null)
                {
                    return ServiceResult<Order>.ErrorResult($"{ResponseMessagesCustomers.ProductNotFoundMessage}. Code: {item.Product.Code}", 404);
                }

                var product = Product.SetExistingInfo(id: findProduct!.Id, code: item.Product.Code, name: item.Product.Name);

                if (!product.IsValid)
                {
                    return ServiceResult<Order>.ErrorResult(ResponseMessagesCustomers.FieldsAreInvalidProduct, 400);
                }

                var newItem = Item.RegisterNew
                (
                    product: product,
                    unitValue: item.UnitValue,
                    quantityOfItens: item.QuantityOfItens
                );

                listItens.Add(newItem);
            }


            var order = Order.RegisterNew
            (
                number: orderDtoRequest.Number,
                date: orderDtoRequest.Date,
                customerId: orderDtoRequest.CustomerId,
                itens: listItens
            );

            if (!order.IsValid)
            {
                return ServiceResult<Order>.ErrorResult("fields in order are invalid", 400);
            }

            _orderRepository.Add(order);

            return ServiceResult<Order>.SuccessResult(order, 201);
        }
    }
}