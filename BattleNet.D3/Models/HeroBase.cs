using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public abstract class HeroBase : D3Object
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int ParagonLevel { get; set; }
        public bool Hardcore { get; set; }
        public Gender Gender { get; set; }
        public string Class { get; set; }

        private string _friendlyClass;
        public string FriendlyClass
        {
            get
            {
                switch (this.Class)
                {
                    case "barbarian":
                       return "Barbarian";
                    case "demon-hunter":
                       return "Demon Hunter";
                    case "wizard":
                       return "Wizard";
                    case "monk":
                       return "Monk";
                    case "witch-doctor":
                       return "Witch Doctor";
                    default:
                       return "Invalid";
                }
            }
            set 
            {
                _friendlyClass = value;
            }
        }

        public string PaperdollPath { get; set; }
    }
}
