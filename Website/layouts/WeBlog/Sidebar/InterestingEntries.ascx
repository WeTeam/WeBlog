<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogInterestingEntries.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogInterestingEntries" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Data.Items" %>

<div class="wb-panel">
    <h3><%= Sitecore.Modules.WeBlog.Globalization.Translator.Render("POPULAR_POSTS") %></h3>
    <asp:Repeater runat="server" ID="ItemList">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <a href="<%# (Container.DataItem as EntryItem).Url %>"><%# (Container.DataItem as EntryItem).Title.Rendered %></a>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
</div>