using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
    /// <summary>
    /// Provides the base class for shifts and exclusions. It contains identifier,
    /// identifier of shift type and data for timezones.
    /// </summary>
    /// <typeparam name="T">Shift data type. This type have to implement
    /// <see cref="IVerifiable"/> interface.</typeparam>
    [Serializable]
    public abstract class BaseShift<T> : BaseObject<int>
    {
        /// <summary>
        /// Constant which represents virtual identifier of repondent timezone.
        /// You should use this variable as timezone identifier to set/retrieve 
        /// respondent timezone data.
        /// </summary>
        public const int RespondentTimezoneId = -1;

        private int? m_shiftTypeId = null;

        private Dictionary<int, T> m_collectionData = new Dictionary<int, T>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseShift()
        {
        }

        /// <summary>
        /// Initialize new instance of the object and fills it with the
        /// data of given object.
        /// </summary>
        /// <param name="obj">Object to copy.</param>
        protected BaseShift(BaseShift<T> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", Strings.ItemNullExceptionMessage);
            }

            Id = obj.Id;
            ShiftTypeId = obj.ShiftTypeId;

            foreach (int timezoneId in obj.GetTimezoneIds())
            {
                T data = obj.GetDataForTimezone(timezoneId);
                SetDataForTimezone(timezoneId, data);
            }
        }

        /// <summary>
        /// Shift type identifier. It is nullable value. If this value is null that means
        /// that object is not proper initialized.
        /// </summary>
        public virtual int? ShiftTypeId
        {
            get { return m_shiftTypeId; }
            set { m_shiftTypeId = value; }
        }

        /// <summary>
        /// This property is used only for serialization of list of overridden timezones
        /// and data. We couldn't serialize IDictionary objects so we have to provide 
        /// special property. This property returns type which could be serialized the way 
        /// we like.
        /// </summary>
        [XmlArray("Timezones")]
        [XmlArrayItem("Timezone")]
        public BaseTimezoneData<T>[] Timezones
        {
            get
            {
                // constructs serialization data from our dictionary
                List<BaseTimezoneData<T>> result = new List<BaseTimezoneData<T>>();
                foreach (KeyValuePair<int, T> pair in m_collectionData)
                {
                    BaseTimezoneData<T> element;
                    if (pair.Key == RespondentTimezoneId)
                    {
                        element = new BaseTimezoneData<T>(pair.Value);
                    }
                    else
                    {
                        element = new BaseTimezoneData<T>(pair.Key, pair.Value);
                    }

                    result.Add(element);
                }

                return result.ToArray();
            }
            set
            {
                // filling our dictionary from serialization data
                m_collectionData.Clear();
                if (value != null)
                {
                    foreach (BaseTimezoneData<T> item in value)
                    {
                        m_collectionData.Add(
                            item.Id.HasValue ? item.Id.Value : RespondentTimezoneId, item.Data);
                    }
                }
            }
        }

        /// <summary>
        /// Returns shift data for specified timezone. The timezone is specified by it's id. 
        /// You must specify RespondentTimezoneId as id to retrieve data of respondent timezone.
        /// </summary>
        /// <param name="timezoneId">Timezone identifier.</param>
        /// <returns>Shift data.</returns>
        /// <exception cref="ArgumentOutOfRangeException">the timezone does not exist in the list of timezones
        /// and is not Respondent timezone.</exception>
        /// <remarks>This function has unit tests in ShiftTest class.</remarks>
        public T GetDataForTimezone(int timezoneId)
        {
            if (!m_collectionData.ContainsKey(timezoneId))
            {
                throw new ArgumentOutOfRangeException("timezoneId", Strings.TimezoneOutOfRangeException);
            }

            return m_collectionData[timezoneId];
        }

        /// <summary>
        /// Returns shift data for specified timezone. The timezone is specified by it's id.
        /// If current shift doesn't contain data for specified timezone, method returns data
        /// for Respondent timezone and true. If shift doesn't contain data for Respondent
        /// timezone, method returns default value for type of data and false.
        /// </summary>
        /// <param name="timezoneId">Timezone identifier.</param>
        /// <param name="data">Returns shift data.</param>
        /// <returns>true, if shift contains data for given or Respondent timezones;
        /// otherwise false.</returns>
        public bool TryGetDataForTimezone(int timezoneId, out T data)
        {
            bool result = true;
            data = default(T);

            if (HasTimezone(timezoneId))
            {
                data = GetDataForTimezone(timezoneId);
            }
            else if (HasTimezone(RespondentTimezoneId))
            {
                data = GetDataForTimezone(RespondentTimezoneId);
            }
            else
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Sets shift data for specified timezone. The timezone is specified by it's identifier. 
        /// You must specify RespondentTimezoneId as id to set data of respondent timezone.
        /// </summary>
        /// <param name="timezoneId">Timezone identifier.</param>
        /// <param name="data">Shift data.</param>
        /// <exception cref="ArgumentNullException">The data parameter is null or in invalid state.</exception>
        /// <exception cref="ArgumentException">The data parameter is in invalid state.</exception>
        /// <remarks>This function has unit tests in ShiftTest class.</remarks>
        public void SetDataForTimezone(int timezoneId, T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", Strings.ItemNullExceptionMessage);
            }

            ErrorCollection errors;
            var validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            if (!validator.Validate(data, out errors))
            {
                throw new ArgumentException(errors.ToString(), "data");
            }

            m_collectionData[timezoneId] = data;
        }

        /// <summary>
        /// Removes shift data for specified timezone.
        /// </summary>
        /// <param name="timezoneId">Timezone identifier.</param>
        public void RemoveDataForTimezone(int timezoneId)
        {
            m_collectionData.Remove(timezoneId);
        }

        /// <summary>
        /// Determines whether shift contains data of specified timezone. 
        /// </summary>
        /// <param name="timezoneId">Timezone identifier.</param>
        /// <returns>true, if shift contains data, otherwise false.</returns>
        public bool HasTimezone(int timezoneId)
        {
            return m_collectionData.ContainsKey(timezoneId);
        }

        /// <summary>
        /// Returns the list of identifiers of all timezones for which current shift is defined.
        /// </summary>
        /// <returns>List of identifiers.</returns>
        /// <remarks>This function has unit tests in ShiftTest class.</remarks>
        public int[] GetTimezoneIds()
        {
            int[] result = new int[m_collectionData.Keys.Count];
            m_collectionData.Keys.CopyTo(result, 0);

            return result;
        }
    }
}
