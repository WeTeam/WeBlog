<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogCommentsList.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogCommentsList" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>

<asp:Panel ID="CommentList" runat="server">
<asp:ListView ID="ListViewComments" runat="server">
    <EmptyDataTemplate>
        <%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("NO_COMMENTS")%>
    </EmptyDataTemplate>
    <LayoutTemplate>
        <div id="entry-comment">
            <h3><%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("COMMENTS")%></h3>
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