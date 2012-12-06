using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Follower : D3Object
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Portrait { get; set; }
        public FollowerSkills Skills { get; set; }
    }
}
