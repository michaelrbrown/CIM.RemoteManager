namespace CIM.RemoteManager.Core.Models
{
    public static class SensorType
    {
        public static string LookupNameByValue(this string value)
        {
            switch (value)
            {
                case "2800":
                    return "DS18B20 Temperature";

                case "7100":
                    return "-200 to 0 RTD *";

                case "7000":
                    return "Temp/RH *";

                case "7001":
                    return "Humidity portion of Temp/RH *";

                case "6900":
                    return "0 to +200 RTD *";

                case "6800":
                    return "differential pressure *";

                case "6700":
                    return "RTD temperature";

                case "1200":
                    return "DS2406-based on/off (CI-20/24/50/70)";

                case "1220":
                    return "CI-20 agitator motion";

                case "1224":
                    return "CI-24 door ajar";

                case "1250":
                    return "CI-50 contact closure";

                case "1270":
                    return "CI-70 120VAC detector";

                case "9100":
                    return "Contact Closure *";

                case "8000":
                    return "Cryo Freezer (CT-13)";

                case "8100":
                    return "Room Temp Sensors (CT-14)";

                case "8200":
                    return "Room Temp/Humidity Sensors (CT-15)";

                case "8201":
                    return "Humidity Value (CT-15)";

                case "8300":
                    return "Thermistor Temp (CT-16)";

                case "8301":
                    return "Thermistor Temp (CT-17)";

                case "8001":
                    return "Oven Temp (CT-18)";

                case "8002":
                    return "Wide Range Temp (CT-19)";

                case "8400":
                    return "CO2(CT-21)";

                case "8410":
                    return "O2 (CT-29)";

                case "8500":
                    return "10.1\" Diff Pressure(CT-22A)";

                case "8501":
                    return "10.1\" Diff Pressure(CT-22A)";

                case "8502":
                    return "10.2\" Diff Pressure(CT-22B)";

                case "8502.1":
                    return "10.5\" Diff Pressure(CT-22C)";

                case "8504":
                    return "11.0\" Diff Pressure(CT-22D)";

                case "8505":
                    return "12.0\" Diff Pressure(CT-22E)";

                case "8506":
                    return "Air Flow(CT-21)";

                case "8600":
                    return "Particle Counter(CT-21)";

                case "8700":
                    return "0-5V Analog Input (CT-30)";

                case "8701":
                    return "0-10V Analog Input (CT-31)";

                case "8702":
                    return "4-20 ma Analog Input (CT-32)";

                case "8800":
                    return "Contact Closure (CT-50)";

                case "8801":
                    return "Magnetic Door Ajar (CT-24)";

                case "8802":
                    return "Bottled Gas Pressure Switch (CT-25)";

                case "8803":
                    return "LN2 Level Detector (CT-26)";

                case "8900":
                    return "Counter (CT-40/41)";

                case "8A00":
                    return "CZ-25 temp/RH";

                case "8A01":
                    return "CZ-25 Humidity value";

                case "8B00":
                    return "CZ-15 temp/RH";

                case "8B01":
                    return "CZ-15 Humidity value";

                case "8C00":
                    return "10.1\" Diff Pressure(CZ-22A)";

                case "8C01":
                    return "10.1\" Diff Pressure(CZ-22A)";

                case "8C02":
                    return "10.2\" Diff Pressure(CC-22B)";

                case "8C03":
                    return "10.5\" Diff Pressure(CZ-22C)";

                case "8C04":
                    return "11.0\" Diff Pressure(CZ-22D)";

                case "8C05":
                    return "12.0\" Diff Pressure(CZ-22E)";

                default:
                    return "";
            }
        }

    }
}
