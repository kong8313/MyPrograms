function StateTemplate() {}

StateTemplate.emptyNameErrorMessage = 'Name cannot be empty';
StateTemplate.emptyPriorityErrorMessage = 'Priority cannot be empty';

StateTemplate.okClick = function (nameControlId, priorityControlId) {
    if (Common.validateRequiredValue(nameControlId, StateTemplate.emptyNameErrorMessage) == false) { return; }
    if (Common.validateRequiredValue(priorityControlId, StateTemplate.emptyPriorityErrorMessage) == false) { return; }

    gridTemplate.closeTemplate(true);
}