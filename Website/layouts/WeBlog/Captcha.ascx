<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Captcha.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.Captcha" %>
<%@ Register TagPrefix="ms" Namespace="MSCaptcha" Assembly="MSCaptcha" %>
<div class="wb-captcha">
    <p>
    <asp:Label ID="lblCaptcha" runat="server" AssociatedControlID="uxCaptchaCode" CssClass="wb-captchaLabel" />
    <div class="blog-captcha-image">
        <ms:CaptchaControl ID="uxCaptchaCode" runat="server" CaptchaBackgroundNoise="Low" CaptchaFontWarping="Low" CaptchaLineNoise="Low" />
    </div>
    <asp:TextBox runat="server" ID="uxCaptchaText" CssClass="wb-captchaInput"></asp:TextBox>
    <asp:CustomValidator runat="server" ID="uxCaptchaValidator" Display="None" OnServerValidate="uxCaptchaValidator_ServerValidate" />
    </p>
</div>