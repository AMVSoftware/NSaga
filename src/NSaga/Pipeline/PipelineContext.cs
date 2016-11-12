using System;

namespace NSaga
{
    public class PipelineContext
    {
        public PipelineContext(ISagaMessage message, IAccessibleSaga saga, OperationResult operationResult = null)
        {
            this.Message = message;
            this.AccessibleSaga = saga;
            SagaData = NSagaReflection.Get(saga, "SagaData");
            OperationResult = operationResult;
        }

        public ISagaMessage Message { get; set; }
        public OperationResult OperationResult { get; set; }
        public IAccessibleSaga AccessibleSaga { get; set; }
        public Object SagaData { get; set; }
    }
}