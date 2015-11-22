<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogCategories.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogCategories" %>

<asp:Panel ID="PanelCategories" runat="server" CssClass="wb-categories wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("CATEGORIES")%></h3>
    <asp:ListView ID="ListViewCategories" runat="server" ItemType="Sitecore.Modules.WeBlog.Data.Items.CategoryItem">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="<%# Item.GetUrl() %>"><%# Item.DisplayTitle %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>