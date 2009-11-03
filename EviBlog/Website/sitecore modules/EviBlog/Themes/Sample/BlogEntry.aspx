<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="blog" Namespace="Sitecore.Modules.Eviblog.Webcontrols" Assembly="Sitecore.Modules.Eviblog" %>
<%@ Register src="../../Controls/AddComment.ascx" tagname="AddComment" tagprefix="blog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><blog:BlogPageTitle runat="server" /></title>
    <link rel="stylesheet" type="text/css" href="default.css" media="screen"/>
    <blog:BlogMetaInfo runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    
    <div class="container">
	
	<div class="main">

		<div class="header">
		
			<div class="title">
				<h1><blog:BlogTitle UseLink="True" runat="server" /></h1>
			</div>

		</div>
		
		<div class="content">
	
			<div class="item">

				<h1><blog:EntryTitle runat="server" /></h1>
				<div class="descr"><blog:EntryPostDate runat="server" /></div>
                
				<blog:EntryIntroduction runat="server" />

				<blog:EntryContent runat="server" />

                <blog:CommentListing runat="server">
                <LayoutTemplate>
                    <h3>Comments</h3>
                    <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                    <blog:AddComment ID="AddComment1" runat="server" />
                    
                </LayoutTemplate>
                <ItemTemplate>
                    <h2><blog:CommentTitle runat="server" /></h2>
                    <blog:CommentText runat="server" />
                </ItemTemplate>
                </blog:CommentListing>

			</div>
	
		</div>

		<div class="sidenav">

			<h1>Categories</h1>
            <blog:BlogCategories runat="server" />
            
            <h1>Archive</h1>
			<blog:BlogArchive runat="server" />
            
            <h1>Tagcloud</h1>
            <blog:Tagcloud runat="server" />

		</div>
	
		<div class="clearer"></div>

	</div>

	<div class="footer">&copy; 2006 <a href="index.html">Website.com</a>. Valid <a href="http://jigsaw.w3.org/css-validator/check/referer">CSS</a> &amp; <a href="http://validator.w3.org/check?uri=referer">XHTML</a>. Template design by <a href="http://templates.arcsin.se">Arcsin</a>
	</div>

</div>

    
    

    
    </form>
</body>
</html>
