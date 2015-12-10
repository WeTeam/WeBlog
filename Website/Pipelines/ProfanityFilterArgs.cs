using System.Collections.Generic;
using Sitecore.Pipelines;

namespace Sitecore.Modules.WeBlog.Pipelines
{
    public class ProfanityFilterArgs : PipelineArgs
    {
        public string Input { get; set; }
        public IEnumerable<string> WordList { get; set; }
        public bool Valid { get; set; }

        public ProfanityFilterArgs()
        {
            Valid = true;
        }
    }
}