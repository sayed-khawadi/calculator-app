namespace CalculatorApp.Models
{
    public class CalculationHistoryItem
    {
        public string Expression { get; }
        public string Result { get; }

        public string DisplayText => $"{Expression} = {Result}";

        public CalculationHistoryItem(string expression, string result)
        {
            Expression = expression;
            Result = result;
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}