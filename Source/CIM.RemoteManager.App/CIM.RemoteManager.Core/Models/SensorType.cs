using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Models
{
    public class SensorTypeResult
    {
        public string SensorImage { get; set; }
        public string SensorLabel { get; set; }
        public string SensorUnitType { get; set; }

        public SensorTypeResult(string sensorImage, string sensorLabel, string sensorUnitType)
        {
            SensorImage = sensorImage;
            SensorLabel = sensorLabel;
            SensorUnitType = sensorUnitType;
        }
    }

    public static class SensorType
    {
        public static SensorTypeResult GetSensorTypeResult(this string value)
        {
            switch (value)
            {
                case "2800":
                    return new SensorTypeResult("temperatureSensor.png", "CI10/CZ11 Temp.", "°C");

                case "7100":
                    return new SensorTypeResult("temperatureSensor.png", "CI13 Temp.", "°C");

                case "7000":
                    return new SensorTypeResult("temperatureSensor.png", "CI15 Temp.", "°C");

                case "7001":
                    return new SensorTypeResult("humiditySensor.png", "CI15H Humidity", "%RH");

                case "6900":
                    return new SensorTypeResult("temperatureSensor.png", "CI18 Temp.", "°C");

                case "6800":
                    return new SensorTypeResult("differentialPressureSensor.png", "CI22 Diff. Pres. ", "WC");

                case "6700":
                    return new SensorTypeResult("temperatureSensor.png", "CI19 Temp.", "°C");

                case "1200":
                    return new SensorTypeResult("temperatureSensor.png", "CI2X Value", "");

                case "1220":
                    return new SensorTypeResult("temperatureSensor.png", "CI20 Value", "");

                case "1224":
                    return new SensorTypeResult("temperatureSensor.png", "CI24 Value", "");

                case "1250":
                    return new SensorTypeResult("temperatureSensor.png", "CI50 Value", "");

                case "1270":
                    return new SensorTypeResult("temperatureSensor.png", "CI70 Value", "");

                case "9100":
                    return new SensorTypeResult("temperatureSensor.png", "CI91 State", "");

                case "8000":
                    return new SensorTypeResult("temperatureSensor.png", "CT13 Temp.", "°C");

                case "8100":
                    return new SensorTypeResult("temperatureSensor.png", "CT14 Temp.", "°C");

                case "8200":
                    return new SensorTypeResult("temperatureSensor.png", "CT15 Temp.", "°C");

                case "8201":
                    return new SensorTypeResult("humiditySensor.png", "CT15H Humidity", "%RH");

                case "8300":
                    return new SensorTypeResult("temperatureSensor.png", "CT16 Temp.", "°C");

                case "8301":
                    return new SensorTypeResult("temperatureSensor.png", "CT17 Temp.", "°C");

                case "8001":
                    return new SensorTypeResult("temperatureSensor.png", "CT18 Temp.", "°C");

                case "8002":
                    return new SensorTypeResult("temperatureSensor.png", "CT19 Temp.", "°C");

                case "8400":
                    return new SensorTypeResult("c02Sensor.png.png", "CT21 CO2", "%");

                case "8410":
                    return new SensorTypeResult("02Sensor.png.png", "CT29 O2", "%");

                case "8500":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.1\" CT22 Diff. Pres.", "Pa");

                case "8501":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.1\" CT22A Diff. Pres.", "Pa");

                case "8502":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.2\" CT22B Diff. Pres.", "Pa");

                case "8503":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.5\" CT22C Diff. Pres.", "Pa");

                case "8504":
                    return new SensorTypeResult("differentialPressureSensor.png", "11.0\" CT22D Diff. Pres.", "Pa");

                case "8505":
                    return new SensorTypeResult("differentialPressureSensor.png", "12.0\" CT22E Diff. Pres.", "Pa");

                case "8506":
                    return new SensorTypeResult("temperatureSensor.png", "CT23 Air Flow", "");

                case "8600":
                    return new SensorTypeResult("temperatureSensor.png", "CT27 Particles", "#");

                case "8700":
                    return new SensorTypeResult("temperatureSensor.png", "CT30 Value", "Vdc");

                case "8701":
                    return new SensorTypeResult("temperatureSensor.png", "CT31 Value", "Vdc");

                case "8702":
                    return new SensorTypeResult("temperatureSensor.png", "CT32 Value", "mA");

                case "8800":
                    return new SensorTypeResult("temperatureSensor.png", "CT50 Count", "sec");

                case "8801":
                    return new SensorTypeResult("temperatureSensor.png", "CT24 Count", "sec");

                case "8802":
                    return new SensorTypeResult("temperatureSensor.png", "CT25 Count", "sec");

                case "8803":
                    return new SensorTypeResult("temperatureSensor.png", "CT26 Count", "sec");

                case "8900":
                    return new SensorTypeResult("temperatureSensor.png", "CT4X Count", "");

                case "8A00":
                    return new SensorTypeResult("temperatureSensor.png", "CZ25 Temp.", "°C");

                case "8A01":
                    return new SensorTypeResult("humiditySensor.png", "CZ25H Humidity", "%RH");

                case "8B00":
                    return new SensorTypeResult("temperatureSensor.png", "CZ15 Temp.", "°C");

                case "8B01":
                    return new SensorTypeResult("humiditySensor.png", "CZ15H Humidity", "%RH");

                case "8C00":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.1\" CZ22 Diff. Pres.", "Pa");

                case "8C01":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.1\" CZ22A Diff. Pres.", "Pa");

                case "8C02":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.2\" CZ22B Diff. Pres.", "Pa");

                case "8C03":
                    return new SensorTypeResult("differentialPressureSensor.png", "10.5\" CZ22C Diff. Pres.", "Pa");

                case "8C04":
                    return new SensorTypeResult("differentialPressureSensor.png", "11.0\" CZ22D Diff. Pres.", "Pa");

                case "8C05":
                    return new SensorTypeResult("differentialPressureSensor.png", "12.0\" CZ22E Diff. Pres.", "Pa");
                default:
                    return new SensorTypeResult("temperatureSensor.png", "", "");
            }
        }

    }
}
