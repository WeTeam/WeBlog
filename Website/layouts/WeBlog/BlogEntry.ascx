<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogEntry" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>

<div class="entry">
    <sc:Image runat="server" ID="EntryImage" Field="Image" CssClass="entry-image" />
    <h2><sc:Text ID="txtTitle" Field="Title" runat="server" /></h2>
    <div class="details">Posted on: <sc:Date runat="server" ID="PostedDate" Field="__created" Format="dddd, MMMM d, yyyy" /> by <%=CurrentEntry.CreatedBy.LocalName%></div>
    <p><sc:Text ID="txtIntroduction" Field="Introduction" runat="server" /></p>
    <p><sc:Text ID="txtContent" Field="Content" runat="server" /></p>
        
    <asp:ListView ID="ListViewCategories" runat="server">
    <LayoutTemplate>
        <div>
            <ul class="entry-categories">
                <li>Posted in&nbsp;</li>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <li>
            <asp:HyperLink ID="hyperlinkCategory" runat="server" NavigateUrl='<%# GetItemUrl(Eval("InnerItem") as Sitecore.Data.Items.Item) %>'>
                <sc:Text ID="txtCategorie" Field="Title" runat="server" DataSource='<%# Eval("ID") %>' />
            </asp:HyperLink>
        </li>
    </ItemTemplate>
    </asp:ListView>
        
    <div id="entry-tags">
        <span>Tags: </span>
        <asp:LoginView ID="LoginViewTags" runat="server">
            <AnonymousTemplate>
                <asp:Repeater runat="server" ID="TagList">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="TagLink" NavigateUrl='<%# GetTagUrl(Container.DataItem as string) %>'>
                            <%# Container.DataItem %>
                        </asp:HyperLink>
                    </ItemTemplate>
                </asp:Repeater>
            </AnonymousTemplate>
            <LoggedInTemplate>
                <sc:Text ID="txtTags" Field="Tags" runat="server" />
            </LoggedInTemplate>
        </asp:LoginView>
    </div>

    <sc:Placeholder runat="server" key="phBlogBelowEntry" />
        
    <asp:Panel ID="CommentsPanel" runat="server"  CssClass="entry-comments">
        <h3><sc:Text ID="txtAddYourComment" Field="titleAddYourComment" runat="server" /></h3>
        <asp:validationsummary id="ValidationSummaryComments" runat="server" headertext="The following fields are not filled in:" forecolor="Red" EnableClientScript="true" CssClass="error"  />

        <asp:Panel runat="server" ID="MessagePanel" CssClass="successtext">
            <asp:Literal runat="server" ID="Message" />
        </asp:Panel>

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
        <asp:Button ID="buttonSaveComment" runat="server" Text="Post" onclick="buttonSaveComment_Click" />
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
                    <% if (CurrentBlog.EnableGravatar.Checked)
                        { %>
                    <img src="<%# GetGravatarUrl((Container.DataItem as CommentItem).Email.Rendered) %>" alt="<%#(Container.DataItem as CommentItem).Name.Rendered%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                    <% } %>
                    <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#(Container.DataItem as CommentItem).Website.Raw%>'>
                        <%#(Container.DataItem as CommentItem).Name.Rendered%>
                    </asp:HyperLink>
                    <% if (!CurrentBlog.ShowEmailWithinComments.Checked)
                        { %>
                    <span class="comment-email">
                        <%#(Container.DataItem as CommentItem).Email.Rendered%>
                    </span>
                    <% } %>
                    <div class="datetime">
                        <%#(Container.DataItem as CommentItem).Created%>
                    </div>
                    <p>
                        <%#(Container.DataItem as CommentItem).Comment.Rendered%>
                    </p>
                </li>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <li class="comment-alternate">
                    <% if (CurrentBlog.EnableGravatar.Checked)
                        { %>
                    <img src="<%# GetGravatarUrl((Container.DataItem as CommentItem).Email.Rendered) %>" alt="<%#(Container.DataItem as CommentItem).Name.Rendered%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                    <% } %>
                    <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#(Container.DataItem as CommentItem).Website.Raw%>'>
                        <%#(Container.DataItem as CommentItem).Name.Rendered%>
                    </asp:HyperLink>
                    <% if (!CurrentBlog.ShowEmailWithinComments.Checked)
                        { %>
                    <span class="comment-email">
                        <%#(Container.DataItem as CommentItem).Email.Rendered%>
                    </span>
                    <% } %>
                    <div class="datetime">
                        <%#(Container.DataItem as CommentItem).Created%>
                    </div>
                    <p>
                        <%#(Container.DataItem as CommentItem).Comment.Rendered%>
                    </p>
                </li>
            </AlternatingItemTemplate>
            </asp:ListView>
        </asp:Panel>   
    </div>