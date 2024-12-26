using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using CustomerManagement.Models;

namespace CustomerManagement.DTO
{
    public class OrderDtoRequest
    {
        public int Number { get; set; } //o pedido deve ter um número
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } //data
        public int CustomerId { get; set; } //qual cliente fez o pedido
        public List<ItemDtoRequest> Itens { get; set; } //itens do pedido
        // public decimal TotalOrderValue { get; set; } //valor total do pedido
    }
}