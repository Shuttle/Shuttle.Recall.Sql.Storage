using System;

namespace Shuttle.Recall.Sql.Storage
{
    public class ConcurrencyExceptionSpecification : IConcurrencyExceptionSpecification
    {
        public bool IsSatisfiedBy(Exception exception)
        {
            return exception != null &&
                   exception.Message.ToLower().Contains("violation of primary key");
        }
    }
}