using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class FollowerSkills : D3Object
    {
        public List<Skill> Active { get; set; }
        public List<Skill> Passive { get; set; }
    }
}
