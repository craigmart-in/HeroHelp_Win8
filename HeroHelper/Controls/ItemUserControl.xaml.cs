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
    public sealed partial class ItemUserControl : UserControl
    {
        private bool _isDirty;

        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;

                if (_isDirty)
                    DirtyBorder.Visibility = Windows.UI.Xaml.Visibility.Visible;
                else
                    DirtyBorder.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        public string DisplayTitle { get; set; }
        public string Key { get; set; }

        public ItemUserControl()
        {
            this.InitializeComponent();
        }
    }
}
