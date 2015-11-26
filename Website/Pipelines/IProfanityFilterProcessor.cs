namespace Sitecore.Modules.WeBlog.Pipelines
{
    public interface IProfanityFilterProcessor
    {
        void Process(ProfanityFilterArgs args);
    }
}