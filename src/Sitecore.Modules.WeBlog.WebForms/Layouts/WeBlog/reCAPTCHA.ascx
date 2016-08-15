﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReCaptcha.ascx.cs" Inherits="Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog.ReCaptcha" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<div class="wb-captcha">
    <asp:Label ID="lblCaptcha" runat="server" AssociatedControlID="uxRecaptcha" CssClass="wb-captchaLabel" />
    <recaptcha:RecaptchaControl
        ID="uxRecaptcha"
        OnInit="uxRecaptcha_Init"
        runat="server"
        PublicKey=""
        PrivateKey="" />
</div>
<%=Sitecore.Modules.WeBlog.Globalization.Translator.Render("CAPTCHA")%>
