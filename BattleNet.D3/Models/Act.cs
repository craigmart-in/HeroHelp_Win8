using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Act : D3Object
    {
        public bool Completed { get; set; }
        public List<CompletedQuest> CompletedQuests { get; set; }
    }
}
