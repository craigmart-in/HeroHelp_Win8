using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNet.D3.Models
{
    public abstract class ItemBase : D3Object
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }

        private string _displayColor;
        public string DisplayColor {
            get
            {
                if (_displayColor == "blue")
                    return "#a0c3ff";
                else
                    return _displayColor;
            }
            set
            {
                _displayColor = value;
            }
        }
        public string TooltipParams { get; set; }

        public Windows.UI.Xaml.Media.Imaging.BitmapImage DisplayIcon { get; set; }
        public Windows.UI.Xaml.Media.Imaging.BitmapImage BackgroundImage { get; set; }
    }
}
