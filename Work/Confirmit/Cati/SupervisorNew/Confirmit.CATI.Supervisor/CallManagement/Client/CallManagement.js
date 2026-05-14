// function for show Filter Add dialog. If new filter is created in dialog - we store return value in m_FilterId hidden field.
function showFilterAddDialog(title, surveyId, m_FilterId, filterId) {
    var settings = { height: "600px", width: "800px", top:"100px" };

    top.overlay.overlayClosedEvent.on(function (args) {
        if (args.result !== true)
            return;

        var returnValue = args.data;
        if (returnValue) {
            $get(m_FilterId).value = returnValue;
        }

        Common.updatePanel('');
    });

    var filterIdRequest = "";
    if (filterId > 0) {
        filterIdRequest = "&fltID=" + filterId;
    }

    top.overlay.show(title, "Filter/FilterAdd.aspx?ID=" + surveyId + filterIdRequest +"&Mode=Modal", null, settings, null);

    return top.overlay;
}

function getSelectedIds(grid_id) {
  var hSelected = document.getElementById(grid_id + "_hSelected");
  var s = hSelected.value;

  if (s == null || s == "") 
  {
      var activeRow = $find(grid_id + "_dataGrid").get_behaviors().get_selection().get_selectedRows().getItem(0);
      
	  return activeRow.get_cellByColumnKey('InterviewID').get_value();
  }

  var sArray = s.split(",");
  
  for (var i = 0; i < sArray.length; i++)
  {
    var str = sArray[i];
    var id = str.substr(0, str.lastIndexOf('_'));
  
    if(i == 0)
       s = id;
    else
       s = s + "," + id;
  }
  return s;
}

function processSelectedCallsUsingOverlay(selectionType, grid_id, surveyId, callState, actionName, url, addParam, title, width, height, topPosition) {
    
    var s = getSelectedIds(grid_id);
    
    if (s == null) {
        alert('No rows selected');
        return null;
    }
    
    var settings = {
        height: height,
        width: width,
        calledWindow: window
    };

    if (topPosition) {
        settings.top = topPosition;
    };

    var params = {
        CallSelectionType: selectionType,
        IDS: s,
        SurveyID: surveyId,
        CallState: callState
    };

    if (addParam && addParam != '') {
        var arr = addParam.split("&");
        for (var i = 0; i < arr.length; i++) {
            var p = arr[i].split("=");
            params[p[0]] = p[1];
        }
    }

    top.overlay.show(title, url, params, settings, null);
    
    return top.overlay;
}

function processFilteredCallsUsingOverlay(selectionType, grid_id, surveyId, callState, actionName, url, addParam, title, filterId, confirmation, width, height, topPosition) {

    if (!confirm(confirmation)) {
        return null;
    }

    var entireListItemsCount = document.getElementById(grid_id + "_hTotalCount").value;       

    var settings = {
        width: width,
        height: height,
        calledWindow: window
    };

    if (topPosition) {
        settings.top = topPosition;
    };

    var params = {
        CallSelectionType: selectionType,
        FilterID: filterId,
        SurveyID: surveyId,
        CallState: callState,
        EntireListItemsCount: entireListItemsCount
    };
       
    if (addParam && addParam != '') {
        var arr = addParam.split("&");
        for (var i = 0; i < arr.length; i++) {
            var p = arr[i].split("=");
            params[p[0]] = p[1];            
        }
    }

    top.overlay.show(title, url, params, settings, null);

    return top.overlay;
}

function processDeleteSelectedCalls(grid_id, confirmation, alertMessage)
{
    if (getSelectedIds(grid_id) == null)
    {
        alert(alertMessage);
        return false;
    }
    
    if(!confirm(confirmation))
    {
        return false;
    } 
    
    return true;
}

function processExport(title, instanceId, resultId) {

    var settings = { height: "330px", width: "550px", top: "100px" };

    top.overlay.overlayClosedEvent.on(function (args) {
        if (args.result !== true)
            return;

        var returnValue = args.data;
        if (returnValue) {
            $get(resultId).value = returnValue;
        }
    });

    top.overlay.show(title, "CallManagement/Export.aspx?CallListInstanceId=" + instanceId, null, settings, null);

    return top.overlay;
}

function showAsyncOperationDialog(title, operationId, closeFunction) {
    
    var url = "AsyncOperations/AsyncOperationProgress.aspx";
    
    var settings = {
        width: 520,
        height: 400,
        top: 120
    };

    var params = {
        OperationId: operationId,
        OperationTitle: title,
        DialogTitle: title        
    };
    
    top.overlay.show(title, url, params, settings, closeFunction);    
}