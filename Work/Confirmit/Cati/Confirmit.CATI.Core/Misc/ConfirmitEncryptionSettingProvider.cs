using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Misc
{
    public class ConfirmitEncryptionSettingProvider : IConfirmitEncryptionSettingProvider
    {
        private readonly ICompanyInfo _companyInfo;
        private readonly IConnectionStrings _connectionStrings;

        public ConfirmitEncryptionSettingProvider(ICompanyInfo companyInfo, IConnectionStrings connectionStrings)
        {
            _companyInfo = companyInfo;
            _connectionStrings = connectionStrings;
        }

        public bool GetAlwaysUseEncryptedFileTransferSetting()
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "select [AlwaysUseEncryptedFileTransfer] " +
                    "from [company]" +
                    "where companyid = @CompanyId";
                command.Parameters.Add(new SqlParameter("@CompanyId", _companyInfo.CompanyId));

                return (bool)command.ExecuteScalar();
            }
        }
    }
}