using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
public partial class EntryItem : CustomItem
{

public static readonly string TemplateId = "{5FA92FF4-4AC2-48E2-92EB-E1E4914677B0}";


#region Boilerplate CustomItem Code

public EntryItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator EntryItem(Item innerItem)
{
	return innerItem != null ? new EntryItem(innerItem) : null;
}

public static implicit operator Item(EntryItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomMultiListField Category
{
	get
	{
		return new CustomMultiListField(InnerItem, InnerItem.Fields["Category"]);
	}
}


public CustomCheckboxField DisableComments
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Disable Comments"]);
	}
}


public CustomTextField Title
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Title"]);
	}
}


public CustomTextField Introduction
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Introduction"]);
	}
}


public CustomTextField Tags
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Tags"]);
	}
}


public CustomTextField Content
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Content"]);
	}
}

public CustomTextField Author
{
    get
    {
        return new CustomTextField(InnerItem, InnerItem.Fields["Author"]);
    }
}

public CustomDateField EntryDate
{
    get
    {
        return new CustomDateField(InnerItem, InnerItem.Fields["Entry Date"]);
    }
}

public CustomImageField Image
{
    get
    {
        return new CustomImageField(InnerItem, InnerItem.Fields["Image"]);
    }
}


public CustomImageField ThumbnailImage
{
    get
    {
        return new CustomImageField(InnerItem, InnerItem.Fields["Thumbnail Image"]);
    }
}


#endregion //Field Instance Methods
}
}