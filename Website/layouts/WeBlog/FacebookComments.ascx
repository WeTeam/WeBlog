﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogFacebookComments.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogFacebookComments" %>

<div id="fb-root"></div>
<script>
    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s);
        js.id = id;
        js.src = "http://connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.5";
        fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));
</script>
<div class="fb-comments" data-href="<%= UrlToCommentOn %>" data-num-posts="<%= NumberOfPosts %>" data-width="<%= Width %>" data-colorscheme="<%= ColorScheme %>"></div>
