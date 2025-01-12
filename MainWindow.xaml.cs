using System.Windows;
using System.Windows.Input;
using Calculator.ViewModel;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            _viewModel.SelectedViewModel?.OnKeyDown(sender, e);
        }
    }
}