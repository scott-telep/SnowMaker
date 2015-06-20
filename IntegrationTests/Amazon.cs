using System;
using System.IO;
using NUnit.Framework;
using SnowMaker;

namespace IntegrationTests.cs
{
    [TestFixture]
    public class Amazon : Scenarios<Amazon.TestScope>
    {
        protected override TestScope BuildTestScope()
        {
            return new TestScope("snowmaker-dev");
        }

        protected override IOptimisticDataStore BuildStore(TestScope scope)
        {
            return new AmazonDataStore(scope.Bucket);
        }

        public class TestScope : ITestScope
        {
            public TestScope(string bucket)
            {
                var ticks = DateTime.UtcNow.Ticks;
                IdScopeName = string.Format("snowmakertest{0}", ticks);
                Bucket = bucket;
                DirectoryPath = Path.Combine(Path.GetTempPath(), IdScopeName);
                Directory.CreateDirectory(DirectoryPath);
            }

            public string IdScopeName { get; private set; }
            public string DirectoryPath { get; private set; }
            public string Bucket { get; private set; }

            public string ReadCurrentPersistedValue()
            {
                var filePath = Path.Combine(DirectoryPath, string.Format("{0}.txt", IdScopeName));
                return System.IO.File.ReadAllText(filePath);
            }

            public void Dispose()
            {
                if (Directory.Exists(DirectoryPath))
                    Directory.Delete(DirectoryPath, true);
            }
        }
    }
}
