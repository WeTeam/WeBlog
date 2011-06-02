<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogCategories.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogCategories" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>

<asp:Panel ID="PanelCategories" runat="server">
    <h3><sc:Text ID="titleCategories" runat="server" Field="titleCategories" /></h3>
    <asp:ListView ID="ListViewCategories" runat="server">
    <LayoutTemplate>
        <ul id="blog-categories" class="sidebar">
            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <li>
            <asp:HyperLink ID="hyperlinkCategory" runat="server" NavigateUrl="<%# GetCategoryUrl(Container.DataItem as CategoryItem) %>">
                <sc:Text ID="txtCategorie" Field="Title" runat="server" DataSource="<%# (Container.DataItem as CategoryItem).ID %>" />
            </asp:HyperLink>
        </li>
    </ItemTemplate>
    </asp:ListView>
</asp:Panel>