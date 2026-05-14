using System;

namespace Confirmit.CATI.Core.Services
{
    public interface IShiftService
    {
        /// <summary>
        /// ID of current schedule object
        /// </summary>
        int ScheduleID { get; }

        bool IsTakingExclusionIntoAccount { get; }

        void CheckConfiguration();

        /// <summary>
        /// Возвращает по ID типа шифта, уникального в рамках скрипта, уникальый ID шифта в рамках системы
        /// </summary>
        /// <param name="shiftTypeID"></param>
        /// <returns>уникальый ID шифта в рамках системы</returns>
        int GetShiftTypeWorkID(int shiftTypeID);

        /// <summary>
        /// Определяет шифт, в который входит максимально допустимое время "звонка" в указанной TZ 
        /// для указанного времени
        /// </summary>
        /// <param name="utcTime">время в UTC, для которого выполняется поиск</param>
        /// <param name="tzID">Таймзона, в которой выполняется поиск</param>
        /// <returns>Найденный шифт</returns>
        ShiftService.MatchingShift GetMatchingShift(DateTime utcTime, int tzID);

        /// <summary>
        /// Возвращает следующий шифт, который полностью не перекрыт exclusion
        /// </summary>
        /// <param name="currentShift">текущий шифт</param>
        /// <param name="tzID">Таймзона, в которой выполняется поиск</param>
        /// <param name="countSkipShifts">количество пропушенных шифтов из за их полного перекрытия exclusions-ами</param>
        /// <returns>Найденный шифт</returns>
        ShiftService.MatchingShift GetNextShift(ShiftService.MatchingShift currentShift, int tzID, out int countSkipShifts);

        /// <summary>
        /// Возврашает шифт через определенное количество шифтов, если этот шифт полностью перекрыт exclusion-ом, 
        /// то возвращает следующий доступный шифт
        /// </summary>
        /// <param name="curentShift">текущий шифт</param>
        /// <param name="tzID">Таймзона, в которой выполняется поиск</param>
        /// <param name="numberOfShifts">количество шифтов, которое следует пропустить</param>
        /// <param name="isTakingExclusionIntoAccount">учитывать шифты, которые полностью перекрыты exclusion-ами или нет?</param>
        /// <returns>Найденный шифт</returns>
        ShiftService.MatchingShift GetShiftAfterNumberOfShifts(ShiftService.MatchingShift curentShift, int tzID, int numberOfShifts, bool isTakingExclusionIntoAccount);

        /// <summary>
        /// Определяет максимально допустимое время "звонка" в указанной TZ,
        /// которое предшествует или равно указанному в utcTime времени .
        /// </summary>
        /// <param name="utcNowTime">время в UTC, для которого выполняется поиск</param>
        /// <param name="tzID">Таймзона, в которой выполняется поиск</param>
        /// <returns>Максимальное допустимое время звонка</returns>
        DateTime GetMatchingTime(DateTime utcNowTime, int tzID);

        ShiftService.MatchingShift GetExactShift(DateTime utcNowTime, int tzID);

        /// <summary>
        /// Возвращает следующий доступный шифт
        /// </summary>
        /// <param name="currentShift">текущий шифт, от которого начинается поиск</param>
        /// <param name="tzID">ID таймзоны, в которой производится поиск</param>
        /// <returns>Следующий доступный шифт</returns>
        ShiftService.MatchingShift GetNextShift(ShiftService.MatchingShift currentShift, int tzID);

        /// <summary>
        /// Вычисляет билижайший валидный шифт, 
        /// который будет доступен через указанное количество минут
        /// </summary>
        /// <param name="utcNowTime">Начальное время поиска</param>
        /// <param name="tzID">ID таймзоны, в которой производится поиск</param>
        /// <param name="countMinutes">Количество минут, через которое должен быть найден шифт</param>
        /// <returns>Найденный шифт</returns>
        ShiftService.MatchingShift GetShiftAfterNumberOfMinutes(DateTime utcNowTime, int tzID, int countMinutes);

        /// <summary>
        /// Возвращает шифт, который будет валиден через указанное количество шифтов
        /// Используется в следующий Action:
        /// Recall after number of shifts
        /// Recall after number of shifts specified by variable
        /// Recall after number of shifts (random time)
        /// </summary>
        /// <param name="utcNowTime">Начальное время поиска</param>
        /// <param name="tzID">ID таймзоны, в которой производится поиск</param>
        /// <param name="numberOfShifts">Количество шифтов, которое нужно пропустить</param>
        /// <returns>найденный шифт</returns>
        ShiftService.MatchingShift GetShiftAfterNumberOfShifts(DateTime utcNowTime, int tzID, int numberOfShifts);

        ShiftService.MatchingShift GetNextShiftOfSpecifiedType(DateTime utcTime, int tzID, int scriptShiftTypeID);
        ShiftService.MatchingShift GetNextShiftByID(DateTime utcTime, int tzID, int scriptShiftID);
    }
}