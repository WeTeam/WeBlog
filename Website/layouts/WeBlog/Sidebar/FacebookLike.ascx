<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogFacebook.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.WeBlog.BlogFacebookLike" %>

<div class="wb-facebook wb-panel">
<script>
    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) { return; }
        js = d.createElement(s); js.id = id;
        fjs.parentNode.insertBefore(js, fjs);
    } (document, 'div', 'fb-root'));
    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) { return; }
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
        fjs.parentNode.insertBefore(js, fjs);
    } (document, 'script', 'facebook-jssdk'));
</script>
<div class="fb-like" data-layout="<%=Helper.GetParam("Layout Style") %>" data-href="<%=Helper.GetParam("Url to Like") %>" data-send="<%=Helper.GetParam("Send Button") == "1" ? "true" : "false" %>" data-width="<%=Helper.GetParam("Width") %>" data-colorscheme="<%=Helper.GetParam("Color Scheme") %>" data-show-faces="<%=Helper.GetParam("Show Faces") == "1" ? "true" : "false" %>"></div>
</div>