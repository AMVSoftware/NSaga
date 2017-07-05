using System.Collections.Generic;
using System.Linq;

namespace NSaga
{
    /// <summary>
    /// A collection of Pipeline hooks that can be executed as one.
    /// </summary>
    /// <seealso cref="NSaga.IPipelineHook" />
    public sealed class CompositePipelineHook : IPipelineHook
    {
        private readonly IList<IPipelineHook> hooks;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePipelineHook"/> class.
        /// </summary>
        public CompositePipelineHook()
        {
            hooks = new List<IPipelineHook>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePipelineHook"/> class.
        /// </summary>
        /// <param name="hooks">The hooks.</param>
        public CompositePipelineHook(IEnumerable<IPipelineHook> hooks)
        {
            Guard.ArgumentIsNotNull(hooks, nameof(hooks));
            this.hooks = hooks.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePipelineHook"/> class.
        /// </summary>
        /// <param name="hooks">The hooks.</param>
        public CompositePipelineHook(params IPipelineHook[] hooks)
        {
            Guard.ArgumentIsNotNull(hooks, nameof(hooks));
            this.hooks = hooks;
        }

        /// <summary>
        /// Adds a new instance of <see cref="IPipelineHook"/> into the existing collection of hooks
        /// </summary>
        /// <param name="pipelineHook">The pipeline hook.</param>
        /// <returns>This instance - for fluent configuration</returns>
        public CompositePipelineHook AddHook(IPipelineHook pipelineHook)
        {
            hooks.Add(pipelineHook);

            return this;
        }

        /// <summary>
        /// Hook executed before the saga have been initialised
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public void BeforeInitialisation(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.BeforeInitialisation(context);
            }
        }

        /// <summary>
        /// After the saga is initialised but before it is saved to the storage
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public void AfterInitialisation(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.AfterInitialisation(context);
            }
        }

        /// <summary>
        /// Hook executed before the message is consumed by the saga
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public void BeforeConsuming(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.BeforeConsuming(context);
            }
        }

        /// <summary>
        /// Hook executed after the message is consimed by the saga, but before saga is saved to the storage
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public void AfterConsuming(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.AfterConsuming(context);
            }
        }

        /// <summary>
        /// Hook executed after the saga is saved.
        /// <para>Please note that not all sagas will be saved - if execution of a message have failed, new saga state will not be preserved, hence this hook will not be called.</para>
        /// </summary>
        /// <param name="context"></param>
        public void AfterSave(PipelineContext context)
        {
            foreach (var pipelineHook in hooks)
            {
                pipelineHook.AfterSave(context);
            }
        }
    }
}
