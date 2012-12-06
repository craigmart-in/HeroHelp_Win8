using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public abstract class HeroBaseExtended : HeroBase
    {
        public HeroItems Items { get; set; }
        public Stats Stats { get; set; }
        public Kills Kills { get; set; }
    }
}
