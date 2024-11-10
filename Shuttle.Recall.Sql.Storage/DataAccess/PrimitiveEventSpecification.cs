using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;

namespace Shuttle.Recall.Sql.Storage;

public class PrimitiveEventSpecification
{
    private readonly List<Type> _eventTypes = new();
    private readonly List<Guid> _ids = new();
    public int Count { get; private set; }

    public IEnumerable<Type> EventTypes => _eventTypes.AsReadOnly();

    public IEnumerable<Guid> Ids => _ids.AsReadOnly();
    public long SequenceNumberStart { get; private set; }

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

    public PrimitiveEventSpecification AddIds(IEnumerable<Guid> ids)
    {
        foreach (var type in ids ?? Enumerable.Empty<Guid>())
        {
            AddId(type);
        }

        return this;
    }

    public PrimitiveEventSpecification WithRange(long sequenceNumberStart, int count)
    {
        SequenceNumberStart = sequenceNumberStart;
        Count = count;

        return this;
    }
}