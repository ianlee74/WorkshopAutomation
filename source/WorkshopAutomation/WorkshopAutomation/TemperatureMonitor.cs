namespace WorkshopAutomation
{
    public enum TemperatureState
    {
        Unknown,
        TooLow,
        Normal,
        TooHigh
    }

    public delegate void TempChangeHandler(object sender, double tempC);

    public class TemperatureMonitor
    {
        // Properties
        public double TargetTempC { get; set; }
        public double TempTolerance { get; set; }
        public TemperatureState TempState { get; private set; }

        public TemperatureMonitor()
        {
            TempState = TemperatureState.Unknown;    
        }

        public double CurrentTemp
        {
            get { return _currentTemp; }
            set
            {
                _currentTemp = value;
                EvaluateCurrentTemp();
            }
        }
        private double _currentTemp;

        private void EvaluateCurrentTemp()
        {
            // Is temp too low?
            if (CurrentTemp < TargetTempC - TempTolerance)
            {
                if (TempState == TemperatureState.TooLow) return;
                OnTempTooLow();
            }
            // Is temp normal?
            if (CurrentTemp > TargetTempC - TempTolerance
                && CurrentTemp < TargetTempC + TempTolerance)
            {
                if (TempState == TemperatureState.Normal) return;
                OnTempNormal();
            }
            // Is temp too high?
            if (CurrentTemp > TargetTempC + TempTolerance)
            {
                if (TempState == TemperatureState.TooHigh) return;
                OnTempTooHigh();
            }
        }

        // Events
        public event TempChangeHandler TempTooHigh;
        protected void OnTempTooHigh()
        {            
            if (TempTooHigh != null)
            {
                TempTooHigh(this, CurrentTemp);
            }
            TempState = TemperatureState.TooHigh;
        }

        public event TempChangeHandler TempTooLow;
        protected void OnTempTooLow()
        {
            if (TempTooLow != null)
            {
                TempTooLow(this, CurrentTemp);
            }
            TempState = TemperatureState.TooLow;
        }

        public event TempChangeHandler TempNormal;
        protected void OnTempNormal()
        {
            if (TempNormal != null)
            {
                TempNormal(this, CurrentTemp);
            }
            TempState = TemperatureState.Normal;
        }
    }
}
