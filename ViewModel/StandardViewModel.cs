using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
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
            OperationCommand = new DelegateCommand(Operation);
            CalculateCommand = new DelegateCommand(Calculate);
        }

        public DelegateCommand NumberCommand { get; }
        public DelegateCommand OperationCommand { get; }
        public DelegateCommand CalculateCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private decimal _operand1;
        private decimal _operand2;
        private EOperation _operation = EOperation.None;
        private EOperation _prevOperation = EOperation.None;
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
                _isOperand2Set = false;
            }

            ResultBar += parameter;
        }

        private void Operation(object? parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            if (_shouldResetInput)
            {
                _operation = (EOperation)parameter;
                ExpressionBar = ResultBar + "".ToString(_operation);
                return;
            }

            _shouldResetInput = true;
            _isOperand2Set = false;
            _prevOperation = _operation;
            _operation = (EOperation)parameter;
            _operand1 = CalculateLastOperation();

            if ((int)_operand1 == _operand1)
            {
                _operand1 = (int)_operand1;
            }

            ResultBar = _operand1.ToString(CultureInfo.CurrentCulture);
            ExpressionBar = ResultBar + "".ToString(_operation);
        }

        private void Calculate(object? parameter)
        {
            var prevOp1 = _operand1;

            _shouldResetInput = true;
            _prevOperation = _operation;
            _operand1 = CalculateLastOperation();

            if ((int)_operand1 == _operand1)
            {
                _operand1 = (int)_operand1;
            }

            ResultBar = _operand1.ToString(CultureInfo.CurrentCulture);

            if (_prevOperation == EOperation.None)
            {
                ExpressionBar = ResultBar + "=";
            }
            else
            {
                ExpressionBar = prevOp1 + "".ToString(_prevOperation) + _operand2 + "=";
            }
        }

        private decimal CalculateLastOperation()
        {
            if (!_isOperand2Set)
            {
                _operand2 = ResultBar.ToDecimal();
                _isOperand2Set = true;
            }

            return _prevOperation switch
            {
                EOperation.None => _operand2,
                EOperation.Add => _operand1 + _operand2,
                EOperation.Subtract => _operand1 - _operand2,
                EOperation.Multiply => _operand1 * _operand2,
                EOperation.Divide => _operand1 / _operand2,
                _ => throw new ArgumentOutOfRangeException()
            };
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
