<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogEntry.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogEntry" %>

<div class="entry">
    <sc:Image runat="server" ID="EntryImage" Field="Image" CssClass="entry-image" />
    <h2><sc:Text ID="txtTitle" Field="Title" runat="server" /></h2>
    <div class="details"><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("POSTED_ON")%>&nbsp;<sc:Date runat="server" ID="PostedDate" Field="__created" Format="dddd, MMMM d, yyyy" /> by <%=CurrentEntry.CreatedBy.LocalName%></div>
    <p><sc:Text ID="txtIntroduction" Field="Introduction" runat="server" /></p>
    <p><sc:Text ID="txtContent" Field="Content" runat="server" /></p>
        
    <asp:ListView ID="ListViewCategories" runat="server">
    <LayoutTemplate>
        <div>
            <ul class="entry-categories">
                <li><%# Sitecore.Modules.WeBlog.Globalization.Translator.Render("POSTED_IN") %>&nbsp;</li>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
            </ul>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <li>
            <asp:HyperLink ID="hyperlinkCategory" runat="server" NavigateUrl='<%# GetItemUrl(Eval("InnerItem") as Sitecore.Data.Items.Item) %>'>
                <sc:Text ID="txtCategorie" Field="Title" runat="server" DataSource='<%# Eval("ID") %>' />
            </asp:HyperLink>
        </li>
    </ItemTemplate>
    </asp:ListView>
        
    <div id="entry-tags">
        <span><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("TAGS")%> </span>
        <asp:LoginView ID="LoginViewTags" runat="server">
            <AnonymousTemplate>
                <asp:Repeater runat="server" ID="TagList">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="TagLink" NavigateUrl='<%# GetTagUrl(Container.DataItem as string) %>'>
                            <%# Container.DataItem %>
                        </asp:HyperLink>
                    </ItemTemplate>
                </asp:Repeater>
            </AnonymousTemplate>
            <LoggedInTemplate>
                <sc:Text ID="txtTags" Field="Tags" runat="server" />
            </LoggedInTemplate>
        </asp:LoginView>
    </div>

    <sc:Placeholder runat="server" key="phBlogBelowEntry" /> 
</div>