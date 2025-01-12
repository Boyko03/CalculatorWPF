using Calculator.Command;

namespace Calculator.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase? _selectedViewModel;
        public StandardViewModel StandardViewModel { get; }
        public DateCalculationViewModel DateCalculationViewModel { get; }

        public MainViewModel(StandardViewModel standardViewModel,
            DateCalculationViewModel dateCalculationViewModel)
        {
            StandardViewModel = standardViewModel;
            DateCalculationViewModel = dateCalculationViewModel;
            SelectedViewModel = StandardViewModel;
            SelectViewModelCommand = new DelegateCommand(SelectViewModel);
        }

        public ViewModelBase? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                if (Equals(value, _selectedViewModel)) return;
                _selectedViewModel = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SelectViewModelCommand { get; }

        private void SelectViewModel(object? parameter)
        {
            SelectedViewModel = parameter as ViewModelBase;
        }
    }
}
