using HotsDraftLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotsDraftHelper
{
    internal sealed class HeroSelectionViewModel : ViewModelBase
    {
        public Hero Hero { get; }
        public string HeroName => Hero.Name;

        private double _diff;
        public double Diff
        {
            get { return _diff; }
            set
            {
                if (_diff == value)
                    return;
                _diff = value;
                CallerPropertyChanged();
            }
        }

        private string _breakdown;
        public string Breakdown
        {
            get { return _breakdown; }
            private set
            {
                if (_breakdown == value)
                    return;
                _breakdown = value;
                CallerPropertyChanged();
            }
        }

        public HeroSelectionViewModel(Hero hero)
        {
            Hero = hero;
        }

        public void SetBreakdown(IReadOnlyList<(string source, double amount)> items)
        {
            if (items == null || items.Count == 0)
            {
                Breakdown = null;
                return;
            }
            double itemsTotal = items.Sum(i => i.amount);
            double scale = itemsTotal == 0 ? 0 : Diff / itemsTotal;
            var sb = new StringBuilder();
            sb.Append("Breakdown:");
            foreach (var item in items)
            {
                sb.AppendLine();
                sb.Append(item.source).Append(": ");
                sb.Append((item.amount * scale).ToString("P"));
            }
            Breakdown = sb.ToString();
        }
    }
}
