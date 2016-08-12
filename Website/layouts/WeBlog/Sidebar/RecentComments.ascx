<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogRecentComments.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogRecentComments" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Extensions" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>

<asp:Panel ID="PanelRecentComments" runat="server" CssClass="wb-recent-comments wb-panel">
    <h3><%= Translator.Render("RECENT_COMMENTS") %></h3>
    <asp:ListView ID="ListViewRecentComments" runat="server" ItemType="Sitecore.Modules.WeBlog.Data.Items.CommentItem">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <div class="wb-details">
                    <span class="wb-comment-author"><%# Item.Name.Text %></span> <%= Translator.Render("ON") %>
                    <asp:HyperLink ID="hyperlinkComment" runat="server" NavigateUrl='<%# RecentCommentsCore.GetEntryUrlForComment(Item) %>'>
                         <%# RecentCommentsCore.GetEntryTitleForComment(Item) %>
                    </asp:HyperLink>
                    <%= Translator.Render("AT") %> <span class="wb-datetime">
                        <%#Translator.Format("COMMENT_DATE", Item.Created)%>
                    </span>
                </div>
                <div class="wb-comment-content">
                    <%# Item.Comment.HtmlEncode().MaxLength(150) %>
                </div>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>