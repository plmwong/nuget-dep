using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace nuget_dep
{
    public class StreamStaticDataSource : IStaticDataSource
    {
        private readonly Stream _stream;

        public StreamStaticDataSource(Stream stream)
        {
            _stream = stream;
            _stream.Position = 0;
        }

        public Stream GetSource() { return _stream; }
    }
}