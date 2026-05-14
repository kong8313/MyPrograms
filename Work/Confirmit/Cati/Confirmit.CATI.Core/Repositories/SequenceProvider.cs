using System.Data;
using Confirmit.CATI.Core.DAL.Framework;

namespace Confirmit.CATI.Core.Repositories
{
    public class SequenceProvider
    {
        private const string GetFirstInRangeScript = @"DECLARE @range_first_value sql_variant ,
        @range_first_value_output sql_variant ;

        EXEC sp_sequence_get_range
        @sequence_name = N'{0}'
        , @range_size = {1}
        , @range_first_value = @range_first_value_output OUTPUT ;

        SELECT @range_first_value_output AS FirstNumber;";

        private const string GetNextValueForSequenceScript =
            "SELECT NEXT VALUE FOR {0} AS FirstUse";

        private const string RestartSequenceScript = "ALTER SEQUENCE {0} RESTART WITH {1}";

        /// <summary>
        /// Generates reserve a range of sequence numbers
        /// </summary>
        /// <param name="sequenceName">Sequence name</param>
        /// <param name="rangeSize">Range size</param>
        /// <returns>First number in a range</returns>
        public int ReserveRange(string sequenceName, int rangeSize)
        {
            return new DatabaseEngine().ExecuteScalar<int>(
                string.Format(GetFirstInRangeScript, sequenceName, rangeSize),
                CommandType.Text);
        }

        /// <summary>
        /// Generates a sequence number from the specified sequence
        /// </summary>
        /// <param name="sequenceName">Full name with schema for example: [dbo].[BvTelephoneBlacklistIdSequence]</param>
        /// <returns></returns>
        public int GetNext(string sequenceName)
        {
            return new DatabaseEngine().ExecuteScalar<int>(
                string.Format(GetNextValueForSequenceScript, sequenceName),
                CommandType.Text);
        }

        /// <summary>
        /// Restarts the sequence
        /// </summary>
        /// <param name="sequenceName">Full name with schema for example: [dbo].[BvTelephoneBlacklistIdSequence]</param>
        /// <param name="restartWith"></param>
        public void RestartSequence(string sequenceName, int restartWith = 1)
        {
            new DatabaseEngine().ExecuteNonQuery(
                string.Format(RestartSequenceScript, sequenceName, restartWith),
                CommandType.Text);
        }
    }
}
