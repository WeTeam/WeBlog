<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogRecentComments, Sitecore.Modules.Eviblog" %>

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
            <asp:HyperLink ID="hyperlinkComment" runat="server"></asp:HyperLink>
        </li>
    </ItemTemplate>
    </asp:ListView>
</asp:Panel>