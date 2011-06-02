<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="reCaptcha.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.reCaptcha" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<div class="captcha">
    <asp:Label ID="lblCaptcha" runat="server" Text="Please confirm you are human by typing the text you see in this image:" AssociatedControlID="uxRecaptcha" CssClass="captchaLabel" />
    <recaptcha:RecaptchaControl
        ID="uxRecaptcha"
        OnInit="uxRecaptcha_Init"
        runat="server"
        PublicKey=""            
        PrivateKey=""
    />
</div>