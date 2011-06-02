<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogArchive.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogArchive" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.Blog" %>
<h3>Archive</h3>

<asp:Repeater runat="server" ID="Years">
    <HeaderTemplate>
        <ul id="blog-archive" class="sidebar">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <a onClick="ToggleVisibility('month-<%# Container.DataItem %>')" class="year"><%# Container.DataItem %></a>
            <asp:Repeater runat="server" ID="Months" DataSource="<%# GetMonths((int)Container.DataItem) %>" OnItemDataBound="MonthDataBound">
                <HeaderTemplate>
                    <ul id="month-<%# (Container.Parent.Parent as RepeaterItem).DataItem %>" class="month">
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <a onClick="ToggleVisibility('entries-<%# Container.DataItem %>')" class="month"><%# GetFriendlyMonthName((int)Container.DataItem) %> (<%# GetEntryCountForYearAndMonth((int)Container.DataItem)%>)</a>
                        <asp:Repeater runat="server" ID="Entries" DataSource="<%# GetEntriesForYearAndMonth((int)Container.DataItem) %>">
                            <HeaderTemplate>
                                <ul id="entries-<%# (Container.Parent.Parent as RepeaterItem).DataItem %>" class="entries" <% if(!ExpandPostsOnLoad) { %>style="display:none;" <% } %>>
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
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>

<%--<asp:Repeater runat="server" ID="Years">
    <HeaderTemplate>
        <ul id="blog-archive" class="sidebar">
    </HeaderTemplate>
    <ItemTemplate>
        <li class="month" onClick="ToggleVisibility('entries-<%# Eval("Year") %>' + year + month + "\")"></li>
        <ul id="month-<%# Eval("Year") %>" class="month">
            <asp:Repeater runat="server" ID="Months">
                <ItemTemplate>
                    
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>--%>