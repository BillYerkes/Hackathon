using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Canoe.Models
{
    public class User
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        public int ID { get; set; }
        public string EmailAddress { get; set; }
        public bool Locked { get; set; }
        public int InvalidLogInAttempts { get; set; }
        public Nullable<System.DateTime> LastDateTimeLoggedIn { get; set; }
        public bool Confirmed { get; set; }
        public int AccessLevelCD { get; set; }
        public bool Deleted { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public bool IsValid(string _username, string _password)
        {

            using (var cn = new SqlConnection("Server = YERKES2\\SQLEXPRESS; Database = CSC451Capstone; persist security info=True;user id=WebApp;password=1;MultipleActiveResultSets=true"))

            {
                string _sql = @"SELECT [Name], [AccessLevelCD] FROM [dbo].[Users] " +
                       @"WHERE [UserID] = @u AND [Password] = @p";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cmd.Parameters
                    .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                    .Value = _password; //Helpers.SHA1.Encode(_password);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    AccessLevelCD = Convert.ToInt32(reader[AccessLevelCD]);

                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }

    }



}

