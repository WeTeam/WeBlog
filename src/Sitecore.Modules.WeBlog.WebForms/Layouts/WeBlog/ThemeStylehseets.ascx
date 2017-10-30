<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThemeStylehseets.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.ThemeStylehseets" %>

<asp:Repeater runat="server" ID="Stylesheets">
    <ItemTemplate>
        <link rel="stylesheet" type="text/css" href="<%# Eval("Url") %>" />
    </ItemTemplate>
</asp:Repeater>