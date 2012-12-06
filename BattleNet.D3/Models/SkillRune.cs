using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class SkillRune : D3Object
    {
        public Skill Skill { get; set; }
        public Rune Rune { get; set; }
    }
}
