<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogPostListEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogPostListEntry" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.WeBlog" %>
    <li class="wb-entry-list-entry">
        <sc:Image runat="server" ID="EntryImage" Item="<%# (((ListViewDataItem)Container).DataItem as EntryItem) %>" Field="Thumbnail Image" CssClass="wb-image" />
        <div class="wb-entry-detail">
            <h2>
                <a href="<%#(((ListViewDataItem)Container).DataItem as EntryItem).Url%>"><%#(((ListViewDataItem)Container).DataItem as EntryItem).DisplayTitle %></a>
            </h2>
            <div class="wb-details"><%#Sitecore.Modules.WeBlog.Globalization.Translator.Format("ENTRY_DETAILS", (((ListViewDataItem)Container).DataItem as EntryItem).EntryDate.DateTime, (((ListViewDataItem)Container).DataItem as EntryItem).AuthorName)%></div>
            
            <%# GetSummary(((ListViewDataItem)Container).DataItem as EntryItem)%>
            
            <asp:HyperLink ID="BlogPostLink" runat="server" CssClass="wb-read-more" NavigateUrl='<%# Eval("Url") %>'><%#Sitecore.Modules.WeBlog.Globalization.Translator.Text("READ_MORE")%></asp:HyperLink>
            <span class="wb-comment-count">
                <%#Sitecore.Modules.WeBlog.Globalization.Translator.Render("COMMENTS")%> (<%#(((ListViewDataItem)Container).DataItem as EntryItem).CommentCount%>)
            </span>
        </div>
    </li>