<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogTagCloud.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogTagCloud" %>
<%@ Import Namespace="System.Collections.Generic" %>

<asp:Panel ID="PanelTagCloud" runat="server" CssClass="wb-tagCloud wb-panel">
    <h3><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("TAGCLOUD") %></h3>
    <div class="wb-entries">
        <asp:Repeater runat="server" ID="TagList">
            <ItemTemplate>
                <a class="wb-weight<%# GetTagWeightClass(((KeyValuePair<string, int>)Container.DataItem).Value) %>" href="<%# GetTagUrl(((KeyValuePair<string, int>)Container.DataItem).Key) %>">
                    <%# ((KeyValuePair<string, int>)Container.DataItem).Key %>
                </a>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>