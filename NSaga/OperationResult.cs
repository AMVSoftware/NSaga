using System;
using System.Collections.Generic;
using System.Linq;


namespace NSaga
{
    public class OperationResult : List<String>
    {
        public bool IsSuccessful => this.Any();

        public OperationResult()
        {
            // nothingness
        }

        public OperationResult(params String[] errorMessages)
        {
            this.AddRange(errorMessages);
        }

        public override string ToString()
        {
            return this.Aggregate("", (current, next) => current + ", " + next);
        }

        public String ToCsv()
        {
            return String.Join(", ", this.Select(e => e.ToString()));
        }

        public OperationResult AddError(String message)
        {
            this.Add(message);
            return this;
        }

        public OperationResult Merge(OperationResult otherErrors)
        {
            this.AddRange(otherErrors);

            return this;
        }
    }
}
