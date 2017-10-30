<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntryNavigation.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.EntryNavigation" %>

<asp:Panel ID="PanelEntryNavigation" runat="server" CssClass="wb-entry-navigation">
    <% if (ShowTitle) { %>
        <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("NAVIGATION")%> </h3>
    <% } %>
    
    <div class="wb-entry-navigation-buttons">
        <% if (PreviousEntry!=null) { %>
            <a href="<%=PreviousEntry.Url%>" class="wb-entry-navigation-previous" title="<%=PreviousEntry.Title.Rendered%>">
                <%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("NAVIGATION_PREVIOUS")%>
            </a>
        <% } %>

        <% if (NextEntry!=null) { %>
            <a href="<%=NextEntry.Url%>" class="wb-entry-navigation-next" title="<%=NextEntry.Title.Rendered%>">
                <%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("NAVIGATION_NEXT")%>
            </a>
        <% } %>
    </div>
</asp:Panel>