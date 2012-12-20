using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public class Skill : D3Object
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Level { get; set; }
        public string CategorySlug { get; set; }
        public string TooltipUrl { get; set; }
        public string Description { get; set; }
        public string SimpleDescription { get; set; }
        public string SkillCalcId { get; set; }

        public string OverlayMargin { get; set; }
        public string OverlayViewRect { get; set; }

        public Windows.UI.Xaml.Media.Imaging.BitmapImage DisplayIcon { get; set; }
    }
}
