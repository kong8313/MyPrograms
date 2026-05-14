<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Hosting" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
  protected void Page_PreRender(object sender, EventArgs e)
  {
      Refresh_Click(sender, e);
  }
  
  protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
  {
      string id = GridView1.Rows[e.RowIndex].Cells[1].Text;
      ApplicationManager appManager = ApplicationManager.GetApplicationManager();
      appManager.ShutdownApplication(id);          
  }

  protected void Refresh_Click(object sender, EventArgs e)
  {
      ApplicationManager appManager = ApplicationManager.GetApplicationManager();
      ApplicationInfo[] appInfo = appManager.GetRunningApplications();
      GridView1.DataSource = appInfo;
      GridView1.DataBind();
      Idle.Text = appManager.IsIdle().ToString();
  }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <meta http-equiv="Content-Type" content="text/html" />
  <title>Loaded applications</title>
</head>
<body>
  <form id="form1" runat="server">
    <asp:GridView ID="GridView1" runat="server" 
        onrowdeleting="GridView1_RowDeleting" 
        AutoGenerateColumns="False">
        <Columns>
            <asp:CommandField ButtonType="Button" ShowDeleteButton="True" DeleteText="Unload"/>
            <asp:BoundField DataField="ID" HeaderText="ID" />
            <asp:BoundField DataField="VirtualPath" HeaderText="VirtualPath" />
        </Columns>
    </asp:GridView>

    <p>Idle:
    <asp:Label ID="Idle" runat="server" />
    </p>
    <p>
    <asp:Button ID="Refresh" runat="server" onclick="Refresh_Click" Text="Refresh" />
    </p>    
  </form>
</body>
</html>


