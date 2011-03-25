

function HideProgressPanel(obj) {
    $('[id$=' + obj + ']').hide();
}

function ShowProgressPanel(obj) {
    $('[id$=' + obj + ']').show();
}

function ShowValidationMessage(validationGroup) {
    $("div[JqueryUpdatePanelValidationGroup='" + validationGroup + "']").show()
}


 function HideValidationMessage(validationGroup) {
     $("div[JqueryUpdatePanelValidationGroup='" + validationGroup + "']").hide()
 }

function JqueryPost(pageName, sender, trigger, jqueryProgress) {
    var JsonAttribute = $('#' + trigger).attr("JqueryPostArgument")
    var ShowProgress=jqueryProgress != ''; 
    if(ShowProgress){
        ShowProgressPanel(jqueryProgress)
    }
    
    $.ajax({
        type: 'POST',
        cache: false,
        url: pageName,
        data: { __pagename: pageName,
            __sender: sender,
            __trigger: trigger,
            __jqueryPost: 'true',
            __arguments: $("form:first").serialize(),
            __jsonAttribute: JsonAttribute
        },
        success: function (data) {
            if (ShowProgress) {
                HideProgressPanel(jqueryProgress)
            }
            jQuery.globalEval(data);
        }
    });
}

      