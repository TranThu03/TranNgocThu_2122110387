namespace TranNgocThu_2122110387.Model
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; } // Liên kết với đơn hàng
        public Order Order { get; set; }
        public int ProductId { get; set; } // Liên kết với sản phẩm
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Giá sản phẩm tại thời điểm đặt hàng
    }

}
