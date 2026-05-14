var stateTemplateBinder = new function () {
    var controlId;
    this.Init = function (controlClientId) {
        controlId = controlClientId;
    };

    this.getValue = function (columnKey) {
        if (columnKey == "Id") {
            return $get(controlId + '_tbxID').value;
        }
        else if (columnKey == "Name") {
            return $get(controlId + '_tbxName').value;
        }
        else if (columnKey == "Priority") {
            return $get(controlId + '_tbxPriority').value;
        }
        else if (columnKey == "DisallowActivation") {
            return $get(controlId + '_cbDA').checked;
        }

        return "";
    };

    this.setValue = function (columnKey, value) {
        if (columnKey == "Id") {
            $get(controlId + '_tbxID').value = value;
            var nameControl = $get(controlId + '_tbxName');
            if (value <= 30) {
                nameControl.disabled = "disabled";
            }
            else {
                nameControl.disabled = "";
            }
        }
        else if (columnKey == "Name") {
            $get(controlId + '_tbxName').value = value;
        }
        else if (columnKey == "Priority") {
            $get(controlId + '_tbxPriority').value = value;
        }
        else if (columnKey == "DisallowActivation") {
            $get(controlId + '_cbDA').checked = value;
        }
    };
};