<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogTagcloud, Sitecore.Modules.Eviblog" %>

<asp:Panel ID="PanelTagCloud" runat="server">
    <h3><sc:Text ID="titleTagcloud" runat="server" Field="titleTagcloud" /></h3>

    <div id="tagCloud" class="sidebar">
        <asp:Literal ID="CloudLiteral" runat="server" />
    </div>
</asp:Panel>