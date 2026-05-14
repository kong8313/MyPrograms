namespace Confirmit.CATI.Core.AsynchronousTrigger.Messages
{
    /// <summary>
    /// Represents parsed message sent from async. trigger.
    /// </summary>
    public class TriggerMessage : ITriggerMessage
    {
        /// <summary>
        /// Gets or sets table name notification received from.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets transaction name table have been changed.
        /// </summary>
        public string TransactionName { get; set; }

        /// <summary>
        /// Gets or sets Date on Sql Server table have been changed.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets unique message id. Needed right now for logging purpose only.
        /// </summary>
        public string Guid { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Table: {0}, TransactionName: {1}, Date: {2} Id: {3}",
                this.TableName,
                this.TransactionName,
                this.Date,
                this.Guid);
        }
    }
}