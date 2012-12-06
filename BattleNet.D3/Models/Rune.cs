using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Rune : D3Object
    {
        public string Slug { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public string SimpleDescription { get; set; }
        public string TooltipParams { get; set; }
        public string SkillCalcId { get; set; }
        public string Order { get; set; }
    }
}
