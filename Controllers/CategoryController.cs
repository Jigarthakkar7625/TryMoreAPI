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
    [RoutePrefix("api/Category")]
    public class CategoryController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();
        TryMoreEntities entities = new TryMoreEntities();
        //therapistEntities entities = new therapistEntities();

        [HttpGet]
        [Route("GetCategories")]
        public async Task<IHttpActionResult> GetCategories()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                return Ok(entities.tblCategories.ToList());
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("SaveCategory")]
        public async Task<IHttpActionResult> SaveCategory(tblCategory tblCategories)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                BaseModel baseModel = new BaseModel();

                if (tblCategories.CategoryID > 0)
                {
                    var checkCategoryName = entities.tblCategories.Where(x => x.CategoryID != tblCategories.CategoryID && x.CategoryName.ToLower() == tblCategories.CategoryName.ToLower()).ToList();
                    if (checkCategoryName != null && checkCategoryName.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "Category Name is already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    var getCategory = entities.tblCategories.Where(x => x.CategoryID == tblCategories.CategoryID).FirstOrDefault();

                    if (getCategory != null)
                    {
                        getCategory.CategoryName = tblCategories.CategoryName;
                        getCategory.CategoryDescription = tblCategories.CategoryDescription;
                        getCategory.ModifiedBy = Convert.ToInt32(UserId);
                        getCategory.ModifiedOn = DateTime.Now;
                        entities.SaveChanges();
                    }
                }
                else
                {
                    var checkCategoryName = entities.tblCategories.Where(x => x.CategoryName.ToLower() == tblCategories.CategoryName.ToLower()).ToList();
                    if (checkCategoryName != null && checkCategoryName.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "Category Name is already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    tblCategory tblCategories1 = new tblCategory();
                    tblCategories1.CategoryName = tblCategories.CategoryName;
                    tblCategories1.CategoryDescription = tblCategories.CategoryDescription;
                    tblCategories1.CreatedBy = Convert.ToInt32(UserId);
                    tblCategories1.CreatedOn = DateTime.Now;
                    entities.tblCategories.Add(tblCategories1);
                    entities.SaveChanges();
                }

                baseModel.success = true;
                baseModel.message = "Category Saved Successfully";
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
        [Route("GetCategoryId")]
        public async Task<IHttpActionResult> GetCategoryId(tblCategory tblCategories)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                if (tblCategories.CategoryID > 0)
                {
                    tblCategories = entities.tblCategories.Where(x => x.CategoryID == tblCategories.CategoryID).FirstOrDefault();

                    return Ok(tblCategories);

                }

                return Ok();
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
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
                        DataTable dtCategoryRecords = dsexcelRecords.Tables[0];
                        for (int i = 0; i < dtCategoryRecords.Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                tblCategory objB = new tblCategory();
                                objB.CategoryName = Convert.ToString(dtCategoryRecords.Rows[i][0]);

                                if (objB.CategoryName == "") {
                                    baseModel.success = false;
                                    baseModel.message = "Category Name is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }

                                var getCategoryName = entities.tblCategories.Where(x => x.CategoryName.ToLower() == objB.CategoryName.ToLower()).FirstOrDefault();
                                if (getCategoryName != null)
                                {

                                    baseModel.success = false;
                                    baseModel.message = "Category Name " + objB.CategoryName + " already exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }

                                objB.CategoryDescription = Convert.ToString(dtCategoryRecords.Rows[i][1]);
                                objB.CreatedBy = Convert.ToInt32(UserId);
                                objB.CreatedOn = DateTime.Now;
                                entities.tblCategories.Add(objB);
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
