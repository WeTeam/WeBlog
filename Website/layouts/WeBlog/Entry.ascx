<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogEntry" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<div class="wb-entry">
    <sc:Image runat="server" ID="EntryImage" Field="Image" CssClass="wb-image" />
    <% if (ShowEntryTitle) { %>
        <h2>
            <% if (!string.IsNullOrEmpty(CurrentEntry["title"]) || Sitecore.Context.PageMode.IsPageEditor) { %>
            <sc:Text ID="txtTitle" Field="Title" runat="server" />
            <% } else { %>
            <%= CurrentEntry.Name %>
            <% } %>
        </h2>
    <% } %>
    <% if (ShowEntryMetadata) { %>
        <div class="wb-details">
            <% if(Sitecore.Context.PageMode.IsPageEditor) { %>
            <%=Translator.Format("ENTRY_DETAILS", CurrentEntry.Created.ToString(Sitecore.Modules.WeBlog.Settings.DateFormat), CurrentEntry.Author.Rendered) %>
            <% } else { %>
            <%=Translator.Format("ENTRY_DETAILS", CurrentEntry.Created.ToString(Sitecore.Modules.WeBlog.Settings.DateFormat), CurrentEntry.AuthorName) %>
            <%} %>
        </div>
    <% } %>
    <% if (ShowEntryIntroduction) { %>
    <sc:Placeholder runat="server" key="weblog-below-entry-title" />
    <sc:Text ID="txtIntroduction" Field="Introduction" runat="server" />    
    <% } %>
    <sc:Text ID="txtContent" Field="Content" runat="server" />
    <sc:Placeholder runat="server" key="weblog-below-entry" /> 
</div>