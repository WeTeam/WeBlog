<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShareEntry-ShareThis.ascx.cs" Inherits="Sitecore.Modules.WeBlog.layouts.Blog.ShareEntry_ShareThis" %>
<div class="entry-share">
    <span class='st_facebook_hcount' displayText='Facebook'></span><span class='st_twitter_hcount' displayText='Tweet'></span><span class='st_sharethis_hcount' displayText='ShareThis'></span>
</div>
<script type="text/javascript">
    var switchTo5x = true;
</script>
<script type="text/javascript" src="http://w.sharethis.com/button/buttons.js"></script>
<%
  if (!string.IsNullOrEmpty(ShareThisPublisherID))
  {  
%>
    <script type="text/javascript">
        stLight.options({ publisher: '<%#ShareThisPublisherID%>' });
    </script>
<%
  }
%>