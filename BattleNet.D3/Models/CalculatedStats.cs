using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class CalculatedStats
    {
        public double Str { get; set; }
        public double Dex { get; set; }
        public double Int { get; set; }
        public double Vit { get; set; }
        public double Arm { get; set; }
        public double AllRes { get; set; }
        public double ArmDR { get; set; }
        public double ResDR { get; set; }
        public double DR { get; set; }
        public double HP { get; set; }
        public double EHP { get; set; }

        public string DisplayEHP
        {
            get { return EHP.ToString("N"); }
        }
    }
}
