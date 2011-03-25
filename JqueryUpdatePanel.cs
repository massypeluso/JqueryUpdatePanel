using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JqueryWebControls
{
    [ToolboxBitmap(typeof(JqueryUpdatePanel), "IC170241.gif")]
    public class JqueryUpdatePanel : Panel
    {
        private string TriggerId { get; set; }

        private readonly List<string> _clientCallBackParameters = new List<string>();

        public delegate bool OnServerValidateDelegate(Dictionary<string, string> arguments);

        public OnServerValidateDelegate OnServerValidateAction { get; set; }

        private bool _isValid = true;

        private bool _autoReferenceJquery = false;

        private Dictionary<string, string> _postBackArguments = new Dictionary<string, string>();

        private bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }

        private readonly List<TriggerControlItem> _triggerControls = new List<TriggerControlItem>();
        /// <summary>
        /// Progress panel that will be showed during the async post back. It can be used to show a "loading" image to notify the user the post back is happening
        /// 
        /// </summary>
        public string JqueryUpdateProgress { get; set; }


        /// <summary>
        /// Client function that will be called at the end of the async post back. The function takes arguments
        /// </summary>
        public string ClientOnCallBack { get; set; }

        public string ClientOnServerValidate { get; set; }

        //public string ValidationGroup { get; set; }

        private bool _isJqueryPost;
        /// <summary>
        /// Paramenter that will be passed to the ClientOnCallBack function as arguments
        /// </summary>
        public List<string> ClientCallBackParameters
        {
            get { return _clientCallBackParameters; }
        }
        /// <summary>
        /// Collection of TriggerControlItem.
        /// </summary>
        public List<TriggerControlItem> TriggerControls
        {
            get { return _triggerControls; }
        }

        public bool IsJQueryPost
        {
            get
            {
                return !string.IsNullOrEmpty(HttpContext.Current.Request.Form["__jqueryPost"]);
            }
        }

        public bool AutoReferenceJquery
        {
            get { return _autoReferenceJquery; }
            set { _autoReferenceJquery = value; }
        }


        public JqueryUpdatePanel()
        {
            _isJqueryPost = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string sender = HttpContext.Current.Request.Form["__sender"];
            if (sender == ClientID)
            {
                _isJqueryPost = true;
                PopulateFormPostArguments();
                PopulateJsonAttribute();
                InvokeOnServerValidate();
                InvokeServerActionDelegate();
            }
            AddClientEventHandler();


        }

        private void PopulateFormPostArguments()
        {
            _postBackArguments.Clear();
            var jsHiddenValues = HttpContext.Current.Request.Form["__arguments"];
            if (!string.IsNullOrEmpty(jsHiddenValues))
            {
                var values = jsHiddenValues.Split('&');
                foreach (var value in values)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        var name = value.Split('=')[0];
                        var Value = value.Split('=')[1];
                        foreach (var control in TriggerControls)
                        {
                            if (name != "__VIEWSTATE" && name != "__EVENTVALIDATION")
                                control.PostBackArguments.Add(name, Value);
                            _postBackArguments.Add(name, Value);
                        }
                    }
                }
            }
        }

        public string GetPostParametrByName(string name)
        {
            return PostArgumentHelper.GetValueByName(_postBackArguments, name);
        }

        private void PopulateJsonAttribute()
        {
            //
            var jsonAttribute = HttpContext.Current.Request.Form["__jsonAttribute"];
            if (!string.IsNullOrEmpty(jsonAttribute))
            {
                foreach (var control in TriggerControls)
                {
                    control.PostBackArguments.Add("JsonAttribute", jsonAttribute);
                    _postBackArguments.Add("JsonAttribute", jsonAttribute);
                }
            }
        }

        private string GetJqueryUpdateProgress()
        {
            return JqueryUpdateProgress ?? string.Empty;
        }

        private void AddClientEventHandler()
        {
            if (_triggerControls == null) return;
            foreach (var triggerControl in _triggerControls)
            {
                var jsToAdd = "JqueryPost('" + GetPageName() +
                              "','" + ClientID +
                              "','" + triggerControl.TriggerWebControl.ClientID +
                              "','" + GetJqueryUpdateProgress() +
                              "');return false";
                AddEventHanlder(triggerControl, jsToAdd);
            }
        }

        public void JqueryDataBind()
        {
            AddClientEventHandler();
        }

        private static void AddEventHanlder(TriggerControlItem triggerControl, string jqueryPostHandler)
        {
            string triggerEvent = triggerControl.TriggerWebControlEvent.ToString();
            try
            {
                var previosHandler = string.Empty;
                if (triggerControl.TriggerWebControl.Attributes[triggerEvent] != null)
                {
                    previosHandler = triggerControl.TriggerWebControl.Attributes[triggerEvent];
                    triggerControl.TriggerWebControl.Attributes.Remove(triggerEvent);
                }
                jqueryPostHandler = previosHandler + jqueryPostHandler;
                triggerControl.TriggerWebControl.Attributes.Add(triggerEvent, jqueryPostHandler);
            }
            catch (System.ArgumentException)
            {
                throw new ArgumentException("TriggerWebControlEvent cannot be null");
            }

        }

        private void InvokeOnServerValidate()
        {
            if (OnServerValidateAction != null)
            {
                IsValid = OnServerValidateAction.Invoke(_postBackArguments);
            }
        }

        private void InvokeServerActionDelegate()
        {
            var hasBeenInvoked = false;
            if (_isJqueryPost == false || IsValid == false) return;
            var control = GetTriggerControlRaised();
            {
                if (hasBeenInvoked == false && control.PostBackAction != null)
                {
                    var postaction = (TriggerControlItem.ActionDelegate)control.PostBackAction.Clone();
                    postaction.Invoke(control.PostBackArguments);
                    hasBeenInvoked = true;
                }
            }
        }

        private TriggerControlItem GetTriggerControlRaised()
        {
            var sender = HttpContext.Current.Request.Form["__trigger"];
            var controlToReturn =
                TriggerControls.ToList().Where(x => x.TriggerWebControl.ClientID == sender).SingleOrDefault();
            return controlToReturn;
        }

        private string GetValidationGroupToShow()
        {
            var control = GetTriggerControlRaised();
            return control != null ? control.TriggerWebControl.Attributes["JQueryValidationGroup"] : string.Empty;
        }


        private string GetPageName()
        {
            return Path.GetFileName(Page.AppRelativeVirtualPath);
        }

        private string GetCallBackFunction()
        {
            string strToRetun = ClientOnCallBack + "(";
            if (string.IsNullOrEmpty(ClientOnCallBack)) return string.Empty;
            foreach (var item in _postBackArguments)
            {
                strToRetun += "'" + item + "',";
            }
            strToRetun = strToRetun.Remove(strToRetun.Length - 1, 1);
            strToRetun += ");";
            return strToRetun;
        }

        protected override void OnPreRender(EventArgs e)
        {

            base.OnPreRender(e);
            AddClientResource();
        }

        private void RewriteRender()
        {
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            var hw = new HtmlTextWriter(tw);
            string data;
            RenderContents(hw);
            var htmlToAdd = sb.ToString();
            htmlToAdd = htmlToAdd.Replace("\\", "\\\\");
            htmlToAdd = htmlToAdd.Replace(Environment.NewLine, string.Empty);
            htmlToAdd = htmlToAdd.Replace(@"""", "\\\"");
            if (IsValid)
            {
                data = @"$(""#" + ClientID + @""").html(""" + htmlToAdd + @""");HideValidationMessage('" + GetValidationGroupToShow() + "');" + GetCallBackFunction();
                //RefreshPanel(ClientID,htmlToAdd,ValidationGroup,callBackFunction,callBackFunctionParameters){
                // data = "RefreshPanel('" + ClientID + "','" + htmlToAdd + "','" + ValidationGroup + "');";
            }
            else
            {
                data = "ShowValidationMessage('" + GetValidationGroupToShow() + "')";
            }
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(data);
        }

        private void AddClientResource()
        {
            const string resourceName = "JqueryWebControls.JqueryController.js";
            var cs = Page.ClientScript;
            cs.RegisterClientScriptResource(typeof(JqueryUpdatePanel), resourceName);
            if (!AutoReferenceJquery) return;
            cs.RegisterClientScriptInclude("JqueryInclude", "http://ajax.googleapis.com/ajax/libs/jquery/1.2.6/jquery.min.js");
        }


        protected override void Render(HtmlTextWriter writer)
        {
            if (_isJqueryPost)
            {
                //foreach (var control in from control in TriggerControls.ToList()
                //                        let sender = HttpContext.Current.Request.Form["__trigger"]
                //                        where sender.Contains(control.TriggerWebControl.ID)
                //                        select control)

                RewriteRender();
                HttpContext.Current.Response.End();
            }
            base.Render(writer);
        }
    }
}
