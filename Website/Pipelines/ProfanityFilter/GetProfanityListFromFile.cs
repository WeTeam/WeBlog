using System.Collections.Generic;
using System.IO;
using Sitecore.IO;

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
            string filePath = ResovePathTokens(FilePath);
            if (FileUtil.Exists(filePath))
            {
                return File.ReadAllLines(filePath, System.Text.Encoding.Default);
            }
            return new string[0];
        }

        private string ResovePathTokens(string filePath)
        {
            return filePath.Replace("$(dataFolder)", Configuration.Settings.DataFolder);
        }
    }
}