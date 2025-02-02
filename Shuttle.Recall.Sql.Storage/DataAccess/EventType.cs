using System;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage;

public class EventType
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = string.Empty;

    public class Specification
    {
        public string? TypeNameMatch { get; set; } = string.Empty;
        public int MaximumRows { get; private set; }

        public Specification WithTypeNameMatch(string typeNameMatch)
        {
            TypeNameMatch = Guard.AgainstNullOrEmptyString(typeNameMatch);

            return this;
        }

        public Specification WithMaximumRows(int maximumRows)
        {
            MaximumRows = maximumRows;

            return this;
        }
    }
}