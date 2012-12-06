using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public abstract class Items : D3Object
    {
        public EquippedItem MainHand { get; set; }
        public EquippedItem OffHand { get; set; }
        public EquippedItem RightFinger { get; set; }
        public EquippedItem LeftFinger { get; set; }
        public EquippedItem Neck { get; set; }
    }
}
