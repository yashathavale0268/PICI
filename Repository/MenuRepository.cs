using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace PICI.Repository
{
    public class MenuRepository
    {
        private readonly string _connectionString;
        public bool Itexists { get; set; }
        public bool IsSuccess { get; set; }
        public MenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainCon");
        }
        //internal async Task<List<MenuModel>> GetAll()
        //{
        //    using (SqlConnection sql = new(_connectionString))
        //    {
        //        using (SqlCommand cmd = new("sp_GetAllUserDetails", sql))
        //        {

        //            cmd.CommandType = CommandType.StoredProcedure;
        //            var response = new List<MenuModel>();
        //            await sql.OpenAsync();

        //            using (var reader = await cmd.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    response.Add(MapToValue(reader));
        //                }
        //            }

        //            return response;
        //        }
        //    }
        //}

        //internal async Task<List<MenuModel>> GetAllDetails(int pageNumber, int pageSize)
        //{
        //    using SqlConnection sql = new(_connectionString);
        //    using SqlCommand cmd = new("sp_GetAllUserDetails", sql);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
        //    cmd.Parameters.AddWithValue("@PageSize", pageSize);
        //    var response = new List<MenuModel>();
        //    await sql.OpenAsync();

        //    using (var reader = await cmd.ExecuteReaderAsync())
        //    {
        //        while (await reader.ReadAsync())
        //        {
        //            response.Add(MapToValue(reader));
        //        }
        //    }

        //    return response;
        //}

        public DataSet GetAllForProjects()
        {
            using SqlConnection sql = new(_connectionString);
            using SqlCommand cmd = new("sp_GetAllForProjects", sql);
            {

                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);

                return dataSet;
            }
        }
        internal DataSet GetMenu(int role)//, int proj,int userid
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllMenus", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Roleid", role);
                    //cmd.Parameters.AddWithValue("@Projid", proj);
                    //cmd.Parameters.AddWithValue("@uid", userid);


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dataSet = new();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
        }

        public void NewMenu(int Menuid,string Menu,string Description,string PageUrl,string Icon, int sortorder) 
        {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllMenus", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", Menuid));
                        cmd.Parameters.Add(new SqlParameter("@Menu", Menu));
                        cmd.Parameters.Add(new SqlParameter("@Description", Description));
                        cmd.Parameters.Add(new SqlParameter("@PageUrl", PageUrl));
                        cmd.Parameters.Add(new SqlParameter("@Icon", Icon));
                        cmd.Parameters.Add(new SqlParameter("@sortorder", sortorder));
                        

                        // cmd.Parameters.Add(new SqlParameter("@Created_at", etype.Created_at));
                        var returncode = new SqlParameter("@Exists", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(returncode);
                        var returnpart = new SqlParameter("@success", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(returnpart);

                        sql.Open();
                        cmd.ExecuteNonQuery();
                        bool itExists = returncode?.Value is not DBNull && (bool)returncode.Value;
                        bool isSuccess = returnpart?.Value is not DBNull && (bool)returnpart.Value;
                        sql.Close();
                        Itexists = itExists;
                        IsSuccess = isSuccess;
                        

                    }
                return;
            }

        }

        //public MenuModel MapToValue(SqlDataReader reader)
        //{
        //    return new MenuModel()
        //        {
                
        //            MenuId = (Int64)reader["MenuId"],
        //        Menu = (string)reader["Menu"],
        //        PageUrl = (string)reader["Menu"],
        //    };
            
        //}

        

        

      

       
    }
}
