namespace Confirmit.CATI.Supervisor.Classes.Filters
{
    public interface ISupervisorFilterFactory
    {
        FilterData Create(int id, string name, string description,
                                                                         string operatorString, string fieldsXml);
    }
}
