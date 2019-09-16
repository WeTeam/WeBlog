<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThemeStylesheets.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.ThemeStylesheets" %>

<asp:Repeater runat="server" ID="Stylesheets">
    <ItemTemplate>
        <link rel="stylesheet" type="text/css" href="<%# Eval("Url") %>" />
    </ItemTemplate>
</asp:Repeater>