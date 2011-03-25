using System;
using System.Web.UI.WebControls;

namespace JqueryWebControls
{
    public class JqueryPanelValidationMessage : Panel
    {
        public string Message { get; set; }
        public string Text { get; set; }
        public string JQueryValidationGroup { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.Attributes.Add("JqueryUpdatePanelValidationGroup", JQueryValidationGroup);
            this.Attributes.Add("style", "display:none;");
        }
    }
}
