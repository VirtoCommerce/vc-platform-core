using System;
using VirtoCommerce.PaymentModule.Core.Contexts;
using VirtoCommerce.PaymentModule.Core.Models;
using VirtoCommerce.PaymentModule.Core.PaymentResults;

namespace VirtoCommerce.PaymentModule.Data
{
    public class DefaultManualPaymentMethod : PaymentMethod
    {
        public DefaultManualPaymentMethod() : base("DefaultManualPaymentMethod")
        {
        }

        public override PaymentMethodType PaymentMethodType => PaymentMethodType.Unknown;

        public override PaymentMethodGroupType PaymentMethodGroupType => PaymentMethodGroupType.Manual;

        public override ProcessPaymentResult ProcessPayment(ProcessPaymentEvaluationContext context)
        {
            //TODO
            //context.Payment.PaymentStatus = PaymentStatus.Paid;
            //context.Payment.OuterId = context.Payment.Number;
            //context.Payment.CapturedDate = DateTime.UtcNow;
            //         context.Payment.IsApproved = true;
            return new ProcessPaymentResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Paid };
        }

        public override PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override VoidProcessPaymentResult VoidProcessPayment(VoidProcessPaymentEvaluationContext context)
        {
            //TODO
            //context.Payment.IsApproved = false;
            //context.Payment.PaymentStatus = PaymentStatus.Voided;
            //context.Payment.VoidedDate = DateTime.UtcNow;
            //context.Payment.IsCancelled = true;
            //context.Payment.CancelledDate = DateTime.UtcNow;
            return new VoidProcessPaymentResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Voided };
        }

        public override CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context)
        {
            //TODO
            //context.Payment.IsApproved = true;
            //context.Payment.PaymentStatus = PaymentStatus.Paid;
            //context.Payment.VoidedDate = DateTime.UtcNow;
            return new CaptureProcessPaymentResult { IsSuccess = true, NewPaymentStatus = PaymentStatus.Paid };
        }

        public override RefundProcessPaymentResult RefundProcessPayment(RefundProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(System.Collections.Specialized.NameValueCollection queryString)
        {
            return new ValidatePostProcessRequestResult { IsSuccess = false };
        }
    }
}
