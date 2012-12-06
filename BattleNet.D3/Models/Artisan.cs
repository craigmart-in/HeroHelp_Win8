using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Artisan : D3Object
    {
        public string Slug { get; set; }
        public int Level { get; set; }
        public int StepMax { get; set; }
        public int StepCurrent { get; set; }
    }
}
