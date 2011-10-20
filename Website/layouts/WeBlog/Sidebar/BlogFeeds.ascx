<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="BlogFeeds.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogFeeds" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Feeds" %>

<asp:Panel ID="PanelFeeds" runat="server" CssClass="wb-feeds wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("SYNDICATION")%></h3>
    <asp:ListView ID="FeedList" runat="server">
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <asp:HyperLink ID="feedImage" runat="server" NavigateUrl="<%#(Container.DataItem as RSSFeedItem).Url%>" ImageUrl="/sitecore modules/WeBlog/Images/feed-icon-14x14.png" CssClass="wb-feed-image" />
                <asp:HyperLink ID="feedText" runat="server" NavigateUrl="<%#(Container.DataItem as RSSFeedItem).Url%>" Text="<%#(Container.DataItem as RSSFeedItem).Title.Text%>" CssClass="wb-feed-text" />
            </li>
        </ItemTemplate>
    </asp:ListView>
</asp:Panel>