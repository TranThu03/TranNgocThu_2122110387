    namespace TranNgocThu_2122110387.Model
    {
        public class Order
        {
            public int OrderId { get; set; }
            public int UserId { get; set; } // Người dùng tạo đơn hàng
            public User User { get; set; } // Liên kết với người dùng
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public List<OrderDetail> OrderDetails { get; set; } // Liên kết với chi tiết đơn hàng
        }

    }
