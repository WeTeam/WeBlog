<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogEntry, Sitecore.Modules.Eviblog" %>

<div class="entry">
        <h2><sc:Text ID="txtTitle" Field="Title" runat="server" /></h2>
        <div class="details">Posted on: <asp:PlaceHolder ID="PostedDate" runat="server"></asp:PlaceHolder>
        by <asp:PlaceHolder ID="PostedBy" runat="server"></asp:PlaceHolder></div>
        <p><sc:Text ID="txtIntroduction" Field="Introduction" runat="server" /></p>
        <p><sc:Text ID="txtContent" Field="Content" runat="server" /></p>
        
        <asp:ListView ID="ListViewCategories" runat="server">
        <LayoutTemplate>
            <ul class="entry-categories">
                <li>Posted in&nbsp;</li>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <asp:HyperLink ID="hyperlinkCategory" runat="server"><sc:Text ID="txtCategorie" Field="Title" runat="server" /></asp:HyperLink>
            </li>
        </ItemTemplate>
        <ItemSeparatorTemplate><li>,&nbsp;</li></ItemSeparatorTemplate>
        </asp:ListView>
        
        <p>&nbsp;</p>
        
        <div id="entry-tags">
            <asp:LoginView ID="LoginViewTags" runat="server">
                <AnonymousTemplate>
                    <asp:PlaceHolder ID="TagsPlaceholder" runat="server"></asp:PlaceHolder>
                </AnonymousTemplate>
                <LoggedInTemplate>
                    Tags: <sc:Text ID="txtTags" Field="Tags" runat="server" />
                </LoggedInTemplate>
            </asp:LoginView>
        </div>
        
        <asp:Panel ID="CommentsPanel" runat="server"  CssClass="entry-comments">
            <h3><sc:Text ID="txtAddYourComment" Field="titleAddYourComment" runat="server" /></h3>
            <asp:validationsummary id="ValidationSummaryComments" runat="server" headertext="The following fields are not filled in:" forecolor="Red" EnableClientScript="true" CssClass="error"  />
            <asp:PlaceHolder ID="Message" runat="server"></asp:PlaceHolder>
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
            <asp:TextBox ID="txtCommentText" runat="server" TextMode="MultiLine" Rows="10" Columns="60"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvCommentText" runat="server" ErrorMessage="Comment" Text="*" ControlToValidate="txtCommentText" SetFocusOnError="true" EnableClientScript="true"></asp:RequiredFieldValidator>
            <sc:PlaceHolder runat="server" key="phBlogCommentForm" />
            <asp:Button ID="buttonSaveComment" runat="server" Text="Post Your Comment" onclick="buttonSaveComment_Click" CssClass="commentSubmit" />
        </asp:Panel>
        
        <asp:Panel ID="CommentList" runat="server">
            <asp:ListView ID="ListViewComments" runat="server">
                <EmptyDataTemplate>
                    No comments yet.
                </EmptyDataTemplate>
                <LayoutTemplate>
                    <div id="entry-comment">
                        <h3><sc:Text ID="titleComments" Field="titleComments" runat="server" /></h3>
                        <ul>
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                        </ul>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <li>
                        <asp:HyperLink ID="hyperlinkUsername" runat="server"></asp:HyperLink> <span class="comment-email"><sc:Text ID="txtCommentEmail" Field="Email" runat="server" /></span><div class="datetime"><asp:Literal ID="LiteralDate" runat="server"></asp:Literal></div>
                        <p>
                            <asp:Literal runat="server" ID="commentText" />
                        </p>
                    </li>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <li class="comment-alternate">
                        <asp:HyperLink ID="hyperlinkUsername" runat="server"></asp:HyperLink> <span class="comment-email"><sc:Text ID="txtCommentEmail" Field="Email" runat="server" /></span><div class="datetime"><asp:Literal ID="LiteralDate" runat="server"></asp:Literal></div>
                        <p>
                            <asp:Literal runat="server" ID="commentText" />
                        </p>
                    </li>
                </AlternatingItemTemplate>
                </asp:ListView>
            </asp:Panel>   
    </div>