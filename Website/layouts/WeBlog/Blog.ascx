<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Blog.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.Blog" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<div class="wb">
    <header id="top">
        <asp:HyperLink ID="HyperlinkBlog" runat="server">
            <sc:Text ID="FieldTextItem" Field="Title" runat="server" />
        </asp:HyperLink>
    </header>
    <div class="wb-wrapper">
        <main>
            <sc:placeholder ID="WeBlogMain" key="weblog-main" runat="server" />
        </main>
        <aside>
            <sc:placeholder ID="WeBlogSidebar" key="weblog-sidebar" runat="server" />
        </aside>
    </div>
    <footer id="bottom">
        <%-- Feel free to remove the following line from your implementation --%>
        <span class="powered-by">
            Powered by <a href="http://github.com/WeTeam/WeBlog">WeBlog</a>
        </span>
        <%= ThemeItem.Credits %>
    </footer>
</div>
