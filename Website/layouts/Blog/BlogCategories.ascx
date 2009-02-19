<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogCategories, Sitecore.Modules.Eviblog" %>

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
            <asp:HyperLink ID="hyperlinkCategory" runat="server"><sc:Text ID="txtCategorie" Field="Title" runat="server" /></asp:HyperLink>
        </li>
    </ItemTemplate>
    </asp:ListView>
</asp:Panel>