<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SitecoreCaptcha.ascx.cs" Inherits="Sitecore.Modules.WeBlog.Layouts.SitecoreCaptcha" %>
<%@ Register TagPrefix="scc" Namespace="Sitecore.Captcha.WebControls" Assembly="Sitecore.Captcha" %>
<div class="blog-captcha">
    <asp:Label ID="lblCaptcha" runat="server" Text="Please confirm you are human by typing the text you see in this image:" AssociatedControlID="uxCaptchaCode" CssClass="captchaLabel" />
    <div class="blog-captcha-image">
        <scc:Captcha ID="uxCaptchaCode" runat="server" OnRefreshButtonClick="uxCaptchaCode_RefreshButtonClick">
            <CaptchaImage CaptchaBackgroundNoise="Low" CaptchaFontWarping="Low" CaptchaLineNoise="Low" />
            <CaptchaPlayButton CausesValidation="false" />
            <CaptchaRefreshButton CausesValidation="false" />
        </scc:Captcha>
    </div>
    <asp:TextBox runat="server" ID="uxCaptchaText" Width="196"></asp:TextBox>
    <asp:CustomValidator runat="server" ID="uxCaptchaValidator" Display="None" OnServerValidate="uxCaptchaValidator_ServerValidate" />
</div>