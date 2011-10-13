<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogPostListEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.layouts.WeBlog.BlogPostListEntry" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>
    <li>
        <sc:Image runat="server" ID="EntryImage" Item="<%# (((ListViewDataItem)Container).DataItem as EntryItem) %>" Field="Thumbnail Image" CssClass="wb-image" />
        <div class="wb-entry-detail">
            <h2>
                <a href="<%#(((ListViewDataItem)Container).DataItem as EntryItem).Url%>"><%#(((ListViewDataItem)Container).DataItem as EntryItem).Title.Rendered%></a>
            </h2>
            <div class="wb-details"><%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("ENTRY_DETAILS", (((ListViewDataItem)Container).DataItem as EntryItem).Created, (((ListViewDataItem)Container).DataItem as EntryItem).CreatedBy.LocalName)%></div>
            
            <%# Eval("IntroductionText") %>
            
            <asp:HyperLink ID="BlogPostLink" runat="server" CssClass="wb-read-more" NavigateUrl='<%# Eval("Url") %>'><%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("READ_MORE")%></asp:HyperLink>
            <span class="wb-comment-count">
                <%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("COMMENTS")%> (<%#(((ListViewDataItem)Container).DataItem as EntryItem).CommentCount%>)
            </span>
        </div>
    </li>