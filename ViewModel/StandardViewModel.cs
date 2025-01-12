using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Calculator.Command;

namespace Calculator.ViewModel
{
    public class StandardViewModel : ViewModelBase
    {
        public override string GetName => "Standard";

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

                if (_resultBar.EndsWith('.'))
                {
                    RaisePropertyChanged();
                    return;
                }

                if (_resultBar.ToDecimal(out var tmp))
                {
                    if ((int)tmp == tmp)
                    {
                        tmp = (int)tmp;
                        _resultBar = tmp.ToString("N0", CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        _resultBar = tmp.ToString("#,##0.################", CultureInfo.CurrentCulture);
                    }
                }

                RaisePropertyChanged();
            }
        }

        public StandardViewModel()
        {
            NumberCommand = new DelegateCommand(Number);
            PointCommand = new DelegateCommand(Point);
            OperationCommand = new DelegateCommand(Operation);
            CalculateCommand = new DelegateCommand(Calculate);

            NegateCommand = new DelegateCommand(Negate);
            OneOverCommand = new DelegateCommand(OneOver);
            SquareCommand = new DelegateCommand(Square);
            SquareRootCommand = new DelegateCommand(SquareRoot);

            ClearCommand = new DelegateCommand(Clear);
            ClearECommand = new DelegateCommand(ClearE);
            BackspaceCommand = new DelegateCommand(Backspace);
        }

        public DelegateCommand NumberCommand { get; }
        public DelegateCommand PointCommand { get; }
        public DelegateCommand OperationCommand { get; }
        public DelegateCommand CalculateCommand { get; }

        public DelegateCommand NegateCommand { get; }
        public DelegateCommand OneOverCommand { get; }
        public DelegateCommand SquareCommand { get; }
        public DelegateCommand SquareRootCommand { get; }

        public DelegateCommand ClearCommand { get; }
        public DelegateCommand ClearECommand { get; }
        public DelegateCommand BackspaceCommand { get; }

        private decimal _operand1;
        private decimal _operand2;
        private EOperation _operation = EOperation.None;
        private EOperation _prevOperation = EOperation.None;
        private bool _isOperand2Set;
        private bool _shouldResetInput = true;
        private string _expressionBar = "";
        private string _resultBar = "0";

        public override void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D8 when Keyboard.Modifiers == ModifierKeys.Shift:
                    Operation(EOperation.Multiply);
                    return;
                case Key.OemPlus when Keyboard.Modifiers == ModifierKeys.Shift:
                    Operation(EOperation.Add);
                    break;

                case Key.D0: case Key.NumPad0: Number(0); break;
                case Key.D1: case Key.NumPad1: Number(1); break;
                case Key.D2: case Key.NumPad2: Number(2); break;
                case Key.D3: case Key.NumPad3: Number(3); break;
                case Key.D4: case Key.NumPad4: Number(4); break;
                case Key.D5: case Key.NumPad5: Number(5); break;
                case Key.D6: case Key.NumPad6: Number(6); break;
                case Key.D7: case Key.NumPad7: Number(7); break;
                case Key.D8: case Key.NumPad8: Number(8); break;
                case Key.D9: case Key.NumPad9: Number(9); break;

                case Key.OemPeriod: case Key.Decimal: Point(null); break;

                case Key.OemMinus: case Key.Subtract: Operation(EOperation.Subtract); break;
                case Key.Multiply: Operation(EOperation.Multiply); break;
                case Key.OemQuestion: case Key.Divide: Operation(EOperation.Divide); break;

                case Key.Enter: case Key.OemPlus: Calculate(null); break;

