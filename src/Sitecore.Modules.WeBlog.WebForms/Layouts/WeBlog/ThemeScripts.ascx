<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThemeScripts.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.ThemeScripts" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<% if (ThemeItem != null)
   {
       foreach(var script in ThemeItem.Scripts) { %>
            <script type="text/javascript" src="<%= script.Url %>"></script>
            <% if (!string.IsNullOrEmpty(script.FallbackUrl) && !string.IsNullOrEmpty(script.VerificationObject))
                { %>
            <script type="text/javascript">
                if(typeof <%= script.VerificationObject %> == "undefined") {
                    document.write(unescape("%3Cscript src='<%= script.FallbackUrl %>' type='text/javascript'%3E%3C/script%3E"));
                }
            </script>
            <% } %>
<%      }
   } %>