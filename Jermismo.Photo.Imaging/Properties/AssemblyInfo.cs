using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Jermismo.Photo.Imaging")]
[assembly: AssemblyDescription("Classes for using and manipulating images.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Jermismo.com")]
[assembly: AssemblyProduct("Jermismo.Photo.Imaging")]
[assembly: AssemblyCopyright("Copyright © 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("87ec97b8-3027-46ae-9605-e2cad9df8aa9")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.2.0")]
[assembly: AssemblyFileVersion("1.2.0")]
[assembly: NeutralResourcesLanguageAttribute("en-US")]

// Enable SIMD instructions on Vector and Matrix classes
[assembly: CodeGeneration(CodeGenerationFlags.EnableFPIntrinsicsUsingSIMD)]