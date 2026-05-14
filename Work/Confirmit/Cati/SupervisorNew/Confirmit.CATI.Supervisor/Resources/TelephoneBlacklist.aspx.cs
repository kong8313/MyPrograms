using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Export;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.BlackList;
using Confirmit.CATI.Supervisor.Core.Timezone;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class TelephoneBlacklist : BaseForm
    {
        private const string ExportFileName = "TelephoneBlacklistData.txt";
        private const string PackageFileName = "TelephoneBlacklistData.zip";
        private readonly IBlackListService _blackListService = ServiceLocator.Resolve<IBlackListService>();
        private readonly ITelephoneBlacklistRepository _telephoneBlacklistRepository = ServiceLocator.Resolve<ITelephoneBlacklistRepository>();
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        
        public override string TopTitle
        {
            get
            {
                return Strings.TelephoneBlacklist;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.HintText = Strings.TelephoneBlacklistHint;
            m_grid.GetPage += delegate(out int totalCount)
            {
                var blackListEntities = _telephoneBlacklistRepository.GetPage(m_grid.PageArguments, out totalCount);

                foreach (var blackListEntity in blackListEntities)
                {
                    blackListEntity.Timestamp = _timezoneProvider.ConvertToLocalTime(blackListEntity.Timestamp);
                }

                return blackListEntities;
            };
        }

        protected void DeleteNumbersFormBlacklist(object sender, EventArgs e)
        {
            try
            {
                using (var transactionScope = new DatabaseTransactionScope("TelBlacklist.DeleteNumbers", DeadlockPriority.Supervisor))
                {
                    var keys = m_grid.SelectedKeysInt;
                    var evt = new DeleteTelephoneNumbersFromBlacklistEvent(keys.Count);

                    _telephoneBlacklistRepository.Delete(m_grid.SelectedKeysInt);

                    evt.Finish();
                    transactionScope.Commit();
                }

                m_grid.ClearSelectedKeys();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void ImportNumbersToBlacklist(object sender, EventArgs e)
        {
            try
            {
                if (FileLoad.HasFile)
                {
                    string inputString;
                    using (var reader = new StreamReader(FileLoad.PostedFile.InputStream))
                    {
                        inputString = reader.ReadToEnd();
                    }

                    IEnumerable<string> numbers;

                    try
                    {
                        numbers = DsvManager.ImportFromDsv(inputString);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        throw new UserMessageException(Strings.ImportNumbersToBlacklist_InvalidFileFormat, ex);
                    }

                    _blackListService.ImportNumbers(numbers);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void ExportTelephoneBlacklist(object sender, EventArgs e)
        {
            var telephoneNumbers = _telephoneBlacklistRepository.GetAll().OrderBy(x => x.Id).ToList();
            
            if (!telephoneNumbers.Any())
            {
                AddUserMessage(Strings.NothingToExport);
                return;
            }

            var fileContent = string.Join("\r\n", telephoneNumbers.Select(x => $"{x.DisplayPattern}\t{_timezoneProvider.ConvertToLocalTime(x.Timestamp)}\t{x.Comment}"));

            try
            {
                var evt = new ExportTelephoneNumbersFromBlacklistEvent(telephoneNumbers.Count());

                string packageFilePath;
                try
                {
                    packageFilePath = new Packaging().CreatePackage(ExportFileName, fileContent);
                }
                catch (Exception ex)
                {
                    ExceptionTraceHelper.TraceException(ex);
                    throw new Exception("Error on creating export file, contact the administrator");
                }

                FileToClientSender.SendFileContent(packageFilePath, PackageFileName, true);
                
                evt.Finish();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void DeleteEntireTelephoneBlacklist(object sender, EventArgs e)
        {
            var deletedRecords = _telephoneBlacklistRepository.DeleteAll();

            if (deletedRecords == 0)
            {
                AddUserMessage(Strings.NothingToDelete);
                return;
            }

            var evt = new DeleteTelephoneNumbersFromBlacklistEvent(deletedRecords);
            evt.Finish();
        }
    }
}
