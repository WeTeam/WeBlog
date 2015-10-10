<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogCategories.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogCategories" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.WeBlog" %>

<asp:Panel ID="PanelCategories" runat="server" CssClass="wb-categories wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("CATEGORIES")%></h3>
    <asp:ListView ID="ListViewCategories" runat="server">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="<%# (Container.DataItem as CategoryItem).GetUrl() %>"><%# (Container.DataItem as CategoryItem).DisplayTitle %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>