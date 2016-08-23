<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="Codeflood.Testing.TestRunner" Buffer="false" %>

<!doctype html>

<html>
<head runat="server">
    <title>codeflood Test Runner</title>
    <style type="text/css">
body {
    font-family: Tahoma, Arial, Sans-Serif;
    font-size: 10pt;
    color: #333333;
}

h1 {
    font-size: 1.5em;
}

h2 {
    font-size: 1.3em;
}

h3 {
    font-size: 1.2em;
}

.filters ul {
    padding: 0;
}

.filters li {
    list-style: none;
}

.pass {
    width: 1em;
    height: 1em;
    background-color: #5eb95e;
}

.fail {
    width: 1em;
    height: 1em;
    background-color: #dd514c;
}

.notRun {
    width: 1em;
    height: 1em;
    background-color: #fad232;
}

.passLabel {
    font-size: 1.3em;
    font-weight: bold;
    color: #5eb95e;
}

.failLabel {
    font-size: 1.3em;
    font-weight: bold;
    color: #dd514c;
}

.hide {
    display: none;
}

.show {
    display: block;
}

#filterCategory, #filterMethod {
    margin-left: 1em;
    padding-left: 1em;
    border-left: solid 3px #999999;
}
    </style>
    <script type="text/javascript">
function checkAll(checked, containerId) {
    var container = document.getElementById(containerId);
    if (!container) {
        return;
    }

    var cbs = container.getElementsByTagName("input");
    for (var i = 0; i < cbs.length; i++) {
        var cb = cbs[i];
        if (cb.type === "checkbox") {
            cb.checked = checked;
        }
    }
}

function toggle() {
    for (var idx in arguments) {
        var id = arguments[idx];
        var element = document.getElementById(id);
        if (element.className === "show") {
            element.className = "hide";
        } else {
            element.className = "show";
        }

    }
}
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>codeflood Test Runner</h1>
            <p>
                <asp:Literal runat="server" ID="ltlStats" />
            </p>
            <p>
                <asp:Label runat="server" ID="lblResult" />
            </p>
            <asp:GridView runat="server" ID="gvResults" AutoGenerateColumns="false" CellPadding="2" CellSpacing="2" ClientIDMode="Static">
                <Columns>
                    <asp:BoundField DataField="test" HeaderText="Test" />
                    <asp:BoundField DataField="result" HeaderText="Result" />
                    <asp:BoundField DataField="time" HeaderText="Time" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <div class="<%# Eval("class") %>">&nbsp;</div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="message" HeaderText="Message" HtmlEncode="False" />
                </Columns>
            </asp:GridView>
            <hr />
            <div class="filters">
                <h2>Filters</h2>
                <p>If no filters are selected the entire test suite will run.</p>
                <h3>Category</h3>
                <p>
                    <a id="hideCategoryFilter" class="hide" href="javascript:void(0)" onclick="toggle('filterCategory', 'hideCategoryFilter', 'showCategoryFilter')">hide</a>
                    <a id="showCategoryFilter" class="show" href="javascript:void(0)" onclick="toggle('filterCategory', 'showCategoryFilter', 'hideCategoryFilter')">show</a>
                </p>
                <div id="filterCategory" class="hide">
                    <p>
                        <a href="javascript:void(0)" onclick="checkAll(true, 'cblCategories');">select all</a>
                        <a href="javascript:void(0)" onclick="checkAll(false, 'cblCategories');">unselect all</a>
                    </p>
                    <asp:CheckBoxList runat="server" ID="cblCategories" RepeatLayout="UnorderedList" ClientIDMode="Static" />
                </div>
                <h3>Test Methods</h3>
                <p>
                    <a id="hideMethodFilter" class="hide" href="javascript:void(0)" onclick="toggle('filterMethod', 'hideMethodFilter', 'showMethodFilter')">hide</a>
                    <a id="showMethodFilter" class="show" href="javascript:void(0)" onclick="toggle('filterMethod', 'showMethodFilter', 'hideMethodFilter')">show</a>
                </p>
                <div id="filterMethod" class="hide">
                    <p>
                        <a href="javascript:void(0)" onclick="checkAll(true, 'cblMethods');">select all</a>
                        <a href="javascript:void(0)" onclick="checkAll(false, 'cblMethods');">unselect all</a>
                    </p>
                    <asp:CheckBoxList runat="server" ID="cblMethods" RepeatLayout="UnorderedList" ClientIDMode="Static" />
                </div>
            </div>
            <hr/>
            <asp:Button runat="server" ID="btnRun" Text="Run" OnClick="RunClick" /><br />
        </div>
    </form>
</body>
</html>

