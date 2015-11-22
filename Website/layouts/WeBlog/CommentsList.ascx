<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogCommentsList.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogCommentsList" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Extensions" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>

<asp:Panel ID="CommentList" runat="server" CssClass="wb-entry-comments wb-panel">
    <h3><%=Translator.Render("COMMENTS") %></h3>
    <asp:ListView ID="ListViewComments" runat="server" ItemType="Sitecore.Modules.WeBlog.Data.Items.CommentItem">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <% if (CurrentBlog.EnableGravatar.Checked)
                    { %>
                <img src="<%# CommentsListCore.GetGravatarUrl(Item.Email.Text) %>" alt="<%#Item.Name.Text%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                <% } %>
                <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#Item.Website.Raw%>' CssClass="wb-comment-name">
                    <%#Translator.Format("COMMENT_NAME", Item.Name.Text) %>
                </asp:HyperLink>
                <% if (CurrentBlog.ShowEmailWithinComments.Checked)
                    { %>
                <span class="wb-comment-email">
                    <%#Translator.Format("COMMENT_EMAIL", Item.Email.Text)%>
                </span>
                <% } %>
                <div class="wb-datetime">
                    <%#Translator.Format("COMMENT_DATE", Item.Created)%>
                </div>
                <p>
                    <%#Item.Comment.HtmlEncode()%>
                </p>
            </li>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <li class="wb-comment-alternate">
                <% if (CurrentBlog.EnableGravatar.Checked)
                    { %>
                <img src="<%# CommentsListCore.GetGravatarUrl(Item.Email.Text) %>" alt="<%#Item.Name.Raw%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                <% } %>
                <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#Item.Website.Raw%>' CssClass="wb-comment-name">
                    <%#Translator.Format("COMMENT_NAME", Item.Name.Text) %>
                </asp:HyperLink>
                <% if (CurrentBlog.ShowEmailWithinComments.Checked)
                    { %>
                <span class="wb-comment-email">
                    <%#Translator.Format("COMMENT_EMAIL", Item.Email.Text)%>
                </span>
                <% } %>
                <div class="wb-datetime">
                    <%#Translator.Format("COMMENT_DATE", Item.Created)%>
                </div>
                <p>
                    <%#Item.Comment.HtmlEncode()%>
                </p>
            </li>
        </AlternatingItemTemplate>
    </asp:ListView>
</asp:Panel>