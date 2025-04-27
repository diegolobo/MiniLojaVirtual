namespace MiniLojaVirtual.Domain.Constants;

public static class DomainConstants
{
	#region Infrastructure

	public const string DefaultConnectionStringName = "UNIQUEIDENTIFIER";

	public const string CodeColumnTypeName = "UNIQUEIDENTIFIER";
	public const string DefaultDateTimeColumnTypeName = "DATETIME(7)";
	public const string DefaultNVarcharColumnTypeName = "NVARCHAR(50)";
	public const string DefaultVarcharColumnTypeName = "VARCHAR(50)";
	public const string DefaultBooleanColumnTypeName = "BIT";

	public const short PasswordRequiredLength = 8;
	public const short LockoutMaxFailedAccessAttempts = 3;

	#endregion
}