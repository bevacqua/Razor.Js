using System.Collections.Generic;

namespace Bevaq.RazorJs.Sample.Models
{
    public class Vehicle
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<Component> Components { get; set; }
    }
}