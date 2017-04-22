using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HotsDraftHelper
{
    internal sealed class SubGroupToColorConverter : IValueConverter
    {
        public Color Tank { get; set; }
        public Color Bruiser { get; set; }
        public Color Healer { get; set; }
        public Color Support { get; set; }
        public Color Ambusher { get; set; }
        public Color BurstDamage { get; set; }
        public Color SustainedDamage { get; set; }
        public Color Siege { get; set; }
        public Color Utility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var subGroup = value as string;
            if (subGroup == null)
                return DependencyProperty.UnsetValue;
            switch (subGroup)
            {
                case "Tank":
                    return new SolidColorBrush(Tank);
                case "Bruiser":
                    return new SolidColorBrush(Bruiser);
                case "Healer":
                    return new SolidColorBrush(Healer);
                case "Support":
                    return new SolidColorBrush(Support);
                case "Ambusher":
                    return new SolidColorBrush(Ambusher);
                case "Burst Damage":
                    return new SolidColorBrush(BurstDamage);
                case "Sustained Damage":
                    return new SolidColorBrush(SustainedDamage);
                case "Siege":
                    return new SolidColorBrush(Siege);
                case "Utility":
                    return new SolidColorBrush(Utility);
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
