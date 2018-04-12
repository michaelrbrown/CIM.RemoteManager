using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Models
{
    public class SensorTypeResult
    {
        /// <summary>
        /// Gets or sets the sensor group.
        /// </summary>
        /// <value>
        /// The sensor group.
        /// </value>
        public string SensorGroup { get; set; }
        /// <summary>
        /// Gets or sets the sensor image.
        /// </summary>
        /// <value>
        /// The sensor image.
        /// </value>
        public string SensorImage { get; set; }
        /// <summary>
        /// Gets or sets the sensor label.
        /// </summary>
        /// <value>
        /// The sensor label.
        /// </value>
        public string SensorLabel { get; set; }
        /// <summary>
        /// Gets or sets the type of the sensor unit.
        /// </summary>
        /// <value>
        /// The type of the sensor unit.
        /// </value>
        public string SensorUnitType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorTypeResult"/> class.
        /// </summary>
        /// <param name="sensorGroup">The sensor group.</param>
        /// <param name="sensorImage">The sensor image.</param>
        /// <param name="sensorLabel">The sensor label.</param>
        /// <param name="sensorUnitType">Type of the sensor unit.</param>
        public SensorTypeResult(string sensorGroup, string sensorImage, string sensorLabel, string sensorUnitType)
        {
            SensorGroup = sensorGroup;
            SensorImage = sensorImage;
            SensorLabel = sensorLabel;
            SensorUnitType = sensorUnitType;
        }
    }

    /// <summary>
    /// Returns a strongly typed SensorTypeResult model containing key sensor information matched
    /// by a sensor type value. This is responsible for grouping sensors, setting the sensor image,
    /// setting the sensor label (type of sensor), and the sensor unit type.
    /// </summary>
    public static class SensorType
    {
        public static SensorTypeResult GetSensorTypeResult(this string value)
        {
            switch (value)
            {
                case "2800":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CI10/CZ11 Temp.", "°C");
                case "2810":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CV12 Dual Temp.", "°C");
                case "7100":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CI13 Temp.", "°C");
                case "7000":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CI15 Temp.", "°C");
                case "7001":
                    return new SensorTypeResult("Humidity", "SensorHumidity.png", "CI15H Humidity", "%RH");
                case "6900":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CI18 Temp.", "°C");
                case "6800":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "CI22 Diff. Pres. ", "WC");
                case "6700":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CI19 Temp.", "°C");
                case "1200":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CI2X Value", "");
                case "1220":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CI20 Value", "");
                case "1224":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CI24 Value", "");
                case "1250":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CI50 Value", "");
                case "1270":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CI70 Value", "");
                case "9100":
                    return new SensorTypeResult("State", "SensorDefault.png", "CI91 State", "");
                case "8000":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CT13 Temp.", "°C");
                case "8100":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CT14 Temp.", "°C");
                case "8200":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CT15 Temp.", "°C");
                case "8201":
                    return new SensorTypeResult("Humidity", "SensorHumidity.png", "CT15H Humidity", "%RH");
                case "8300":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CT16 Temp.", "°C");
                case "8301":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CT17 Temp.", "°C");
                case "8001":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CT18 Temp.", "°C");
                case "8002":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CT19 Temp.", "°C");
                case "8400":
                    return new SensorTypeResult("CO2", "SensorC02.png", "CT21 CO2", "%");
                case "8410":
                    return new SensorTypeResult("O2", "SensorOxygen.png", "CT29 O2", "%");
                case "8500":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.1\" CT22 Diff. Pres.", "Pa");
                case "8501":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.1\" CT22A Diff. Pres.", "Pa");
                case "8502":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.2\" CT22B Diff. Pres.", "Pa");
                case "8503":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.5\" CT22C Diff. Pres.", "Pa");
                case "8504":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "11.0\" CT22D Diff. Pres.", "Pa");
                case "8505":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "12.0\" CT22E Diff. Pres.", "Pa");
                case "8506":
                    return new SensorTypeResult("Air Flow", "SensorAirflow.png", "CT23 Air Flow", "");
                case "8600":
                    return new SensorTypeResult("Particles", "SensorParticleCount.png", "CT27 Particles", "#");
                case "8700":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CT30 Value", "Vdc");
                case "8701":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CT31 Value", "Vdc");
                case "8702":
                    return new SensorTypeResult("Value", "SensorDefault.png", "CT32 Value", "mA");
                case "8800":
                    return new SensorTypeResult("Count", "SensorDefault.png", "CT50 Count", "sec");
                case "8801":
                    return new SensorTypeResult("Count", "SensorDefault.png", "CT24 Count", "sec");
                case "8802":
                    return new SensorTypeResult("Count", "SensorDefault.png", "CT25 Count", "sec");
                case "8803":
                    return new SensorTypeResult("Count", "SensorDefault.png", "CT26 Count", "sec");
                case "8900":
                    return new SensorTypeResult("Count", "SensorDefault.png", "CT4X Count", "");
                case "8A00":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CZ25 Temp.", "°C");
                case "8A01":
                    return new SensorTypeResult("Humidity", "SensorHumidity.png", "CZ25H Humidity", "%RH");
                case "8B00":
                    return new SensorTypeResult("Temperature", "SensorTemperature.png", "CZ15 Temp.", "°C");
                case "8B01":
                    return new SensorTypeResult("Humidity", "SensorHumidity.png", "CZ15H Humidity", "%RH");
                case "8C00":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.1\" CZ22 Diff. Pres.", "Pa");
                case "8C01":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.1\" CZ22A Diff. Pres.", "Pa");
                case "8C02":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.2\" CZ22B Diff. Pres.", "Pa");
                case "8C03":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "10.5\" CZ22C Diff. Pres.", "Pa");
                case "8C04":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "11.0\" CZ22D Diff. Pres.", "Pa");
                case "8C05":
                    return new SensorTypeResult("Differential Pressure", "SensorDifferentialPressure.png", "12.0\" CZ22E Diff. Pres.", "Pa");
                default:
                    return new SensorTypeResult("Unknown", "SensorDefault.png", "", "");
            }
        }

    }
}
