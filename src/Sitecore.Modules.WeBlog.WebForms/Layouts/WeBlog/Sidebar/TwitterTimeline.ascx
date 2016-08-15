<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TwitterTimeline.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog.Sidebar.BlogTwitter" %>

<div class="wb-twitter wb-panel">
    <a class="twitter-timeline"
        data-screen-name="<%= Username%>"
        data-widget-id="<%= WidgetId%>"
        data-theme="<%= Theme%>"
        data-tweet-limit="<%= NumberOfTweets %>"
        data-chrome="<%= Chrome%>"
        data-border-color="<%= BorderColour %>"
        data-link-color="<%= LinkColour%>"
        height="<%= Height%>"
        width="<%= Width%>">
        <% if (String.IsNullOrEmpty(WidgetId) && Sitecore.Context.PageMode.IsPageEditor)
            { %>
        <b>WidgetId</b> parameter is missing.
        <% } %>
    </a>
    <script>!function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0], p = /^http:/.test(d.location) ? 'http' : 'https'; if (!d.getElementById(id)) { js = d.createElement(s); js.id = id; js.src = p + "://platform.twitter.com/widgets.js"; fjs.parentNode.insertBefore(js, fjs); } }(document, "script", "twitter-wjs");</script>
</div>