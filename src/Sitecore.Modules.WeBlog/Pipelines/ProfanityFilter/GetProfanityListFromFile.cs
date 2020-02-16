using Sitecore.IO;
using Sitecore.Web;
using System.Collections.Generic;
using System.IO;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class GetProfanityListFromFile : IProfanityFilterProcessor
    {
        public string FilePath { get; set; }

        public void Process(ProfanityFilterArgs args)
        {
            if (args.WordList == null)
            {
                args.WordList = GetProfanityFileContent();
            }
        }

        private IEnumerable<string> GetProfanityFileContent()
        {
            var filePath = MapPath(FilePath);

            if (FileUtil.Exists(filePath))
            {
                return File.ReadAllLines(filePath, System.Text.Encoding.Default);
            }
            return new string[0];
        }

        private string MapPath(string path)
        {
            var server = WebUtil.GetServer();
            if (server != null)
                return server.MapPath(path);

            return path;
        }
    }
}