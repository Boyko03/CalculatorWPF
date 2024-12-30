using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Calculator.Command;

namespace Calculator.ViewModel
{
    public class StandardViewModel : INotifyPropertyChanged
    {
        private string _expressionBar = "";
        private int _result = 0;

        public int Result
        {
            get => _result;
            set
            {
                if (value == _result) return;
                _result = value;
                RaisePropertyChanged();
            }
        }

        private string NumberString { get; set; } = "0";

        public string ExpressionBar
        {
            get => _expressionBar;
            set
            {
                _expressionBar = value;
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

        private int _operand1;
        private int _operand2;
        private EOperation _operation = EOperation.None;
        private bool _isOp2Set;
        private bool _shouldResetInput;

        private void Number(object? parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            if (_shouldResetInput)
            {
                NumberString = "";
                _shouldResetInput = false;
            }

            NumberString += parameter;
            if (int.TryParse(NumberString, out var i))
            {
                Result = i;
            }
            else if (decimal.TryParse(NumberString, out var result))
            {
                // TODO Add floating point support
            }
            else
            {
                Result = (int)parameter;
                NumberString = Result.ToString();
            }
        }

        private void Add(object? obj)
        {
            if (ExpressionBar.Length == 0)
            {
                ExpressionBar = Result + "+";
            }
            if (ExpressionBar.EndsWith('+') || ExpressionBar.EndsWith('-')
                                            || ExpressionBar.EndsWith('*') || ExpressionBar.EndsWith('/'))
            {
                ExpressionBar = ExpressionBar.Remove(ExpressionBar.Length - 1, 1) + '+';
            }

            _operand1 += Result;
            _operation = EOperation.Add;
            _isOp2Set = false;
            _shouldResetInput = true;
        }

        private void Subtract(object? parameter)
        {
            if (ExpressionBar.Length == 0)
            {
                ExpressionBar = Result + "-";
            }
            if (ExpressionBar.EndsWith('+') || ExpressionBar.EndsWith('-')
                                            || ExpressionBar.EndsWith('*') || ExpressionBar.EndsWith('/'))
            {
                ExpressionBar = ExpressionBar.Remove(ExpressionBar.Length - 1, 1) + '-';
            }

            _operand1 -= Result;
            _operation = EOperation.Subtract;
            _isOp2Set = false;
            _shouldResetInput = true;
        }

        private void Multiply(object? parameter)
        {
            if (ExpressionBar.Length == 0)
            {
                ExpressionBar = Result + "*";
            }
            if (ExpressionBar.EndsWith('+') || ExpressionBar.EndsWith('-')
                                            || ExpressionBar.EndsWith('*') || ExpressionBar.EndsWith('/'))
            {
                ExpressionBar = ExpressionBar.Remove(ExpressionBar.Length - 1, 1) + '*';
            }

            _operand1 *= Result;
            _operation = EOperation.Multiply;
            _isOp2Set = false;
            _shouldResetInput = true;
        }

        private void Divide(object? parameter)
        {
            if (ExpressionBar.Length == 0)
            {
                ExpressionBar = Result + "/";
            }
            if (ExpressionBar.EndsWith('+') || ExpressionBar.EndsWith('-')
                                            || ExpressionBar.EndsWith('*') || ExpressionBar.EndsWith('/'))
            {
                ExpressionBar = ExpressionBar.Remove(ExpressionBar.Length - 1, 1) + '/';
            }

            _operand1 /= Result;
            _operation = EOperation.Divide;
            _isOp2Set = false;
            _shouldResetInput = true;
        }

        private void Calculate(object? parameter)
        {
            if (ExpressionBar.Length == 0)
            {
                ExpressionBar = Result + "=";
            }
            else if (!_isOp2Set)
            {
                ExpressionBar = ExpressionBar + Result + "=";
            }
            else
            {
                ExpressionBar = _operand1 + "".ToString(_operation) + _operand2 + "=";
            }

            if (!_isOp2Set)
            {
                _operand2 = Result;
                _isOp2Set = true;
            }

            switch (_operation)
            {
                case EOperation.None:
                    break;
                case EOperation.Add:
                    Result = _operand1 + _operand2;
                    break;
                case EOperation.Subtract:
                    Result = _operand1 - _operand2;
                    break;
                case EOperation.Multiply:
                    Result = _operand1 * _operand2;
                    break;
                case EOperation.Divide:
                    Result = _operand1 / _operand2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _operand1 = Result;
            _shouldResetInput = true;
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
    }
}
