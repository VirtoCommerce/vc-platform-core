using System;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Model.Requests;

namespace VirtoCommerce.PaymentModule.Data
{
    public class DefaultManualPaymentMethod : PaymentMethod
    {
        public DefaultManualPaymentMethod() : base("DefaultManualPaymentMethod")
        {
        }

        [Obsolete("Need to use localized strings on clients side instead")]
        public string Name => "Test payment method";

        public override PaymentMethodType PaymentMethodType => PaymentMethodType.Unknown;

        public override PaymentMethodGroupType PaymentMethodGroupType => PaymentMethodGroupType.Manual;

        public override ProcessPaymentRequestResult ProcessPayment(ProcessPaymentRequest request)
        {
            //TODO
            //context.Payment.PaymentStatus = PaymentStatus.Paid;
            //context.Payment.OuterId = context.Payment.Number;
            //context.Payment.CapturedDate = DateTime.UtcNow;
            //         context.Payment.IsApproved = true;
            return new ProcessPaymentRequestResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Paid };
        }

        public override PostProcessPaymentRequestResult PostProcessPayment(PostProcessPaymentRequest request)
        {
            throw new NotImplementedException();
        }

        public override VoidPaymentRequestResult VoidProcessPayment(VoidPaymentRequest request)
        {
            //TODO
            //context.Payment.IsApproved = false;
            //context.Payment.PaymentStatus = PaymentStatus.Voided;
            //context.Payment.VoidedDate = DateTime.UtcNow;
            //context.Payment.IsCancelled = true;
            //context.Payment.CancelledDate = DateTime.UtcNow;
            return new VoidPaymentRequestResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Voided };
        }

        public override CapturePaymentRequestResult CaptureProcessPayment(CapturePaymentRequest request)
        {
            //TODO
            //context.Payment.IsApproved = true;
            //context.Payment.PaymentStatus = PaymentStatus.Paid;
            //context.Payment.VoidedDate = DateTime.UtcNow;
            return new CapturePaymentRequestResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Paid };
        }

        public override RefundPaymentRequestResult RefundProcessPayment(RefundPaymentRequest request)
        {
            throw new NotImplementedException();
        }

        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(System.Collections.Specialized.NameValueCollection queryString)
        {
            return new ValidatePostProcessRequestResult { IsSuccess = false };
        }

    }
}
