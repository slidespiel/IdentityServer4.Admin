using System.Reflection;

using SqlMigrationAssembly = Skoruba.IdentityServer4.Admin.EntityFramework.SqlServer.Helpers.MigrationAssembly;

namespace Skoruba.IdentityServer4.Admin.Configuration.Database
{
    public static class MigrationAssemblyConfiguration
    {
        public static string GetMigrationAssemblyByProvider()
        {
            return typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name;
        }
    }
}