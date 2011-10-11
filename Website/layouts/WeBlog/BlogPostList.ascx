<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogPostList.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogPostList" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Managers" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>

<asp:ListView ID="EntryList" runat="server" OnItemDataBound="EntryDataBound">
<LayoutTemplate>
    <ul class="wb-entry-list wb-panel">
        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
    </ul>
</LayoutTemplate>
<ItemTemplate>
    <li>
        <sc:Image runat="server" ID="EntryImage" Item="<%# (Container.DataItem as EntryItem) %>" Field="Thumbnail Image" CssClass="wb-image" />
        <div class="wb-entry-detail">
            <h2>
                <a href="<%#(Container.DataItem as EntryItem).Url%>"><%#(Container.DataItem as EntryItem).Title.Rendered%></a>
            </h2>
            <div class="wb-details"><%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("ENTRY_DETAILS", (Container.DataItem as EntryItem).Created, (Container.DataItem as EntryItem).CreatedBy.LocalName)%></div>
            
            <%# Eval("IntroductionText") %>
            
            <asp:HyperLink ID="BlogPostLink" runat="server" CssClass="wb-read-more" NavigateUrl='<%# Eval("Url") %>'><%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("READ_MORE")%></asp:HyperLink>
            <span class="wb-comment-count">
                <%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("COMMENTS")%> (<%#(Container.DataItem as EntryItem).CommentCount%>)
            </span>
        </div>
    </li>
</ItemTemplate>
<EmptyDataTemplate>
    <%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("NO_POSTS_FOUND")%>
</EmptyDataTemplate>
</asp:ListView>
<div class="wb-view-more-wrapper wb-panel">
    <a runat="server" id="ancViewMore" class="wb-view-more" href="#"><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("VIEW_MORE")%></a>
    <span class="wb-loading-animation" style="display:none;" >Loading...</span>
</div>
