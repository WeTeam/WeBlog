<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogPostList, Sitecore.Modules.Eviblog" %>

<asp:ListView ID="ListView1" runat="server">
<LayoutTemplate>
    <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
</LayoutTemplate>
<ItemTemplate>
    <div class="entry">
        <h2><asp:HyperLink ID="lnkBlogPostTitle" runat="server" CssClass="postTitle" /></h2>
        <div class="details">Posted on: <asp:PlaceHolder ID="PostedDate" runat="server"></asp:PlaceHolder>
        by <asp:PlaceHolder ID="PostedBy" runat="server"></asp:PlaceHolder></div>
        
        <asp:Literal runat="server" ID="txtIntroduction" />
        
        <div class="readmore">
            <asp:HyperLink ID="BlogPostLink" runat="server">Read more...</asp:HyperLink> <asp:PlaceHolder ID="CommentsPlaceholder" runat="server"></asp:PlaceHolder>
        </div>
    </div>
</ItemTemplate>
<EmptyDataTemplate>
    no posts found!
</EmptyDataTemplate>
</asp:ListView>
<div class="viewMoreWrapper">
    <a runat="server" id="ancViewMore" class="viewMore" href="#">View More</a>
    <img src="/sitecore modules/EviBlog/Images/ajax-loader.gif" class="loadingAnimation" alt="Loading..." />
</div>

