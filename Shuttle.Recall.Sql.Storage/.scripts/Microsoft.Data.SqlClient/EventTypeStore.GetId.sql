merge
	EventType t
using
	(
		select @TypeName
	) s (TypeName)
on
	s.TypeName = t.TypeName
when not matched by target then
	insert
	(
		Id,
		TypeName
	)
	values
	(
		newid(),
		@TypeName
	);

select
	Id
from
	EventType
where
	TypeName = @TypeName