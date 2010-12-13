<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogPostList, Sitecore.Modules.Eviblog" %>

<asp:ListView ID="ListView1" runat="server">
<LayoutTemplate>
    <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
</LayoutTemplate>
<ItemTemplate>
    <div class="entry">
        <h2><sc:Text ID="txtTitle" Field="Title" runat="server" /></h2>
        <div class="details">Posted on: <asp:PlaceHolder ID="PostedDate" runat="server"></asp:PlaceHolder></div>
        
        <asp:Literal runat="server" ID="txtIntroduction" />
        
        <br /><br />
        
        <asp:HyperLink ID="BlogPostLink" runat="server" CssClass="readmore">Read more...</asp:HyperLink> <asp:PlaceHolder ID="CommentsPlaceholder" runat="server"></asp:PlaceHolder>
    </div>
</ItemTemplate>
<EmptyDataTemplate>
    no posts found!
</EmptyDataTemplate>
</asp:ListView>



