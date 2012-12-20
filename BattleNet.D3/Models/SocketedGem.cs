using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class SocketedGem : D3Object
    {
        public Item Item;
        public Dictionary<string, MinMax> AttributesRaw { get; set; }
        public List<string> Attributes { get; set; }
    }
}
