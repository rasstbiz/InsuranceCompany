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
            var notFoundResult = NotFound(new
            {
                nameOfInsuredObject,
                effectiveDate
            });
            try
            {
                var insuranceCompany = insuranceCompanyService.Get();
                return Ok(insuranceCompany.GetPolicy(nameOfInsuredObject, effectiveDate));
            }
            catch (EffectivePolicyNotFoundException)
            {
                return notFoundResult;
            }
            catch (MultipleEffectivePoliciesFoundException)
            {
                return notFoundResult;
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("addrisk")]
        public IActionResult AddRisk(
            [FromBody] string nameOfInsuredObject,
            [FromBody] DateTime validFrom,
            [FromBody] Risk risk,
            [FromBody] DateTime effectiveDate)
        {
            try
            {
                var insuranceCompany = insuranceCompanyService.Get();
                insuranceCompany.AddRisk(nameOfInsuredObject, risk, validFrom, effectiveDate);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("removerisk")]
        public IActionResult RemoveRisk(
            [FromBody] string nameOfInsuredObject,
            [FromBody] DateTime validTill,
            [FromBody] Risk risk,
            [FromBody] DateTime effectiveDate)
        {
            try
            {
                var insuranceCompany = insuranceCompanyService.Get();
                insuranceCompany.RemoveRisk(nameOfInsuredObject, risk, validTill, effectiveDate);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
