using System;

using BlockTotalCreator.ViewModels;

using Windows.UI.Xaml.Controls;

namespace BlockTotalCreator.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
