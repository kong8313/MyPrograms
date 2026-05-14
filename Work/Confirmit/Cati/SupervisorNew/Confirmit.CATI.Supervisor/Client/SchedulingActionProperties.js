function SchedulingActionProperties() {
}

SchedulingActionProperties.EnterParameterValue = "Enter parameter value.";
SchedulingActionProperties.IncorrectParameterFormat = "Incorrect parameter format.";

SchedulingActionProperties.validate = function (ddlActionID, rbConst, tbConst, ddlSchedulingParams, hdnParamValue, hdnIsSchedulingParam) {

    var ddlAction = document.getElementById(ddlActionID);    
    var rbConstChecked = document.getElementById(rbConst).checked;
    var tbConstParameter = document.getElementById(tbConst);
    var tbSchedulingPatameter = document.getElementById(ddlSchedulingParams);
    var hdnParamControl = document.getElementById(hdnParamValue);
    var hdnIsSchedulingParamControl = document.getElementById(hdnIsSchedulingParam);
    var parameterValue;
    var checkedConrol;
    if (rbConstChecked == true) {
        parameterValue = tbConstParameter.value.trim();
        checkedConrol = tbConstParameter;
    }
    else {
        parameterValue = tbSchedulingPatameter.value;
        checkedConrol = tbSchedulingPatameter;
    }
    hdnParamControl.value = parameterValue;
    hdnIsSchedulingParamControl.value = (!rbConstChecked).toString();

    var actionId = ddlAction.value;
    var hasParameter = ddlAction.options[ddlAction.selectedIndex].getAttribute("hasParameter");

    if (hasParameter.toLowerCase() == "true") {
        if (parameterValue == "")  /* there is no parameter specified*/
        {
            alert(SchedulingActionProperties.EnterParameterValue);
            checkedConrol.focus();
            return false;
        }
        else  /* there is parameter*/
        {
            if (rbConstChecked == true) {
                if (SchedulingActionProperties.validateAction(actionId, parameterValue) == false) /* wrong parameter format*/
                {
                    alert(SchedulingActionProperties.IncorrectParameterFormat);
                    return false;
                }
            }
        }
    }

    return true;

};

SchedulingActionProperties.radioButtonClicked = function (rbConstID, rbParamID, tbConstID, ddlSchedulingParamsID) {
    
    var rbConst = document.getElementById(rbConstID);
    var rbParam = document.getElementById(rbParamID);
    var tbConst = document.getElementById(tbConstID);
    var ddlSchedulingParams = document.getElementById(ddlSchedulingParamsID);
    if (rbConst.checked) {
        tbConst.disabled = false;
        ddlSchedulingParams.disabled = true;
    }
    else {
        tbConst.disabled = true;
        ddlSchedulingParams.disabled = false;
    }
};

SchedulingActionProperties.onActionChange = function (ddlAction, textAreaDescription, rblParameters, rbParameter, rbConst, tbConst, ddlSchedulingParams) {
    
    SchedulingActionProperties.onActionSet(ddlAction, textAreaDescription, rblParameters, rbParameter, rbConst, tbConst, ddlSchedulingParams);

    var ddlAction = document.getElementById(ddlAction);
    var areaDescription = document.getElementById(textAreaDescription);
    var areaParameters = document.getElementById(rblParameters);
    var areaConstParameter = document.getElementById(tbConst);
    var areaSchedulingParameter = document.getElementById(ddlSchedulingParams);
    var parameterType = ddlAction.options[ddlAction.selectedIndex].getAttribute("parameterType");
    var rbParameterControl = document.getElementById(rbParameter);
    var rbConstControl = document.getElementById(rbConst);

    if (parameterType != "") /* custom parameters are supported*/
    {
        rbParameterControl.disabled = false;
        rbConstControl.disabled = false;
        areaSchedulingParameter.disabled = true;
        areaConstParameter.disabled = false;
        areaSchedulingParameter.selectedIndex = -1;
        rbConstControl.checked = true;
        rbParameterControl.checked = false;
    }
};

SchedulingActionProperties.onActionSet = function (ddlAction, textAreaDescription, rblParameters, rbParameter, rbConst, tbConst, ddlSchedulingParams) {
    var ddlAction = document.getElementById(ddlAction);
    var areaDescription = document.getElementById(textAreaDescription);
    var areaParameters = document.getElementById(rblParameters);
    var areaConstParameter = document.getElementById(tbConst);
    var areaSchedulingParameter = document.getElementById(ddlSchedulingParams);

    if (ddlAction != null && areaDescription != null && areaParameters != null) {
        var description = ddlAction.options[ddlAction.selectedIndex].getAttribute("description");
        var hasParameter = ddlAction.options[ddlAction.selectedIndex].getAttribute("hasParameter");
        
        areaDescription.value = description;

        if (hasParameter.toLowerCase() == "false") /* action has no parameter value*/
        {                        
            areaConstParameter.Value = "";
            areaSchedulingParameter.value = 0;
            Y.one(areaParameters).all("input, select").set("disabled", true);
        }
        else  /* action has parameter value*/
        {
            var rbParameterControl = document.getElementById(rbParameter);
            var rbConstControl = document.getElementById(rbConst);
            var parameterType = ddlAction.options[ddlAction.selectedIndex].getAttribute("parameterType");
            
            Y.one(areaParameters).all("input, select").set("disabled", false);
            
            if (parameterType == "") /* custom parameters are not supported*/
            {
                rbParameterControl.disabled = true;
                areaSchedulingParameter.disabled = true;
                rbConstControl.disabled = false;
                rbConstControl.checked = true;
                areaConstParameter.disabled = false;
                areaSchedulingParameter.selectedIndex = -1;
            }
            else {
                rbParameterControl.disabled = false;
            }
        }
    }

    SchedulingActionProperties.showOnlyValidParams(parameterType, ddlSchedulingParams);
};

SchedulingActionProperties.onActionParameterChange = function (hdnisschedulingparameter, hdnparamvalue, tbConst, ddlSchedulingParams, rbConstID, rbParamID) {
    
    var hdnisparam = document.getElementById(hdnisschedulingparameter);
    var hdnvalue = document.getElementById(hdnparamvalue);
    var areaConstParameter = document.getElementById(tbConst);
    var areaSchedulingParameter = document.getElementById(ddlSchedulingParams);
    var rbConst = document.getElementById(rbConstID);
    var rbParam = document.getElementById(rbParamID);
    if (hdnisparam.value.toLowerCase() == "true") {
        areaConstParameter.value = "";
        areaSchedulingParameter.value = hdnvalue.value;
        rbConst.checked = false;
        rbParam.checked = true;
        areaConstParameter.disabled = true;
        areaSchedulingParameter.disabled = false;
    }
    else {
        areaConstParameter.value = hdnvalue.value;
        areaSchedulingParameter.value = 0;
        rbConst.checked = true;
        rbParam.checked = false;
        areaConstParameter.disabled = rbConst.disabled;
        areaSchedulingParameter.disabled = true;
    }
};
