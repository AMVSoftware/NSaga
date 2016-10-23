using System;
using System.Collections.Generic;
using NSaga.SqlServer;
using PetaPoco;

namespace Tests.SqlServer
{
    public static class DatabaseHelpers
    {
        public static SagaData GetSagaData(Database database, Guid correlationId)
        {
            var data = database.SingleOrDefault<SagaData>($"select * from {SqlSagaRepository.SagaDataTableName} where correlationId = @0", correlationId);

            return data;
        }

        public static IEnumerable<SagaHeaders> GetSagaHeaders(Database database,  Guid correlationId)
        {
            var headers = database.Query<SagaHeaders>($"select * from {SqlSagaRepository.HeadersTableName} where correlationId = @0", correlationId);

            return headers;
        }
    }
    [TableName("NSaga.Sagas")]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    public class SagaData
    {
        public Guid CorrelationId { get; set; }
        public String BlobData { get; set; }
    }

    [TableName("NSaga.Headers")]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    public class SagaHeaders
    {
        public Guid CorrelationId { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
    }
}
