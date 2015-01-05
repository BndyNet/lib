CREATE FUNCTION [dbo].[Function1]
(
	@param1 int,
	@param2 char(5)
)
RETURNS TABLE
(
	c1 int,
	c2 char(5)
)
AS EXTERNAL NAME SomeAssembly.SomeType.SomeMethod
