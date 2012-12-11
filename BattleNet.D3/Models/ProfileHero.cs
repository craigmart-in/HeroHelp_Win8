using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class ProfileHero : HeroBase
    {
        public bool Dead { get; set; }

        [JsonProperty("last-updated")]
        public long LastUpdated { get; set; }

        public Portrait Portrait { get; set; }
        public string PortraitMargin { get; set; }
        public string PortraitViewRect { get; set; }
    }
}
