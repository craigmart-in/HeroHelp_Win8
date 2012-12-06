using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Kills : D3Object
    {
        public long Monsters { get; set; }
        public long Elites { get; set; }
        public long HardcoreMonsters { get; set; }
    }
}
