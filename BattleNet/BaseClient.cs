using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet
{
    public abstract class BaseClient
    {
        HttpClient _httpClient;

        public Region Region { get; set; }

        public Localization Localization { get; set; }

        public Uri BaseUri { get; set; }

        public BaseClient(Region region)
        {
            this.Region = region;
            this.Localization = (Localization)Enum.Parse(typeof(Localization), CultureInfo.CurrentCulture.Name.Replace("-", "_"), true);
            this.BaseUri = BattleNet.GetBaseUriByRegion(Region);

            HttpMessageHandler handler = new HttpClientHandler();
            _httpClient = new HttpClient(handler);
        }

        public async Task<string> GetJsonFromUri(Uri uri)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(uri);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
