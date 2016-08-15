<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmitComment.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog.BlogSubmitComment" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>

<asp:PlaceHolder ID="CommentScroll" runat="server" Visible="false" EnableViewState="false">
    <script type="text/javascript">
        jQuery(function() {
            var offset = jQuery('.wb-entry-add-comment').offset();
            window.scrollTo(offset.left, offset.top);
        });
    </script>
</asp:PlaceHolder>

<asp:Panel ID="CommentsPanel" runat="server"  CssClass="wb-entry-add-comment wb-panel">
    <h3><%=Translator.Render("ADD_COMMENT")%></h3>
    <asp:validationsummary id="ValidationSummaryComments" runat="server" headertext="The following fields are not filled in:" forecolor="Red" EnableClientScript="true" CssClass="wb-error"  />

    <asp:Panel runat="server" ID="MessagePanel" CssClass="wb-successtext" Visible="false">
        <p>
            <asp:Literal runat="server" ID="Message" />
        </p>
    </asp:Panel>
    
    <p>
        <asp:Label ID="lblCommentName" runat="server" AssociatedControlID="txtCommentName"><%=Translator.Render("NAME")%></asp:Label>
        <asp:TextBox ID="txtCommentName" runat="server" CssClass="wb-textbox"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvCommentName" runat="server" Text="*" ErrorMessage="Username" ControlToValidate="txtCommentName" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
    </p>
    <p>
        <asp:Label ID="lblCommentEmail" runat="server" AssociatedControlID="txtCommentEmail"><%=Translator.Render("EMAIL")%></asp:Label>   
        <asp:TextBox ID="txtCommentEmail" runat="server" CssClass="wb-textbox"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvCommentEmail" runat="server" ErrorMessage="Email" Text="*" ControlToValidate="txtCommentEmail" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
    </p>
    <p>
        <asp:Label ID="lblCommentWebsite" runat="server" Text="Website" AssociatedControlID="txtCommentWebsite"><%=Translator.Render("WEBSITE")%></asp:Label>
        <asp:TextBox ID="txtCommentWebsite" runat="server" CssClass="wb-textbox"></asp:TextBox>
    </p>
    <p>
        <asp:Label ID="lblCommentText" runat="server" AssociatedControlID="txtCommentText"><%=Translator.Render("COMMENT")%></asp:Label>  
        <asp:TextBox ID="txtCommentText" runat="server" TextMode="MultiLine" Rows="10" Columns="60"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvCommentText" runat="server" ErrorMessage="Comment" Text="*" ControlToValidate="txtCommentText" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
    </p>
    <p class="wb-comment-form-wrapper">
        <sc:PlaceHolder runat="server" key="weblog-comment-form" />
    </p>
    <p>
        <asp:Button ID="buttonSaveComment" runat="server" Text="Post" onclick="buttonSaveComment_Click" CssClass="wb-submit" />
    </p>
</asp:Panel>

<script type="text/javascript">
    //lame workaround for .NET client side validation jumping to the top of the page, and also to hide any visible success message
    if (typeof (ValidationSummaryOnSubmit) != 'undefined') {
        var ValidationSummaryOnSubmitOrig = ValidationSummaryOnSubmit;
        var ValidationSummaryOnSubmit = function () {
            var scrollToOrig = window.scrollTo;
            window.scrollTo = function () { };
            ValidationSummaryOnSubmitOrig();
            window.scrollTo = scrollToOrig;
            jQuery('.wb-successtext').hide();
        }
    }
</script>