update
	dbo.KeyStore
set
	[Key] = @Rekey
where
	[Key] = @Key