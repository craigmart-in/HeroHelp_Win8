using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Salvage : D3Object
    {
        public double Chance { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
