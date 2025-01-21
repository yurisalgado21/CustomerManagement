using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.DTO
{
    public class OrderDtoResponse
    {
        public int OrderId { get; set; }
        public string Number { get; set; } //o pedido deve ter um número
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } //data
        public int CustomerId { get; set; } //qual cliente fez o pedido
        public List<ItemDtoResponse> Itens { get; set; } //itens do pedido
        public decimal TotalOrderValue { get; set; } //valor total do pedido
    }
}