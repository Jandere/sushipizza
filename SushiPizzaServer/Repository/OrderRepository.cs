﻿using Contracts;
using Entites;
using Entites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryContext context) : base(context)
        {
        }

        public IEnumerable<Order> GetOrdersWithDetails()
        {
            return RepositoryContext.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).AsNoTracking();
        }

        public IEnumerable<Order> GetOrdersWithDetailsForUser(int userId)
        {
            return RepositoryContext.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).Where(op => op.UserId == userId).AsNoTracking();
        }

        public void CreateOrder(Order order)
        {
            if (order.UserId != 0 && order.UserId != null)
            {
                User user = RepositoryContext.Users.Find(order.UserId);
                order.Customer = user;
                user.TotalSpend += order.TotalPrice;
                order.TotalPrice *= (1 - user.Discount / 100);
                user.Discount = CalculateDiscount(user.TotalSpend);
            }

            Create(order);
            RepositoryContext.OrderProducts.AddRange(order.OrderProducts);
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await GetByCondition(o => o.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderWithDetailsById(int id)
        {
            return await RepositoryContext.Orders
                .Include(o => o.OrderProducts).ThenInclude(op => op.Product).AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public void DeleteOrder(Order order)
        {
            var OPs = RepositoryContext.OrderProducts.Where(op => op.Order == order);
            RepositoryContext.OrderProducts.RemoveRange(OPs);
            Delete(order);
        }

        public void UpdateOrder(Order order, Order orderNew)
        {
            //if (order.UserId != 0 && order.UserId != null)
            //{
            //    User user = RepositoryContext.Users.Find(order.UserId);
            //    order.Customer = user;
            //    order.CustomerPhoneNumber = order.Customer.PhoneNumber;

            //}
            //order.OrderProducts = order.OrderProducts.Select(op => RepositoryContext.OrderProducts.FirstOrDefault(opi => opi.ProductId == op.ProductId && opi.OrderId == order.Id)).ToArray();
            //foreach (var orderProduct in order.OrderProducts)
            //{
            //    if (orderProduct is null)
            //    {
            //        RepositoryContext.OrderProducts.Add(new OrderProduct { OrderId = order.Id, ProductId = orderProduct.ProductId });
            //    }
            //}
            //order.TotalPrice = order.OrderProducts.Select(op => op.Product).Sum(p => p.Price);
            //Update(order);
            //RepositoryContext.OrderProducts.UpdateRange(order.OrderProducts);
            DeleteOrder(order);
            CreateOrder(orderNew);
        }

        private double CalculateDiscount(double userTotalSpend)
        {
            if (userTotalSpend > 10000 && userTotalSpend < 20000)
                return 5;
            else if (userTotalSpend >= 20000)
                return 10;
            return 0;
        }
    }
}
