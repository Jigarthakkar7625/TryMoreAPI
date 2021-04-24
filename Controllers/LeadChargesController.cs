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

namespace TryMoreAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/LeadCharges")]
    public class LeadChargesController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();
        TryMoreEntities entities = new TryMoreEntities();
        //therapistEntities entities = new therapistEntities();

        [HttpGet]
        [Route("GetLeadCharges")]
        public async Task<IHttpActionResult> GetLeadCharges()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                return Ok(entities.tblLeadCharges.ToList());
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("SaveLeadCharges")]
        public async Task<IHttpActionResult> SaveLeadCharges(tblLeadCharge tblLeadCharge)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                BaseModel baseModel = new BaseModel();

                if (tblLeadCharge.LeadChargesID > 0)
                {
                    var checkLeadProductAmountID = entities.tblLeadCharges.Where(x => x.LeadChargesID != tblLeadCharge.LeadChargesID && x.LeadProductAmountID == tblLeadCharge.LeadProductAmountID).ToList();
                    if (checkLeadProductAmountID != null && checkLeadProductAmountID.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "Lead Charges already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    var getLeadProduct = entities.tblLeadCharges.Where(x => x.LeadChargesID == tblLeadCharge.LeadChargesID).FirstOrDefault();


                    if (getLeadProduct != null)
                    {
                        getLeadProduct.LeadProductAmountID = tblLeadCharge.LeadProductAmountID;
                        getLeadProduct.LeadCharges = tblLeadCharge.LeadCharges;
                        getLeadProduct.ModifiedBy = Convert.ToInt32(UserId);
                        getLeadProduct.ModifiedOn = DateTime.Now;
                        entities.SaveChanges();
                    }
                }
                else
                {
                    var checkLeadProductAmountID = entities.tblLeadCharges.Where(x => x.LeadProductAmountID == tblLeadCharge.LeadProductAmountID).ToList();
                    if (checkLeadProductAmountID != null && checkLeadProductAmountID.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "Lead Charges already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    tblLeadCharge tblLeadCharge1 = new tblLeadCharge();
                    tblLeadCharge1.LeadProductAmountID = tblLeadCharge.LeadProductAmountID;
                    tblLeadCharge1.LeadCharges = tblLeadCharge.LeadCharges;
                    tblLeadCharge1.CreatedBy = Convert.ToInt32(UserId);
                    tblLeadCharge1.CreatedOn = DateTime.Now;
                    entities.tblLeadCharges.Add(tblLeadCharge1);
                    entities.SaveChanges();
                }

                baseModel.success = true;
                baseModel.message = "Lead Charges Saved Successfully";
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
        [Route("GetLeadChargesId")]
        public async Task<IHttpActionResult> GetLeadChargesId(tblLeadCharge tblLeadCharge)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                if (tblLeadCharge.LeadChargesID > 0)
                {
                    tblLeadCharge = entities.tblLeadCharges.Where(x => x.LeadChargesID == tblLeadCharge.LeadChargesID).FirstOrDefault();

                    return Ok(tblLeadCharge);

                }

                return Ok();
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }
    }
}
