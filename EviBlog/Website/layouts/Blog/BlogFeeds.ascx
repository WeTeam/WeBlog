<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="BlogFeeds.ascx.cs" Inherits="Sitecore.Modules.Blog.Layouts.BlogFeeds" %>
<%@ Import Namespace="Sitecore.Modules.Blog.Items.Feeds" %>

<asp:ListView ID="FeedList" runat="server">
    <LayoutTemplate>
        <h3>Syndication</h3>
        <ul id="blog-feeds" class="sidebar">
            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <li>
            <asp:HyperLink ID="feedImage" runat="server" NavigateUrl="<%#(Container.DataItem as RSSFeedItem).Url%>" ImageUrl="/sitecore modules/Blog/Images/feed-icon-14x14.png" CssClass="feedImage" />
            <asp:HyperLink ID="feedText" runat="server" NavigateUrl="<%#(Container.DataItem as RSSFeedItem).Url%>" Text="<%#(Container.DataItem as RSSFeedItem).Title.Text%>" CssClass="feedText" />
        </li>
    </ItemTemplate>
</asp:ListView>