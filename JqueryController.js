

$(document).ready(function () {
    //initialize all the event for the trigger control
    var panelToRefresh;
    var onServerAction;
    var onServerValidate;
    var validationGroup;
    var jqueryPostEvent;
    var sender;
    var progress;
    jQuery('[JQueryPostEvent]').each(function (index) {
        panelToRefresh = $(this).attr('TriggedPanel');
        onServerAction = $(this).attr('OnServerAction');
        onServerValidate = $(this).attr('OnServerValidate');
        JQueryValidationGroup = $(this).attr('JQueryValidationGroup');
        progress = $(this).attr('JqueryProgress');
        sender = $(this).attr('id');
        jqueryPostEvent = $(this).attr('JQueryPostEvent');
        $(this).bind(jqueryPostEvent, function () {
            // alert('');
            JqueryPost(window.location.pathname, panelToRefresh, sender, JQueryValidationGroup, onServerValidate, onServerAction, JQueryValidationGroup,progress);
        });
    });
});



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

function JqueryPost(pageName, sender, trigger, jqueryProgress, onServerValidate,OnServerAction,jqueryValidation,jqueryProgress) {
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
            __jsonAttribute: JsonAttribute,
            __OnServerValidate: onServerValidate,
            __actionDelegate: OnServerAction,
            __JQueryValidationGroup:jqueryValidation,
            __jqueryProgress: jqueryProgress
        },
        success: function (data) {
            if (ShowProgress) {
                HideProgressPanel(jqueryProgress)
            }
            jQuery.globalEval(data);
        }
    });
}

      