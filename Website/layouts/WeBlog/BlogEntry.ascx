<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogEntry" %>
<%@ Register TagPrefix="gl" Namespace="Sitecore.Modules.WeBlog.Globalization" Assembly="Sitecore.Modules.WeBlog" %>

<div class="wb-entry">
    <sc:Image runat="server" ID="EntryImage" Field="Image" CssClass="wb-image" />
    <h2><sc:Text ID="txtTitle" Field="Title" runat="server" /></h2>
    <div class="wb-details"><%=Sitecore.Modules.WeBlog.Globalization.Translator.Format("ENTRY_DETAILS", CurrentEntry.Created, CurrentEntry.CreatedBy.LocalName) %></div>
    <sc:Placeholder runat="server" key="phBlogBelowEntryTitle" />
    <p><sc:Text ID="txtIntroduction" Field="Introduction" runat="server" /></p>
    <p><sc:Text ID="txtContent" Field="Content" runat="server" /></p>
    <sc:Placeholder runat="server" key="phBlogBelowEntry" /> 
</div>