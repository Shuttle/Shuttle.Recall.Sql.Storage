using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventSpecification
{
    private readonly List<Type> _eventTypes = new();
    private readonly List<Guid> _ids = new();
    private List<long> _sequenceNumbers = new();

    public IEnumerable<Type> EventTypes => _eventTypes.AsReadOnly();
    public bool HasEventTypes => _eventTypes.Any();

    public IEnumerable<Guid> Ids => _ids.AsReadOnly();
    public bool HasIds => _ids.Any();
    public long SequenceNumberStart { get; private set; }
    public int MaximumRows { get; private set; }
    public long SequenceNumberEnd { get; private set; }
    public IEnumerable<long> SequenceNumbers => _sequenceNumbers;
    public bool HasSequenceNumbers => _sequenceNumbers.Any();

    public PrimitiveEventSpecification AddEventType<T>()
    {
        AddEventType(typeof(T));

        return this;
    }

    public PrimitiveEventSpecification AddEventType(Type type)
    {
        Guard.AgainstNull(type, nameof(type));

        if (!_eventTypes.Contains(type))
        {
            _eventTypes.Add(type);
        }

        return this;
    }

    public PrimitiveEventSpecification AddEventTypes(IEnumerable<Type> types)
    {
        foreach (var type in types ?? Enumerable.Empty<Type>())
        {
            AddEventType(type);
        }

        return this;
    }

    public PrimitiveEventSpecification AddId(Guid id)
    {
        Guard.AgainstNull(id, nameof(id));

        if (!_ids.Contains(id))
        {
            _ids.Add(id);
        }

        return this;
    }

    public PrimitiveEventSpecification AddSequenceNumber(long sequenceNumber)
    {
        if (!_sequenceNumbers.Contains(sequenceNumber))
        {
            _sequenceNumbers.Add(sequenceNumber);
        }

        return this;
    }

    public PrimitiveEventSpecification AddSequenceNumbers(IEnumerable<long> sequenceNumbers)
    {
        foreach (var sequenceNumber in sequenceNumbers)
        {
            AddSequenceNumber(sequenceNumber);
        }

        return this;
    }

    public PrimitiveEventSpecification WithSequenceNumbers(IEnumerable<long> sequenceNumbers)
    {
        _sequenceNumbers = [..sequenceNumbers];

        return this;
    }

    public PrimitiveEventSpecification AddIds(IEnumerable<Guid> ids)
    {
        foreach (var type in ids ?? Enumerable.Empty<Guid>())
        {
            AddId(type);
        }

        return this;
    }

    public PrimitiveEventSpecification WithSequenceNumberEnd(long sequenceNumberEnd)
    {
        SequenceNumberEnd = sequenceNumberEnd;

        return this;
    }

    public PrimitiveEventSpecification WithSequenceNumberStart(long sequenceNumberStart)
    {
        SequenceNumberStart = sequenceNumberStart;

        return this;
    }

    public PrimitiveEventSpecification WithMaximumRows(int maximumRows)
    {
        MaximumRows = maximumRows;

        return this;
    }
}