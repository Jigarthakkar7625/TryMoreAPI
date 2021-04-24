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
    [RoutePrefix("api/Product")]
    public class ProductController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();
        TryMoreEntities entities = new TryMoreEntities();
        //therapistEntities entities = new therapistEntities();

        [HttpGet]
        [Route("GetProducts")]
        public async Task<IHttpActionResult> GetProducts()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var UserType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals("UserType")).Value;

                var getProduct = (from p in entities.tblProducts
                                  join c in entities.tblCategories on p.CategoryID equals (int?)c.CategoryID
                                  join b in entities.tblBrands on p.BrandID equals (int?)b.BrandID
                                  join u in entities.tblUsers on p.SellerID equals u.UserID
                                  select new
                                  {
                                      p.ProductID,
                                      p.ProductName,
                                      c.CategoryName,
                                      b.BrandName,
                                      p.IsApprove,
                                      SellerName = u.FirstName + " " + u.LastName

                                  }).ToList();

                return Ok(getProduct);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("SaveProduct")]
        public async Task<IHttpActionResult> SaveProduct(tblProduct tblProduct)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                tblProduct productResponse = new tblProduct();

                if (tblProduct.ProductID > 0)
                {
                    var checkProductName = entities.tblProducts.Where(x => x.ProductID != tblProduct.ProductID && x.SellerID != tblProduct.SellerID && x.ProductName.ToLower() == tblProduct.ProductName.ToLower()).ToList();
                    if (checkProductName != null && checkProductName.Count > 0)
                    {
                        productResponse.success = false;
                        productResponse.message = "Product Name is already exist!";
                        productResponse.code = 500;
                        return Ok(productResponse);
                    }

                    var getProduct = entities.tblProducts.Where(x => x.ProductID == tblProduct.ProductID).FirstOrDefault();

                    if (getProduct != null)
                    {
                        getProduct.ProductName = tblProduct.ProductName;
                        getProduct.ProductDescription = tblProduct.ProductDescription;
                        getProduct.CategoryID = tblProduct.CategoryID;
                        getProduct.BrandID = tblProduct.BrandID;
                        getProduct.QuantityPerUnit = tblProduct.QuantityPerUnit;
                        getProduct.UnitPrice = tblProduct.UnitPrice;
                        getProduct.Size = tblProduct.Size;
                        getProduct.Color = tblProduct.Color;
                        getProduct.Discount = tblProduct.Discount;
                        getProduct.UnitWeight = tblProduct.UnitWeight;
                        getProduct.UnitsInStack = tblProduct.UnitsInStack;
                        getProduct.Picture = tblProduct.Picture;
                        getProduct.Ranking = tblProduct.Ranking;
                        getProduct.Note = tblProduct.Note;
                        getProduct.ModifiedBy = Convert.ToInt32(UserId);
                        getProduct.ModifiedOn = DateTime.Now;
                        if (tblProduct.FileExt != "")
                        {
                            string fileName = "/" + UserId + "/" + Guid.NewGuid() + Path.GetExtension(tblProduct.FileExt);
                            getProduct.Picture = fileName;
                            productResponse.Picture = fileName;
                        }

                        entities.SaveChanges();
                    }
                }
                else
                {
                    tblProduct.SellerID = 3;
                    var checkProductName = entities.tblProducts.Where(x => x.ProductName.ToLower() == tblProduct.ProductName.ToLower() && x.SellerID == tblProduct.SellerID).ToList();
                    if (checkProductName != null && checkProductName.Count > 0)
                    {
                        productResponse.success = false;
                        productResponse.message = "Product Name is already exist!";
                        productResponse.code = 500;
                        return Ok(productResponse);
                    }

                    tblProduct getProduct1 = new tblProduct();
                    getProduct1.ProductName = tblProduct.ProductName;
                    getProduct1.ProductDescription = tblProduct.ProductDescription;
                    getProduct1.SellerID = tblProduct.SellerID;
                    getProduct1.CategoryID = tblProduct.CategoryID;
                    getProduct1.BrandID = tblProduct.BrandID;
                    getProduct1.QuantityPerUnit = tblProduct.QuantityPerUnit;
                    getProduct1.UnitPrice = tblProduct.UnitPrice;
                    getProduct1.Size = tblProduct.Size;
                    getProduct1.Color = tblProduct.Color;
                    getProduct1.Discount = tblProduct.Discount;
                    getProduct1.UnitWeight = tblProduct.UnitWeight;
                    getProduct1.UnitsInStack = tblProduct.UnitsInStack;
                    getProduct1.Picture = tblProduct.Picture;
                    getProduct1.Ranking = tblProduct.Ranking;
                    getProduct1.Note = tblProduct.Note;
                    getProduct1.IsApprove = 0;
                    getProduct1.CreatedOn = DateTime.Now;
                    getProduct1.CreatedBy = Convert.ToInt32(UserId);
                    if (tblProduct.FileExt != "")
                    {
                        string fileName = "/" + UserId + "/" + Guid.NewGuid() + Path.GetExtension(tblProduct.FileExt);
                        getProduct1.Picture = fileName;
                        productResponse.Picture = fileName;
                    }
                    else
                    {
                        getProduct1.Picture = "";
                    }

                    entities.tblProducts.Add(getProduct1);
                    entities.SaveChanges();
                }

                productResponse.success = true;
                productResponse.message = "Product Saved Successfully";
                productResponse.code = 200;

                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("GetProductById")]
        public async Task<IHttpActionResult> GetProductById(tblProduct tblProduct)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                if (tblProduct.ProductID > 0)
                {
                    tblProduct = entities.tblProducts.Where(x => x.ProductID == tblProduct.ProductID).FirstOrDefault();
                }
                else
                {
                    tblProduct = new tblProduct();
                }
                tblProduct.lstBrands = entities.tblBrands.ToList();
                tblProduct.lstCategories = entities.tblCategories.ToList();
                return Ok(tblProduct);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("ProductApprove")]
        public IHttpActionResult ProductApprove(int productId, int approve)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                //var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                RegisterDL obj = new RegisterDL();
                BaseModel baseModel = new BaseModel();
                var getProducts = entities.tblProducts.Where(x => x.ProductID == productId).FirstOrDefault();
                if (getProducts != null)
                {
                    getProducts.IsApprove = approve;
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
                        DataTable dtproductRecords = dsexcelRecords.Tables[0];
                        for (int i = 0; i < dtproductRecords.Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                tblProduct objB = new tblProduct();
                                string ProductName = Convert.ToString(dtproductRecords.Rows[i][0]);
                                if (ProductName == "")
                                {
                                    baseModel.success = false;
                                    baseModel.message = "Product Name is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }
                                else
                                {
                                    var getProductName = entities.tblProducts.Where(x => x.ProductName.ToLower() == ProductName.ToLower()).FirstOrDefault();
                                    if (getProductName != null)
                                    {
                                        baseModel.success = false;
                                        baseModel.message = "Product Name is already exist!";
                                        baseModel.code = 500;
                                        return Ok(baseModel);
                                    }
                                    else
                                    {
                                        objB.ProductName = Convert.ToString(dtproductRecords.Rows[i][0]);
                                    }

                                }


                                objB.ProductDescription = Convert.ToString(dtproductRecords.Rows[i][1]);
                                string categoryName = Convert.ToString(dtproductRecords.Rows[i][2]);
                                if (categoryName == "")
                                {
                                    baseModel.success = false;
                                    baseModel.message = "Category Name is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }
                                else
                                {
                                    var getCategory = entities.tblCategories.Where(x => x.CategoryName.ToLower() == categoryName.ToLower()).FirstOrDefault();
                                    if (getCategory == null)
                                    {
                                        baseModel.success = false;
                                        baseModel.message = "Category Name is not exist! Please add category from Category Master.";
                                        baseModel.code = 500;
                                        return Ok(baseModel);
                                    }
                                    else {
                                        objB.CategoryID = Convert.ToInt32(getCategory.CategoryID);
                                    }

                                }


                                string brandName = Convert.ToString(dtproductRecords.Rows[i][3]);
                                if (brandName == "")
                                {
                                    baseModel.success = false;
                                    baseModel.message = "Brand Name is not exist!";
                                    baseModel.code = 500;
                                    return Ok(baseModel);
                                }
                                else
                                {
                                    var getBrand = entities.tblBrands.Where(x => x.BrandName.ToLower() == brandName.ToLower()).FirstOrDefault();
                                    if (getBrand == null)
                                    {
                                        baseModel.success = false;
                                        baseModel.message = "Brand Name is not exist! Please add Brand from Brand Master.";
                                        baseModel.code = 500;
                                        return Ok(baseModel);
                                    }
                                    else
                                    {
                                        objB.BrandID = Convert.ToInt32(getBrand.BrandID);
                                    }

                                }
                                objB.QuantityPerUnit = Convert.ToInt32(dtproductRecords.Rows[i][4]);
                                objB.UnitPrice = Convert.ToInt32(dtproductRecords.Rows[i][5]);
                                objB.Size= Convert.ToString(dtproductRecords.Rows[i][6]);
                                objB.UnitWeight = Convert.ToInt32(dtproductRecords.Rows[i][7]);
                                objB.CreatedOn = DateTime.Now;
                                objB.SellerID = 3;
                                entities.tblProducts.Add(objB);
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
