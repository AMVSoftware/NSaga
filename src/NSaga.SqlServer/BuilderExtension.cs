using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSaga.SqlServer
{
    public static class BuilderExtension
    {
        public static SagaMediatorBuilder UseSqlServerStorage(this SagaMediatorBuilder builder, string connectionString)
        {
            //TODO register connectionstring for repository. DUH!

            builder.UseRepository<SqlSagaRepository>();
            return builder;
        }
    }
}
