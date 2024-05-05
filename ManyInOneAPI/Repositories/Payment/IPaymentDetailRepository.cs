using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Payment;

namespace ManyInOneAPI.Repositories.Payment
{
    public interface IPaymentDetailRepository
    {
        public Task<Result<IEnumerable<PaymentDetail>>> GetPaymentDetails(CancellationToken cancellationToken = default);

        public Task<Result<PaymentDetail>> GetPaymentDetailById(int id, CancellationToken cancellationToken = default);

        public Task<Result<PaymentDetail>> AddPaymentDetail(PaymentDetail paymentDetail, CancellationToken cancellationToken = default);

        public Task<Result<PaymentDetail>> UpdatePaymentDetailById(int id, PaymentDetail paymentDetail, CancellationToken cancellationToken = default);

        public Task<Result<string>> DeletePaymentDetailById(int id, CancellationToken cancellationToken = default);
    }
}
