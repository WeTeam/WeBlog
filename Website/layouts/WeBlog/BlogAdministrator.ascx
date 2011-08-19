<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogAdministrator.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogAdministrator" %>

<div id="blog-administration">
    
    <asp:Panel ID="LoggedInPanel" runat="server" Visible="false">
        <h3><sc:Text ID="titleAdministration" runat="server" Field="titleAdministration" /></h3>

        <p><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("LOGGED_IN_AS") %>&nbsp;<asp:LoginName ID="LoginName1" runat="server" /></p>
                
        <asp:Label ID="lblTheme" runat="server" Text="Theme" AssociatedControlID="Theme" />
        <asp:DropDownList ID="Theme" runat="server" AutoPostBack="true"></asp:DropDownList>
        
        <p>&nbsp;</p>
        
        
    </asp:Panel>
</div>