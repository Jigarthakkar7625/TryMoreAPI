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
using System.Data.Entity;

namespace TryMoreAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Brand")]
    public class BrandController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();
        TryMoreEntities entities = new TryMoreEntities();
        //therapistEntities entities = new therapistEntities();

        [HttpGet]
        [Route("GetBrands")]
        public async Task<IHttpActionResult> GetBrands()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                return Ok(entities.tblBrands.ToList());
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("SaveBrand")]
        public async Task<IHttpActionResult> SaveBrand(tblBrand tblBrand)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                BaseModel baseModel = new BaseModel();

                if (tblBrand.BrandID > 0)
                {
                    var checkBrandName = entities.tblBrands.Where(x => x.BrandID != tblBrand.BrandID && x.BrandName.ToLower() == tblBrand.BrandName.ToLower()).ToList();
                    if (checkBrandName != null && checkBrandName.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "Brand Name is already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    var getBrand = entities.tblBrands.Where(x => x.BrandID == tblBrand.BrandID).FirstOrDefault();

                    if (getBrand != null)
                    {
                        getBrand.BrandName = tblBrand.BrandName;
                        getBrand.BrandDescription = tblBrand.BrandDescription;
                        getBrand.ModifiedBy = Convert.ToInt32(UserId);
                        getBrand.ModifiedOn = DateTime.Now;
                        entities.SaveChanges();
                    }
                }
                else
                {
                    var checkBrandName = entities.tblBrands.Where(x => x.BrandName.ToLower() == tblBrand.BrandName.ToLower()).ToList();
                    if (checkBrandName != null && checkBrandName.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "Brand Name is already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    tblBrand tblBrand1 = new tblBrand();
                    tblBrand1.BrandName = tblBrand.BrandName;
                    tblBrand1.BrandDescription = tblBrand.BrandDescription;
                    tblBrand1.CreatedBy = Convert.ToInt32(UserId);
                    tblBrand1.CreatedOn = DateTime.Now;
                    entities.tblBrands.Add(tblBrand1);
                    entities.SaveChanges();
                }

                baseModel.success = true;
                baseModel.message = "Brand Saved Successfully";
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
        [Route("GetBrandId")]
        public async Task<IHttpActionResult> GetBrandId(tblBrand tblBrand)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                if (tblBrand.BrandID > 0)
                {
                    tblBrand = entities.tblBrands.Where(x => x.BrandID == tblBrand.BrandID).FirstOrDefault();

                    return Ok(tblBrand);

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
                        DataTable dtBrandRecords = dsexcelRecords.Tables[0];
                        for (int i = 0; i < dtBrandRecords.Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                tblBrand objB = new tblBrand();
                                objB.BrandName = Convert.ToString(dtBrandRecords.Rows[i][0]);


                                if (objB.BrandName == "")
                                {
                                    baseModel.success = false;
                                    baseModel.message = "Brand Name is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }

                                var getBrandName = entities.tblBrands.Where(x => x.BrandName.ToLower() == objB.BrandName.ToLower()).FirstOrDefault();
                                if (getBrandName != null)
                                {

                                    baseModel.success = false;
                                    baseModel.message = "Brand Name " + objB.BrandName + " already exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }

                                objB.BrandDescription = Convert.ToString(dtBrandRecords.Rows[i][1]);
                                objB.CreatedBy = Convert.ToInt32(UserId);
                                objB.CreatedOn = DateTime.Now;
                                entities.tblBrands.Add(objB);
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
