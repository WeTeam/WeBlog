<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogEntry" %>
<%@ Register TagPrefix="gl" Namespace="Sitecore.Modules.WeBlog.Globalization" Assembly="Sitecore.Modules.WeBlog" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<div class="wb-entry">
    <sc:Image runat="server" ID="EntryImage" Field="Image" CssClass="wb-image" />
    <% if (ShowEntryTitle) { %>
        <h2><sc:Text ID="txtTitle" Field="Title" runat="server" /></h2>
    <% } %>
    <% if (ShowEntryMetadata) { %>
        <div class="wb-details"><%=Sitecore.Modules.WeBlog.Globalization.Translator.Format("ENTRY_DETAILS", CurrentEntry.Created, CurrentEntry.AuthorName)%></div>
    <% } %>
    <% if (ShowEntryIntroduction) { %>
    <sc:Placeholder runat="server" key="weblog-below-entry-title" />
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
</div>