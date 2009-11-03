<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogAdministrator, Sitecore.Modules.Eviblog" %>

<div id="eviblog-administration">
    
    <asp:Panel ID="LoggedInPanel" runat="server" Visible="false">
        <h3><sc:Text ID="titleAdministration" runat="server" Field="titleAdministration" /></h3>

        <p>You are logged in as: <asp:LoginName ID="LoginName1" runat="server" /></p>
                
        <asp:Label ID="lblTheme" runat="server" Text="Theme" AssociatedControlID="Theme" />
        <asp:DropDownList ID="Theme" runat="server" AutoPostBack="true"></asp:DropDownList>
        
        <p>&nbsp;</p>
        
        
    </asp:Panel>
</div>