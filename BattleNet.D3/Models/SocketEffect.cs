using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class SocketEffect : D3Object
    {
        public string ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public Dictionary<string, MinMax> AttributesRaw { get; set; }
        public List<String> Attributes { get; set; }
    }
}
