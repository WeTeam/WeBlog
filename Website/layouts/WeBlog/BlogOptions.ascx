<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogOptions.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogOptions" %>

<asp:Panel ID="EditModePanel" Visible="false" runat="server" CssClass="wb-settings wb-panel">
    <b><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("SETTINGS") %></b><br />
    <table style="width: 300px;">
    <tr>
        <td><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("NO_ITEMS_DISPLAY") %></td>
        <td><sc:Text ID="txtItemCount" Field="DisplayItemCount" runat="server" /></td>
    </tr>
    <tr>
        <td><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("NO_COMMENTS_DISPLAY") %></td>
        <td><sc:Text ID="Text1" Field="DisplayCommentSidebarCount" runat="server" /></td>
    </tr>
    <tr>
        <td><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("ENABLE_RSS") %></td>
        <td><asp:CheckBox ID="CheckBoxEnableRSS" runat="server" />
        <sc:Text ID="txtCheckBoxEnableRSS" Field="CheckBoxEnableRSS" runat="server" /></td>
    </tr>
    <tr>
        <td><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("SHOW_EMAIL_COMMENTS") %></td>
        <td><asp:CheckBox ID="CheckBoxCommentsEmail" runat="server" />
            <sc:Text ID="txtCheckBoxCommentsEmail" Field="CheckBoxCommentsEmail" runat="server" />
        </td>
    </tr>
    </table>
</asp:Panel>