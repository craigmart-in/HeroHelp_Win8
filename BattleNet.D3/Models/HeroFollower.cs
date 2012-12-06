using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class HeroFollower : D3Object
    {
        public string Slug { get; set; }
        public int Level { get; set; }
        public FollowerItems Items { get; set; }
        public FollowerStats Stats { get; set; }
        public List<Skill> Skills { get; set; }
    }
}
