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

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                TitleTextBlock.Text = _title = value;
            }
        }

        public string Key { get; set; }

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
            foreach (TextBox tb in _inputs)
            {
                tb.Text = String.Empty;
            }

            // Don't set values if there aren't any.
            if (_compareItem == null || _compareItem.AttributesRaw == null)
                return;

            // Get all of the raw attributes
            List<Dictionary<string, MinMax>> rawAttributes = new List<Dictionary<string, MinMax>>();
            rawAttributes.Add(_compareItem.AttributesRaw);

            if (_compareItem.Gems != null)
            {
                foreach (SocketedGem gem in _compareItem.Gems)
                {
                    rawAttributes.Add(gem.AttributesRaw);
                }
            }

            double armFromItems = 0;

            double allResFromItems = 0;
            Dictionary<string, double> resFromItems = new Dictionary<string, double>();
            resFromItems.Add("Fire", 0);
            resFromItems.Add("Lightning", 0);
            resFromItems.Add("Poison", 0);
            resFromItems.Add("Physical", 0);
            resFromItems.Add("Arcane", 0);
            resFromItems.Add("Cold", 0);
            double strFromItems = 0;
            double dexFromItems = 0;
            double intFromItems = 0;
            double vitFromItems = 0;
            double lifePctFromItems = 0;

            double critDamage = 0;
            double critChance = 0;
            double ias = 0;
            double aps = 1;

            double eleDmg = 0;
            double loh = 0;
            double minDmg = 0;
            double maxDmg = 0;

            double eliteBonus = 0;
            double demonBonus = 0;

            double ls = 0;
            double lifeRegen = 0;

            double blockChance = 0;
            double blockMin = 0;
            double blockMax = 0;

            foreach (Dictionary<string, MinMax> raw in rawAttributes)
            {
                Domain.CalculateStats.CalculateStatsFromRawAttributes(raw, false, ref allResFromItems, ref strFromItems,
                            ref dexFromItems, ref intFromItems, ref vitFromItems, ref lifePctFromItems, ref armFromItems,
                            ref critDamage, ref critChance, ref ias, ref aps, ref resFromItems,
                            ref eleDmg, ref loh, ref minDmg, ref maxDmg,
                            ref eliteBonus, ref demonBonus, ref ls, ref lifeRegen,
                            ref blockChance, ref blockMin, ref blockMax);
            }


            if (_compareItem.Dps != null)
            {
                double mhPhysMinDmg;
                double mhPhysMaxDmg;
                Domain.CalculateStats.CalculateWeaponDamageFromRawAttributes(
                    _compareItem.AttributesRaw, out mhPhysMinDmg, out mhPhysMaxDmg,
                    out minDmg, out maxDmg);
            }

            MinDTextBox.Text = minDmg.ToString();
            MaxDTextBox.Text = maxDmg.ToString();
            ArmTextBox.Text = armFromItems.ToString("N2");
            StrTextBox.Text = strFromItems.ToString();
            DexTextBox.Text = dexFromItems.ToString();
            IntTextBox.Text = intFromItems.ToString();
            VitTextBox.Text = vitFromItems.ToString();
            ResTextBox.Text = allResFromItems.ToString();
            LifeTextBox.Text = (lifePctFromItems * 100).ToString();
            CritDTextBox.Text = (critDamage * 100).ToString();
            CritCTextBox.Text = (critChance * 100).ToString();
            IasTextBox.Text = (ias * 100).ToString();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox tb in _inputs)
            {
                tb.Text = String.Empty;
            }

            CompareItem = PreviousItem.DeepCopyForCompare();

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

                    if (_attributeStrings[i] == "Hitpoints_Max_Percent_Bonus_Item" ||
                        _attributeStrings[i] == "Crit_Damage_Percent" ||
                        _attributeStrings[i] == "Crit_Percent_Bonus_Capped" ||
                        _attributeStrings[i] == "Attacks_Per_Second_Percent")
                    {
                        value = value / 100;
                    }

                    AttributesRaw[_attributeStrings[i]] = new MinMax() { Min = value, Max = value };
                }
                else
                {
                    CompareItem = PreviousItem.DeepCopyForCompare();
                    return;
                }
            }

            CompareItem.AttributesRaw = AttributesRaw;

            CompareItem.IsDirty = true;

            EquipButtonTapped(this, EventArgs.Empty);
        }
    }
}
