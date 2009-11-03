<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Sitecore.Modules.Eviblog.UserControls.BlogEntryByCategorie, Sitecore.Modules.Eviblog" %>

<asp:ListView ID="BlogEntriesByCategorieListView" runat="server">
    <LayoutTemplate>
        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
    </LayoutTemplate>
    <ItemTemplate>
        <div class="entry">
            <h2><sc:Text ID="Text1" DataSource='<%#Eval("ID") %>' Field="Title" runat="server" /></h2>
            
            <sc:Text ID="Text2" DataSource='<%#Eval("ID") %>' Field="Introduction" runat="server" />
            
            <asp:HyperLink ID="BlogPostLink" runat="server">Read more...</asp:HyperLink>
        </div>
    </ItemTemplate>
    <EmptyDataTemplate>
        No entries found
    </EmptyDataTemplate>
</asp:ListView>
