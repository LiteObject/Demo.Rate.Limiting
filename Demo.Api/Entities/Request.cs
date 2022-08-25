using System.ComponentModel.DataAnnotations;

namespace Demo.Api.Entities
{
    public class Request
    {
        [Key]
        [Required]
        public string Token { get; set; }

        [Required]
        public int Count { get; set; }

        public DateTime First { get; set; } = DateTime.Now;

        public DateTime Last { get; set; } = DateTime.Now;
    }
}
