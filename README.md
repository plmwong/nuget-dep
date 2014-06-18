# nuget-dep #

nuget-dep is a small command line utility that will replace the *<dependencies>* section of a .nupkg's .nuspec file with a given *<dependencies>* section in an xml file.

### Why? ###

* The "nuget.exe pack" command seems to have trouble with generating .nupkgs when a .nuspec file contains more than one targetFramework group. E.g.

```
...
<dependencies>
  <group targetFramework="net35">
    <dependency id="System.Threading.Tasks" version="[3.0.1,)" />
    <dependency id="log4net" version="[2.0.2,)" />
    <dependency id="Mindscape.Raygun4Net" version="[2.1.0,)" />
  </group>
  <group targetFramework="net40">
    <dependency id="log4net" version="[2.0.2,)" />
    <dependency id="Mindscape.Raygun4Net" version="[2.1.0,)" />
  </group>
</dependencies>
...
```

The above section would generate an 'Item has already been added' exception, due to dependencies appearing twice across the two groups, even though they are in separate groups and dependency groups aren't inherited.

* nuget-dep allows for replacing the *<dependencies>* section after the .nupkg file has already been processed/generated through nuget.exe. They can then be fixed up automatically as part of a build script, prior to pushing the package up to the nuget.org server.

### Usage ###

```
nuget-dep.exe <xml file> <nupkg file>
```

The xml file must take the form of '*<full filename of nuspec file>.dependencies.xml*'.

Example file (*log4net.Raygun.nuspec.dependencies.xml*):

```
<?xml version="1.0" encoding="utf-8" ?>
<dependencies xmlns="http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd">
  <group targetFramework="net35">
    <dependency id="System.Threading.Tasks" version="[3.0.1,)" />
    <dependency id="log4net" version="[2.0.2,)" />
    <dependency id="Mindscape.Raygun4Net" version="[2.1.0,)" />
  </group>
  <group targetFramework="net40">
    <dependency id="log4net" version="[2.0.2,)" />
    <dependency id="Mindscape.Raygun4Net" version="[2.1.0,)" />
  </group>
</dependencies>
```