using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWebApi.Models
{
    public class Fruit
    {
        public long Id { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
    }
}
