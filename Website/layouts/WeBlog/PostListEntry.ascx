<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogPostListEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogPostListEntry" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Data.Items" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>
<li class="wb-entry-list-entry">
        <sc:Image runat="server" ID="EntryImage" Item="<%# (((ListViewDataItem)Container).DataItem as EntryItem) %>" Field="Thumbnail Image" CssClass="wb-image" />
        <div class="wb-entry-detail">
            <h2>
                <a href="<%#(((ListViewDataItem)Container).DataItem as EntryItem).Url%>"><%#(((ListViewDataItem)Container).DataItem as EntryItem).DisplayTitle %></a>
            </h2>
            <div class="wb-details"><%#Translator.Format("ENTRY_DETAILS", (((ListViewDataItem)Container).DataItem as EntryItem).Created.ToString(Sitecore.Modules.WeBlog.Settings.DateFormat), (((ListViewDataItem)Container).DataItem as EntryItem).AuthorName)%></div>
            
            <%# GetSummary(((ListViewDataItem)Container).DataItem as EntryItem)%>
            
            <asp:HyperLink ID="BlogPostLink" runat="server" CssClass="wb-read-more" NavigateUrl='<%# Eval("Url") %>'><%#Translator.Text("READ_MORE")%></asp:HyperLink>
            <span class="wb-comment-count" runat="server" Visible="<%# ShowComments(((ListViewDataItem)Container).DataItem as EntryItem) %>">
                <%#Translator.Render("COMMENTS")%> (<%#(((ListViewDataItem)Container).DataItem as EntryItem).CommentCount%>)
            </span>
        </div>
    </li>