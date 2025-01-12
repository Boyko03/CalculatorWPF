using System.Windows;
using Calculator.ViewModel;

namespace Calculator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow(new MainViewModel(new StandardViewModel(),
                new DateCalculationViewModel()));
            mainWindow.Show();
        }
    }

}
