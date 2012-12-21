using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Set : D3Object
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public List<Rank> Ranks { get; set; }

        public int CharCount { get; set; }
    }
}
