using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;

namespace WorkshopAutomation
{
    public partial class Program
    {
        private TemperatureMonitor _tempMonitor;

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            // Setup the _tempMonitor.
            _tempMonitor = new TemperatureMonitor
                            {
                                TargetTempC = 12.0, 
                                TempTolerance = 2.0
                            };
            _tempMonitor.TempTooHigh += TempMonitorOnTempTooHigh;
            _tempMonitor.TempNormal += TempMonitorOnTempNormal;
            _tempMonitor.TempTooLow += TempMonitorOnTempTooLow;

            temperatureHumidity.MeasurementComplete +=
                (sender, temperature, humidity) =>
                {
                    Debug.Print("Temp = " + temperature);
                    _tempMonitor.CurrentTemp = temperature;
                };
            temperatureHumidity.StartContinuousMeasurements();
        }

        private void TempMonitorOnTempTooLow(object sender, double tempC)
        {
            Debug.Print("Temp is too low. [" + tempC + "]");
            relayX1.TurnOn();
        }

        private void TempMonitorOnTempNormal(object sender, double tempC)
        {
            Debug.Print("Temp is normal. [" + tempC + "]");
            // Leave the heater on a bit longer so we aren't bouncing off the bottom threashold.
            if (_tempMonitor.TempState == TemperatureState.TooLow)
            {
                var startTime = DateTime.UtcNow;
                while (DateTime.UtcNow.Ticks - startTime.Ticks < TimeSpan.TicksPerSecond * 60)
                {
#if (DEBUG)
                    Debug.Print("Waiting...");
#endif
                }
            }
            relayX1.TurnOff();
        }

        private void TempMonitorOnTempTooHigh(object sender, double tempC)
        {
            Debug.Print("Temp too high! [" + tempC + "]");
            relayX1.TurnOff();
        }
    }
}

