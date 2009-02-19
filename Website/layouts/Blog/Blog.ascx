<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.Blog, Sitecore.Modules.Eviblog" %>

<div class="eviblog-header">
    <asp:HyperLink ID="HyperlinkBlog" runat="server"><sc:Text ID="fieldtextItem" Field="Title" runat="server" /></asp:HyperLink>
</div>

<div class="eviblog">

    <div class="eviblog-leftcolumn">
        <sc:placeholder ID="phBlogMain" key="phBlogMain" runat="server" />
    </div>

    <div class="eviblog-rightcolumn">
        <sc:placeholder ID="phBlogSidebar" key="phBlogSidebar" runat="server" />
    </div>

    <div class="eviblog-footer">Powered by EviBlog</div>
</div>

