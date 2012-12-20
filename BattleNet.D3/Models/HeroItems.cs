using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class HeroItems : Items
    {
        public Item Head { get; set; }
        public Item Torso { get; set; }
        public Item Feet { get; set; }
        public Item Hands { get; set; }
        public Item Shoulders { get; set; }
        public Item Legs { get; set; }
        public Item Bracers { get; set; }
        public Item Waist { get; set; }
    }
}
