using BattleNet.D3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace HeroHelper.Controls
{
    public sealed partial class ItemCompareUserControl : UserControl
    {
        private List<TextBox> _inputs;
        private List<string> _attributeStrings;

        public Item PreviousItem;

        private Item _compareItem;
        public Item CompareItem
        {
            get { return _compareItem; }
            set
            {
                _compareItem = value;
                UpdateTexboxValues();
            }
        }
        public Dictionary<string, MinMax> AttributesRaw;

        public delegate void EquipButtonTappedEventHandler(object sender, EventArgs e);
        public event EquipButtonTappedEventHandler EquipButtonTapped;

        public ItemCompareUserControl()
        {
            this.InitializeComponent();

            _inputs = new List<TextBox>();
            _inputs.Add(MinDTextBox);
            _inputs.Add(MaxDTextBox);
            _inputs.Add(ArmTextBox);
            _inputs.Add(StrTextBox);
            _inputs.Add(DexTextBox);
            _inputs.Add(IntTextBox);
            _inputs.Add(VitTextBox);
            _inputs.Add(ResTextBox);
            _inputs.Add(LifeTextBox);
            _inputs.Add(CritDTextBox);
            _inputs.Add(CritCTextBox);
            _inputs.Add(IasTextBox);

            _attributeStrings = new List<string>();
            _attributeStrings.Add("Damage_Min#Physical");
            _attributeStrings.Add("Damage_Delta#Physical");
            _attributeStrings.Add("Armor_Item");
            _attributeStrings.Add("Strength_Item");
            _attributeStrings.Add("Dexterity_Item");
            _attributeStrings.Add("Intelligence_Item");
            _attributeStrings.Add("Vitality_Item");
            _attributeStrings.Add("Resistance_All");
            _attributeStrings.Add("Hitpoints_Max_Percent_Bonus_Item");
            _attributeStrings.Add("Crit_Damage_Percent");
            _attributeStrings.Add("Crit_Percent_Bonus_Capped");
            _attributeStrings.Add("Attacks_Per_Second_Percent");

            AttributesRaw = new Dictionary<string, MinMax>();
        }

        private void UpdateTexboxValues()
        {
            foreach (KeyValuePair<string, MinMax> rawAttribute in _compareItem.AttributesRaw)
            {
                switch (rawAttribute.Key)
                {
                    case "Damage_Min#Physical":
                        MinDTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Damage_Delta#Physical":
                        double value = rawAttribute.Value.Min + _compareItem.AttributesRaw["Damage_Min#Physical"].Min;
                        MaxDTextBox.Text = value.ToString();
                        break;
                    case "Armor_Item":
                        ArmTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Strength_Item":
                        StrTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Dexterity_Item":
                        DexTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Intelligence_Item":
                        IntTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Vitality_Item":
                        VitTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Resistance_All":
                        ResTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Hitpoints_Max_Percent_Bonus_Item":
                        LifeTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Crit_Damage_Percent":
                        CritDTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Crit_Percent_Bonus_Capped":
                        CritCTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                    case "Attacks_Per_Second_Percent":
                        IasTextBox.Text = rawAttribute.Value.Min.ToString();
                        break;
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox tb in _inputs)
            {
                tb.Text = String.Empty;
            }

            CompareItem = PreviousItem;
            EquipButtonTapped(this, EventArgs.Empty);
        }

        private void EquipButton_Click(object sender, RoutedEventArgs e)
        {
            EquipItem();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (tb.Text.Length == 0) return;

            var text = tb.Text;

            double result;
            var isValid = double.TryParse(text, out result);
            if (isValid) return;

            tb.Text = text.Remove(text.Length - 1);
            tb.SelectionStart = text.Length;
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EquipItem();
            }
        }

        private void EquipItem()
        {
            // Get rid of set/gem information
            CompareItem.Gems = null;
            CompareItem.Set = null;

            for (int i = 0; i < _inputs.Count; i++)
            {
                if (String.IsNullOrEmpty(_inputs[i].Text))
                    continue;

                double value = 0;

                if (double.TryParse(_inputs[i].Text, out value))
                {
                    if (_attributeStrings[i] == "Damage_Delta#Physical")
                    {
                        value -= AttributesRaw["Damage_Min#Physical"].Min;
                    }

                    //if (_attributeStrings[i] == "Hitpoints_Max_Percent_Bonus_Item" ||
                    //    _attributeStrings[i] == "Crit_Damage_Percent" ||
                    //    _attributeStrings[i] == "Crit_Percent_Bonus_Capped" ||
                    //    _attributeStrings[i] == "Attacks_Per_Second_Percent")
                    //{
                    //    value = value / 100;
                    //}

                    AttributesRaw[_attributeStrings[i]] = new MinMax() { Min = value, Max = value };
                }
                else
                {
                    CompareItem = PreviousItem;
                    return;
                }
            }

            CompareItem.AttributesRaw = AttributesRaw;

            EquipButtonTapped(this, EventArgs.Empty);
        }
    }
}
