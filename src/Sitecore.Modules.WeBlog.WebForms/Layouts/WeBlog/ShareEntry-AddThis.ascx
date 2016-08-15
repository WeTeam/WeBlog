<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShareEntry-AddThis.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog.ShareEntryAddThis" %>
<div class="wb-entry-share wb-panel">
    <h3><%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("SHARE")%></h3>
    <div class="wb-entries">
        <%-- Data binding syntax needed to avoid parser error from fb:like:layout --%>
        <!-- AddThis Button BEGIN -->
        <div class="addthis_toolbox addthis_default_style ">
            <a class="addthis_button_facebook_like" <%#"fb:like:layout=\"button_count\""%>></a>
            <a class="addthis_button_tweet"></a>
            <a class="addthis_counter addthis_pill_style"></a>
        </div>
        <script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js<%#AddThisAccountName%>"></script>
        <!-- AddThis Button END -->
    </div>
</div>
