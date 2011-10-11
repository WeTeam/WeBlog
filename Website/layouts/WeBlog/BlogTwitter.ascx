<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogTwitter.ascx.cs" Inherits="Sitecore.Modules.WeBlog.layouts.WeBlog.BlogTwitter" %>

<div class="wb-twitter wb-panel">
<script src="http://widgets.twimg.com/j/2/widget.js"></script>
<script>
    new TWTR.Widget({
        version: 2,
        type: 'profile',
        rpp: <%=Helper.GetParam("Number of Tweets") %>,
        interval: 30000,
        width: <%=string.IsNullOrEmpty(Helper.GetParam("Width")) ? "'auto'" : Helper.GetParam("Width") %>,
        height: <%=Helper.GetParam("Height") %>,
        theme: {
            shell: {
                background: '<%=Helper.GetParam("Shell Background") %>',
                color: '<%=Helper.GetParam("Shell Text") %>'
            },
            tweets: {
                background: '<%=Helper.GetParam("Tweet Background") %>',
                color: '<%=Helper.GetParam("Tweet Text") %>',
                links: '<%=Helper.GetParam("Links") %>'
            }
        },
        features: {
            scrollbar: <%=Helper.GetParam("Scrollbar") == "1" ? "true" : "false" %>,
            loop: false,
            live: <%=Helper.GetParam("Polling") == "1" ? "true" : "false" %>,
            hashtags: <%=Helper.GetParam("Hashtags") == "1" ? "true" : "false" %>,
            timestamp: <%=Helper.GetParam("Timestamp") == "1" ? "true" : "false" %>,
            avatars: <%=Helper.GetParam("Avatars") == "1" ? "true" : "false" %>,
            behavior: 'all'
        }
    }).render().setUser('<%=Helper.GetParam("Username") %>').start();
</script>
</div>