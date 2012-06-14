using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Download_aspx : System.Web.UI.Page {

    void Page_Load(object sender, EventArgs e) {
        if (!IsPostBack) {
            int i = Convert.ToInt32(Request.QueryString["Page"]);
            if (i >= 0) FormView1.PageIndex = i;
        }
    }

}