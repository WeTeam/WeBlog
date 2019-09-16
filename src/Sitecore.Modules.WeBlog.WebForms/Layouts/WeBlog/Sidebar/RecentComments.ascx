<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentComments.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar.BlogRecentComments" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Extensions" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>
<%@ Import Namespace="Sitecore.Links" %>

<asp:Panel ID="PanelRecentComments" runat="server" CssClass="wb-recent-comments wb-panel">
    <h3><%= Translator.Render("RECENT_COMMENTS") %></h3>
    <asp:ListView ID="ListViewRecentComments" runat="server" ItemType="Sitecore.Modules.WeBlog.Model.EntryComment">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <div class="wb-details">
                    <span class="wb-comment-author"><%# Item.Comment.AuthorName %></span> <%= Translator.Render("ON") %>
                    <asp:HyperLink ID="hyperlinkComment" runat="server" NavigateUrl='<%# LinkManager.GetItemUrl(Item.Entry) %>'>
                         <%# Item.Entry.Title.Text %>
                    </asp:HyperLink>
                    <%= Translator.Render("AT") %> <span class="wb-datetime">
                        <%#Translator.Format("COMMENT_DATE", Item.Comment.Created)%>
                    </span>
                </div>
                <div class="wb-comment-content">
                    <%# Item.Comment.Text.HtmlEncode().MaxLength(150) %>
                </div>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>