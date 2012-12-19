using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class CompareStat
    {
        public string Name { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string Difference { get; set; }
        public string DifferenceColor { get; set; }
    }
}
