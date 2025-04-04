namespace TranNgocThu_2122110387.Model
{
    public class Category
    {
        public int Id { get; set; } // Khóa chính
        public string Name { get; set; } // Tên danh mục
        public string Description { get; set; } // Mô tả danh mục
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo
        public bool IsActive { get; set; } = true; // Trạng thái danh mục
    }
}
