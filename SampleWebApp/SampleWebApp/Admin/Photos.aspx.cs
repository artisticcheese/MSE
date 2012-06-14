using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.OleDb;
using System.IO;

public partial class Admin_Photos_aspx : System.Web.UI.Page {

	protected void FormView1_ItemInserting(object sender, FormViewInsertEventArgs e) {
		if (((Byte[])e.Values["BytesOriginal"]).Length == 0) e.Cancel = true;
	}

	protected void Button1_Click(object sender, ImageClickEventArgs e) {
		DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/Upload"));
		foreach (FileInfo f in d.GetFiles("*.jpg")) {
			byte[] buffer = new byte[f.OpenRead().Length];
			f.OpenRead().Read(buffer, 0, (int)f.OpenRead().Length);
			PhotoManager.AddPhoto(Convert.ToInt32(Request.QueryString["AlbumID"]), f.Name, buffer);
		}
		GridView1.DataBind();
	}

}