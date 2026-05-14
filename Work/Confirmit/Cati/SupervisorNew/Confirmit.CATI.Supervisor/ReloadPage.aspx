<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReloadPage.aspx.cs" Inherits="Confirmit.CATI.Supervisor.ReloadPage" %>
<html>
<head>
    <meta charset="UTF-8">
    <title>Cati Login Page Redirection</title>
</head>
<body>
    <script>
        if (top.reloadApplication)
            top.reloadApplication();
        if (opener && opener.top.reloadApplication)
            opener.top.reloadApplication();
    </script>
</body>
</html>
