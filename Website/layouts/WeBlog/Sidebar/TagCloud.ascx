<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogTagCloud.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogTagCloud" %>

<asp:Panel ID="PanelTagCloud" runat="server" CssClass="wb-tag-cloud wb-panel">
    <h3><%= Sitecore.Modules.WeBlog.Globalization.Translator.Render("TAGCLOUD") %></h3>
    <div class="wb-entries">
        <asp:Repeater runat="server" ID="TagList" ItemType="System.Collections.Generic.KeyValuePair`2[System.String,System.Int32]">
            <ItemTemplate>
                <a class="wb-weight<%# TagCloudCore.GetTagWeightClass(Item.Value) %>" href="<%# TagCloudCore.GetTagUrl(Item.Key) %>"><%# Item.Key %></a>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>