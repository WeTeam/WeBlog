<%@ Page Language="C#" AutoEventWireup="true" Inherits="Sitecore.Modules.Eviblog.Commands.UI.NewBlog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>EviBlog Configuration</title>
    <link href="/sitecore modules/EviBlog/Admin/EviBlogAdmin.css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">
    function onClose() 
    {
        window.returnValue = form1.hidCreatedId.value;
        window.close();
    }
    function onCancel() 
    {
        if (confirm("Are you sure you want to cancel?")) 
        {
            window.close();
        }
        window.event.cancelBubble = true; 
        return false;
    } 
    </script>
    <base target="_self" />
</head>
<body>
    <form id="form1" runat="server">
    
    <h1>Create new blog</h1>
    
    <p>You can setup your blog within a few seconds.</p>
    
    <asp:validationsummary id="ValidationSummaryBlog" runat="server" headertext="The following fields are not filled in:" forecolor="Red" EnableClientScript="true" CssClass="error"  />
    
    <fieldset id="general" title="General Settings">
        <legend>General Settings (required)</legend>
        <asp:Label ID="lblTitle" runat="server" Text="Blog title" AssociatedControlID="tbTitle"></asp:Label><asp:TextBox ID="tbTitle" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvBlogTitle" runat="server" Text="*" ErrorMessage="Title" ControlToValidate="tbTitle" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="^[\w\*\$][\w\s\-\$]*(\(\d{1,}\)){0,1}$" runat="server" ErrorMessage="Title is invalid" ControlToValidate="tbTitle" EnableClientScript="true"></asp:RegularExpressionValidator>
        <asp:Label ID="lbEmail" runat="server" Text="Your email" AssociatedControlID="tbEmail"></asp:Label><asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvBlogEmail" runat="server" Text="*" ErrorMessage="Email" ControlToValidate="tbEmail" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator><br />
        <asp:Label ID="lbItemCount" runat="server" Text="Number of item for homepage" AssociatedControlID="tbItemCount"></asp:Label><asp:TextBox ID="tbItemCount" runat="server" MaxLength="2" Width="20"></asp:TextBox><br />
        <asp:RequiredFieldValidator ID="rfvBlogItemCount" runat="server" Text="*" ErrorMessage="Number of item for homepage" ControlToValidate="tbItemCount" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator><br />
        <asp:Label ID="lbCommentsSidebarCount" runat="server" Text="Number of comments shown in the sidebar" AssociatedControlID="tbCommentsSidebarCount"></asp:Label><asp:TextBox ID="tbCommentsSidebarCount" runat="server" MaxLength="2" Width="20"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvBlogCommentsCount" runat="server" Text="*" ErrorMessage="Number of comments shown in the sidebar" ControlToValidate="tbCommentsSidebarCount" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
    </fieldset>
    
    <fieldset>
        <legend>Options</legend>
        <asp:Label ID="lbEnableRss" runat="server" Text="Enable RSS" AssociatedControlID="chkEnableRSS" /><asp:CheckBox ID="chkEnableRSS" runat="server" CssClass="checkbox" />
        <asp:Label ID="lbEnableComments" runat="server" Text="Enable Comments" AssociatedControlID="chkEnableComments"/><asp:CheckBox ID="chkEnableComments" runat="server" CssClass="checkbox" />
        <asp:Label ID="lbShowEmailInComments" runat="server" Text="Show email in comments" AssociatedControlID="chkShowEmailInComments" /><asp:CheckBox ID="chkShowEmailInComments" runat="server" CssClass="checkbox" />
        <asp:Label ID="lbEnableLiveWriter" runat="server" Text="Enable Windows Live Writer integration" AssociatedControlID="chkEnableLiveWriter" /><asp:CheckBox ID="chkEnableLiveWriter" runat="server" CssClass="checkbox" />
    </fieldset>
    
    <p>&nbsp;</p>
    
    <asp:button runat="server" id="btnCancel" text="Cancel" onclientclick="onCancel();" />
    <asp:button runat="server" id="btnCreate" text="Create" onclick="btnCreate_Click" /> 
    
    <input type="hidden" runat="server" id="hidCreatedId" /><br />
    </form>
</body>
</html>
