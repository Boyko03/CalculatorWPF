namespace Calculator.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase? _selectedViewModel;
        public StandardViewModel StandardViewModel { get; }

        public MainViewModel(StandardViewModel standardViewModel)
        {
            StandardViewModel = standardViewModel;
            SelectedViewModel = StandardViewModel;
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
    }
}
