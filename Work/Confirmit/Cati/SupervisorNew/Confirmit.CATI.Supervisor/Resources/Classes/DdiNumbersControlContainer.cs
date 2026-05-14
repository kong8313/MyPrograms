using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.Resources.Classes
{
    public class DdiNumbersControlsContainer
    {
        public TextBox UrlTextBox { get; private set; }
        public TextBox DefaultUrlTextBox { get; private set; }
        public DropDownList PlayBehaviorDropDownList { get; private set; }
        public Label DefaultRepeatCountTextBox { get; private set; }
        public Label LabelAsterisk { get; private set; }

        public DdiNumbersControlsContainer(TextBox urlTextBox, TextBox defaultUrlTextBox, DropDownList playBehaviorDropDownList, Label defaultRepeatCountTextBox, Label labelAsterisk)
        {
            UrlTextBox = urlTextBox;
            DefaultUrlTextBox = defaultUrlTextBox;
            PlayBehaviorDropDownList = playBehaviorDropDownList;
            DefaultRepeatCountTextBox = defaultRepeatCountTextBox;
            LabelAsterisk = labelAsterisk;
        }
    }
}