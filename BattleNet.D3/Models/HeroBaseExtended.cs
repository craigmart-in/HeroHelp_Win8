using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public abstract class HeroBaseExtended : HeroBase
    {
        public Dictionary<string, Item> Items { get; set; }
        public Dictionary<string, Item> CompareItems { get; set; }
        public Dictionary<string, double> Stats { get; set; }
        public CalculatedStats CalculatedStats { get; set; }
        public Kills Kills { get; set; }
    }
}
