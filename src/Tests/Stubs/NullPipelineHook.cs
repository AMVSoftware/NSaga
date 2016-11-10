using NSaga;

namespace Tests.Stubs
{
    public class NullPipelineHook : IPipelineHook
    {
        public void BeforeInitialisation(PipelineContext context)
        {
        }

        public void AfterInitialisation(PipelineContext context)
        {
        }

        public void BeforeConsuming(PipelineContext context)
        {
        }

        public void AfterConsuming(PipelineContext context)
        {
        }

        public void AfterSave(PipelineContext context)
        {
        }
    }
}
