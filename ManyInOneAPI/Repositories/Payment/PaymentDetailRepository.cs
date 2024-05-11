using ManyInOneAPI.Data;
using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Payment;
using Microsoft.EntityFrameworkCore;

namespace ManyInOneAPI.Repositories.Payment
{
    public class PaymentDetailRepository : IPaymentDetailRepository
    {
        private readonly ManyInOneDbContext _dbContext;
        //private readonly ManyInOnePgDbContext _dbContext;
        public PaymentDetailRepository(ManyInOneDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result<PaymentDetail>> GetPaymentDetailById(int id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var paymentDetail = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.PaymentDetailId == id);

            if(paymentDetail is not null)
            {
                return Result<PaymentDetail>.Success(paymentDetail);
            }
            return Result<PaymentDetail>.Failure(Error.NotFound("Not Found", $"No Payment detail found with the given Id : {id}. "));
        }

        public async Task<Result<IEnumerable<PaymentDetail>>> GetPaymentDetails(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allPayments = await _dbContext.PaymentDetails.ToListAsync(cancellationToken);

            if (allPayments is not null)
            {
                return Result<IEnumerable<PaymentDetail>>.Success(allPayments);
            }
            return Result<IEnumerable<PaymentDetail>>.Failure(Error.NotFound("Not Found", $"No Payment details found. "));
        }
        public async Task<Result<PaymentDetail>> AddPaymentDetail(PaymentDetail paymentDetail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // check the card number that it exists or not
            var cardNumberExists = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.CardNumber == paymentDetail.CardNumber, cancellationToken);

            if(cardNumberExists is null)
            {
                var isAdded = await _dbContext.PaymentDetails.AddAsync(paymentDetail, cancellationToken);

                if (isAdded is not null)
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return Result<PaymentDetail>.Success(isAdded.Entity);
                }

                return Result<PaymentDetail>.Failure(Error.Failure("Failed to Add", $"Same Payment detail found with the given Card number : {paymentDetail.CardNumber}. "));
            }

            return Result<PaymentDetail>.Failure(Error.Conflict("Conflict", $"Same Payment detail found with the given Card number : {paymentDetail.CardNumber}. "));
        }


        public async Task<Result<PaymentDetail>> UpdatePaymentDetailById(int id, PaymentDetail paymentDetail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var oldPayment = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.PaymentDetailId == id, cancellationToken);

            if (oldPayment is not null)
            {
                oldPayment.CardOwnerName = paymentDetail.CardOwnerName;
                oldPayment.CardNumber = paymentDetail.CardNumber;
                oldPayment.ExpirationDate = paymentDetail.ExpirationDate;
                oldPayment.SecurityCode = paymentDetail.SecurityCode;

                await _dbContext.SaveChangesAsync(cancellationToken);
                return Result<PaymentDetail>.Success(oldPayment);
            }
            return Result<PaymentDetail>.Failure(Error.Validation("Failed", $"No Payment detail found with the given Card number. "));
        }

        public async Task<Result<string>> DeletePaymentDetailById(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var payment = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.PaymentDetailId == id, cancellationToken);

            if (payment is not null)
            {
                _dbContext.PaymentDetails.Remove(payment);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result<string>.Success("Deleted successfully !!");
            }
            return Result<string>.Failure(Error.NotFound("Not Found", $"No Payment detail found with the given Card number. "));
        }
    }
}
