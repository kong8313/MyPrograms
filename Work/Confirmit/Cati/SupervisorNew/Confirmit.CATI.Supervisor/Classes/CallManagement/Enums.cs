namespace Confirmit.CATI.Supervisor.Classes.CallManagement
{
    public enum ColumnTypeField
    {
        Call,
        Interview,
        Appointment,
        ConfirmitVariable
    }

    /// <summary>
    /// Type of call selection
    /// </summary>
    public enum CallSelectionType
    {
        Selected = 0,
        Filtered = 1,
        QuotaCellFiltered = 2,
    }

    public enum CallMoveType
    {
        Move = 0,
        MoveAndReschedule = 1
    }

    public enum ShowTimeMode
    {
        Interviewer = 0,
        Respondent = 1
    }
}
