using BattleNet.D3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
            Uri profileUri = new Uri(BattleNet.GetBaseUriByRegion(Region), "api/d3/profile/" + btag + "/");

            String json = await GetJsonFromUri(profileUri);

            return JsonConvert.DeserializeObject<Profile>(json);
        }
    }
}
