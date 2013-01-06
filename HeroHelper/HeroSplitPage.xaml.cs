using BattleNet.D3;
using BattleNet.D3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace HeroHelper
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class HeroSplitPage : HeroHelper.Common.LayoutAwarePage
    {
        private D3Client _d3Client;
        private Profile _profile;

        private Hero[] _heroes;

        private ObservableCollection<CompareStat> _compareStats;

        private bool _isFirstLoad = true;

        public HeroSplitPage()
        {
            this.InitializeComponent();
        }

        #region Page state management

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            string selectedHeroJson = (string)navigationParameter;
            SelectedHero selectedHero = JsonConvert.DeserializeObject<SelectedHero>(selectedHeroJson);

            _profile = selectedHero.Profile;
            _heroes = new Hero[_profile.Heroes.Count];
            _d3Client = new D3Client(_profile.Region);

            _compareStats = new ObservableCollection<CompareStat>();
            compareStatsItemsViewSource.Source = _compareStats;

            try
            {
                StorageFile sampleFile =
                            await ApplicationData.Current.LocalFolder.GetFileAsync(_profile.BattleTag.Replace("#", "-") + ".txt");

                string heroes = await FileIO.ReadTextAsync(sampleFile);

                if (!String.IsNullOrEmpty(heroes))
                {
                    _heroes = JsonConvert.DeserializeObject<Hero[]>(heroes);
                }
            }
            catch (FileNotFoundException) { } // File doesn't exist.

            this.DefaultViewModel["Group"] = _profile;
            this.DefaultViewModel["Heroes"] = _profile.Heroes;

            if (pageState == null)
            {
                // When this is a new page, select the first item automatically unless logical page
                // navigation is being used (see the logical page navigation #region below.)
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    if (_isFirstLoad && itemListView.SelectedIndex == selectedHero.HeroIndex)
                        LoadHero(itemListView.Items[0] as ProfileHero, 0);

                    _isFirstLoad = false;
                    itemListView.SelectedIndex = selectedHero.HeroIndex;
                }
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (pageState.ContainsKey("SelectedHero") && this.itemsViewSource.View != null)
                {
                    _isFirstLoad = false;
                    itemListView.SelectedIndex = (int)pageState["SelectedHero"];
                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = this.itemsViewSource.View.CurrentItem;
                // TODO: Derive a serializable navigation parameter and assign it to
                pageState["SelectedHero"] = this.itemsViewSource.View.CurrentPosition;
            }
        }

        #endregion

        #region Logical page navigation

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed, or null
        /// for the current view state.  This parameter is optional with null as the default
        /// value.</param>
        /// <returns>True when the view state in question is portrait or snapped, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is Snapped)
        /// displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isFirstLoad)
                return;
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped ||
                Windows.UI.ViewManagement.ApplicationView.TryUnsnap() == true)
            {
                toolTip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                Selector list = sender as Selector;
                ProfileHero selectedItem = list.SelectedItem as ProfileHero;

                if (selectedItem != null)
                {
                    LoadHero(selectedItem, list.SelectedIndex);
                }
            }
        }

        private async void LoadHero(ProfileHero selectedItem, int selectedHero)
        {
            if (_heroes[selectedHero] == null)
            {
                Hero hero = await _d3Client.GetHeroAsync(_profile.BattleTag.Replace("#", "-"), selectedItem.Id);
                hero.PaperdollPath = "Assets/paperdoll-" + selectedItem.Class + "-" + selectedItem.Gender + ".jpg";

                LoadHeroSkills(hero);

                await LoadHeroItems(hero);

                _heroes[selectedHero] = hero;

                // Clear out previous hero details
                itemDetail.DataContext = null;

                SaveHeroes();
            }

            // Clear out previous hero details
            itemDetail.DataContext = null;
            itemDetail.DataContext = _heroes[selectedHero];
        }

        /// <summary>
        /// Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.itemListView.SelectedItem = null;
            }
            else
            {
                // When logical page navigation is not in effect, or when there is no selected
                // item, use the default back button behavior.
                base.GoBack(sender, e);
            }
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed.</param>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Determine visual states for landscape layouts based not on the view state, but
            // on the width of the window.  This page has one layout that is appropriate for
            // 1366 virtual pixels or wider, and another for narrower displays or when a snapped
            // application reduces the horizontal space available to less than 1366.
            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            // When in portrait or snapped start with the default visual state name, then add a
            // suffix when viewing details instead of the list
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion

        private async void SaveHeroes()
        {
            string heroesJson = JsonConvert.SerializeObject(_heroes);

            StorageFile sampleFile =
                await ApplicationData.Current.LocalFolder.CreateFileAsync(_profile.BattleTag.Replace("#", "-") + ".txt",
                CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(sampleFile, heroesJson);
        }

        private async Task<bool> LoadHeroItems(Hero hero)
        {
            string size = "large";

            Dictionary<string, Item> temp = new Dictionary<string, Item>();

            foreach (KeyValuePair<string, Item> item in hero.Items)
            {
                temp[item.Key] = await _d3Client.GetItemAsync(item.Value.TooltipParams);
                temp[item.Key].DisplayIcon = _d3Client.GetItemIcon(size, item.Value.Icon);
                temp[item.Key].BackgroundImage = GetItemBackgroundImage(item.Value.DisplayColor);
            }

            hero.Items = temp;
            // Clone original hero items to compare items to start off.
            hero.CompareItems = (from x in temp
                                 select x).ToDictionary(x => x.Key, x => x.Value.DeepCopyForCompare());

            hero.CalculatedStats = Domain.CalculateStats.CalculateStatsFromHero(hero, hero.Items);

            return true;
        }

        private void LoadHeroSkills(Hero hero)
        {
            for (int i = 0; i < hero.Skills.Active.Count; i++)
            {
                int x = 22 * (i % 2);
                int y = 0;
                if (i == 2 || i == 3)
                    y = 22;
                if (i == 4 || i == 5)
                    y = 44;

                if (hero.Skills.Active[i].Skill == null)
                {
                    hero.Skills.Active[i].Skill = new Skill();
                    //hero.Skills.Active[i].Skill.DisplayIcon = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/skill-overlays.png"));

                    //hero.Skills.Active[i].Skill.DisplayIconViewRect = String.Format("{0},{1},42,42", new object[] { x, y });
                    //hero.Skills.Active[i].Skill.DisplayIconMargin = String.Format("{0},{1},0,0", new object[] { x * -1, y * -1 });
                }

                hero.Skills.Active[i].Skill.OverlayViewRect = String.Format("{0},{1},22,22", new object[] { x, y });
                hero.Skills.Active[i].Skill.OverlayMargin = String.Format("{0},{1},0,0", new object[] { x * -1, y * -1 });

                hero.Skills.Active[i].Skill.DisplayIcon = _d3Client.GetSkillIcon("42", hero.Skills.Active[i].Skill.Icon);
            }

            for (int i = 0; i < hero.Skills.Passive.Count; i++)
            {
                if (hero.Skills.Passive[i].Skill == null)
                {
                    hero.Skills.Passive[i].Skill = new Skill();
                }

                hero.Skills.Passive[i].Skill.DisplayIcon = _d3Client.GetSkillIcon("42", hero.Skills.Passive[i].Skill.Icon);
            }
        }

        private Windows.UI.Xaml.Media.Imaging.BitmapImage GetItemBackgroundImage(string displayColor)
        {
            switch (displayColor)
            {
                case "blue":
                    return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/blue.png"));
                case "green":
                    return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/green.png"));
                case "orange":
                    return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/orange.png"));
                case "yellow":
                    return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/yellow.png"));
                case "gray":
                case "white":
                default:
                    return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(this.BaseUri, "Assets/brown.png"));
            }
        }

        private void ItemUserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Item equippedItem = (sender as HeroHelper.Controls.ItemUserControl).DataContext as Item;
            if (equippedItem != null)
            {
                string tooltipParams = equippedItem.TooltipParams;
                UpdateTooltip(tooltipParams);
            }
        }

        private void ItemUserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            toolTip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void toolTip_LoadCompleted(object sender, NavigationEventArgs e)
        {
            toolTip.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void ItemUserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Item equippedItem = (sender as HeroHelper.Controls.ItemUserControl).DataContext as Item;
            if (equippedItem != null)
            {
                string tooltipParams = equippedItem.TooltipParams;
                UpdateTooltip(tooltipParams);

                if (!ItemTooltipPopup.IsOpen) { ItemTooltipPopup.IsOpen = true; }

                // Reposition the tooltip to be next to the cursor.
                //Point point = e.GetPosition(itemDetail);
                //double newY = point.Y;

                //if ((point.Y + 350) >= itemDetail.ExtentHeight)
                //    newY = itemDetail.ExtentHeight - 375;

                //toolTip.Margin = new Thickness(point.X - 400, newY, 0, 0);
            }
        }

        private async void UpdateTooltip(string tooltipParams)
        {
            // Don't show tool tip in portrait
            //if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.FullScreenPortrait)
            //{
            //    toolTip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //    return;
            //}

            //if (toolTip.Visibility != Windows.UI.Xaml.Visibility.Visible)
            //{
                string tooltipHtml = await _d3Client.GetItemToolTip(tooltipParams);
                tooltipHtml =
                            "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" +
                            "<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"http://us.battle.net/d3/static/css/tooltips.css?v51\" />" +
                            "<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"http://us.battle.net/d3/static/local-common/css/common-ie.css?v42\" />" +
                            "<meta charset=\"UTF-8\">" +
                            "<body scroll=\"no\" style='padding:0;margin:0;background-color:black' >" +
                            tooltipHtml +
                            "</body>";
                toolTip.NavigateToString(tooltipHtml);
            //}
            //else
            //{
            //    toolTip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //}
        }

        private void BaseStatsTab_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            switch (BaseStatDetails.Visibility)
            {
                case Windows.UI.Xaml.Visibility.Collapsed:
                    BaseStatDetails.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    BaseStatTabText.Text = "-";
                    break;
                case Windows.UI.Xaml.Visibility.Visible:
                    BaseStatDetails.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    BaseStatTabText.Text = "+";
                    break;
            }
        }

        private void DefStatsTab_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            switch (DefStatDetails.Visibility)
            {
                case Windows.UI.Xaml.Visibility.Collapsed:
                    DefStatDetails.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    DefStatTabText.Text = "-";
                    break;
                case Windows.UI.Xaml.Visibility.Visible:
                    DefStatDetails.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    DefStatTabText.Text = "+";
                    break;
            }
        }

        private void DamStatsTab_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            switch (DamStatDetails.Visibility)
            {
                case Windows.UI.Xaml.Visibility.Collapsed:
                    DamStatDetails.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    DamStatTabText.Text = "-";
                    break;
                case Windows.UI.Xaml.Visibility.Visible:
                    DamStatDetails.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    DamStatTabText.Text = "+";
                    break;
            }
        }

        private void LifeStatsTab_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            switch (LifeStatDetails.Visibility)
            {
                case Windows.UI.Xaml.Visibility.Collapsed:
                    LifeStatDetails.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    LifeStatTabText.Text = "-";
                    break;
                case Windows.UI.Xaml.Visibility.Visible:
                    LifeStatDetails.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    LifeStatTabText.Text = "+";
                    break;
            }
        }

        private void RefreshHeroButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the bottom app bar.
            BottomAppBar.IsOpen = false;
        }

        private void ItemUserControl_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ShowItemCompare(sender);
        }

        private void ItemUserControl_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowItemCompare(sender);
        }

        private void ShowItemCompare(object sender)
        {
            if (!ItemComparePopup.IsOpen)
            {
                /* The UI guidelines for a proper 'Settings' flyout are such that it should fill the height of the 
                current Window and be either narrow (346px) or wide (646px)
                Using the measurements of the Window.Curent.Bounds will help you position correctly.
                This sample here shows a simple *example* of this using the Width to get the HorizontalOffset but
                the app developer will have to perform these measurements depending on the structure of the app's 
                views in their code */
                ItemCompareUC.Width = 346;

                Item previousItem = (sender as HeroHelper.Controls.ItemUserControl).DataContext as Item;

                int selectedIndex = itemListView.SelectedIndex;
                foreach (KeyValuePair<string, Item> item in _heroes[selectedIndex].CompareItems)
                {
                    if (item.Value.Id == previousItem.Id)
                    {
                        ItemCompareUC.CompareItem = item.Value;
                        break;
                    }
                }
                ItemCompareUC.PreviousItem = previousItem;

                ItemComparePopup.HorizontalOffset = grid.Width - 346;

                ItemComparePopup.IsOpen = true;
            }
        }

        private void ItemCompareUC_EquipButtonTapped(object sender, EventArgs e)
        {
            ItemComparePopup.IsOpen = false;
            int count = _compareStats.Count;
            for (int i = 0; i < count; i++)
            {
                _compareStats.RemoveAt(0);
            }

            int selectedIndex = itemListView.SelectedIndex;
            string key = String.Empty;

            foreach (KeyValuePair<string, Item> item in _heroes[selectedIndex].Items)
            {
                if (item.Value.Id == ItemCompareUC.PreviousItem.Id)
                {
                    key = item.Key;
                }
            }

            _heroes[selectedIndex].CompareItems[key] = ItemCompareUC.CompareItem;

            CalculatedStats compareItemStats = Domain.CalculateStats.CalculateStatsFromHero(_heroes[selectedIndex], _heroes[selectedIndex].CompareItems);
            
            for (int i = 0; i < _heroes[selectedIndex].CalculatedStats.BaseStats.Count; i++)
            {
                CalculateCompareStat(compareItemStats, _heroes[selectedIndex].CalculatedStats.BaseStats[i], compareItemStats.BaseStats[i]);
            }

            for (int i = 0; i < _heroes[selectedIndex].CalculatedStats.DamageStats.Count; i++)
            {
                CalculateCompareStat(compareItemStats, _heroes[selectedIndex].CalculatedStats.DamageStats[i], compareItemStats.DamageStats[i]);
            }

            for (int i = 0; i < _heroes[selectedIndex].CalculatedStats.DefenseStats.Count; i++)
            {
                CalculateCompareStat(compareItemStats, _heroes[selectedIndex].CalculatedStats.DefenseStats[i], compareItemStats.DefenseStats[i]);
            }

            for (int i = 0; i < _heroes[selectedIndex].CalculatedStats.LifeStats.Count; i++)
            {
                CalculateCompareStat(compareItemStats, _heroes[selectedIndex].CalculatedStats.LifeStats[i], compareItemStats.LifeStats[i]);
            }

            for (int i = 0; i < _heroes[selectedIndex].CalculatedStats.AdventureStats.Count; i++)
            {
                CalculateCompareStat(compareItemStats, _heroes[selectedIndex].CalculatedStats.AdventureStats[i], compareItemStats.AdventureStats[i]);
            }

            if (_compareStats.Count > 0)
            {
                CompareResultsGrid.Visibility = Visibility.Visible;
                CalculatedStatsScrollViewer.Visibility = Visibility.Collapsed;

                double dpsChange = compareItemStats.DPS - _heroes[selectedIndex].CalculatedStats.DPS;
                dpsChangeTextBlock.Text = String.Format("{0:N}", dpsChange);
                dpsChangeTextBlock.Foreground = dpsChange > 0 ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 245, 0)) : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 245, 0, 0));
                dpsTextBlock.Text = String.Format("{0:N}", compareItemStats.DPS);

                double ehpChange = compareItemStats.EHP - _heroes[selectedIndex].CalculatedStats.EHP;
                ehpChangeTextBlock.Text = String.Format("{0:N}", ehpChange);
                ehpChangeTextBlock.Foreground = ehpChange > 0 ? new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 245, 0)) : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 245, 0, 0));
                ehpTextBlock.Text = String.Format("{0:N}", compareItemStats.EHP);
            }
            else
            {
                CompareResultsGrid.Visibility = Visibility.Collapsed;
                CalculatedStatsScrollViewer.Visibility = Visibility.Visible;

                dpsTextBlock.Text = String.Format("{0:N}", _heroes[selectedIndex].CalculatedStats.DPS);
                ehpTextBlock.Text = String.Format("{0:N}", _heroes[selectedIndex].CalculatedStats.EHP);
            }
        }

        private void CalculateCompareStat(CalculatedStats compareItemStats, CalculatedStat oldStat, CalculatedStat newStat)
        {
            //for (int j = 0; j < oldStat.Values.Length; j++)
            //{
            //    if (oldStat.Values[j] != newStat.Values[j])
            //    {
            //        double[] values = new double[oldStat.Values.Length];

            //        for (int k = 0; k < oldStat.Values.Length; k++)
            //        {
            //            values[k] = newStat.Values[k] - oldStat.Values[k];
            //        }

            //        object[] obj = new object[values.Length];
            //        for (int l = 0; l < values.Length; l++)
            //            obj[i] = values[l];

            //        double diffValue
            //    }
            //}

            if (oldStat.Value != newStat.Value)
            {
                double diffValue = newStat.Value - oldStat.Value;

                string diff = String.Format(oldStat.Format, new object[] { diffValue });
                string color = diffValue < 0 ? "Red" : "Green";
                _compareStats.Add(new CompareStat() { Name = oldStat.Name, Before = oldStat.DisplayValue, After = newStat.DisplayValue, Difference = diff.ToString(), DifferenceColor = color });
            }
        }
    }
}
