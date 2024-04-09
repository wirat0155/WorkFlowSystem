using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using Dapper;
using Microsoft.Data.SqlClient;

namespace WorkFlowSystem.Services
{
    public class DapperService
    {
        private readonly string _connectionString;

        public DapperService(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task Execute(string query, object param = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(query, param, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        public async Task<IEnumerable<dynamic>> Query(string query, object param = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<dynamic>(query, param);
        }

        public async Task<dynamic> QueryFirstOrDefaultAsync(string query, object param = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<dynamic>(query, param);
        }
    }
}
