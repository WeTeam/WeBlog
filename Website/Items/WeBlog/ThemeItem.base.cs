using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
public partial class ThemeItem : CustomItem
{

public static readonly string TemplateId = "{1BF19354-D79F-4F32-80F0-5A644811C137}";


#region Boilerplate CustomItem Code

public ThemeItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator ThemeItem(Item innerItem)
{
	return innerItem != null ? new ThemeItem(innerItem) : null;
}

public static implicit operator Item(ThemeItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomTextField FileLocation
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["File Location"]);
	}
}


#endregion //Field Instance Methods
}
}