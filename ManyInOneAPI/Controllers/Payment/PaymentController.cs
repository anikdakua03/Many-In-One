using ManyInOneAPI.Models.Payment;
using ManyInOneAPI.Repositories.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManyInOneAPI.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentDetailRepository _paymentDeatilsRepo;

        public PaymentController(IPaymentDetailRepository paymentDeatilsRepo)
        {
            _paymentDeatilsRepo = paymentDeatilsRepo;
        }

        // GET : api/PaymentDeatils
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDetail>>> GetPaymentDetails()
        {
            var res = await _paymentDeatilsRepo.GetPaymentDetails(HttpContext.RequestAborted);
            //if (res == null)
            //{
            //    return NotFound("There is no payment deatils yet !!");
            //}

            return Ok(res);
        }

        // GET : api/PaymentDeatils/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetail>> GetPaymentDetailById(int id)
        {

            var res = await _paymentDeatilsRepo.GetPaymentDetailById(id, HttpContext.RequestAborted);
            //if (res == null)
            //{
            //    return NotFound($"There is no payment deatils with this id {id} !!");
            //}

            return Ok(res);

        }

        // POST : api/PaymentDeatils/
        [HttpPost]
        public async Task<ActionResult<PaymentDetail>> AddPaymentDetail([FromBody] PaymentDetail paymentDetail)
        {
            var res = await _paymentDeatilsRepo.AddPaymentDetail(paymentDetail, HttpContext.RequestAborted);
            //if (res == null)
            //{
            //    return BadRequest("Unable to add payment deatils !!");
            //}

            return CreatedAtAction("GetPaymentDetailById", new { id = res.Data!.PaymentDetailId }, res);

        }

        // PUT : api/PaymentDeatils/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentDetail>> UpdatePaymentDetailById(int id, PaymentDetail paymentDetail)
        {

            if (id != paymentDetail.PaymentDetailId)
                return BadRequest("Payment detail Id mismatch");

            var res = await _paymentDeatilsRepo.UpdatePaymentDetailById(id, paymentDetail, HttpContext.RequestAborted);

            //if (res == null)
            //    return NotFound($"Payment detail with Id = {id} not found");

            return Ok(res);

        }

        // DELETE : api/PaymentDeatils/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePaymentDetailById(int id)
        {
            var res = await _paymentDeatilsRepo.DeletePaymentDetailById(id, HttpContext.RequestAborted);

            return Ok(res);
        }
    }
}
