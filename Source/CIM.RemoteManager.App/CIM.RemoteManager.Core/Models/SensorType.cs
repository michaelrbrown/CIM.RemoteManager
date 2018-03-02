namespace CIM.RemoteManager.Core.Models
{
    public static class SensorType
    {
        public static string LookupNameByValue(this string value)
        {
            switch (value)
            {
                case "2800":
                    return "CI10/CZ11 Temp.";

                case "7100":
                    return "CI13 Temp.";

                case "7000":
                    return "CI15 Temp.";

                case "7001":
                    return "CI15H Humidity";

                case "6900":
                    return "CI18 Temp.";

                case "6800":
                    return "CI22 Diff. Pres. ";

                case "6700":
                    return "CI19 Temp.";

                case "1200":
                    return "CI2X Value";

                case "1220":
                    return "CI20 Value";

                case "1224":
                    return "CI24 Value";

                case "1250":
                    return "CI50 Value";

                case "1270":
                    return "CI70 Value";

                case "9100":
                    return "CI91 State";

                case "8000":
                    return "CT13 Temp.";

                case "8100":
                    return "CT14 Temp.";

                case "8200":
                    return "CT15 Temp.";

                case "8201":
                    return "CT15H Humidity";

                case "8300":
                    return "CT16 Temp.";

                case "8301":
                    return "CT17 Temp.";

                case "8001":
                    return "CT18 Temp.";

                case "8002":
                    return "CT19 Temp.";

                case "8400":
                    return "CT21 CO2";

                case "8410":
                    return "CT29 O2";

                case "8500":
                    return "10.1\" CT22 Diff. Pres.";

                case "8501":
                    return "10.1\" CT22A Diff. Pres.";

                case "8502":
                    return "10.2\" CT22B Diff. Pres.";

                case "8503":
                    return "10.5\" CT22C Diff. Pres.";

                case "8504":
                    return "11.0\" CT22D Diff. Pres.";

                case "8505":
                    return "12.0\" CT22E Diff. Pres.";

                case "8506":
                    return "CT23 Air Flow";

                case "8600":
                    return "CT27 Particles";

                case "8700":
                    return "CT30 Value";

                case "8701":
                    return "CT31 Value";

                case "8702":
                    return "CT32 Value";

                case "8800":
                    return "CT50 Count";

                case "8801":
                    return "CT24 Count";

                case "8802":
                    return "CT25 Count";

                case "8803":
                    return "CT26 Count";

                case "8900":
                    return "CT4X Count";

                case "8A00":
                    return "CZ25 Temp.";

                case "8A01":
                    return "CZ25H Humidity";

                case "8B00":
                    return "CZ15 Temp.";

                case "8B01":
                    return "CZ15H Humidity";

                case "8C00":
                    return "10.1\" CZ22 Diff. Pres.";

                case "8C01":
                    return "10.1\" CZ22A Diff. Pres.";

                case "8C02":
                    return "10.2\" CZ22B Diff. Pres.";

                case "8C03":
                    return "10.5\" CZ22C Diff. Pres.";

                case "8C04":
                    return "11.0\" CZ22D Diff. Pres.";

                case "8C05":
                    return "12.0\" CZ22E Diff. Pres.";
                default:
                    return "";
            }
        }

    }
}
