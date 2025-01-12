using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calculator.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public virtual string GetName => "";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
