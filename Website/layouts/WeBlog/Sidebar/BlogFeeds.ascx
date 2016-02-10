<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="BlogFeeds.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogFeeds" %>

<asp:Panel ID="PanelFeeds" runat="server" CssClass="wb-feeds wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("SYNDICATION")%></h3>
    <asp:ListView ID="FeedList" runat="server" ItemType="Sitecore.Modules.WeBlog.Data.Items.RssFeedItem">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <asp:HyperLink ID="feedText" runat="server" NavigateUrl="<%#Item.Url%>" Text="<%#Item.Title.Text%>" CssClass="wb-feed-text" />
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>