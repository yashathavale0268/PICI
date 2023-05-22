using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PICI.Models;

namespace PICI.Repository
{
    public class ProjectRepository
    {
        private readonly string _connectionString;
        public bool Itexists { get; set; }
        public bool IsSuccess { get; set; }
        public ProjectRepository(IConfiguration configuration)
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

        internal DataSet SearchProject(int pageNumber, int pageSize, string searchTerm)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SearchAllProject_Paginated", sql))
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

        internal DataSet GetProjectid(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SearchAllProject_Paginated", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);


                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet dataSet = new();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
        }

        public void Insert(ProjectModel proj)
        {
            using SqlConnection sql = new(_connectionString);
            using SqlCommand cmd = new("sp_ProjectCreate", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@id", proj.Pmid));
            cmd.Parameters.Add(new SqlParameter("@Pid", proj.Pid));
            // cmd.Parameters.Add(new SqlParameter("@CreatedDate", proj.CreatedDate));
            cmd.Parameters.Add(new SqlParameter("@CID", proj.CID));
            cmd.Parameters.Add(new SqlParameter("@Name", proj.Name));
            cmd.Parameters.Add(new SqlParameter("@Techstack", proj.Techstack));
            cmd.Parameters.Add(new SqlParameter("@PMName", proj.PMName));
            cmd.Parameters.Add(new SqlParameter("@Type", proj.Type));
            cmd.Parameters.Add(new SqlParameter("@Type", proj.CreatedBy));
            cmd.Parameters.Add(new SqlParameter("@Type", proj.UpdatedBy));


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
            using SqlCommand cmd = new("sp_DeleteProject", sql);
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

        public async Task<SenderMail> CheckEmails(SenderMail mail)
        {
            using SqlConnection sql = new(_connectionString);
            using SqlCommand cmd = new("sp_checkemail", sql);
            {

                cmd.CommandType = CommandType.StoredProcedure;

                await sql.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        mail = MapToValue(reader);
                    }
                }

                return mail;
            }
        }

        

        internal void SendUpdatesEmail(SenderMail mail)
        {
            if (mail.CreatorName is not null)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Kayley Rosenbaum", "kayley.rosenbaum19@ethereal.email"));
                message.To.Add(new MailboxAddress("creator", mail.Email1));
                message.To.Add(new MailboxAddress("reciever", mail.Email2));
                message.Subject = "New Project Added";
                message.Body = new TextPart("plain")
                {
                    Text = $" A new project was created by " + mail.CreatorName + $"with Project ID " + mail.Pid + $" AND Name" +
                    mail.SubjectName + $"Created on " + mail.Created_on.Date
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                     client.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                     client.Authenticate("kayley.rosenbaum19@ethereal.email", "C95Zf2Cpb46SkgyJW6");
                    client.Send(message);
                     client.Disconnect(true);
                }
            }
            else if (mail.UpdaterName is not null) {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Kayley Rosenbaum", "kayley.rosenbaum19@ethereal.email"));
                message.To.Add(new MailboxAddress("Updater", mail.Email1));
                message.To.Add(new MailboxAddress("reciever", mail.Email2));
                message.Subject = "New Project Added";
                message.Body = new TextPart("plain")
                {
                    Text = $" A Entry for Project ID" + mail.Pid + 
                    $" AND Name" +mail.SubjectName + 
                    $"by " + mail.UpdaterName + 
                    $"Updated on " + mail.Updated_on.Date
                };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                     client.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                    client.Authenticate("kayley.rosenbaum19@ethereal.email", "C95Zf2Cpb46SkgyJW6");
                     client.Send(message);
                     client.Disconnect(true);
                }
            }
        }

        internal SenderMail MapToValue(SqlDataReader reader)
        {
                return new SenderMail()
                {
                    Email1= reader.IsDBNull(reader.GetOrdinal("Email1")) ? null : (string)reader["Email1"],
                    Email2 = reader.IsDBNull(reader.GetOrdinal("Email2")) ? null : (string)reader["Email2"],
                    //Userid = (int)reader["UserId"],
                    //Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : (string)reader["Email"],
                    //Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : (string)reader["Username"],
                    //First_name = reader.IsDBNull(reader.GetOrdinal("First_name")) ? null : (string)reader["First_name"],
                    //Last_name = reader.IsDBNull(reader.GetOrdinal("Last_name")) ? null : (string)reader["Last_name"],
                    ////Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? 0 : (int)reader["Department"],
                    ////Branch = reader.IsDBNull(reader.GetOrdinal("Branch")) ? 0 : (int)reader["Branch"],
                    ////Floor = reader.IsDBNull(reader.GetOrdinal("Floor")) ? null : (string)reader["Floor"],
                    ////Company = reader.IsDBNull(reader.GetOrdinal("Company")) ? 0 : (int)reader["Company"],
                    //Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? 0 : (int)reader["Role"],
                    //RoleName = reader.IsDBNull(reader.GetOrdinal("RoleName")) ? null : (string)reader["RoleName"],
                    ////Created_at = (reader["Created_at"] != DBNull.Value) ? Convert.ToDateTime(reader["Created_at"]) : DateTime.MinValue,
                    ////active = (bool)reader["active"],
                    ////DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? null : (string)reader["DepartmentName"],
                    ////CompanyName = reader.IsDBNull(reader.GetOrdinal("CompanyName")) ? null : (string)reader["CompanyName"],
                    ////BranchName = reader.IsDBNull(reader.GetOrdinal("BranchName")) ? null : (string)reader["BranchName"],
                    //Full_name = reader.IsDBNull(reader.GetOrdinal("Full_name")) ? null : (string)reader["Full_name"],
                    //totalrecord = reader.IsDBNull(reader.GetOrdinal("totalrecord")) ? 0 : (int)reader["totalrecord"]
                };
            }

            //internal async Task<UserModel> GetByemail(SenderMail email)
            //{
            //    using (SqlConnection sql = new(_connectionString))
            //    using (SqlCommand cmd = new("sp_checkemail", sql))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.AddWithValue("@uid");

            //        var returncode = new SqlParameter("@exists", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            //        cmd.Parameters.Add(returncode);
            //        await sql.OpenAsync();
            //        UserModel response = new();
            //        // var response = new List<AssetModel>();
            //        using (var reader = await cmd.ExecuteReaderAsync())
            //        {
            //            while (await reader.ReadAsync())
            //            {
            //                response = getnamebyemail(reader);
            //            }
            //        }
            //        await sql.CloseAsync();

            //        bool itexists = returncode?.Value is not DBNull && (bool)returncode.Value;

            //        Itexists = itexists;


            //        return response;
            //    }

            //}

        }
}