                case Key.Escape: Clear(null); break;
                case Key.Delete: ClearE(null); break;
            }
        }

        private void Number(object? parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            if (_shouldResetInput)
            {
                if (_operation == EOperation.None)
                {
                    ExpressionBar = "";
                    _prevOperation = EOperation.None;
                }
                ResultBar = "";
                _shouldResetInput = false;
                _isOperand2Set = false;
            }

            ResultBar += parameter;
        }

        private void Point(object? parameter)
        {
            if (!_shouldResetInput && ResultBar.Contains('.')) return;

            if (_shouldResetInput)
            {
                if (_operation == EOperation.None)
                {
                    ExpressionBar = "";
                    _prevOperation = EOperation.None;
                }
                ResultBar = "0";
                _shouldResetInput = false;
                _isOperand2Set = false;
            }

            ResultBar += ".";
        }

        private void Operation(object? parameter)
        {
            ArgumentNullException.ThrowIfNull(parameter);

            if (_shouldResetInput)
            {
                _operation = (EOperation)parameter;
                ResultBar = _operand1.ToString(CultureInfo.CurrentCulture);
                ExpressionBar = ResultBar + "".ToString(_operation);
                return;
            }

            _shouldResetInput = true;
            _isOperand2Set = false;
            _prevOperation = _operation;
            _operation = (EOperation)parameter;

            try
            {
                _operand1 = CalculateLastOperation();

                if ((int)_operand1 == _operand1)
                {
                    _operand1 = (int)_operand1;
                }

                ResultBar = _operand1.ToString(CultureInfo.CurrentCulture);
                ExpressionBar = ResultBar + "".ToString(_operation);
            }
            catch (DivideByZeroException)
            {
                _operand1 = _operand2 = 0;
                _shouldResetInput = true;
                _isOperand2Set = false;
                _prevOperation = _operation = EOperation.None;

                ResultBar = "Result is undefined";
                ExpressionBar = "";
            }
        }

        private void Calculate(object? parameter)
        {
            var prevOp1 = _operand1;

            if (!_shouldResetInput)
            {
                _shouldResetInput = true;
            }

            if (_operation != EOperation.None)
            {
                _prevOperation = _operation;
                _operation = EOperation.None;
            }

            try

            {
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
            catch (DivideByZeroException)
            {
                _operand1 = _operand2 = 0;
                _shouldResetInput = true;
                _isOperand2Set = false;
                _prevOperation = _operation = EOperation.None;

                ResultBar = "Result is undefined";
                ExpressionBar = "";
            }
        }

        private void Negate(object? parameter)
        {
            ResultBar.ToDecimal(out var d);
            ResultBar = (-d).ToString(CultureInfo.CurrentCulture);

            _isOperand2Set = false;
        }

        private void OneOver(object? parameter)
        {
            throw new NotImplementedException();
        }

        private void Square(object? parameter)
        {
            throw new NotImplementedException();
        }

        private void SquareRoot(object? parameter)
        {
            throw new NotImplementedException();
        }

        private void Clear(object? parameter)
        {
            _isOperand2Set = false;
            _shouldResetInput = true;
            _prevOperation = _operation = EOperation.None;

            ExpressionBar = "";
            ResultBar = "0";

            _operand1 = 0;
        }

        private void ClearE(object? parameter)
        {
            if (_operation == EOperation.None)
            {
                _shouldResetInput = true;

                //ExpressionBar = "";
                ResultBar = "0";

                _operand1 = 0;
            }
            else
            {
                _isOperand2Set = false;
                _shouldResetInput = true;

                ResultBar = "0";
            }
        }

        private void Backspace(object? parameter)
        {
            throw new NotImplementedException();
        }

        private decimal CalculateLastOperation()
        {
            if (!_isOperand2Set)
            {
                ResultBar.ToDecimal(out _operand2);
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

        public static bool ToDecimal(this string expression, out decimal d)
        {
            expression = expression.Replace(",", "");

            if (decimal.TryParse(expression, out var result))
            {
                d = result;
                return true;
            }

            if (int.TryParse(expression, out var iResult))
            {
                d = iResult;
                return true;
            }

            d = 0;
            return false;
        }
    }
}
