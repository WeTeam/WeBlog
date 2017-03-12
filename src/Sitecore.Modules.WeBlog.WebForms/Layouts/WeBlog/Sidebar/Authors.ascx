<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Authors.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar.BlogAuthors" %>

<asp:Panel ID="PanelAuthors" runat="server" CssClass="wb-authors wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("AUTHORS")%></h3>
    <asp:ListView ID="ListViewAuthors" runat="server" ItemType="System.String">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <a href="<%# GetAuthorUrl(Item) %>"><%# Item %></a>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>