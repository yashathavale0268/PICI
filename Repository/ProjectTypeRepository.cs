using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PICI.Models;
using Microsoft.Extensions.Configuration;

namespace PICI.Repository
{ 
    public class ProjectTypeRepository
    {
        private readonly string _connectionString;
        public bool Itexists { get; set; }
        public bool IsSuccess { get; set; }
        public ProjectTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainCon");
        }

        internal DataSet GetRolePerms(int role, int Menu)
        {
            using (SqlConnection sql = new(_connectionString))
            {
                using (SqlCommand cmd = new("sp_GetRolePerms", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@menuid", Menu);

                    SqlDataAdapter adapter = new(cmd);
                    DataSet dataSet = new();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
        }
        internal DataSet SearchProjType(int pageNumber, int pageSize, string searchTerm)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SearchAllProjectType_Paginated", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    cmd.Parameters.AddWithValue("@SearchTerm", searchTerm);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dataSet = new();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
        }

        internal DataSet GetProjTypebyId(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SearchAllProjectType_Paginated", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);


                    SqlDataAdapter adapter = new(cmd);
                    DataSet dataSet = new();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
        }

        public void Insert(ProjecttypeModel ptype)
        {
            using SqlConnection sql = new(_connectionString);
            using SqlCommand cmd = new("sp_ProjectTypeCreate", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", ptype.ProjTypid));
            cmd.Parameters.Add(new SqlParameter("@Name", ptype.ProjTyp));

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
            return;
        }

        public void DeleteById(int id)
        {
            using SqlConnection sql = new(_connectionString);
            using SqlCommand cmd = new("sp_DeleteProjType", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", id));
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
            return;
        }
    }
}
