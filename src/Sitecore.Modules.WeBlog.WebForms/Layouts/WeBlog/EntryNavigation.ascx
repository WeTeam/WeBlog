<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntryNavigation.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.EntryNavigation" %>

<asp:Panel ID="PanelEntryNavigation" runat="server" CssClass="wb-entry-navigation">
    <% if (ShowTitle) { %>
        <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("NAVIGATION")%> </h3>
    <% } %>
    
    <div class="wb-entry-navigation-buttons">
        <div class="wb-entry-navigation-previous">
            <% if (PreviousEntry!=null) { %>
                <p class="wb-entry-navigation-label">
                    <%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("NAVIGATION_PREVIOUS")%>
                </p>
                <p class="wb-entry-navigation-title">
                    <a href="<%=PreviousEntry.Url%>" title="<%=PreviousEntry.Title.Raw%>">
                        <%=PreviousEntry.Title.Raw%>
                    </a>
                </p>
            <% } %>
        </div>
        <div class="wb-entry-navigation-next">
            <% if (NextEntry!=null) { %>
                <p class="wb-entry-navigation-label">
                    <%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("NAVIGATION_NEXT")%>
                </p>
                <p class="wb-entry-navigation-title">
                    <a href="<%=NextEntry.Url%>" title="<%=NextEntry.Title.Raw%>">
                        <%=NextEntry.Title.Raw%>
                    </a>
                </p>
            <% } %>
        </div>
    </div>
</asp:Panel>