namespace NifTyPredictor
{
    public class Company
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Identifier { get; set; }
        public decimal Open { get; set; }
        public decimal DayHigh { get; set; }
        public decimal DayLow { get; set; }
        public decimal LastPrice { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal Change { get; set; }
        public decimal PChange { get; set; }
        public decimal YearHigh { get; set; }
        public decimal YearLow { get; set; }
        public decimal ffmc { get; set; }
        public double totalTradedVolume { get; set; }
        
        public decimal PredictedValueEMA { get; set; } // Add this for EMA prediction
        public decimal PredictedValueLR { get; set; } // Add this for Linear Regression prediction

    }
}
