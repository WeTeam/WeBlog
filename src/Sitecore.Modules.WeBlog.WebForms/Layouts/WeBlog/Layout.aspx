<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Layout.aspx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.BlogLayout" %>
<%@ Register TagPrefix="wb" Namespace="Sitecore.Modules.WeBlog.WebForms.WebControls" Assembly="Sitecore.Modules.WeBlog.WebForms" %>

<!doctype html>
<html>
    <head runat="server">
        <meta charset="utf-8">
        <meta http-equiv="x-ua-compatible" content="ie=edge">
        <title><%= GetItemTitle() %></title>
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <wb:Syndication runat="server" Cacheable="true" VaryByData="true" />
        <wb:RsdIncludes runat="server" Cacheable="true" VaryByData="true" />
        <sc:Sublayout runat="server" RenderingID="{1B45CBD5-C8B4-44E9-827D-90EA75DF84D1}" Path="/layouts/WeBlog/ThemeStylesheets.ascx" />
        <sc:VisitorIdentification runat='server'/> 
    </head>
    <body>
        <form method="post" runat="server" id="mainform">
            <sc:placeholder key="weblog-content" runat="server" />
        </form>
        <sc:Sublayout runat="server" RenderingID="{8357ABA8-BCA5-49F8-8227-567F0149AFF4}" Path="/layouts/WeBlog/ThemeScripts.ascx" />
    </body>
</html>
