using ManyInOneAPI.Models.Payment;

namespace ManyInOneAPI.Repositories.Payment
{
    public interface IPaymentDetailRepository
    {
        public Task<IEnumerable<PaymentDetail>> GetPaymentDetails();

        public Task<PaymentDetail> GetPaymentDetailById(int id);

        public Task<PaymentDetail> AddPaymentDetail(PaymentDetail paymentDetail);

        public Task<PaymentDetail> UpdatePaymentDetailById(int id, PaymentDetail paymentDetail);

        public Task<string> DeletePaymentDetailById(int id);
    }
}
