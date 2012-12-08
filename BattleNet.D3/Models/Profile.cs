using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BattleNet.D3.Models
{
    public class Profile : D3Object
    {
        public List<ProfileHero> Heroes { get; set; }

        /// <summary>
        /// Id of last played hero
        /// </summary>
        public long LastHeroPlayed { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastUpdated { get; set; }

        public List<Artisan> Artisans { get; set; }
        public List<Artisan> HardcoreArtisans { get; set; }

        public Kills Kills { get; set; }
        public TimePlayed TimePlayed { get; set; }

        public List<FallenHero> FallenHeroes { get; set; }

        public string BattleTag { get; set; }
        public Region Region { get; set; }

        public Progression Progression { get; set; }
        public Progression HardcoreProgression { get; set; }

        public Windows.UI.Xaml.Media.Imaging.BitmapImage ProfilePortrait { get; set; }
    }
}
