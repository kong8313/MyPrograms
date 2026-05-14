using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    // Class implements N-dimensional array from QuotaCellInfo objects
    public class QuotaMatrix
    {
        // Linear array of QuotaCellInfo
        private readonly QuotaCellInfo[] _array;

        /// <summary>
        /// Array of dictionry with 'subarray' indexes by precodes. Index of array is index of subspace.
        /// </summary>
        private readonly Dictionary<string, int>[] _precodesToIndexsByDimensions;

        /// <summary>
        /// Array of dictionry with precodes by 'subarray' indexes. Index of array is index of subspace.
        /// </summary>
        private readonly Dictionary<int, string>[] _indexsToPrecodesByDimensions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="precodesByDimensions">Array of precodes array by subsapces</param>
        public QuotaMatrix(string[][] precodesByDimensions)
        {
            int size = 1;

            _precodesToIndexsByDimensions = new Dictionary<string, int>[precodesByDimensions.Length];
            _indexsToPrecodesByDimensions = new Dictionary<int, string>[precodesByDimensions.Length];

            // Processiong each dimepnsion
            for (int dimensionIndex = 0; dimensionIndex < precodesByDimensions.Length; dimensionIndex++)
            {
                string[] precodes = precodesByDimensions[dimensionIndex];


                size *= precodes.Length;//Calculate lineer array size

                //
                // Initialize dictionry with 'subarray' indexes by preocodes and
                // dictionry with preocodes by 'subarray' indexes.
                // Note: we should convert NULL precode to empty string ( "" ) for precodeToIndexs dictionry, 
                // because key value can't be NULL
                //
                var precodeToIndexs = new Dictionary<string, int>();
                var indexsToprecode = new Dictionary<int, string>();

                for (int precodeIndex = 0; precodeIndex < precodes.Length; precodeIndex++)
                {
                    precodeToIndexs.Add(precodes[precodeIndex] ?? "", precodeIndex);
                    indexsToprecode.Add(precodeIndex, precodes[precodeIndex]);
                }

                _precodesToIndexsByDimensions[dimensionIndex] = precodeToIndexs;
                _indexsToPrecodesByDimensions[dimensionIndex] = indexsToprecode;
            }

            //
            // Create and initialize linear array.
            //
            _array = new QuotaCellInfo[size];
            for (int index = 0; index < _array.Length; index++)
            {
                string[] key = IndexToKey(index);

                int defaultLimit = DefaultLimit(key);

                _array[index] = new QuotaCellInfo
                {
                    Id = 0,
                    Counter = defaultLimit,
                    Limit = defaultLimit,
                    Key = key,
                    IsOpen = false
                };
            }
        }

        /// <summary>
        /// this operator give public access to N-dimensional array by N-dimensional index(key)
        /// </summary>
        /// <param name="key">index of N-dimensional array</param>
        /// <returns></returns>
        public QuotaCellInfo this[string[] key]
        {
            get
            {
                return _array[KeyToIndex(key)];
            }
            set
            {
                _array[KeyToIndex(key)] = value;
            }
        }

        /// <summary>
        /// Linear array of QuotaCellInfo
        /// </summary>
        public IEnumerable<QuotaCellInfo> Cells
        {
            get
            {
                return _array;
            }
        }

        /// <summary>
        /// Calculate amount of child cells for unrecord cell, otherwise 0
        /// </summary>
        /// <param name="key">index of N-dimensional array</param>
        /// <returns>amount of child cells</returns>
        private int DefaultLimit(string[] key)
        {
            int result = 0;
            for (int index = 0; index < key.Length; index++)
            {
                if (key[index] == null)
                {
                    // we should decrement value on 1, 
                    // because we should not take into account unrecord cell
                    result += _precodesToIndexsByDimensions[index].Count - 1;
                }
            }
            return result;
        }

        /// <summary>
        /// This method calculate index of linear array by index of N-dimensional array.
        /// </summary>
        /// <param name="key">index of N-dimensional array</param>
        /// <returns></returns>
        private int KeyToIndex(string[] key)
        {
            var index = TryConvertKeyToIndex(key);

            if (index == null)
                throw new Exception("Unknowen  cell key.");

            return (int)index;
        }

        private int? TryConvertKeyToIndex(string[] key)
        {
            if (key.Length != _precodesToIndexsByDimensions.Length)
                throw new IndexOutOfRangeException();

            int result = 0;
            int subDimensionSize = 1;

            for (int i = 0; i < key.Length; i++)
            {
                int precodeIndex;

                //The unknown precode is NOT interpreted as Unrecorded
                if (!_precodesToIndexsByDimensions[i].TryGetValue(key[i] ?? "", out precodeIndex))
                    return null;

                result += subDimensionSize * precodeIndex;
                subDimensionSize *= _precodesToIndexsByDimensions[i].Count;
            }

            return result;
        }

        /// <summary>
        /// This method calculate index of N-dimensional array(key) by index of linear array.
        /// </summary>
        /// <param name="index">index in linear array</param>
        /// <returns></returns>
        private string[] IndexToKey(int index)
        {
            var result = new string[_precodesToIndexsByDimensions.Length];

            for (int i = 0; i < _precodesToIndexsByDimensions.Length; i++)
            {
                int curDimensionSize = _precodesToIndexsByDimensions[i].Count;
                int precodeindex = index % curDimensionSize;
                result[i] = _indexsToPrecodesByDimensions[i][precodeindex];
                index = (index - precodeindex) / curDimensionSize;
            }

            return result;
        }
    }
}
