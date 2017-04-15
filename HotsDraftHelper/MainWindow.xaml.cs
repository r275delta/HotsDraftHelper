using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HotsDraftHelper
{
    partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnDoubleClickAvailableAlly(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel)?.SelectAllyHero(((sender as DataGrid)?.SelectedItem as HeroSelectionViewModel)?.Hero);
        }

        private void OnDoubleClickSelectedAlly(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel)?.UnselectAllyHero(((sender as DataGrid)?.SelectedItem as HeroSelectionViewModel)?.Hero);
        }

        private void OnDoubleClickSelectedEnemy(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel)?.UnselectEnemyHero(((sender as DataGrid)?.SelectedItem as HeroSelectionViewModel)?.Hero);
        }

        private void OnDoubleClickAvailableEnemy(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MainViewModel)?.SelectEnemyHero(((sender as DataGrid)?.SelectedItem as HeroSelectionViewModel)?.Hero);
        }
    }

    internal sealed class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
