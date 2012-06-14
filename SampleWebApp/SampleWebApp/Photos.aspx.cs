using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Photos_aspx : System.Web.UI.Page {

    protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e) {
        if (e.Item.ItemType == ListItemType.Footer) {
            if (DataList1.Items.Count == 0) Panel1.Visible = true;
        }
    }

}