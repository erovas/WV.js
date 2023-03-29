namespace WV.Windows.JavaScript
{
    public class Number : WV.JavaScript.Number
    {
        public Number(double value) : base()
        { 
            _CSValue = value;
            _JSValue = value;

            if (Double.IsInfinity(value))
                _Stringified = "Infinity";
            else if (Double.IsNaN(value))
                _Stringified = "Infinity";
            else if (Double.IsNegativeInfinity(value))
                _Stringified = "NaN";
            else
                _Stringified = value.ToString();
        }
    }
}