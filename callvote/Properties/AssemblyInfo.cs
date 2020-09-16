using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(Callvote.AssemblyInfo.Name)]
[assembly: AssemblyDescription(Callvote.AssemblyInfo.Description)]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Universal Gaming Alliance")]
[assembly: AssemblyProduct(Callvote.AssemblyInfo.Name)]
[assembly: AssemblyCopyright("Copyright © 2018 Nicholas Solin a.k.a. PatPeter")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0952a53a-a70e-4daa-86a6-6560f2f3607b")]

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
[assembly: AssemblyVersion(Callvote.AssemblyInfo.Version)]
[assembly: AssemblyFileVersion(Callvote.AssemblyInfo.Version)]

namespace Callvote
{
	static internal class AssemblyInfo
	{
		internal const string Author = "PatPeter";
		internal const string Name = "callvote";
		internal const string Description = "callvote command like in the Source engine. Vote to kick users, restart round, or make your own custom votes.";
		internal const string Id = "patpeter.callvote";
		internal const string ConfigPrefix = "callvote";
		internal const string LangFile = "callvote";
		internal const string Version = "2.1.0.24";
	}
}
