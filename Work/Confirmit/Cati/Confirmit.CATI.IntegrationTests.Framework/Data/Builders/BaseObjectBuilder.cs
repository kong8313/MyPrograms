using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public abstract class BaseObjectBuilder<TData> : IObjectBuilder
    {
        public TestDataContext Context { get; private set; }

        public TData Data { get; private set; }

        public DataGenerator DataGenerator { get; private set; }

        protected BaseObjectBuilder(
            TestDataContext context,
            TData data,
            DataGenerator dataGenerator
            )
        {
            Data = data;
            Context = context;
            DataGenerator = dataGenerator;
        }

        public abstract void Create();

        public virtual void Setup(){}
    }
}
