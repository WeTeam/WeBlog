<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogTwitter.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.BlogTwitter" %>

<div class="wb-twitter wb-panel">
<script src="http://widgets.twimg.com/j/2/widget.js"></script>
<script>
    new TWTR.Widget({
        version: 2,
        type: 'profile',
        rpp: <%= NumberOfTweets %>,
        interval: 30000,
        width: <%= Width %>,
        height: <%= Height %>,
        theme: {
            shell: {
                background: '<%= ShellBackground %>',
                color: '<%= ShellText %>'
            },
            tweets: {
                background: '<%= TweetBackground %>',
                color: '<%= TweetText %>',
                links: '<%= Links %>'
            }
        },
        features: {
            scrollbar: <%= Scrollbar %>,
            loop: false,
            live: <%= Polling %>,
            hashtags: <%= Hashtags %>,
            timestamp: <%= Timestamps %>,
            avatars: <%= Avatars %>,
            behavior: 'all'
        }
    }).render().setUser('<%= Username %>').start();
</script>
</div>