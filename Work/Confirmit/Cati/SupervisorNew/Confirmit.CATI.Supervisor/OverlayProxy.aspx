<%@ Page Language="c#" CodeBehind="OverlayProxy.aspx.cs" AutoEventWireup="True" Inherits="Confirmit.CATI.Supervisor.OverlayProxy" %>

<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <style>
        html, body, form
        {
            height: 100%;
            margin: 0px;
            padding: 0px;
        }
    </style>
</head>

<body style="margin: 0;" scroll="no">
	<form name="main" target="_self" method="post">
	<table id="loadImage" width="100%" height="100%">
		<tr>
			<td width="100%" align="center" valign="middle">
				<img src="images/clientfunctions/progress.gif" border="0" alt="">
			</td>
		</tr>
	</table>
	</form>
	<script type="text/javascript" language="javascript">
		var params = window.parent.overlayArguments;

		if (params) {

			var form = document.getElementsByTagName("form")[0];

			for (var name in params.data) {
				var hf = document.createElement("input");
				hf.type = "hidden";
				hf.name = name;
				hf.value = params.data[name];
				form.appendChild(hf);
			}
			form.action = params.url;
			form.submit();
		}

	</script>
</body>
</html>
