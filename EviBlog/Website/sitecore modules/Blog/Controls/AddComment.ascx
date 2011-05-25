<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddComment.ascx.cs" Inherits="Sitecore.Modules.Blog.Controls.AddComment" %>

<div id="addcomment">
    <h3><sc:Text ID="txtAddYourComment" Field="titleAddYourComment" runat="server" /></h3>
    
    <asp:validationsummary id="ValidationSummaryComments" runat="server" headertext="The following fields are not filled in:" forecolor="Red" EnableClientScript="true" CssClass="error"  />
    
    <asp:PlaceHolder ID="CaptchaErrorText" runat="server"></asp:PlaceHolder>
    <asp:Label ID="lblCommentName" runat="server" Text="Name" AssociatedControlID="txtCommentName" />
    <asp:TextBox ID="txtCommentName" runat="server" CssClass="textbox" Width="220"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvCommentName" runat="server" Text="*" ErrorMessage="Username" ControlToValidate="txtCommentName" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
    <br />
    <asp:Label ID="lblCommentEmail" runat="server" Text="Email" AssociatedControlID="txtCommentEmail" />

    <asp:TextBox ID="txtCommentEmail" runat="server" CssClass="textbox" Width="220"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvCommentEmail" runat="server" ErrorMessage="Email" Text="*" ControlToValidate="txtCommentEmail" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
    <br />
    <asp:Label ID="lblCommentWebsite" runat="server" Text="Website" AssociatedControlID="txtCommentWebsite" />
    <asp:TextBox ID="txtCommentWebsite" runat="server" CssClass="textbox" Text="http://" Width="220"></asp:TextBox>
    <br />            
    <asp:Label ID="lblCommentText" runat="server" Text="Comment" AssociatedControlID="txtCommentText" />
    <asp:RequiredFieldValidator ID="rfvCommentText" runat="server" ErrorMessage="Comment" Text="*" ControlToValidate="txtCommentText" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
    <asp:TextBox ID="txtCommentText" runat="server" TextMode="MultiLine" Rows="10" Columns="60"></asp:TextBox>
    <br />
    <asp:Image Height="30" Width="80" ID="CaptchaImage" runat="server" /><br />
    <asp:Label ID="lblCaptcha" runat="server" Text="Fill in the code shown above" AssociatedControlID="CaptchaText" />
    <asp:TextBox ID="CaptchaText" runat="server" CssClass="captcha"></asp:TextBox>

    <p>&nbsp;</p>
    <asp:Button ID="buttonSaveComment" runat="server" Text="Post" onclick="buttonSaveComment_Click" CssClass="savecomment" />
    <p>&nbsp;</p>
</div>