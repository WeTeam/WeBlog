using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;

namespace Sitecore.Modules.Blog.Items.Blog
{
public partial class CommentItem : CustomItem
{

public static readonly string TemplateId = "{70949D4E-35D8-4581-A7A2-52928AA119D5}";


#region Boilerplate CustomItem Code

public CommentItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator CommentItem(Item innerItem)
{
	return innerItem != null ? new CommentItem(innerItem) : null;
}

public static implicit operator Item(CommentItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomTextField Name
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Name"]);
	}
}


public CustomTextField Email
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Email"]);
	}
}


public CustomTextField Comment
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Comment"]);
	}
}


public CustomTextField Website
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Website"]);
	}
}


public CustomTextField IPAddress
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["IP Address"]);
	}
}


#endregion //Field Instance Methods
}
}