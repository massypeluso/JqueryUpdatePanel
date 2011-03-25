using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JqueryWebControls
{
    public class TriggerControlItem
    {
        private bool _isValid;
        /// <summary>
        /// Delegate that is called by the JqueryUpdatePanle before render back the response
        /// </summary>
        /// <param name="arguments">All the hidden field hooked up to this control</param>
      


        public string CommandName { get; set; }

        /// <summary>
        /// Control that raise the async post back(Es:asp button)
        /// </summary>
        public WebControl TriggerWebControl { get; set; }
        /// <summary>
        /// The JS event that has to raise the post back(Es: onclick)
        /// </summary>
        public JsEventEnum TriggerWebControlEvent { get; set; }

        /// <summary>
        /// Action needs to be execute on the server side
        /// </summary>
        public ActionDelegate PostBackAction { get; set; }

        public string ValidationGroup { get; set; }
      

        private Dictionary<string, string> _postBackArguments = new Dictionary<string, string>();

        /// <summary>
        /// The list of the hidden fields that will be posted.
        /// </summary>
        public Dictionary<string, string> PostBackArguments
        {
            get { return _postBackArguments; }
            set { _postBackArguments = value; }
        }
    }

}
