<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xml:lang="en" xmlns="http://www.w3.org/1999/xhtml">
  <head>
    <sc:VisitorIdentification ID="VisitorIdentification1" runat="server"/>
    <%-- Required for EviBlog--%>
    <title><asp:placeholder id="phEviblogTitle" runat="server"></asp:placeholder></title>
    <link href="/sitecore modules/EviBlog/Includes/Common.css" rel="stylesheet" />
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.3/jquery.min.js" type="text/javascript"></script>
    <script src="/sitecore modules/EviBlog/Includes/jquery.url.js" type="text/javascript"></script>
    <script src="/sitecore modules/EviBlog/Includes/functions.js" type="text/javascript"></script>
    <asp:placeholder id="phEviblog" runat="server"></asp:placeholder>
  </head>
  <body>
  <form method="post" runat="server" id="mainform">
  
    <sc:placeholder ID="phContent" key="phContent" runat="server" />
  
  </form>
  </body>
</html>
