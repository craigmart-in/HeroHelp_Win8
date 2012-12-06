using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Followers : D3Object
    {
        public HeroFollower Templar { get; set; }
        public HeroFollower Scoundrel { get; set; }
        public HeroFollower Enchantress { get; set; }
    }
}
