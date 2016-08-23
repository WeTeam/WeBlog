<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PostListEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.BlogPostListEntry" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Data.Items" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>
<section>
    <header>
            <h2>
                <a href="<%#(((ListViewDataItem)Container).DataItem as EntryItem).Url%>"><%#(((ListViewDataItem)Container).DataItem as EntryItem).DisplayTitle %></a>
            </h2>
        <div class="thumbnail">
            <sc:Image runat="server" ID="EntryImage" Item="<%# (((ListViewDataItem)Container).DataItem as EntryItem) %>" Field="Thumbnail Image" CssClass="wb-image" />
        </div>
        <div class="wb-details">
            <%#Translator.Format("ENTRY_DETAILS", (((ListViewDataItem)Container).DataItem as EntryItem).Created.ToString(Sitecore.Modules.WeBlog.Settings.DateFormat), (((ListViewDataItem)Container).DataItem as EntryItem).AuthorName)%>
        </div>
    </header>
    <div class="description">
        <div class="summary">
            <%# GetSummary(((ListViewDataItem)Container).DataItem as EntryItem)%>
        </div>
        <span class="wb-comment-count" runat="server" Visible="<%# ShowComments(((ListViewDataItem)Container).DataItem as EntryItem) %>">
                <%#Translator.Render("COMMENTS")%> (<%#(((ListViewDataItem)Container).DataItem as EntryItem).CommentCount%>)
            </span>
        <asp:HyperLink ID="BlogPostLink" runat="server" CssClass="wb-read-more" NavigateUrl='<%# Eval("Url") %>'><%#Translator.Text("READ_MORE")%></asp:HyperLink>
        </div>
</section>