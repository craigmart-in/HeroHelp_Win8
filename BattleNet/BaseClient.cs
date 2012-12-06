using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet
{
    public abstract class BaseClient
    {
        HttpClient _httpClient;

        private Region _region;

        public Region Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public BaseClient(Region region)
        {
            this.Region = region;

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
