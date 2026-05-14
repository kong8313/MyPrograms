using System;
using System.Threading;

namespace Confirmit.CATI.IntegrationTests.Tests.MultiUserEnvironment.Tools
{
    internal class Tools
    {
        private object lockRepeatableID = new Object();
        private object lockPersonName = new Object();

        private int curCountIterationOfID = 0;
        private int curCountIterationOfName = 0;

        private int repeatCnt = 5;

        private int maxValueOfRepeatableID = 0;
        private string name = Guid.NewGuid().ToString();
        private int uniqueID = 0;

        internal int CountIterationOfRepeatableID
        {
            set
            {
                repeatCnt = value;
            }
        }

        internal int MaxValueOfRepeatableID
        {
            set
            {
                maxValueOfRepeatableID = value;
            }
        }

        internal int RepeatableID
        {
            get
            {
                lock (lockRepeatableID)
                {
                    curCountIterationOfID++;
                    if (curCountIterationOfID > repeatCnt)
                    {
                        curCountIterationOfID = 0;
                        maxValueOfRepeatableID--;
                    }
                    return maxValueOfRepeatableID;
                }
            }
        }

        internal string Name
        {
            get
            {
                lock (lockPersonName)
                {
                    curCountIterationOfName++;
                    if (curCountIterationOfName > repeatCnt)
                    {
                        curCountIterationOfName = 0;
                        name = Guid.NewGuid().ToString();
                    }
                    return name;
                }
            }
        }

        internal int UniqueID
        {
            get
            {
                return Interlocked.Increment(ref uniqueID);
            }
        }
    }
}