using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Skills : D3Object
    {
        public List<SkillRune> Active { get; set; }
        public List<SkillRune> Passive { get; set; }
    }
}
