using Sitecore.Modules.WeBlog.Components;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class RsdIncludesController : LinkIncludeController
    {
        protected IRsdInclude RsdLink;

        public RsdIncludesController() : this(new RsdLink()) { }

        public RsdIncludesController(IRsdInclude rl = null) : base(rl)
        {
            RsdLink = rl ?? new RsdLink();
        }
    }
}