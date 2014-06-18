using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;

namespace nuget_dep
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: nuget-dep <dependency xml file> <nuget package>");
                Environment.Exit(1);
            }

            var dependencyXmlFile = args[0];
            var dependencyXmlFileName = Path.GetFileName(dependencyXmlFile);

            var nuspecFileName = dependencyXmlFileName.Substring(0, dependencyXmlFileName.LastIndexOf(".dependencies.xml", StringComparison.Ordinal));
            var nugetPackage = args[1];

            Console.WriteLine("Applying dependencies listed in '{0}' to '{1}'", dependencyXmlFile, nugetPackage);

            using (var dependencyFileStream = File.Open(dependencyXmlFile, FileMode.Open))
            using (var dependencyStreamReader = new StreamReader(dependencyFileStream))
            {
                var dependencyXml = XDocument.Load(dependencyStreamReader);
                var dependenciesElement = dependencyXml.Root;

                using (var nugetFile = new ZipFile(nugetPackage))
                {
                    var nuspecFile = nugetFile.GetEntry(nuspecFileName);

                    using (var modifiedNuspecStream = new MemoryStream())
                    using (var modifiedNuspecWriter = new StreamWriter(modifiedNuspecStream))
                    {
                        nugetFile.BeginUpdate();

                        using (var nuspecInputStream = nugetFile.GetInputStream(nuspecFile))
                        using (var nuspecStreamReader = new StreamReader(nuspecInputStream))
                        {
                            XNamespace nuspecNamespace = "http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd";
                            var nuspecXml = XDocument.Load(nuspecStreamReader);

                            var nuspecDependencies = nuspecXml.Descendants(nuspecNamespace + "dependencies").Single();
                            nuspecDependencies.ReplaceWith(dependenciesElement);

                            nuspecXml.Save(modifiedNuspecWriter);
                        }

                        var memoryStaticDataSource = new StreamStaticDataSource(modifiedNuspecStream);

                        nugetFile.Add(memoryStaticDataSource, nuspecFileName);

                        nugetFile.CommitUpdate();

                        Console.WriteLine("Done.");
                    }
                }
            }
        }
    }
}
