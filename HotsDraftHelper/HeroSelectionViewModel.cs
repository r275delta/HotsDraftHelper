using HotsDraftHelper.Data;

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
            set
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
    }
}
