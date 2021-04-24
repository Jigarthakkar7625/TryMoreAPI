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
    [RoutePrefix("api/SMSPlan")]
    public class SMSPlanController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();
        TryMoreEntities entities = new TryMoreEntities();
        //therapistEntities entities = new therapistEntities();

        [HttpGet]
        [Route("GetSMSPlan")]
        public async Task<IHttpActionResult> GetAllSMSPlan()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                return Ok(entities.tblSMSPlans.ToList());
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("SaveSMSPlan")]
        public async Task<IHttpActionResult> SaveSMSPlan(tblSMSPlan tblSMSPlan)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                BaseModel baseModel = new BaseModel();

                if (tblSMSPlan.SMSPlanID > 0)
                {
                    var checkPlanName =  entities.tblSMSPlans.Where(x => x.SMSPlanID != tblSMSPlan.SMSPlanID && x.PlanName.ToLower() == tblSMSPlan.PlanName.ToLower()).ToList();
                    if (checkPlanName != null && checkPlanName.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "SMS Plan Name already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    var getSMSPlan = entities.tblSMSPlans.Where(x => x.SMSPlanID == tblSMSPlan.SMSPlanID).FirstOrDefault();


                    if (getSMSPlan != null)
                    {
                        getSMSPlan.PlanName = tblSMSPlan.PlanName;
                        getSMSPlan.Amount = tblSMSPlan.Amount;
                        getSMSPlan.NoOfSMS = tblSMSPlan.NoOfSMS;
                        getSMSPlan.ModifiedBy = Convert.ToInt32(UserId);
                        getSMSPlan.ModifiedOn = DateTime.Now;
                        getSMSPlan.IsActive = tblSMSPlan.IsActive;
                        entities.SaveChanges();
                    }
                }
                else
                {
                    var checkPlanName = entities.tblSMSPlans.Where(x => x.PlanName.ToLower() == tblSMSPlan.PlanName.ToLower()).ToList();
                    if (checkPlanName != null && checkPlanName.Count > 0)
                    {
                        baseModel.success = false;
                        baseModel.message = "SMS Plan Name already exist!";
                        baseModel.code = 500;
                        return Ok(baseModel);
                    }

                    tblSMSPlan tblSMSPlan1 = new tblSMSPlan();
                    tblSMSPlan1.PlanName = tblSMSPlan.PlanName;
                    tblSMSPlan1.Amount = tblSMSPlan.Amount;
                    tblSMSPlan1.NoOfSMS = tblSMSPlan.NoOfSMS;
                    tblSMSPlan1.CreatedBy = Convert.ToInt32(UserId);
                    tblSMSPlan1.CreatedOn = DateTime.Now;
                    tblSMSPlan1.IsActive = tblSMSPlan.IsActive;
                    entities.tblSMSPlans.Add(tblSMSPlan1);
                    entities.SaveChanges();
                }

                baseModel.success = true;
                baseModel.message = "SMS Plan Saved Successfully";
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
        [Route("GetSMSPlanById")]
        public async Task<IHttpActionResult> GetSMSPlanById(tblSMSPlan tblSMSPlan)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                if (tblSMSPlan.SMSPlanID > 0)
                {
                    tblSMSPlan = entities.tblSMSPlans.Where(x => x.SMSPlanID == tblSMSPlan.SMSPlanID).FirstOrDefault();

                    return Ok(tblSMSPlan);

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
