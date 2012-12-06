using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class HeroItems : Items
    {
        public EquippedItem Head { get; set; }
        public EquippedItem Torso { get; set; }
        public EquippedItem Feet { get; set; }
        public EquippedItem Hands { get; set; }
        public EquippedItem Shoulders { get; set; }
        public EquippedItem Legs { get; set; }
        public EquippedItem Bracers { get; set; }
        public EquippedItem Waist { get; set; }
    }
}
