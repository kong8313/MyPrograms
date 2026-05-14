using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public class QuestionHistoryBuilder
    {
        /// <summary>
        /// Initializes new instance of QuestionHistory class and fills it with given data.
        /// </summary>
        /// <param name="entry">History entry.</param>
        public QuestionHistory GetQuestionHistory(InterviewHistoryEntry entry)
        {
            var quest = new QuestionHistory();

            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            quest.QuestionId = entry.QuestionId;
            quest.UrlQuery = entry.UrlQuery;
            quest.QuestionName = entry.QuestionName;

            return quest;
        }

         /// <summary>
        /// Initializes new instance of QuestionHistoryCollection and fills it with given data.
        /// </summary>
        /// <param name="historyEntries">History entries.</param>
        public QuestionHistoryCollection GetQuestionHistoryCollection(IEnumerable<InterviewHistoryEntry> historyEntries)
        {
            if (historyEntries == null)
            {
                throw new ArgumentNullException("historyEntries");
            }

            var list = new QuestionHistoryCollection();
            list.AddRange(historyEntries.Select(entry => this.GetQuestionHistory(entry)));

            return list;
        }
    }
}
