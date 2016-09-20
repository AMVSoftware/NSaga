using System;

namespace NSaga
{
    public class PipelineContext
    {
        public ISagaRepository SagaRepository { get; set; }
        public ISagaMessage Message { get; set; }
        public OperationResult OperationResult { get; set; }
        public IAccessibleSaga AccessibleSaga { get; set; }
        public Object SagaData { get; set; }
    }
}