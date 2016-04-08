using System.Data.SqlTypes;
using DBTools;

public partial class DBToolsFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static double CalculateJaroWinkler(SqlString source, SqlString target)
    {
        return  JaroWinklerDistance.distance(source.Value, target.Value);
    }
}