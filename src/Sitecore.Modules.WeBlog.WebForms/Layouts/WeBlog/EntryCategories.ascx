<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntryCategories.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.BlogEntryCategories" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Extensions" %>

<asp:Panel ID="PanelEntryCategories" runat="server" CssClass="wb-entry-categories">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("POSTED_IN")%></h3>
    <asp:ListView ID="ListViewCategories" runat="server" ItemType="Sitecore.Modules.WeBlog.Data.Items.CategoryItem">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <asp:HyperLink ID="hyperlinkCategory" runat="server" NavigateUrl='<%# Item.InnerItem.GetUrl() %>'>
                    <sc:Text ID="txtCategory" Field="Title" runat="server" DataSource='<%# Item.ID %>' />
                </asp:HyperLink>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>