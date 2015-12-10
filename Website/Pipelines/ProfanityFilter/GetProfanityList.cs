using System.Collections.Generic;
using System.IO;
using Sitecore.IO;

namespace Sitecore.Modules.WeBlog.Pipelines.ProfanityFilter
{
    public class GetProfanityList: IProfanityFilterProcessor
    {
        public void Process(ProfanityFilterArgs args)
        {
            args.WordList =  GetProfanityFileContent();
        }

        private IEnumerable<string> GetProfanityFileContent()
        {
            string filePath = Settings.ProfanityFilterFile;
            if (FileUtil.Exists(filePath))
            {
                return File.ReadAllLines(filePath, System.Text.Encoding.Default);
            }
            return new string[0];
        }
    }
}