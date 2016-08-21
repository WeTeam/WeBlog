<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InterestingEntries.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar.BlogInterestingEntries" %>

<asp:Panel ID="PanelInteresingEntries" runat="server" CssClass="wb-interesting-entries wb-panel">
    <h3><%= Sitecore.Modules.WeBlog.Globalization.Translator.Render("POPULAR_POSTS") %></h3>
    <asp:Repeater runat="server" ID="ItemList" ItemType="Sitecore.Modules.WeBlog.Data.Items.EntryItem">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <a href="<%# Item.Url %>"><%# Item.Title.Rendered %></a>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
</asp:Panel>
