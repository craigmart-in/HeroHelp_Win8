using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class ItemType : D3Object
    {
        public string Id { get; set; }
        public bool TwoHanded { get; set; }
    }
}
