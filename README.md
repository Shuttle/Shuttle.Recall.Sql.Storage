# Shuttle.Recall.Sql.Storage

A Sql Server implementation of the `Shuttle.Recall` event sourcing `EventStore`.

### Event Sourcing

~~~ c#
// use any of the supported DI containers
var container = new WindsorComponentContainer(new WindsorContainer());

EventStore.Register(container);

var eventStore = EventStore.Create(container);
~~~

