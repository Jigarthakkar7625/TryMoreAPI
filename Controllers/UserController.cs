using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using TryMoreAPI.Models;
using TryMoreAPI.DataAccess;
using System.Threading.Tasks;
using ExcelDataReader;
using System.Data;

namespace TryMoreAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();
        TryMoreEntities entities = new TryMoreEntities();
        //therapistEntities entities = new therapistEntities();

        [HttpPost]
        [Route("GetUsers")]
        public IHttpActionResult GetAllUsers(UsersModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                //var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                RegisterDL obj = new RegisterDL();
                List<UsersModel> list = obj.GetAllUsers(model);

                //list.success = true;
                //list.message = "Get booking details Successfully";
                //list.code = 200;

                return Ok(list);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("SellerApprove")]
        public IHttpActionResult SellerApprove(int userId, int approve)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                //var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                RegisterDL obj = new RegisterDL();
                BaseModel baseModel = new BaseModel();
                var getUsers = entities.tblUsers.Where(x => x.UserID == userId).FirstOrDefault();
                if (getUsers != null)
                {
                    getUsers.Approve = approve;
                    entities.SaveChanges();
                }
                baseModel.success = true;
                baseModel.message = "Save Successfully";
                baseModel.code = 200;

                return Ok(baseModel);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                RegisterDL obj = new RegisterDL();
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                int userIDConverted = Convert.ToInt32(UserId);

                var getUsers = entities.tblUsers.Where(x => x.UserID == userIDConverted).FirstOrDefault();
                if (getUsers != null)
                {
                    if (getUsers.Password == model.OldPassword)
                    {
                        getUsers.Password = model.NewPassword;
                        entities.SaveChanges();
                    }
                    else
                    {
                        responseData.success = false;
                        responseData.code = 500;
                        responseData.message = "Old password is not correct!";
                        return Ok(responseData);
                    }
                }
                responseData.success = true;
                responseData.code = 200;
                responseData.message = "Password changed successfully";
                return Ok(responseData);

            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("UpdateProfile")]
        public IHttpActionResult UpdateProfile(UsersModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                int userIDConverted = Convert.ToInt32(UserId);

                var getUsers = entities.tblUsers.Where(x => x.UserID == userIDConverted).FirstOrDefault();

                if (getUsers != null)
                {
                    var getEmail = entities.tblUsers.Where(x => x.UserID != userIDConverted && x.Email.ToLower() == model.Email.ToLower()).FirstOrDefault();

                    if (getEmail != null)
                    {
                        responseData.success = false;
                        responseData.code = 500;
                        responseData.message = "Email is already exist!";
                        return Ok(responseData);
                    }

                    var getUserName = entities.tblUsers.Where(x => x.UserID != userIDConverted && x.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                    if (getUserName != null)
                    {
                        responseData.success = false;
                        responseData.code = 500;
                        responseData.message = "User Name is already exist!";
                        return Ok(responseData);
                    }

                    var getPhone = entities.tblUsers.Where(x => x.UserID != userIDConverted && x.Phone.ToLower() == model.Phone.ToLower()).FirstOrDefault();
                    if (getPhone != null)
                    {
                        responseData.success = false;
                        responseData.code = 500;
                        responseData.message = "Phone number is already exist!";
                        return Ok(responseData);
                    }

                    getUsers.FirstName = model.FirstName;
                    getUsers.LastName = model.LastName;
                    getUsers.UserName = model.UserName;
                    getUsers.Email = model.Email;
                    getUsers.Phone = model.Phone;
                    getUsers.PostalCode = model.PostalCode;
                    getUsers.AddrLine1 = model.AddrLine1;
                    getUsers.AddrLine2 = model.AddrLine2;
                    getUsers.City = model.City;
                    getUsers.State = model.State;
                    getUsers.Country = model.Country;
                    getUsers.Gender = model.Gender;
                    getUsers.ProfileImage = model.ProfileImage;
                    getUsers.ModifiedOn = DateTime.Now;
                    getUsers.ModifiedBy = userIDConverted;
                    entities.SaveChanges();
                }
                else
                {
                    responseData.success = false;
                    responseData.code = 500;
                    responseData.message = "No User Found!";
                    return Ok(responseData);
                }
                responseData.success = true;
                responseData.code = 200;
                responseData.message = "Profile Update Successfully.";
                return Ok(responseData);

            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpGet]
        [Route("GetUsersProfile")]
        public IHttpActionResult GetUsersProfile()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                int userIDConverted = Convert.ToInt32(UserId);

                var getUsers = entities.tblUsers.Where(x => x.UserID == userIDConverted).FirstOrDefault();

                //list.success = true;
                //list.message = "Get booking details Successfully";
                //list.code = 200;

                return Ok(getUsers);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("UsersUpdateProfile")]
        public IHttpActionResult UsersUpdateProfile(UsersModel usersModel)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                int userIDConverted = Convert.ToInt32(UserId);

                var getUsers = entities.tblUsers.Where(x => x.UserID == userIDConverted).FirstOrDefault();
                if (getUsers != null)
                {
                    getUsers.FirstName = usersModel.FirstName;
                    getUsers.LastName = usersModel.LastName;
                    getUsers.UserName = usersModel.UserName;
                    getUsers.Email = usersModel.Email;
                    getUsers.Phone = usersModel.Phone;
                    getUsers.AddrLine1 = usersModel.AddrLine1;
                    getUsers.AddrLine2 = usersModel.AddrLine2;
                    getUsers.PostalCode = usersModel.PostalCode;
                    getUsers.City = usersModel.City;
                    getUsers.Country = usersModel.Country;

                    if (usersModel.isFiles)
                    {
                        string fileName = "/Uploads/" + UserId + "/" + Guid.NewGuid() + Path.GetExtension(usersModel.FileExt);
                        getUsers.ProfileImage = fileName;
                    }

                    entities.SaveChanges();
                }
                getUsers = entities.tblUsers.Where(x => x.UserID == userIDConverted).FirstOrDefault();

                return Ok(getUsers);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }


        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }



        [Route("Upload")]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                HttpFileCollection files = HttpContext.Current.Request.Files;
                Stream FileStream = null;
                IExcelDataReader reader = null;
                System.Web.HttpPostedFile Inputfile = files[0];
                FileStream = Inputfile.InputStream;
                string message = "";
                DataSet dsexcelRecords = new DataSet();
                List<tblBrand> objBrand = new List<tblBrand>();
                BaseModel baseModel = new BaseModel();

                //using (DbContextTransaction transaction = entities.Database.BeginTransaction())
                //{

                if (Inputfile != null && FileStream != null)
                {
                    if (Inputfile.FileName.EndsWith(".xls"))
                        reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                    else if (Inputfile.FileName.EndsWith(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                    else
                        message = "The file format is not supported.";

                    dsexcelRecords = reader.AsDataSet();
                    reader.Close();

                    if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                    {
                        DataTable dtSellerRecords = dsexcelRecords.Tables[0];
                        for (int i = 0; i < dtSellerRecords.Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                tblUser objB = new tblUser();
                                objB.FirstName = Convert.ToString(dtSellerRecords.Rows[i][0]);
                                objB.LastName = Convert.ToString(dtSellerRecords.Rows[i][1]);
                                string userName = Convert.ToString(dtSellerRecords.Rows[i][2]);
                                if (userName == "")
                                {
                                    baseModel.success = false;
                                    baseModel.message = "User Name is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }
                                else
                                {
                                    var getUserName = entities.tblUsers.Where(x => x.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
                                    if (getUserName != null)
                                    {
                                        baseModel.success = false;
                                        baseModel.message = "User Name " + getUserName + " already exist!";
                                        baseModel.code = 500;
                                        return Ok(baseModel);
                                    }
                                    else
                                    {
                                        objB.UserName = Convert.ToString(dtSellerRecords.Rows[i][2]);
                                    }

                                }

                                string password = Convert.ToString(dtSellerRecords.Rows[i][3]);
                                if (password == "")
                                {
                                    baseModel.success = false;
                                    baseModel.message = "Password is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }
                                else
                                {
                                    objB.Password = Convert.ToString(dtSellerRecords.Rows[i][3]);
                                }


                                string email = Convert.ToString(dtSellerRecords.Rows[i][4]);
                                if (email == "")
                                {
                                    baseModel.success = false;
                                    baseModel.message = "Email is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }
                                else
                                {
                                    var getEmail = entities.tblUsers.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
                                    if (getEmail != null)
                                    {
                                        baseModel.success = false;
                                        baseModel.message = "Email " + getEmail + " already exist!";
                                        baseModel.code = 500;
                                        return Ok(baseModel);
                                    }
                                    else
                                    {
                                        objB.Email = Convert.ToString(dtSellerRecords.Rows[i][4]);
                                    }

                                }



                                objB.Phone = Convert.ToString(dtSellerRecords.Rows[i][5]);
                                objB.PostalCode = Convert.ToString(dtSellerRecords.Rows[i][6]);
                                objB.Age = Convert.ToInt32(dtSellerRecords.Rows[i][7]);
                                objB.AddrLine1 = Convert.ToString(dtSellerRecords.Rows[i][8]);
                                objB.AddrLine2 = Convert.ToString(dtSellerRecords.Rows[i][9]);
                                objB.City = Convert.ToString(dtSellerRecords.Rows[i][10]);
                                objB.State = Convert.ToString(dtSellerRecords.Rows[i][11]);
                                objB.Country = Convert.ToString(dtSellerRecords.Rows[i][12]);
                                string gender = Convert.ToString(dtSellerRecords.Rows[i][13]);
                                if (gender.ToUpper() == "MALE") {
                                    objB.Gender = 1;
                                }
                                else if (gender.ToUpper() == "FEMALE")
                                {
                                    objB.Gender = 2;

                                }
                                objB.Approve = 1;
                                objB.UserType = 2;
                                entities.tblUsers.Add(objB);
                            }

                        }

                        entities.SaveChanges();
                    }
                }



                baseModel.success = true;
                baseModel.message = "File upload successfully.";
                baseModel.code = 200;
                return Ok(baseModel);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

    }
}
