using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Sitecore.Modules.WeBlog.Import.Providers
{
    public class FileBasedProvider : IWpPostProvider
    {
        private readonly string _fileLocation;

        /// <summary>
        ///  Imports the specified file.
        /// </summary>
        /// <param name="fileLocation">The file location.</param>
        public FileBasedProvider(string fileLocation)
        {
            _fileLocation = fileLocation;
        }

        public List<WpPost> GetPosts(WpImportOptions options)
        {
            var nsm = new XmlNamespaceManager(new NameTable());
            nsm.AddNamespace("atom", "http://www.w3.org/2005/Atom");

            var parseContext = new XmlParserContext(null, nsm, null, XmlSpace.Default);
            using (var reader = XmlReader.Create(_fileLocation, null, parseContext))
            {
                var doc = XDocument.Load(reader);

                var posts = (from item in doc.Descendants("item")
                             select new WpPost(item, options)).ToList();

                return posts;
            }
        }
    }
}