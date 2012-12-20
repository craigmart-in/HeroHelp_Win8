using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public abstract class Items : D3Object
    {
        public Item MainHand { get; set; }
        public Item OffHand { get; set; }
        public Item RightFinger { get; set; }
        public Item LeftFinger { get; set; }
        public Item Neck { get; set; }
    }
}
