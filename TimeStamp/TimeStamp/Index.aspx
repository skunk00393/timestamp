<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="TimeStamp.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Time Stamp</title>
    <link rel="stylesheet" href="Style.css" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/q.js/1.4.1/q.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/spark-md5/2.0.2/spark-md5.min.js"></script>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/FileSaver.js/1.3.8/FileSaver.js"></script>
    <script type="text/javascript" src="JavaScript.js"></script>
</head>
<body>
    <div id="content">
        <div>
            <h1 id="title">TimeStamp</h1>
        </div>
        <div id="forms">
            <div id="formUploadFile" class="formUpload">
                <h3>Create Time Stamp</h3>
                <input type="file" id="file" class="files"/>
                <input type="button" onclick="getFileInfo();" value="Create" class="buttons" />  
            </div>
            <br />


            <div id="formCheckCertificate" class="formUpload">
                 <h3>Verify TimeStamp with Certificate</h3>
                <form runat="server">
                    <asp:FileUpload ID="CheckCertificate" runat="server" class="files" />
                    <asp:Button runat="server" Text="Verify Certificate" ID="CheckCertificate_btn" OnClick="CheckCertificate_btn_Click" class="buttons" />
                </form>
            </div><br />


            <div id="formCheckFile" class="formUpload">
                <h3>Verify TimeStamp with Original File</h3>
                <input type="file" id="CheckFile" class="files"/>
                <input type="button" onclick="checkFileInfo();" value="Verify File" class="buttons" />  
            </div>
        </div>
    </div>
</body>
</html>