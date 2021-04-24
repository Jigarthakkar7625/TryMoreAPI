using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using TryMoreAPI.Models;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Net.Http;
//using TherapistAPI.Common;

namespace TryMoreAPI.DataAccess
{
    public class RegisterDL
    {
        SqlConnection conn = new
                        SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        Common objCommon = new Common();



        public UsersModel UserLogin(UsersModel UserLogin)
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("SP_UserLogin", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmailId", UserLogin.Email);
            cmd.Parameters.AddWithValue("@Password", UserLogin.Password);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            List<UsersModel> lstUsers = new List<UsersModel>();
            lstUsers = objCommon.ConvertDataTable<UsersModel>(ds.Tables[0]);

            if (lstUsers.Count == 0)
            {
                throw new InvalidOperationException("Incorrect UserName and Password!");
            }
            
            return lstUsers[0];
        }

        public List<UsersModel> GetAllUsers(UsersModel users)
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("SP_GetAllUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserType", users.UserType);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            List<UsersModel> lstUsers = new List<UsersModel>();
            lstUsers = objCommon.ConvertDataTable<UsersModel>(ds.Tables[0]);

            return lstUsers;
        }

        


    }
}