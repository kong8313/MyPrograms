var OperationStatus =
    {
        NotStarted: 0,
        InProgress: 1,
        Completed: 2,
        PartiallyCompleted: 3,
        Failed: 4
    };

    AsyncOperationProgress = function () {

        var self = this;
        var currentState = OperationStatus.NotStarted;
        var minimumOperationDuration = 3; //seconds

        return {
            init: function (operationId, refreshSeconds, lblTextClientId, lblStartTimeClientId, lblEndTimeClientId, lblStatusClientId, cbCloseOnFinish) {
                self.operationId = operationId;
                self.refreshSeconds = refreshSeconds;
                self.lblTextClientId = lblTextClientId;
                self.lblStartTimeClientId = lblStartTimeClientId;
                self.lblEndTimeClientId = lblEndTimeClientId;
                self.lblStatusClientId = lblStatusClientId;
                self.cbCloseOnFinishClienId = cbCloseOnFinish;
                self.operationStartTime = new Date().getTime();
                setTimeout(function () { AsyncOperationProgress.getProgress(); }, 500);
            },

            getProgress: function () {
                if (self.operationId > 0) {
                    PageMethods.GetOperationProgress(self.operationId, onSuccess, onFail);
                }
            },

            setProgress: function (operationProgress) {
                onSuccess(operationProgress);
            },

            getCurrentState: function () { return currentState; }
        };

        function onSuccess(operationProgress) {
            
            currentState = operationProgress.Status;
            
            var startDateElement = document.getElementById(self.lblStartTimeClientId);
            if (operationProgress.StartTime && startDateElement.innerHTML == "-") {
                setDateTimeValue(self.lblStartTimeClientId, operationProgress.StartTime);
            }

            var percent = operationProgress.PercentageComplete;
            if (percent == null) { percent = 0; }            
            
            if (isOperationCompleted(operationProgress.Status)) //finished - force to 100%
            {
                percent = 100;
                if (operationProgress.EndTime != null) {
                    setDateTimeValue(self.lblEndTimeClientId, operationProgress.EndTime);
                }

                if (operationProgress.Status == OperationStatus.Completed) {

                    var operationDuration = getOperationDuration();
                    var dialogDisplayDelay = (operationDuration < minimumOperationDuration) ? (minimumOperationDuration - operationDuration) : 0;
                    
                    setTimeout(function () {
                        
                        var cbCloseOnFinishElement = document.getElementById(self.cbCloseOnFinishClienId);
                        
                        if (cbCloseOnFinishElement && cbCloseOnFinishElement.checked) {
                            top.overlay.closeLast(true);
                        }
                        
                    }, dialogDisplayDelay * 1000);
                }
                else if (operationProgress.Status == OperationStatus.PartiallyCompleted ||
                         operationProgress.Status == OperationStatus.Failed) {
                     //Since we show the whole progress log it's apprpriate to show it in red
                     //document.getElementById(lblTextClientId).style.color = "#FF0000";
                }
            }
            else {
                setTimeout(function () { AsyncOperationProgress.getProgress(); }, self.refreshSeconds * 1000);
            }

            document.getElementById('progresscell').style.width = percent + '%';
            document.getElementById('bgcell').style.width = (100 - percent) + '%';
            document.getElementById(lblTextClientId).innerHTML = operationProgress.Text;
            document.getElementById(lblStatusClientId).innerHTML = operationProgress.StatusDescription;
            document.getElementById('divProgressBar').style.display = 'block';
        }

        function onFail(result) {
            currentState = OperationStatus.Failed;
            alert('An error ocurred while retrieving operation progress data.');
            self.refreshSeconds = 0;
        }

        function isOperationCompleted(status) {

            return status == OperationStatus.Completed ||
               status == OperationStatus.PartiallyCompleted ||
               status == OperationStatus.Failed;
        }

        function setDateTimeValue(elementId, dateTimeString) {
            var dateFormat = "d"; //for some reason, the format "g", which should output date + time, doesn't work as advertised...
            var timeFormat = "T";
            var dt = new Date(dateTimeString);
            dt = getDateTimeWithTimezoneOffset(dt);
            var element = document.getElementById(elementId);
            element.innerHTML = dt.localeFormat(dateFormat) + " " + dt.localeFormat(timeFormat);
        }

        //Calculates duration of operation (how long operation dialog has been shown) in seconds
        function getOperationDuration() {
            var elapsed = (new Date().getTime() - self.operationStartTime) / 1000;
            return elapsed;
        }

        function getDateTimeWithTimezoneOffset(dateTime) {
            //getTimezoneOffset() method returns the time-zone offset in minutes and has negative value
            var utcTime = dateTime.getTime() + dateTime.getTimezoneOffset() * 60000;
            return new Date(utcTime);
        }
    } ();
