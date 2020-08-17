<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsList.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.BlogCommentsList" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Extensions" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>

<asp:Panel ID="CommentList" runat="server" CssClass="wb-entry-comments wb-panel">
    <h3><%=Translator.Render("COMMENTS") %></h3>
    <asp:ListView ID="ListViewComments" runat="server" ItemType="Sitecore.Modules.WeBlog.Model.CommentContent">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>            
            <li id="comment-<%#Item.Uri.ItemID.ToShortID()%>">
                <a href="#comment-<%#Item.Uri.ItemID.ToShortID()%>">#</a>
                <% if (CurrentBlog.EnableGravatar.Checked)
                    { %>
                <img src="<%# CommentsListCore.GetGravatarUrl(Item.AuthorEmail) %>" alt="<%#Item.AuthorName%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                <% } %>
                <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#Item.AuthorWebsite%>' CssClass="wb-comment-author">
                    <%#Translator.Format("COMMENT_NAME", Item.AuthorName) %>
                </asp:HyperLink>
                <% if (CurrentBlog.ShowEmailWithinComments.Checked)
                    { %>
                <span class="wb-comment-email">
                    <%#Translator.Format("COMMENT_EMAIL", Item.AuthorEmail)%>
                </span>
                <% } %>
                <div class="wb-datetime">
                    <%#Translator.Format("COMMENT_DATE", Item.Created)%>
                </div>
                <p>
                    <%#Item.Text.HtmlEncode()%>
                </p>
            </li>
        </ItemTemplate>
        <AlternatingItemTemplate>            
            <li class="wb-comment-alternate" id="comment-<%#Item.Uri.ItemID.ToShortID()%>">
                <a href="#comment-<%#Item.Uri.ItemID.ToShortID()%>">#</a>
                <% if (CurrentBlog.EnableGravatar.Checked)
                    { %>
                <img src="<%# CommentsListCore.GetGravatarUrl(Item.AuthorEmail) %>" alt="<%#Item.AuthorName%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                <% } %>
                <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#Item.AuthorWebsite%>' CssClass="wb-comment-author">
                    <%#Translator.Format("COMMENT_NAME", Item.AuthorName) %>
                </asp:HyperLink>
                <% if (CurrentBlog.ShowEmailWithinComments.Checked)
                    { %>
                <span class="wb-comment-email">
                    <%#Translator.Format("COMMENT_EMAIL", Item.AuthorEmail)%>
                </span>
                <% } %>
                <div class="wb-datetime">
                    <%#Translator.Format("COMMENT_DATE", Item.Created)%>
                </div>
                <p>
                    <%#Item.Text.HtmlEncode()%>
                </p>
            </li>
        </AlternatingItemTemplate>
    </asp:ListView>
</asp:Panel>