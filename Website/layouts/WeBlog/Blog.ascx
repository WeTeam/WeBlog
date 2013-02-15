<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Blog.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.Blog" %>

<div class="wb">
    <div class="wb-header">
        <asp:HyperLink ID="HyperlinkBlog" runat="server"><sc:Text ID="fieldtextItem" Field="Title" runat="server" /></asp:HyperLink>
    </div>

    <div class="wb-wrapper">

        <div class="wb-leftcolumn">
            <sc:placeholder ID="phBlogMain" key="phBlogMain" runat="server" />
        </div>

        <div class="wb-rightcolumn">
            <sc:placeholder ID="phBlogSidebar" key="phBlogSidebar" runat="server" />
        </div>

        <div class="wb-footer">Powered by <a href="http://github.com/WeTeam/WeBlog">WeBlog</a></div>
    </div>
</div>
