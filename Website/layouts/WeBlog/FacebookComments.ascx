<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogFacebookComments.ascx.cs" Inherits="Sitecore.Modules.WeBlog.layouts.WeBlog.BlogFacebookComments" %>

<div class="wb-facebook-comments wb-panel">
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
<div class="fb-comments" data-href="<%=UrlToCommentOn %>" data-num-posts="<%=Helper.GetParam("Number of Posts") %>" data-width="<%=Helper.GetParam("Width") %>" data-colorscheme="<%=Helper.GetParam("Color Scheme") %>"></div>
</div>