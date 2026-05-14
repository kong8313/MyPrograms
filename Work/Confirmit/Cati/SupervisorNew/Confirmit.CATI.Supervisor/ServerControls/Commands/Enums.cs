namespace Confirmit.CATI.Supervisor.ServerControls.Commands
{
    /// <summary>
    /// Specifies dialog type
    /// </summary>
    public enum DialogMode
    {
        Create = 0, //create new object, no check for selected rows
        ViewEdit = 1, //view/edit information about selected object, check for selected rows
    };

    /// <summary>
    /// Defines number of rows needs to be selected in grid to make the command work properly
    /// </summary>
    public enum CommandGridSelectMode
    {
        No,
        SingleRow,
        MultiRow
    }

    public enum DialogWindowMode
    {
        Frame = 0, //Show window in frame
        Modal = 1, //Show window in modal window
        Floating = 2, //Equal Modal
    };
}