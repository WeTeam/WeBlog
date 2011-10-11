<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntryTags.ascx.cs" Inherits="Sitecore.Modules.WeBlog.layouts.WeBlog.BlogEntryTags" %>

<asp:Panel ID="PanelEntryTags" runat="server" CssClass="wb-entry-tags wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("TAGS")%> </h3>
    <asp:LoginView ID="LoginViewTags" runat="server">
        <AnonymousTemplate>
            <asp:ListView ID="TagList" runat="server">
                <LayoutTemplate>
                    <ul>
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                    </ul>
                </LayoutTemplate>
                <ItemTemplate>
                    <li>
                        <asp:HyperLink runat="server" ID="TagLink" NavigateUrl='<%# GetTagUrl(Container.DataItem as string) %>'>
                            <%# Container.DataItem %>
                        </asp:HyperLink>
                    </li>
                </ItemTemplate>
            </asp:ListView>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <sc:Text ID="txtTags" Field="Tags" runat="server" />
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Panel>