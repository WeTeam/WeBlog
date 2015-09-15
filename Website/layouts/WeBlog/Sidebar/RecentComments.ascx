<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogRecentComments.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogRecentComments" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.WeBlog" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Extensions" %>

<asp:Panel ID="PanelRecentComments" runat="server" CssClass="wb-recent-comments wb-panel">
    <h3><%= Sitecore.Modules.WeBlog.Globalization.Translator.Render("RECENT_COMMENTS") %></h3>
    <asp:ListView ID="ListViewRecentComments" runat="server">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <span class="wb-comment-author"><%# (Container.DataItem as CommentItem).Name.Text %></span> <%= Sitecore.Modules.WeBlog.Globalization.Translator.Render("ON") %>
                <asp:HyperLink ID="hyperlinkComment" runat="server" NavigateUrl='<%# GetEntryUrlForComment(Container.DataItem as CommentItem) %>'>
                     <%# GetEntryTitleForComment(Container.DataItem as CommentItem) %>
                </asp:HyperLink>
                <%= Sitecore.Modules.WeBlog.Globalization.Translator.Render("AT") %> <span class="wb-datetime">
                    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("COMMENT_DATE", (Container.DataItem as CommentItem).Created)%>
                </span>
                <span class="wb-comment-content"><%# (Container.DataItem as CommentItem).Comment.HtmlEncode().MaxLength(150) %></span>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>
