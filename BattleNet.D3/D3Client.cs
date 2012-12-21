using BattleNet.D3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
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

        public static Dictionary<string, MinMax> ParseAttributesRawFromAttributes(List<string> attributes)
        {
            Match match;
            Dictionary<string, MinMax> attributesRaw = new Dictionary<string, MinMax>();
            string name = "";
            double val = 0;

            foreach (string attribute in attributes)
            {
                if ((match = Regex.Match(attribute, @"^\+(\d+) (Dexterity|Intelligence|Strength|Vitality)$", RegexOptions.IgnoreCase)).Success)
                {
                    name = match.Groups[2].Value + "_Item";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Int32.Parse(match.Groups[1].Value);
                }
                else if ((match = Regex.Match(attribute, @"^\+(\d+) Resistance to All Elements$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Resistance_All";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Int32.Parse(match.Groups[1].Value);
                }
                else if ((match = Regex.Match(attribute, @"^Critical Hit Chance Increased by (\d+\.\d+)%$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Crit_Percent_Bonus_Capped";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^Attack Speed Increased by (\d+)%$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Attacks_Per_Second_Item_Percent";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^\+(\d+)% Life$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Hitpoints_Max_Percent_Bonus_Item";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^Critical Hit Damage Increased by (\d+)%$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Crit_Damage_Percent";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^Reduces damage from melee attacks by (\d+)%\.?$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Damage_Percent_Reduction_From_Melee";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^Reduces damage from ranged attacks by (\d+)%\.?$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Damage_Percent_Reduction_From_Ranged";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^Increases Damage Against Elites by (\d+)%$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Damage_Percent_Bonus_Vs_Elites";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^\+(\d+)% Damage to Demons$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Damage_Percent_Bonus_Vs_Monster_Type#Demon";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^Reduces damage from elites by (\d+)%\.?$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Damage_Percent_Reduction_From_Elites";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^Regenerates (\d+) Life per Second$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Hitpoints_Regen_Per_Second";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }
                else if ((match = Regex.Match(attribute, @"^(\d+\.\d+)% of Damage Dealt Is Converted to Life$", RegexOptions.IgnoreCase)).Success)
                {
                    name = "Steal_Health_Percent";
                    val = attributesRaw.ContainsKey(name) ? attributesRaw[name].Min : 0;
                    val += Double.Parse(match.Groups[1].Value) / 100;
                }

                if (!name.Equals(""))
                    attributesRaw[name] = new MinMax { Max = val, Min = val };
            }

            return attributesRaw;
        }
    }
}
