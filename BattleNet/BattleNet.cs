using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleNet
{
    public static class BattleNet
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public static void GetBaseUriByRegion(Region region, out Uri apiUri, out Uri mediaUri)
        {
            switch (region)
            {
                case Region.EU:
                    {
                        apiUri = new Uri("http://eu.battle.net/");
                        mediaUri = new Uri("http://eu.media.blizzard.com/");
                        break;
                    }
                case Region.KR:
                    {
                        apiUri = new Uri("http://kr.battle.net/");
                        mediaUri = new Uri("http://kr.media.blizzard.com/");
                        break;
                    }
                case Region.TW:
                    {
                        apiUri = new Uri("http://tw.battle.net/");
                        mediaUri = new Uri("http://tw.media.blizzard.com/");
                        break;
                    }
                case Region.CN:
                    {
                        apiUri = new Uri("http://www.battlenet.com.cn/");
                        mediaUri = new Uri("http://us.media.blizzard.com/");
                        break;
                    }
                case Region.US:
                default:
                    {
                        apiUri = new Uri("http://us.battle.net/");
                        mediaUri = new Uri("http://us.media.blizzard.com/");
                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public static Uri GetBaseUriByRegion(Region region)
        {
            switch (region)
            {
                case Region.US:
                    {
                        return new Uri("http://us.battle.net/");
                    }
                case Region.EU:
                    {
                        return new Uri("http://eu.battle.net/");
                    }
                case Region.KR:
                    {
                        return new Uri("http://kr.battle.net/");
                    }
                case Region.TW:
                    {
                        return new Uri("http://tw.battle.net/");
                    }
                case Region.CN:
                    {
                        return new Uri("http://www.battlenet.com.cn/");
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        /// <summary>
        /// Determines if the battle tag is valid.
        /// </summary>
        /// <param name="battleTag"></param>
        /// <returns></returns>
        public static bool IsValidBattleTag(string battleTag)
        {
            string validBattleTagPattern = @"^\D.{2,11}#\d{4}$";
            bool valid = false;

            Match match = Regex.Match(battleTag, validBattleTagPattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                valid = true;
            }

            return valid;
        }
    }
}
