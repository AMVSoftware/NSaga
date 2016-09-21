using System;

namespace NSaga
{
    public class PipelineContext
    {
        public ISagaMessage Message { get; set; }
        public OperationResult OperationResult { get; set; }
        public IAccessibleSaga AccessibleSaga { get; set; }
        public Object SagaData { get; set; }
    }
}