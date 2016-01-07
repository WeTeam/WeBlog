<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogPostList.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogPostList" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>

<section class="wb-entry-list">
    <asp:ListView ID="EntryList" runat="server" OnItemDataBound="EntryDataBound">
        <LayoutTemplate>
            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
        </LayoutTemplate>
        <ItemTemplate>
            <%-- ItemTemplate loaded dynamically --%>
        </ItemTemplate>
        <EmptyDataTemplate>
            <%#Translator.Render("NO_POSTS_FOUND")%>
        </EmptyDataTemplate>
    </asp:ListView>
    <section class="wb-view-more-wrapper">
        <a runat="server" id="ancViewMore" class="wb-view-more" href="#"><%=Translator.Render("VIEW_MORE")%></a>
        <span class="wb-loading-animation" style="display: none;">Loading...</span>
    </section>
</div>
