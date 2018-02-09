using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.RemoteManager.Core.Common
{
    public static class CoreConstants
    {
        public static string SampleImagesUrl = "https://tagit.blob.core.windows.net/sample-images/";

        public static ulong ImageSizeLimit = 4194304; //THIS NUMBER IS THE COMPUTER VISION UPPER LIMIT
        public static int ImageCountLimit = 100; //THIS NUMBER IS AN ARBITRARY LIMIT TO PREVENT MASSIVE IMAGE COUNTS
    }

    public static class UiConstants
    {

    }
}
