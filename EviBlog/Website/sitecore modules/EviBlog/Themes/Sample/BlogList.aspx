<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="blog" Namespace="Sitecore.Modules.Eviblog.Webcontrols" Assembly="Sitecore.Modules.Eviblog" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><blog:BlogPageTitle runat="server" /></title>
    <link rel="stylesheet" type="text/css" href="default.css" media="screen"/>
</head>
<body>
    <form id="form1" runat="server">
    
    
    <div class="container">
	
	<div class="main">

		<div class="header">
		
			<div class="title">
				<h1><blog:BlogTitle id="blogtitle1" runat="server" /></h1>
			</div>

		</div>
		
		<div class="content">
	
            <div class="item">

                <%--<blog:EntryListing runat="server" ID="MyListView1">
                    <LayoutTemplate>
                        <ul>
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                        </ul>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <li><blog:EntryTitle runat="server" /></li>
                    </ItemTemplate>
                </blog:EntryListing>--%>
                
                <blog:EntryListing runat="server" ID="EntryListing1"></blog:EntryListing>
            </div>
            
		</div>

		<div class="sidenav">

            <h1>Categories</h1>
            <blog:BlogCategories runat="server" />
            
            <h1>Archive</h1>
			<blog:BlogArchive runat="server" />
            
            <h1>Tagcloud</h1>
            <blog:Tagcloud runat="server" />
            
            <h1>Recent Comments</h1>
            <blog:CommentListing runat="server" ShowRecentComments="true" />

		</div>
	
		<div class="clearer"><span></span></div>

	</div>

	<div class="footer">&copy; 2006 <a href="index.html">Website.com</a>. Valid <a href="http://jigsaw.w3.org/css-validator/check/referer">CSS</a> &amp; <a href="http://validator.w3.org/check?uri=referer">XHTML</a>. Template design by <a href="http://templates.arcsin.se">Arcsin</a>
	</div>

</div>

    
    </form>
</body>
</html>
