using System.Collections.Generic;
using System.Linq;

namespace NSaga
{
    public class CompositePipelineHook : IPipelineHook
    {
        private readonly IList<IPipelineHook> hooks;

        public CompositePipelineHook()
        {
            hooks = new List<IPipelineHook>();
        }

        public CompositePipelineHook(IEnumerable<IPipelineHook> hooks)
        {
            Guard.ArgumentIsNotNull(hooks, nameof(hooks));
            this.hooks = hooks.ToList();
        }

        public CompositePipelineHook(params IPipelineHook[] hooks)
        {
            Guard.ArgumentIsNotNull(hooks, nameof(hooks));
            this.hooks = hooks;
        }

        public CompositePipelineHook AddHook(IPipelineHook pipelineHook)
        {
            hooks.Add(pipelineHook);

            return this;
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
