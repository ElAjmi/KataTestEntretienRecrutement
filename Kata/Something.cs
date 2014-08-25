using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kata
{
    public class Thing
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string Owner { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class AnotherThing
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public string Comment { get; set; }
        public double? Price { get; set; }
        public string Name { get; set; }
    }
}