using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Difficulty : D3Object
    {
        public Act Act1 { get; set; }
        public Act Act2 { get; set; }
        public Act Act3 { get; set; }
        public Act Act4 { get; set; }
    }
}
