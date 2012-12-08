using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Hero : HeroBaseExtended
    {
        public Skills Skills { get; set; }
        public Followers Followers { get; set; }
        public Progression Progress { get; set; }
        public bool Dead { get; set; }

        [JsonProperty("last-updated")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastUpdated { get; set; }
    }
}
