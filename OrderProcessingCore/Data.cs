using System;
using System.Collections.Generic;

namespace OrderProcessingCore
{
    [Flags] public enum AddOrderResult
    { 
        Sucess          = 0,
        InvalidID       = 0b0001,
        InvalidWeight   = 0b0010,
        InvalidDistrict = 0b0100,
        InvalidTime     = 0b1000,
    }

    public class Data
    {
        private List<Order> _orders;

        public AddOrderResult AddOrder(Order order)
        {
            int result = (int)AddOrderResult.Sucess;

            if (order.weight <= 0f) result |= (int)AddOrderResult.InvalidWeight;
            if (order.orderDistrict == null || order.orderDistrict == "") result |= (int)AddOrderResult.InvalidDistrict;

            if (result == (int)AddOrderResult.Sucess) _orders.Add(order);

            OrderAdded?.Invoke(order, (AddOrderResult)result);

            return (AddOrderResult)result;
        }
        public void AddOrders(IEnumerable<Order> orders) { foreach (Order order in orders) AddOrder(order); }

        public Order[] GetOrders(string districtFilter = null, DateTime? timeFilter = null)
        {
            List<Order> ret = new List<Order>(_orders);

            if (districtFilter != null && districtFilter != "") ret.RemoveAll((order) => !(order.orderDistrict == districtFilter));
            if (timeFilter != null) ret.RemoveAll((order) => !(order.orderTime == timeFilter));

            OrdersRequested?.Invoke(districtFilter, timeFilter, ret.Count);

            return ret.ToArray();
        }

        public Data()
        {
            _orders = new List<Order>();
            Initialized?.Invoke();
        }

        public event Action Initialized;
        public event Action<Order, AddOrderResult> OrderAdded;
        public event Action<string, DateTime?, int> OrdersRequested;
    }
}