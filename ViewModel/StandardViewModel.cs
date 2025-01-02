using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Calculator.Command;

namespace Calculator.ViewModel
{
    public class StandardViewModel : INotifyPropertyChanged
    {
        public string ExpressionBar
        {
            get => _expressionBar;
            set
            {
                if (value == _expressionBar) return;
                _expressionBar = value;
                RaisePropertyChanged();
            }
        }

        public string ResultBar
        {
            get => _resultBar;
            set
            {
                if (value == _resultBar) return;
                _resultBar = value;
                RaisePropertyChanged();
            }
        }

        public StandardViewModel()
        {
            NumberCommand = new DelegateCommand(Number);
            AddCommand = new DelegateCommand(Add);
            SubtractCommand = new DelegateCommand(Subtract);
            MultiplyCommand = new DelegateCommand(Multiply);
            DivideCommand = new DelegateCommand(Divide);
            CalculateCommand = new DelegateCommand(Calculate);
        }

        public DelegateCommand NumberCommand { get; }
        public DelegateCommand AddCommand { get; }
        public DelegateCommand SubtractCommand { get; }
        public DelegateCommand MultiplyCommand { get; }
        public DelegateCommand DivideCommand { get; }
        public DelegateCommand CalculateCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private decimal _operand1;
        private decimal _operand2;
        private EOperation _operation = EOperation.None;
        private bool _isOperand2Set;
        private bool _shouldResetInput = true;
        private string _expressionBar;
        private string _resultBar = "0";

        private void Number(object? parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            if (_shouldResetInput)
            {
                ResultBar = "";
                _shouldResetInput = false;
            }

            ResultBar += parameter;
        }

        private void Add(object? parameter)
        {
            if (_shouldResetInput) { return; }

            _shouldResetInput = true;
            _operation = EOperation.Add;

            _operand1 += ResultBar.ToDecimal();

            ResultBar = _operand1.ToString(CultureInfo.CurrentCulture);
            ExpressionBar = ResultBar + "+";
        }

        private void Subtract(object? parameter)
        {
        }

        private void Multiply(object? parameter)
        {
        }

        private void Divide(object? parameter)
        {
        }

        private void Calculate(object? parameter)
        {
        }
    }

    internal enum EOperation
    {
        None,
        Add,
        Subtract,
        Multiply,
        Divide
    }

    internal static class OperationToString
    {
        public static string ToString(this string text, EOperation operation)
        {
            switch (operation)
            {
                case EOperation.None:
                    break;
                case EOperation.Add:
                    return "+";
                case EOperation.Subtract:
                    return "-";
                case EOperation.Multiply:
                    return "*";
                case EOperation.Divide:
                    return "/";
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            return "";
        }

        public static decimal ToDecimal(this string expression)
        {
            if (decimal.TryParse(expression, out var result))
            {
                return result;
            }

            return int.TryParse(expression, out var iResult) ? iResult : 0;
        }
    }
}
