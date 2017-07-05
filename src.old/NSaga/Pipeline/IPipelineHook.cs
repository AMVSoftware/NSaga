namespace NSaga
{
    /// <summary>
    /// Pipeline hooks are executed in key moments of Saga message processing by mediator. 
    /// </summary>
    public interface IPipelineHook
    {
        /// <summary>
        /// Hook executed before the saga have been initialised
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        void BeforeInitialisation(PipelineContext context);

        /// <summary>
        /// After the saga is initialised but before it is saved to the storage
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        void AfterInitialisation(PipelineContext context);

        /// <summary>
        /// Hook executed before the message is consumed by the saga
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        void BeforeConsuming(PipelineContext context);

        /// <summary>
        /// Hook executed after the message is consimed by the saga, but before saga is saved to the storage
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        void AfterConsuming(PipelineContext context);


        /// <summary>
        /// Hook executed after the saga is saved.
        /// <para>Please note that not all sagas will be saved - if execution of a message have failed, new saga state will not be preserved, hence this hook will not be called.</para>
        /// </summary>
        /// <param name="context"></param>
        void AfterSave(PipelineContext context);
    }

    /// <summary>
    /// Base non-implementation of a Pipeline Hook. This can be used to start a new pipeline hook, especially if only some of the methods to be executed
    /// </summary>
    /// <seealso cref="NSaga.IPipelineHook" />
    public abstract class BasePipelineHook : IPipelineHook
    {
        /// <summary>
        /// Hook executed before the saga have been initialised
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public virtual void BeforeInitialisation(PipelineContext context)
        {
        }

        /// <summary>
        /// After the saga is initialised but before it is saved to the storage
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public virtual void AfterInitialisation(PipelineContext context)
        {
        }

        /// <summary>
        /// Hook executed before the message is consumed by the saga
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public virtual void BeforeConsuming(PipelineContext context)
        {
        }

        /// <summary>
        /// Hook executed after the message is consimed by the saga, but before saga is saved to the storage
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public virtual void AfterConsuming(PipelineContext context)
        {
        }

        /// <summary>
        /// Hook executed after the saga is saved.
        /// <para>Please note that not all sagas will be saved - if execution of a message have failed, new saga state will not be preserved, hence this hook will not be called.</para>
        /// </summary>
        /// <param name="context"></param>
        public virtual void AfterSave(PipelineContext context)
        {
        }
    }
}
