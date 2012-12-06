using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Progression : D3Object
    {
        public Difficulty Normal { get; set; }
        public Difficulty Nightmare { get; set; }
        public Difficulty Hell { get; set; }
        public Difficulty Inferno { get; set; }
    }
}
