using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DapperBaseRepository
    {

        private readonly IConfiguration _configuration;

        public DapperBaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object parameters = null)
        {
            try
            {
                using NpgsqlConnection conn
                       = new(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                return  await conn.QueryFirstOrDefaultAsync<T>(query, parameters);
            }
            catch (Exception)
            {
                //Handle the exception
                throw;
            }
        }


        public async Task<bool> InsertData(string query, object parameters = null)
        {
            try
            {
                using NpgsqlConnection conn
                       = new(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                // it returns affected count, if affected count is greater than zero then its saved
                var affected = await conn.ExecuteAsync(query, parameters);

                if (affected ==0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                //Handle the exception
                throw;
            }
        }

        public async Task<bool> UpdateData(string query, object parameters = null)
        {
            try
            {
                using NpgsqlConnection conn
                       = new(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                // it returns affected count, if affected count is greater than zero then its updated
                var affected = await conn.ExecuteAsync(query, parameters);

                if (affected == 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                //Handle the exception
                throw;
            }
        }

        public async Task<bool> DeleteData(string query, object parameters = null)
        {
            try
            {
                using NpgsqlConnection conn
                       = new(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                var affected = await conn.ExecuteAsync(query, parameters);

                if (affected == 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                //Handle the exception
                throw;
            }
        }
    }
}
