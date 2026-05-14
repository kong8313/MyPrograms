namespace Confirmit.CATI.Supervisor.Classes.Script
{
    public interface IShiftInfo
    {         
        int? Id
        {
            get;
        }
             
        int ShiftTypeId
        {
            get;
        }

        ShiftStatus ShiftStatus
        {
            get;            
        }                                       
        
        string StartDayName
        {
            get; 
        }
        
        string EndDayName
        {
            get;
        }

        string StartTimeToString
        {
            get;            
        }

        string EndTimeToString
        {
            get;            
        }
        
        bool HasRespondentTimeZone
        {
            get;
        }
    }
}