<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="Codeflood.Testing.TestRunner" Buffer="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Codeflood Test Runner</title>
   <style type="text/css">
		body { font-family: Tahoma, Arial, Sans-Serif; font-size: 10pt; }
        h1 { font-size: 1.5em; }
        h2 { font-size: 1.3em; }
		.pass { width: 1em; height: 1em; background-color: green; }
		.fail { width: 1em;height: 1em; background-color: red; }
		.notRun { width: 1em; height: 1em; background-color: yellow; }
		.passLabel { font-size: 1.3em; font-weight: bold; color: green; }
		.failLabel { font-size: 1.3em; font-weight: bold; color: red; }
   </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<h1>Codeflood Test Runner</h1> 
		<p> 
			<asp:Literal runat="server" ID="ltlStats" />
		</p>
		<p>
			<asp:Label runat="server" ID="lblResult" />
		</p>
		<asp:GridView runat="server" ID="gvResults" AutoGenerateColumns="false">
			<Columns>
				<asp:BoundField DataField="test" HeaderText="Test" />
				<asp:BoundField DataField="result" HeaderText="Result"/>
                <asp:BoundField DataField="time" HeaderText="Time" />
				<asp:TemplateField>
					<ItemTemplate>
						<div class="<%# Eval("class") %>">&nbsp;</div>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:BoundField DataField="message" HeaderText="Message" />
			</Columns>
		</asp:GridView>
		<hr />
		<h2>Category Filter</h2>
		If no categories are selected the entire suite will run.
		<asp:CheckBoxList runat="server" ID="cblCategories" RepeatColumns="8" RepeatDirection="Horizontal" RepeatLayout="Table" />
		<asp:Button runat="server" ID="btnRun" Text="Run" OnClick="RunClick" /><br />
    </div>
    </form>
</body>
</html>

