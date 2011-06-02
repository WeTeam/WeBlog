<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogPostList.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogPostList" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Managers" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>

<asp:ListView ID="EntryList" runat="server" OnItemDataBound="EntryDataBound">
<LayoutTemplate>
    <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
</LayoutTemplate>
<ItemTemplate>
    <div class="entry">
        <sc:Image runat="server" ID="EntryImage" Item="<%# (Container.DataItem as EntryItem) %>" Field="Thumbnail Image" CssClass="entry-list-image" />
        <div class="entry-detail">
            <h2>
                <a href="<%#(Container.DataItem as EntryItem).Url%>"><%#(Container.DataItem as EntryItem).Title.Rendered%></a>
            </h2>
            <div class="details">Posted on: <%#(Container.DataItem as EntryItem).Created.ToString("dddd, MMMM d, yyyy")%> by <%#(Container.DataItem as EntryItem).CreatedBy.LocalName%></div>
        
            <%#(Container.DataItem as EntryItem).Introduction.Rendered%>
        
            <asp:HyperLink ID="BlogPostLink" runat="server" CssClass="readmore" NavigateUrl='<%# Eval("Url") %>'>Read more...</asp:HyperLink>
        
            <asp:PlaceHolder ID="CommentsPlaceholder" runat="server">
                Comments (<%#(Container.DataItem as EntryItem).CommentCount%>)
            </asp:PlaceHolder>
        </div>
    </div>
</ItemTemplate>
<EmptyDataTemplate>
    no posts found!
</EmptyDataTemplate>
</asp:ListView>
<div class="viewMoreWrapper">
    <a runat="server" id="ancViewMore" class="viewMore" href="#">View More</a>
    <img src="/sitecore modules/Blog/Images/ajax-loader.gif" class="loadingAnimation" alt="Loading..." />
</div>
