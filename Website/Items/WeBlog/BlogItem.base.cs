using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.Blog
{
public partial class BlogItem : CustomItem
{

public static readonly string TemplateId = "{46663E05-A6B8-422A-8E13-36CD2B041278}";


#region Boilerplate CustomItem Code

public BlogItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator BlogItem(Item innerItem)
{
	return innerItem != null ? new BlogItem(innerItem) : null;
}

public static implicit operator Item(BlogItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomCheckboxField EnableRSS
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable RSS"]);
	}
}


public CustomTextField Title
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Title"]);
	}
}


public CustomTextField titleCategories
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["titleCategories"]);
	}
}


public CustomTextField Email
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Email"]);
	}
}


public CustomCheckboxField EnableComments
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable Comments"]);
	}
}


public CustomTextField titleRecentComments
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["titleRecentComments"]);
	}
}


public CustomCheckboxField ShowEmailWithinComments
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Show Email Within Comments"]);
	}
}


public CustomLookupField Theme
{
	get
	{
		return new CustomLookupField(InnerItem, InnerItem.Fields["Theme"]);
	}
}


public CustomTextField titleAdministration
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["titleAdministration"]);
	}
}


public CustomTextField DisplayItemCount
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["DisplayItemCount"]);
	}
}


public CustomCheckboxField EnableLiveWriter
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["EnableLiveWriter"]);
	}
}


public CustomTextField titleTagcloud
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["titleTagcloud"]);
	}
}


public CustomTextField DisplayCommentSidebarCount
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["DisplayCommentSidebarCount"]);
	}
}


public CustomCheckboxField EnableGravatar
{
	get
	{
		return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable Gravatar"]);
	}
}


public CustomTextField titleComments
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["titleComments"]);
	}
}


public CustomTextField GravatarSize
{
    get
    {
        return new CustomTextField(InnerItem, InnerItem.Fields["Gravatar Size"]);
    }
}


public CustomTextField MaximumGeneratedIntroductionCharacters
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Maximum Generated Introduction Characters"]);
	}
}


public CustomTextField titleAddYourComment
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["titleAddYourComment"]);
	}
}


public CustomMultiListField DefaultGravatarStyle
{
    get
    {
        return new CustomMultiListField(InnerItem, InnerItem.Fields["Default Gravatar Style"]);
    }
}


public CustomTextField MaximumEntryImageSize
{
    get
    {
        return new CustomTextField(InnerItem, InnerItem.Fields["Maximum Entry Image Size"]);
    }
}


public CustomMultiListField GravatarRating
{
    get
    {
        return new CustomMultiListField(InnerItem, InnerItem.Fields["Gravatar Rating"]);
    }
}


public CustomTextField MaximumThumbnailImageSize
{
    get
    {
        return new CustomTextField(InnerItem, InnerItem.Fields["Maximum Thumbnail Image Size"]);
    }
}

public CustomLookupField CustomDictionaryFolder
{
    get
    {
        return new CustomLookupField(InnerItem, InnerItem.Fields["Custom Dictionary Folder"]);
    }
}

#endregion //Field Instance Methods
}
}