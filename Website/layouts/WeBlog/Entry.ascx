<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogEntry" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Globalization" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<article>
    <% if (ShowEntryTitle) { %>
        <h1>
            <% if (!string.IsNullOrEmpty(CurrentEntry["title"]) || Sitecore.Context.PageMode.IsPageEditor) { %>
                <sc:Text ID="txtTitle" Field="Title" runat="server" />
            <% } else { %>
                <%= CurrentEntry.Name %>
            <% } %>
        </h1>
    <% } %>
    <div class="thumbnail">
        <sc:Image runat="server" ID="EntryImage" Field="Image" CssClass="wb-image" />
    </div>
    <% if (ShowEntryMetadata) { %>
        <div class="wb-details">
            <% if(Sitecore.Context.PageMode.IsPageEditor) { %>
                <%=Translator.Format("ENTRY_DETAILS", CurrentEntry.Created.ToString(Settings.DateFormat), CurrentEntry.Author.Rendered) %>
            <% } else { %>
                <%=Translator.Format("ENTRY_DETAILS", CurrentEntry.Created.ToString(Settings.DateFormat), CurrentEntry.AuthorName) %>
            <%} %>
        </div>
    <% } %>
    <sc:Placeholder runat="server" key="weblog-below-entry-title" />
    <% if (ShowEntryIntroduction) { %>
        <% if(DoesFieldRequireWrapping("Introduction")) { %>
            <p>
        <% } %>
        <sc:Text ID="txtIntroduction" Field="Introduction" runat="server" />
        <% if(DoesFieldRequireWrapping("Introduction")) { %>
            </p>
        <% } %>
    <% } %>
    <% if(DoesFieldRequireWrapping("Content")) { %>
        <p>
    <% } %>
    <sc:Text ID="txtContent" Field="Content" runat="server" />
    <% if(DoesFieldRequireWrapping("Content")) { %>
        </p>
    <% } %>
    <sc:Placeholder runat="server" key="weblog-below-entry" /> 
</article>