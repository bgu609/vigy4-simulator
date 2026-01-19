using Rti.Dds.Core;
using Rti.Dds.Core.Status;
using Rti.Dds.Subscription;

namespace Pa5455CmsDds.Component
{
    public class AnyDataReaderResource : IDisposable
    {
        public AnyDataReader dataReader { get; private set; }
        public Condition.TriggeredEventHandler conditionTrigger { get; private set; }



        public AnyDataReaderResource(AnyDataReader dataReader, Condition.TriggeredEventHandler conditionTrigger, StatusMask enabledStatuses)
        {
            dataReader.StatusCondition.EnabledStatuses = enabledStatuses;
            dataReader.StatusCondition.Triggered += conditionTrigger;

            this.dataReader = dataReader;
            this.conditionTrigger = conditionTrigger;
        }



        public void Dispose()
        {
            lock (this.dataReader)
            {
                this.dataReader.StatusCondition.Triggered -= this.conditionTrigger;
                this.dataReader.Dispose();
            }
        }
    }
}