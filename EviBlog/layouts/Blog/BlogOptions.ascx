<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogOptions, Sitecore.Modules.Eviblog" %>

<asp:Panel ID="EditModePanel" Visible="false" runat="server" CssClass="settings">
    <b>Settings</b><br />
    <table style="width: 300px;">
    <tr>
        <td>Number of items to display</td>
        <td><sc:Text ID="txtItemCount" Field="DisplayItemCount" runat="server" /></td>
    </tr>
    <tr>
        <td>Number of comments to display on sidebar</td>
        <td><sc:Text ID="Text1" Field="DisplayCommentSidebarCount" runat="server" /></td>
    </tr>
    <tr>
        <td>Enable RSS</td>
        <td><asp:CheckBox ID="CheckBoxEnableRSS" runat="server" />
        <sc:Text ID="txtCheckBoxEnableRSS" Field="CheckBoxEnableRSS" runat="server" /></td>
    </tr>
    <tr>
        <td>Enable comments</td>
        <td><%--<asp:CheckBox ID="CheckBoxEnableComments" runat="server" />--%>
        <sc:Text ID="txtCheckBoxEnableComments" Field="CheckBoxEnableComments" runat="server" /></td>
    </tr>
    <tr>
        <td>Show email in  comments</td>
        <td><%--<asp:CheckBox ID="CheckBoxCommentsEmail" runat="server" />--%>
            <sc:Text ID="txtCheckBoxCommentsEmail" Field="CheckBoxCommentsEmail" runat="server" />
        </td>
    </tr>
    </table>
</asp:Panel>