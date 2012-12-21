using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Rank : D3Object
    {
        public int Required { get; set; }
        public List<string> Attributes { get; set; }
    }
}
