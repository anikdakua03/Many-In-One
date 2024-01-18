using ManyInOneAPI.Data;
using ManyInOneAPI.Models.Payment;
using Microsoft.EntityFrameworkCore;

namespace ManyInOneAPI.Repositories.Payment
{
    public class PaymentDetailRepository : IPaymentDetailRepository
    {
        private readonly ManyInOneDbContext _dbContext;
        public PaymentDetailRepository(ManyInOneDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PaymentDetail> GetPaymentDetailById(int id)
        {
            var payment = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.PaymentDetailId == id);

            if(payment is not null)
            {
                return payment;
            }
            return null!;
        }

        public async Task<IEnumerable<PaymentDetail>> GetPaymentDetails()
        {
            var allPayments = await _dbContext.PaymentDetails.ToListAsync();

            if (allPayments is not null)
            {
                return allPayments;
            }
            return null!;
        }
        public async Task<PaymentDetail> AddPaymentDetail(PaymentDetail paymentDetail)
        {
            // check the card number that it exists or not
            var cardNumberExists = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.cardNumber == paymentDetail.cardNumber);

            if(cardNumberExists is null)
            {
                var isAdded = await _dbContext.PaymentDetails.AddAsync(paymentDetail);

                if (isAdded is not null)
                {
                    await _dbContext.SaveChangesAsync();
                    return isAdded.Entity;
                }
            }

            return null!;
        }


        public async Task<PaymentDetail> UpdatePaymentDetailById(int id, PaymentDetail paymentDetail)
        {
            var oldPayment = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.PaymentDetailId == id);

            if (oldPayment is not null)
            {
                oldPayment.CardOwnerName = paymentDetail.CardOwnerName;
                oldPayment.cardNumber = paymentDetail.cardNumber;
                oldPayment.ExpirationDate = paymentDetail.ExpirationDate;
                oldPayment.SecurityCode = paymentDetail.SecurityCode;

                await _dbContext.SaveChangesAsync();
                return oldPayment;
            }
            return null!;
        }

        public async Task<string> DeletePaymentDetailById(int id)
        {
            var payment = await _dbContext.PaymentDetails.FirstOrDefaultAsync(a => a.PaymentDetailId == id);

            if (payment is not null)
            {
                _dbContext.PaymentDetails.Remove(payment);
                await _dbContext.SaveChangesAsync();

                return "Deleted successfully !!";
            }
            return "Paymnet doesn't exists !!";
        }
    }
}
