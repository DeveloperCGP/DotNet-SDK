using System.Runtime.Serialization;
using DotNetPayment.Core.Domain.Enums;

namespace DotNetPaymentSDK.src.Parameters.Notification.Operation
{
    [DataContract]
    public class Operation
    {
        [DataMember(Name = "amount", IsRequired = false)]
        public string? Amount { get; set; }
        [DataMember(Name = "currency", IsRequired = false)]
        public Currency? Currency { get; set; }
        [DataMember(Name = "details", IsRequired = false)]
        public string? Details { get; set; }
        [DataMember(Name = "merchantTransactionId", IsRequired = false)]
        public string? MerchantTransactionId { get; set; }
        [DataMember(Name = "paySolTransactionId", IsRequired = false)]
        public string? PaySolTransactionId { get; set; }
        [DataMember(Name = "service", IsRequired = false)]
        public string? Service { get; set; }
        [DataMember(Name = "status", IsRequired = false)]
        public string? Status { get; set; }
        [DataMember(Name = "transactionId", IsRequired = false)]
        public string? TransactionId { get; set; }
        [DataMember(Name = "respCode", IsRequired = false)]
        public ResponseCode? RespCode { get; set; }
        [DataMember(Name = "operationType", IsRequired = false)]
        public OperationTypes? OperationType { get; set; }
        [DataMember(Name = "paymentDetails", IsRequired = false)]
        public PaymentDetails? PaymentDetails { get; set; }
        [DataMember(Name = "mpi", IsRequired = false)]
        public Mpi? MPI { get; set; }
        [DataMember(Name = "paymentCode", IsRequired = false)]
        public string? PaymentCode { get; set; }
        [DataMember(Name = "paymentMessage", IsRequired = false)]
        public string? PaymentMessage { get; set; }
        [DataMember(Name = "message", IsRequired = false)]
        public string? Message { get; set; }
        [DataMember(Name = "paymentMethod", IsRequired = false)]
        public string? PaymentMethod { get; set; }
        [DataMember(Name = "paymentSolution", IsRequired = false)]
        public PaymentSolutions? PaymentSolution { get; set; }
        [DataMember(Name = "authCode", IsRequired = false)]
        public string? AuthCode { get; set; }
        [DataMember(Name = "rad", IsRequired = false)]
        public string? Rad { get; set; }
        [DataMember(Name = "radMessage", IsRequired = false)]
        public string? RadMessage { get; set; }
        [DataMember(Name = "redirectionResponse", IsRequired = false)]
        public string? RedirectionResponse { get; set; }
        [DataMember(Name = "subscriptionPlan", IsRequired = false)]
        public string? SubscriptionPlan { get; set; }
    }
}