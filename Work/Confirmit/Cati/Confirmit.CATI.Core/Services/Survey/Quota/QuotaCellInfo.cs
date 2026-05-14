using System.Diagnostics;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    //This class implements QuotaCellInfo objects for 
    [DebuggerDisplay("Id={Id}, Key={System.String.Join(\",\",Key)}, C/L/S={Counter}/{Limit}/{IsOpen} ")]
    public class QuotaCellInfo
    {
        /// <summary>
        /// Id of cell
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Achieved value
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// Target value
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Live achieved value
        /// </summary>
        public int LiveCounter { get; set; }

        /// <summary>
        /// Live target value
        /// </summary>
        public int LiveLimit { get; set; }

        /// <summary>
        /// Cell state
        /// </summary>
        public bool IsOpen { get; set; }

        public bool IsDisabled { get; set; }

        private string[] _key;

        /// <summary>
        /// Index of N-dimensional array
        /// </summary>
        public string[] Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;

                //
                // Calculate KeyUnrecordMask
                //
                int keyMask = 0;

                for (int i = 0; i < _key.Length; i++)
                    if (_key[i] == null)
                        keyMask |= 1 << i;

                KeyUnrecordMask = keyMask;

            }
        }

        /// <summary>
        /// Bit map of unrecord items in key. 
        /// Note: we use int type for mask, 
        /// because we have limitation of amount of question in quota( now it 5 ).That will do.
        /// Note: This mask is used in search engine.
        /// </summary>
        public int KeyUnrecordMask { get; set; }
    }
}
