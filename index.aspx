<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="btnUpload" Text="Upload" runat="server" OnClick="UploadFile" />
            <hr />            
            <p id="r1" runat="server"></p>
        </div>
    </form>
</body>
</html>
