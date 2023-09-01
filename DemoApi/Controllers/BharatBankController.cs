using DemoApi.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static DemoApi.ViewModel.MainCode;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BharatBankController : Controller
    {
     
        [AllowAnonymous]
        [HttpPost("generateAuthToken")]
        public async Task<ActionResult<DgftAuthResponse>> generateAuthToken(DgftAuthRequest dgftAuthRequest)
        {
            try
            {
                MainCode mainCode = new MainCode();
                return await mainCode.AuthTokenApi(dgftAuthRequest);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        //pushIRMToDGFT API
        [AllowAnonymous]
        [HttpPost("pushIRMToDGFT")]
        public async Task<ActionResult<string>> pushIRMToDGFT(JsonRequestData jsonRequest)
        {
            try
            {
                MainCode mainCode = new MainCode();
                return await mainCode.IRMToDGFT(jsonRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
