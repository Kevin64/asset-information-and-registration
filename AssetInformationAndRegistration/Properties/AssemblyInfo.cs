using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Asset Information and Registration")]
[assembly: AssemblyDescription("Collects IT asset hardware details and registers on a APCS database")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Asset Information and Registration")]
[assembly: AssemblyCopyright("Copyright © 2023 Kevin Vinícius Teixeira Costa")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a7afe7a2-f74a-400e-9dfa-02bc6a8e4f9b")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("4.0.0.2305")]
[assembly: AssemblyFileVersion("4.0.0.2305")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif