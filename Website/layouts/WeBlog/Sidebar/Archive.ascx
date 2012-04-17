<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogArchive.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogArchive" %>
<%@ Import Namespace="Sitecore.Modules.WeBlog.Items.WeBlog" %>

<div class="wb-archive wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("ARCHIVE")%></h3>
    <asp:Repeater runat="server" ID="Years">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <a onClick="ToggleVisibility('month-<%# Container.DataItem %>', this)" class="wb-year <%# ExpandMonthsOnLoad ? "expanded" : "" %>"><%# Container.DataItem %></a>
                <asp:Repeater runat="server" ID="Months" DataSource="<%# GetMonths((int)Container.DataItem) %>" OnItemDataBound="MonthDataBound">
                    <HeaderTemplate>
                        <ul id="month-<%# (Container.Parent.Parent as RepeaterItem).DataItem %>" class="wb-month" <% if(!ExpandMonthsOnLoad) { %>style="display:none;"<% } %>>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <a onClick="ToggleVisibility('entries-<%# Container.DataItem %>', this)" class="wb-month <%# ExpandPostsOnLoad ? "expanded" : "" %>"><%# GetFriendlyMonthName((int)Container.DataItem) %> (<%# GetEntryCountForYearAndMonth((int)Container.DataItem)%>)</a>
                            <asp:Repeater runat="server" ID="Entries" DataSource="<%# GetEntriesForYearAndMonth((int)Container.DataItem) %>">
                                <HeaderTemplate>
                                    <ul id="entries-<%# (Container.Parent.Parent as RepeaterItem).DataItem %>" class="wb-entries" <% if(!ExpandPostsOnLoad) { %>style="display:none;" <% } %>>
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
</div>
