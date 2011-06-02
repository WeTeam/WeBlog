<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogRecentComments.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogRecentComments" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>

<asp:Panel ID="PanelRecentComments" runat="server">
    <h3><sc:Text ID="titleRecentComments" runat="server" Field="titleRecentComments" /></h3>
    <asp:ListView ID="ListViewRecentComments" runat="server">
    <LayoutTemplate>
        <ul id="blog-comments" class="sidebar">
            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <li>
            <asp:HyperLink ID="hyperlinkComment" runat="server" NavigateUrl='<%# GetEntryUrlForComment(Container.DataItem as CommentItem) %>'>
                <%#(Container.DataItem as CommentItem).Name.Rendered%> wrote: <%#(Container.DataItem as CommentItem).Comment.Rendered%>
            </asp:HyperLink>
        </li>
    </ItemTemplate>
    </asp:ListView>
</asp:Panel>