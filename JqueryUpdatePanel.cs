using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JqueryWebControls
{
    [ToolboxBitmap(typeof(JqueryUpdatePanel), "IC170241.gif")]
    public class JqueryUpdatePanel : Panel
    {
        public delegate bool OnServerValidateDelegate(IList<ArgumentItem> arguments);

        public delegate void ActionDelegate(IList<ArgumentItem> arguments);



        private string ActionDelegateName
        {
            get
            {
                return HttpContext.Current.Request.Form["__actionDelegate"] ?? string.Empty;
            }
        }

        private string OnServerValidate
        {
            get
            {
                return HttpContext.Current.Request.Form["__OnServerValidate"] ?? string.Empty;
            }
        }

        private readonly List<string> _clientCallBackParameters = new List<string>();




        private bool _isValid = true;

        private bool _autoReferenceJquery = false;

        private IList<ArgumentItem> _postBackArguments = new List<ArgumentItem>();

        private bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }


        /// <summary>
        /// Progress panel that will be showed during the async post back. It can be used to show a "loading" image to notify the user the post back is happening
        /// 
        /// </summary>
        public string JqueryUpdateProgress { 
            get
            {
                return HttpContext.Current.Request["__jqueryProgress"];
            }
        }


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
            if (!(string.IsNullOrEmpty(sender)) && ClientID.EndsWith(sender, true, null))
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
                        if (name != "__VIEWSTATE" && name != "__EVENTVALIDATION")
                            _postBackArguments.Add(new ArgumentItem(name, Value));
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
            var jsonAttribute = HttpContext.Current.Request.Form["__jsonAttribute"];
            if (!string.IsNullOrEmpty(jsonAttribute))
            {
                _postBackArguments.Add(new ArgumentItem("JsonAttribute", jsonAttribute));
            }
        }

        private string GetJqueryUpdateProgress()
        {
            return JqueryUpdateProgress ?? string.Empty;
        }

        private void AddClientEventHandler()
        {
            //if (_triggerControls == null) return;
            //foreach (var triggerControl in _triggerControls)
            //{
            //    var jsToAdd = "JqueryPost('" + GetPageName() +
            //                  "','" + ClientID +
            //                  "','" + triggerControl.TriggerWebControl.ClientID +
            //                  "','" + GetJqueryUpdateProgress() +
            //                  "');return false";
            //    AddEventHanlder(triggerControl, jsToAdd);
            //}
        }

        public void JqueryDataBind()
        {
            AddClientEventHandler();
        }

        //private static void AddEventHanlder(TriggerControlItem triggerControl, string jqueryPostHandler)
        //{
        //    //string triggerEvent = triggerControl.TriggerWebControlEvent.ToString();
        //    //try
        //    //{
        //    //    var previosHandler = string.Empty;
        //    //    if (triggerControl.TriggerWebControl.Attributes[triggerEvent] != null)
        //    //    {
        //    //        previosHandler = triggerControl.TriggerWebControl.Attributes[triggerEvent];
        //    //        triggerControl.TriggerWebControl.Attributes.Remove(triggerEvent);
        //    //    }
        //    //    jqueryPostHandler = previosHandler + jqueryPostHandler;
        //    //    triggerControl.TriggerWebControl.Attributes.Add(triggerEvent, jqueryPostHandler);
        //    //}
        //    //catch (System.ArgumentException)
        //    //{
        //    //    throw new ArgumentException("TriggerWebControlEvent cannot be null");
        //    //}

        //}

        private void InvokeOnServerValidate()
        {
            if (string.IsNullOrEmpty(OnServerValidate)) return;
            var delegateToInvoke = (OnServerValidateDelegate)Delegate.CreateDelegate(
                typeof(OnServerValidateDelegate), Page, OnServerValidate, true);
            IsValid = delegateToInvoke.Invoke(_postBackArguments);
        }

        private void InvokeServerActionDelegate()
        {
            if (string.IsNullOrEmpty(ActionDelegateName)) return;
            var delegateToInvoke = (ActionDelegate)Delegate.CreateDelegate(
                               typeof(OnServerValidateDelegate), Page, ActionDelegateName, true);
            delegateToInvoke.Invoke(_postBackArguments);
        }

        //private TriggerControlItem GetTriggerControlRaised()
        //{
        //    var sender = HttpContext.Current.Request.Form["__trigger"];
        //    var controlToReturn =
        //        TriggerControls.ToList().Where(x => x.TriggerWebControl.ClientID == sender).SingleOrDefault();
        //    return controlToReturn;
        //}

        private string GetValidationGroupToShow()
        {
            return HttpContext.Current.Request.Form["__JQueryValidationGroup"];
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
            string data = string.Empty;
            RenderContents(hw);
            var htmlToAdd = sb.ToString();
            htmlToAdd = htmlToAdd.Replace("\\", "\\\\");
            htmlToAdd = htmlToAdd.Replace(Environment.NewLine, string.Empty);
            htmlToAdd = htmlToAdd.Replace(@"""", "\\\"");

            if (IsValid)
            {
                if (htmlToAdd != string.Empty)
                {
                    data = @"$(""#" + ClientID + @""").html(""" + htmlToAdd + @""");HideValidationMessage('" + GetValidationGroupToShow() + "');" + GetCallBackFunction();
                }
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
            if (!AutoReferenceJquery) return;
            cs.RegisterClientScriptInclude("JqueryInclude", "http://ajax.googleapis.com/ajax/libs/jquery/1.2.6/jquery.min.js");
            cs.RegisterClientScriptResource(typeof(JqueryUpdatePanel), resourceName);

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
