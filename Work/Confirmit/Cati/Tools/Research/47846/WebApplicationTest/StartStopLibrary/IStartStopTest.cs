using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StartStopTest
{
    public interface IStartStopTest<T>
    {
        
        T CreateTestObject();
        void ClearTestObject(T obj);

        void Start(T obj);
        void Stop(T obj);
        
    }

    public delegate void StartStopTestProgress(int count, int progress);

    public interface IStartStopTest
    {

        void Init();
        void Done();

        void Create(int count, StartStopTestProgress progress);
        void Delete(StartStopTestProgress progress);

        void Start(StartStopTestProgress progress);
        void Stop(StartStopTestProgress progress);
    
    }

    public class StartStop<T> : IStartStopTest
    {
        IStartStopTest<T> factory;
        List<T> entities = new List<T>();

        public StartStop(IStartStopTest<T> factory)
        {
            this.factory = factory;
        }


        #region IStartStopTest Members

        public void Init()
        {
        }

        public void Done()
        {
            Delete(null);
        }

        public void Create(int count, StartStopTestProgress progress)
        {
            for (int i = 0; i < count; i++)
            {                
                T entity = factory.CreateTestObject();
                entities.Add(entity);
                if (progress != null) progress(count, i);
            }
        }

        public void Delete(StartStopTestProgress progress)
        {
            int i = 0;
            int count = entities.Count;
            foreach (T entity in entities)
            {                
                factory.ClearTestObject(entity);
                if (progress != null) progress(count, i);
                i++;
            }
            entities.Clear();
        }

        public void Start(StartStopTestProgress progress)
        {
            int i = 0;
            int count = entities.Count;
            foreach (T entity in entities)
            {
                factory.Start(entity);
                if (progress != null) progress(count, i);
                i++;
            }
        }

        public void Stop(StartStopTestProgress progress)
        {
            int i = 0;
            int count = entities.Count;
            foreach (T entity in entities)
            {
                factory.Stop(entity);
                if (progress != null) progress(count, i);
                i++;
            }
        }

        #endregion
    }

}
