using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotsDraftHelper
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected void CallerPropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
