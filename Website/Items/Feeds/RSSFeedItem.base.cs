using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.Feeds
{
public partial class RSSFeedItem : CustomItem
{

public static readonly string TemplateId = "{B960CBE4-381F-4A2B-9F44-A43C7A991A0B}";


#region Boilerplate CustomItem Code

public RSSFeedItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator RSSFeedItem(Item innerItem)
{
	return innerItem != null ? new RSSFeedItem(innerItem) : null;
}

public static implicit operator Item(RSSFeedItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomCheckboxField Cacheable
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Cacheable"]);
	}
}


public CustomImageField Image
{
	get
	{
		return new CustomImageField(InnerItem, InnerItem.Fields["Image"]);
	}
}


//Could not find Field Type for Source


public CustomTextField Type
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Type"]);
	}
}


public CustomTextField CacheDuration
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Cache Duration"]);
	}
}


public CustomTextField Copyright
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Copyright"]);
	}
}


public CustomTextField Title
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Title"]);
	}
}


public CustomTextField Description
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Description"]);
	}
}


public CustomTextField Managingeditor
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Managing editor"]);
	}
}


public CustomGeneralLinkField Link
{
	get
	{
		return new CustomGeneralLinkField(InnerItem, InnerItem.Fields["Link"]);
	}
}


#endregion //Field Instance Methods
}
}