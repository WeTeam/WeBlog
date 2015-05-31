using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Test
{
  public static class TestUtil
  {
    public static Item CreateContentFromFile(string filename, Item parent, bool changeIds = true)
    {
      var xml = File.ReadAllText(HttpContext.Current.Server.MapPath(filename));
      if (string.IsNullOrEmpty(xml))
        return null;

      return parent.PasteItem(xml, changeIds, PasteMode.Merge);
    }

    public static bool IsGermanRegistered(Database database)
    {
      return (from l in database.Languages
              where l.Name == "de"
              select l).Any();
    }

    public static Item RegisterGermanLanaguage(Database database)
    {
      using (new SecurityDisabler())
      {
        var languageRoot = database.GetItem(ItemIDs.LanguageRoot);
        return TestUtil.CreateContentFromFile("test data\\German Language.xml", languageRoot);
      }
    }
  }
}