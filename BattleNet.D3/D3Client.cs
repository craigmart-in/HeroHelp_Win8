using BattleNet.D3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BattleNet.D3
{
    public class D3Client : BaseClient
    {
        public D3Client(Region region)
            : base(region)
        {
            
        }

        public async Task<Profile> GetProfileAsync(String btag)
        {
            Uri profileUri = new Uri(this.BaseUri, "api/d3/profile/" + btag + "/?locale=" + this.Localization);

            String json = await GetJsonFromUri(profileUri);

            return JsonConvert.DeserializeObject<Profile>(json);
        }

        public async Task<Hero> GetHeroAsync(string btag, long heroId)
        {
            Uri heroUri = new Uri(this.BaseUri, "api/d3/profile/" + btag + "/hero/" + heroId + "?locale=" + this.Localization);

            String json = await GetJsonFromUri(heroUri);

            return JsonConvert.DeserializeObject<Hero>(json);
        }

        public async Task<Item> GetItemAsync(string tooltipParams)
        {
            Uri itemUri = new Uri(this.BaseUri, "api/d3/data/" + tooltipParams + "?locale=" + this.Localization);

            String json = await GetJsonFromUri(itemUri);

            return JsonConvert.DeserializeObject<Item>(json);
        }

        public BitmapImage GetItemIcon(string size, string icon)
        {
            Uri itemIconUri = new Uri(this.MediaUri, "d3/icons/items/" + size + "/" + icon + ".png");
            BitmapImage itemIcon = new BitmapImage(itemIconUri);
            return itemIcon;
        }

        public BitmapImage GetSkillIcon(string size, string icon)
        {
            Uri itemIconUri = new Uri(this.MediaUri, "d3/icons/skills/" + size + "/" + icon + ".png");
            BitmapImage itemIcon = new BitmapImage(itemIconUri);
            return itemIcon;
        }

        public async Task<string> GetItemToolTip(string tooltipParams)
        {
            Uri itemUri = new Uri(this.BaseUri, "d3/tooltip/" + tooltipParams + "?locale=" + this.Localization);

            String tooltipHtml = await GetJsonFromUri(itemUri);

            return tooltipHtml;
        }
    }
}
