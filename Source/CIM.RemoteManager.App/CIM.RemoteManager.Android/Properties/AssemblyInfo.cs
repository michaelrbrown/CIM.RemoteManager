using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CIM.RemoteManager.Android")]
[assembly: AssemblyDescription("CIMTechniques Remote Manager app will search for Bluetooth LE remotes (DA-12-BTLE currently) and connect. Sensor data will be displayed in real-time for all sensors on the remote chain.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DotComCoders, Inc.")]
[assembly: AssemblyProduct("CIM.RemoteManager.Android")]
[assembly: AssemblyCopyright("Copyright DotComCoders, Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.3.0")]
[assembly: AssemblyVersion("1.3.0")]
[assembly: AssemblyFileVersion("1.3.0")]

// Add some common permissions, these can be removed if not needed
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
