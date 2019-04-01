using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/policies")]
    public class PolicyController : Controller
    {
        private readonly IInsuranceCompanyService insuranceCompanyService;

        public PolicyController(IInsuranceCompanyService insuranceCompanyService)
        {
            this.insuranceCompanyService = insuranceCompanyService;
        }
        
        [HttpGet]
        public IActionResult Get([FromQuery]string nameOfInsuredObject, [FromQuery]DateTime effectiveDate)
        {
            try
            {
                var insuranceCompany = insuranceCompanyService.Get();
                return Ok(insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
            }
            catch (EffectivePolicyNotFoundException)
            {
                return NotFound(new
                {
                    nameOfInsuredObject,
                    effectiveDate
                });
            }
        }

        [HttpPost]
        [Route("sell")]
        public IActionResult Sell([FromBody]SellRequest request)
        {
            try
            {
                var insuranceCompany = insuranceCompanyService.Get();
                var result = insuranceCompany.SellPolicy(
                    request.NameOfInsuredObject, 
                    request.ValidFrom, 
                    request.ValidMonths, 
                    request.SelectedRisks.ToList());
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("addrisk")]
        public IActionResult AddRisk([FromBody]AddRiskRequest request)
        {
            try
            {
                var insuranceCompany = insuranceCompanyService.Get();
                insuranceCompany.AddRisk(
                    request.NameOfInsuredObject, 
                    request.Risk, 
                    request.ValidFrom, 
                    request.EffectiveDate);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("removerisk")]
        public IActionResult RemoveRisk([FromBody]RemoveRiskRequest request)
        {
            try
            {
                var insuranceCompany = insuranceCompanyService.Get();
                insuranceCompany.RemoveRisk(
                    request.NameOfInsuredObject, 
                    request.Risk, 
                    request.ValidTill, 
                    request.EffectiveDate);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
