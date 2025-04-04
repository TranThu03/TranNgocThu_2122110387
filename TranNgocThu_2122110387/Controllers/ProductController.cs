//using Microsoft.AspNetCore.Mvc;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace TranNgocThu_2122110387.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductController : ControllerBase
//    {
//        // GET: api/<ProductController>
//        [HttpGet]
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET api/<ProductController>/5
//        [HttpGet("{id}")]
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST api/<ProductController>
//        [HttpPost]
//        public void Post([FromBody] string value)
//        {
//        }

//        // PUT api/<ProductController>/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE api/<ProductController>/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using TranNgocThu_2122110387.Model;
using System.Collections.Generic;
using System.Linq;

namespace TranNgocThu_2122110387.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        // Danh sách sản phẩm giả lập (thay thế bằng cơ sở dữ liệu thực tế trong ứng dụng thật)
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Image = "image-url-1", Price = "100", CreateDate = DateTime.Now },
            new Product { Id = 2, Name = "Product 2", Image = "image-url-2", Price = "150", CreateDate = DateTime.Now }
        };

        // GET: api/<ProductController>
        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return Ok(_products); // Trả về danh sách sản phẩm
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }
            return Ok(product);
        }

        // POST api/<ProductController>
        [HttpPost]
        public ActionResult<Product> Post([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null.");
            }

            // Gán ID mới cho sản phẩm
            product.Id = _products.Max(p => p.Id) + 1;
            product.CreateDate = DateTime.Now; // Tự động thêm ngày tạo

            _products.Add(product);

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product); // Trả về sản phẩm vừa thêm
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }

            // Cập nhật thông tin sản phẩm
            existingProduct.Name = product.Name;
            existingProduct.Image = product.Image;
            existingProduct.Price = product.Price;
            existingProduct.CreateDate = product.CreateDate;

            return NoContent(); // Trả về trạng thái thành công
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }

            _products.Remove(product); // Xóa sản phẩm
            return NoContent(); // Trả về trạng thái thành công
        }
    }
}
