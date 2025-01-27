using System.Text.Json.Serialization;
using CustomerManagement.Utils;

namespace CustomerManagement.Models
{
    public class Order
    {
        public int OrderId { get; private set; }
        private string _number { get; set; }
        private DateTime _date { get; set; }
        private decimal _totalOrderValue;
        private int _customerId;
        public string Number => _number; //o pedido deve ter um número
        public DateTime Date => _date; //data
        public int CustomerId => _customerId;
        [JsonIgnore]
        public Customer Customer { get; set; } //qual cliente fez o pedido
        public ICollection<Item> Itens { get; private set; } = new List<Item>(); //itens do pedido
        public decimal TotalOrderValue => _totalOrderValue; //valor total do pedido

        public bool IsValid { get; private set; }

        //constructors
        //private contructor
        private Order(){}
        private Order
        (
            int orderId,
            int customerId,
            string number,
            DateTime date,
            List<Item> itens,
            decimal totalOrderValue
        )
        {
            OrderId = orderId;
            _customerId = customerId;
            _number = number;
            _date = date;
            Itens = itens;
            _totalOrderValue = totalOrderValue;
        }

        //public methods
        public static Order RegisterNew
        (
            string number,
            DateTime date,
            int customerId,
            List<Item> itens
        )
        {
            var order = new Order();
            order.SetNumber(number: number);
            order.SetDate(date: date);
            order.SetCustomerId(customerId: customerId);
            order.SetItens(itens: itens);
            order.SetTotalOrderValue(itens: itens);
            order.Validate();

            return order;
        }

        public static Order SetExistingInfo
        (
            int orderId,
            int customerId,
            string number,
            DateTime date,
            List<Item> itens,
            decimal totalOrderValue
        )
        {
            var order = new Order
            (
                orderId: orderId,
                customerId: customerId,
                number: number,
                date: date,
                itens: itens,
                totalOrderValue: totalOrderValue
            );
            order.Validate();

            return order;
        }

        // private methods
        private void SetNumber(string number)
        {
            if (number.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "The length of the number cannot be less than 1 characters.");
            }

            _number = number;
        }

        private void SetDate(DateTime date)
        {
            var dateNow = DateTime.UtcNow;
            if (date.ToUniversalTime().Date > dateNow.Date)
            {
                throw new ArgumentOutOfRangeException(nameof(date), ResponseMessagesCustomers.OrderDateError);
            }

            _date = date;
        }

        private void SetTotalOrderValue(List<Item> itens)
        {
            var totalValue = from item in itens select item.TotalValue;
            _totalOrderValue = totalValue.Sum();
        }

        private void SetCustomerId(int customerId)
        {
            if (customerId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(customerId), ResponseMessagesCustomers.CustomerIdMustBeGreaterThanZero);
            }

            _customerId = customerId;
        }

        private void SetItens(List<Item> itens)
        {
            if (itens.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(itens), ResponseMessagesCustomers.OrderMustHaveAtLeastOneItem);
            }

            Itens = itens;
        }

        private void Validate()
        {
            var dateNow = DateTime.UtcNow;
            IsValid = _number.Length >= 1 && _date.ToUniversalTime().Date <= dateNow.Date && _totalOrderValue > 0 && _customerId > 0 && Itens.Count > 0;
        }
    }
}