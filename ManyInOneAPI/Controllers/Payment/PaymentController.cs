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
          try
            {
                var res = await _paymentDeatilsRepo.GetPaymentDetails();
                if (res == null)
                {
                    return NotFound("There is no payment deatils yet !!");
                }

                return Ok(res.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET : api/PaymentDeatils/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDetail>> GetPaymentDetailById(int id)
        {
                      try
            {
                var res = await _paymentDeatilsRepo.GetPaymentDetailById(id);
                if (res == null)
                {
                    return NotFound($"There is no payment deatils with this id {id} !!");
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST : api/PaymentDeatils/
        [HttpPost]
        public async Task<ActionResult<PaymentDetail>> AddPaymentDetail([FromBody]PaymentDetail paymentDetail)
        {
                      try
            {
                var res = await _paymentDeatilsRepo.AddPaymentDetail(paymentDetail);
                if (res == null)
                {
                    return BadRequest("Unable to add payment deatils !!");
                }

                return CreatedAtAction("GetPaymentDetailById", new { id = res.PaymentDetailId }, res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT : api/PaymentDeatils/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentDetail>> UpdatePaymentDetailById(int id, PaymentDetail paymentDetail)
        {
            try
            {
                if (id != paymentDetail.PaymentDetailId)
                    return BadRequest("Payment detail Id mismatch");

                var res = await _paymentDeatilsRepo.UpdatePaymentDetailById(id, paymentDetail);

                if (res == null)
                    return NotFound($"Payment detail with Id = {id} not found");

                return res;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        // DELETE : api/PaymentDeatils/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePaymentDetailById(int id)
        {
            try
            {
                var res = await _paymentDeatilsRepo.DeletePaymentDetailById(id);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
