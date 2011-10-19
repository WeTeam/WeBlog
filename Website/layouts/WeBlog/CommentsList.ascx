<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogCommentsList.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogCommentsList" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.WeBlog" %>
<%@ Register TagPrefix="gl" Namespace="Sitecore.Modules.WeBlog.Globalization" Assembly="Sitecore.Modules.WeBlog" %>

<asp:Panel ID="CommentList" runat="server" CssClass="wb-entry-comments wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("COMMENTS") %></h3>
    <asp:ListView ID="ListViewComments" runat="server">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <% if (CurrentBlog.EnableGravatar.Checked)
                    { %>
                <img src="<%# GetGravatarUrl((Container.DataItem as CommentItem).Email.Rendered) %>" alt="<%#(Container.DataItem as CommentItem).Name.Rendered%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                <% } %>
                <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#(Container.DataItem as CommentItem).Website.Raw%>' CssClass="wb-comment-name">
                    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("COMMENT_NAME", (Container.DataItem as CommentItem).Name.Text) %>
                </asp:HyperLink>
                <% if (CurrentBlog.ShowEmailWithinComments.Checked)
                    { %>
                <span class="wb-comment-email">
                    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("COMMENT_EMAIL", (Container.DataItem as CommentItem).Email.Text)%>
                </span>
                <% } %>
                <div class="wb-datetime">
                    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("COMMENT_DATE", (Container.DataItem as CommentItem).Created)%>
                </div>
                <p>
                    <%#(Container.DataItem as CommentItem).Comment.Rendered%>
                </p>
            </li>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <li class="wb-comment-alternate">
                <% if (CurrentBlog.EnableGravatar.Checked)
                    { %>
                <img src="<%# GetGravatarUrl((Container.DataItem as CommentItem).Email.Rendered) %>" alt="<%#(Container.DataItem as CommentItem).Name.Rendered%>'s gravatar" width="<%= CurrentBlog.GravatarSizeNumeric %>" height="<%= CurrentBlog.GravatarSizeNumeric %>" />
                <% } %>
                <asp:HyperLink ID="hyperlinkUsername" runat="server" NavigateUrl='<%#(Container.DataItem as CommentItem).Website.Raw%>' CssClass="wb-comment-name">
                    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("COMMENT_NAME", (Container.DataItem as CommentItem).Name.Text) %>
                </asp:HyperLink>
                <% if (CurrentBlog.ShowEmailWithinComments.Checked)
                    { %>
                <span class="wb-comment-email">
                    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("COMMENT_EMAIL", (Container.DataItem as CommentItem).Email.Text)%>
                </span>
                <% } %>
                <div class="wb-datetime">
                    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("COMMENT_DATE", (Container.DataItem as CommentItem).Created)%>
                </div>
                <p>
                    <%#(Container.DataItem as CommentItem).Comment.Rendered%>
                </p>
            </li>
        </AlternatingItemTemplate>
    </asp:ListView>
</asp:Panel>  