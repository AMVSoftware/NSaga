using System;
using System.Collections.Generic;
using System.Linq;


namespace NSaga
{
    /// <summary>
    /// Every Saga operation retuns some result. Sometimes the validation inside the saga does not pass and the whole operation is not valid.
    /// For that add error messages to OperationResult class.
    /// </summary>
    public sealed class OperationResult
    {
        /// <summary>
        /// Creates an instance of <see cref="OperationResult"/>
        /// </summary>
        public OperationResult()
        {
            Errors = new List<string>();
        }


        /// <summary>
        /// Creates an instance of <see cref="OperationResult"/> with a number of error messages already added.
        /// </summary>
        /// <param name="errorMessages"></param>
        public OperationResult(params String[] errorMessages)
        {
            Errors = new List<string>(errorMessages);
        }

        /// <summary>
        /// Contains a list of error messages
        /// </summary>
        public List<String> Errors { get; }

        /// <summary>
        /// Indicates whether the operation was successful, i.e. there are no errors
        /// </summary>
        public bool IsSuccessful => !Errors.Any();

        /// <summary>
        /// Indicates whether the operation completed with errors, i.e. there are errors
        /// </summary>
        public bool HasErrors => Errors.Any();


        /// <summary>
        /// Returns a comma-separated list of error messages
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Join(", ", Errors.Select(e => e.ToString()));
        }

        /// <summary>
        /// Adds an error message to the list.
        /// Operates as a builder - returns the instance the operation applied to.
        /// </summary>
        /// <param name="error">Error message to be added</param>
        /// <returns>Current object of OperationResult</returns>
        public OperationResult AddError(String error)
        {
            Errors.Add(error);
            return this;
        }

        /// <summary>
        /// Adds a collection of errors to the list. 
        /// Operates as a builder - returns the instance the operation applied to.
        /// </summary>
        /// <param name="errors">List of errors to add</param>
        /// <returns>Current object of OperationResult</returns>
        public OperationResult AddErrors(IEnumerable<String> errors)
        {
            Errors.AddRange(errors);
            return this;
        }


        /// <summary>
        /// Adds a collection of errors to the list. 
        /// Operates as a builder - returns the instance the operation applied to.
        /// </summary>
        /// <param name="errors">List of errors to add</param>
        /// <returns>Current object of OperationResult</returns>
        public OperationResult AddErrors(params String[] errors)
        {
            Errors.AddRange(errors);
            return this;
        }


        /// <summary>
        /// Payload is an object that can be returned by Saga to the client code. 
        /// This may or may not be set.
        /// </summary>
        public Object PayLoad { get; set; }

        /// <summary>
        /// Adds payload to the operation result
        /// </summary>
        /// <param name="payload">An object to be returned to the clent</param>
        /// <returns>Current instance of OperationResult</returns>
        public OperationResult AddPayload(object payload)
        {
            PayLoad = payload;
            return this;
        }
    }
}
