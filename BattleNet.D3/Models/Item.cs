using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Item : ItemBase
    {
        public int RequiredLevel { get; set; }
        public int ItemLevel { get; set; }
        public int BonusAffixes { get; set; }
        public MinMax Dps { get; set; }
        public string FlavorText { get; set; }
        public string TypeName { get; set; }
        public ItemType Type { get; set; }
        public MinMax Armor { get; set; }
        public List<string> Attributes { get; set; }
        //public List<SocketEffect> SocketEffects { get; set; }

        //public List<Salvage> Salvage { get; set; }
        //public List<SocketedGem> Gems { get; set; }
        public MinMax AttacksPerSecond { get; set; }
        public MinMax MinDamage { get; set; }
        public MinMax MaxDamage { get; set; }
    }
}
