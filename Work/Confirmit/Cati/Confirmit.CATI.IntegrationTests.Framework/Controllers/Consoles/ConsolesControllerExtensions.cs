using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework.Data;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles
{
    public static class ConsolesControllerExtensions
    {
        private static IEnumerable<T> ForEach<T>(IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
                //yield return item;
            }

            return items;
        }

        public static IEnumerable<ConsoleController> Login(this PersonController[] persons)
        {
            return persons.Select(x => x.Console.Login()).ToArray();
        }

        public static IEnumerable<ConsoleController> Login(this PersonController[] persons, SurveyController survey)
        {
            return persons.Select(x => x.Console.Login(survey)).ToArray();
        }

        public static IEnumerable<ConsoleController> Login(this PersonController[] persons, string surveyTag)
        {
            return persons.Select(x => x.Login(persons.FirstOrDefault()?.Context.GetSurvey(surveyTag))).ToArray();
        }

        public static IEnumerable<T> Login<T>(this IEnumerable<T> consoles) where T : ConsoleController
        {
            return ForEach(consoles, c => c.Login());
        }

        public static IEnumerable<T> Login<T>(this IEnumerable<T> consoles, SurveyController survey) where T : ConsoleController
        {
            return ForEach(consoles, c => c.Login(survey));
        }

        public static IEnumerable<T> Start<T>(this IEnumerable<T> consoles) where T : ConsoleController
        {
            return ForEach(consoles, c => c.Start());
        }

        public static IEnumerable<T> Wait<T>(this IEnumerable<T> consoles) where T : ConsoleController
        {
            return ForEach(consoles, c => c.Wait());
        }

        public static IEnumerable<T> Break<T>(this IEnumerable<T> consoles) where T : ConsoleController
        {
            return ForEach(consoles, c => c.Break());
        }
        
    }
}