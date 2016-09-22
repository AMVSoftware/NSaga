using System.Collections.Generic;

namespace NSaga.Pipeline
{
    public class CompositePipelineHook : IPipelineHook
    {
        private readonly IEnumerable<IPipelineHook> hooks;

        public CompositePipelineHook(IEnumerable<IPipelineHook> hooks)
        {
            Guard.ArgumentIsNotNull(hooks, nameof(hooks));
            this.hooks = hooks;
        }

        public CompositePipelineHook(params IPipelineHook[] hooks)
        {
            Guard.ArgumentIsNotNull(hooks, nameof(hooks));
            this.hooks = hooks;
        }

        public void BeforeInitialisation(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.BeforeInitialisation(context);
            }
        }

        public void AfterInitialisation(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.AfterInitialisation(context);
            }
        }

        public void BeforeConsuming(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.BeforeConsuming(context);
            }
        }

        public void AfterConsuming(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.AfterConsuming(context);
            }
        }

        public void AfterSave(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.AfterSave(context);
            }
        }
    }
}
