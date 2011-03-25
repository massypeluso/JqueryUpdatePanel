using System.Drawing;
using System.Web.UI.WebControls;

namespace JqueryWebControls
{
    [ToolboxBitmap(typeof(JqueryUpdatePanel), "ico_ajax_loader.gif")]
    public class JqueryUpdateProgress : Panel
    {
        protected override void OnPreRender(System.EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(
                this.GetType(), "HideProf",
                "HideProgressPanel('" + ClientID + "');", true);
        }
    }
}
