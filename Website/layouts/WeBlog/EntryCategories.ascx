<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntryCategories.ascx.cs" Inherits="Sitecore.Modules.WeBlog.layouts.WeBlog.BlogEntryCategories" %>

<asp:Panel ID="PanelEntryCategories" runat="server" CssClass="wb-entry-categories wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("POSTED_IN")%></h3>
    <asp:ListView ID="ListViewCategories" runat="server">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <asp:HyperLink ID="hyperlinkCategory" runat="server" NavigateUrl='<%# GetItemUrl(Eval("InnerItem") as Sitecore.Data.Items.Item) %>'>
                    <sc:Text ID="txtCategory" Field="Title" runat="server" DataSource='<%# Eval("ID") %>' />
                </asp:HyperLink>
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>