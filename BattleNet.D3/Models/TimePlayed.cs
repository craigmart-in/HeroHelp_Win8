using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class TimePlayed : D3Object
    {
        public double Barbarian { get; set; }
        public double DemonHunter { get; set; }
        public double Monk { get; set; }
        public double WitchDoctor { get; set; }
        public double Wizard { get; set; }
    }
}
