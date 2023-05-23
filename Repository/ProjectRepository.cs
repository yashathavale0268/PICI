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
            cmd.Parameters.Add(new SqlParameter("@CreatedBy", proj.CreatedBy));
            cmd.Parameters.Add(new SqlParameter("@UpdatedBy", proj.UpdatedBy));


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

        public SenderMail CheckEmails(string type)
        {
            using SqlConnection sql = new(_connectionString);
            using SqlCommand cmd = new("sp_checkemail", sql);
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Type",type));
                sql.Open();
                SenderMail mail = new();
                using (var reader =  cmd.ExecuteReader())
                {
                    while ( reader.Read())
                    { 
                        mail = MapToValue(reader);
                    }
                }

                return mail;
            }
        }

        

        internal void SendUpdatesEmail(SenderMail mail)
        {
            if (mail.Type=="Create")
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Kayley Rosenbaum", "kayley.rosenbaum19@ethereal.email"));
                message.To.Add(new MailboxAddress("creator", mail.Email1));
                message.To.Add(new MailboxAddress("reciever", mail.Email2));

                var templatePath = mail.TemplateBody;
                var templateContent = System.IO.File.ReadAllText(templatePath);
                var modifiedContent = templateContent//.Replace("{{Placeholder}}", "Dynamic Content")
                    .Replace("{{CreatorName}}", mail.CreatorName)
                    .Replace("{{Pid}}", mail.Pid)
                    .Replace("{{SubjectName}}", mail.SubjectName)
                    .Replace("{{Created_on}}", mail.Created_on.ToString("yyyy-mm-dd"));


                // Set the HTML body
                var body = new TextPart("html")
                {
                    Text = modifiedContent
                };
                message.Body = body;
                message.Subject = mail.Subject;
                //message.Body = new TextPart("plain")
                //{
                //    Text = $" A new project was created by " + mail.CreatorName + 
                //    $" " +
                //    $"with Project ID " + mail.Pid + $" " +
                //    $" " +
                //    $"AND Name " + mail.SubjectName + $"  " +
                //    $" " +
                //    $"Created on " + mail.Created_on.Date
                //};

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                     client.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                     client.Authenticate("kayley.rosenbaum19@ethereal.email", "C95Zf2Cpb46SkgyJW6");
                    client.Send(message);
                     client.Disconnect(true);
                }
            }
            else if (mail.Type == "Update") {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Kayley Rosenbaum", "kayley.rosenbaum19@ethereal.email"));
                message.To.Add(new MailboxAddress("Updater", mail.Email1));
                message.To.Add(new MailboxAddress("reciever", mail.Email2));
                string templatePath = mail.TemplateBody;
                var templateContent = templatePath;
                var modifiedContent = templateContent//.Replace("{{Placeholder}}", "Dynamic Content")
                    .Replace("{{UpdaterName}}", mail.UpdaterName)
                    .Replace("{{Pid}}", mail.Pid)
                    .Replace("{{SubjectName}}", mail.SubjectName)
                    .Replace("{{Updated_on}}", mail.Updated_on.ToString("yyyy-mm-dd"));


                // Set the HTML body
                var body = new TextPart("html")
                {
                    Text = modifiedContent
                };
                message.Body = body;
                message.Subject = mail.Subject;

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
                    SubjectName= reader.IsDBNull(reader.GetOrdinal("SubjectName")) ? null : (string)reader["SubjectName"],
                    Creatorid= reader.IsDBNull(reader.GetOrdinal("Creatorid")) ? 0 : (int)reader["Creatorid"],
                     Updaterid= reader.IsDBNull(reader.GetOrdinal("Updaterid")) ? 0 : (int)reader["Updaterid"],
                    Recieverid = reader.IsDBNull(reader.GetOrdinal("Recieverid")) ? 0 : (Int64)reader["Recieverid"],
                    Reciever = reader.IsDBNull(reader.GetOrdinal("Reciever")) ? null : (string)reader["Reciever"],
                    Created_on = (reader["Created_on"] != DBNull.Value) ? Convert.ToDateTime(reader["Created_on"]) : DateTime.MinValue,
                    Updated_on = (reader["Updated_on"] != DBNull.Value) ? Convert.ToDateTime(reader["Updated_on"]) : DateTime.MinValue,
                    UpdaterName = reader.IsDBNull(reader.GetOrdinal("UpdaterName")) ? null : (string)reader["UpdaterName"],
                    CreatorName = reader.IsDBNull(reader.GetOrdinal("CreatorName")) ? null : (string)reader["CreatorName"],
                    Pid= reader.IsDBNull(reader.GetOrdinal("Pid")) ? null : (string)reader["Pid"],
                    Type =reader.IsDBNull(reader.GetOrdinal("Type"))? null :(string)reader["Type"],
                    Subject = reader.IsDBNull(reader.GetOrdinal("Subject")) ? null : (string)reader["Subject"],
                    TemplateBody = reader.IsDBNull(reader.GetOrdinal("TemplateBody")) ? null : (string)reader["TemplateBody"],

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