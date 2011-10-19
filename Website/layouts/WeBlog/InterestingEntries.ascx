<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogInterestingEntries.ascx.cs" Inherits="Sitecore.Modules.WeBlog.layouts.WeBlog.BlogInterestingEntries" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.WeBlog" %>

<div class="wb-panel">
    <h3><%= Title %></h3>
    <asp:Repeater runat="server" ID="Items">
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