<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Facebook.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar.BlogFacebookLike" %>

<div class="wb-facebook wb-panel">
    <script>
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.id = id;
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'div', 'fb-root'));
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    </script>
    <div class="fb-like" data-layout="<%= LayoutStyle %>" data-href="<%= UrlToLike %>" data-send="<%= SendButton %>" data-width="<%= Width %>" data-colorscheme="<%= ColorScheme %>" data-show-faces="<%= ShowFaces %>"></div>
</div>
