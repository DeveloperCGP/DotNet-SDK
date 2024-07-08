using System;

namespace DotNetPaymentSDK.src.Requests.Utils
{
    public class RequestsPaths
    {
        public const string HOSTED_STG = "https://checkout-stg.addonpayments.com/EPGCheckout/rest/online/tokenize";
        public const string HOSTED_PROD = "https://checkout.addonpayments.com/EPGCheckout/rest/online/tokenize";

        public const string H2H_STG = "https://checkout-stg.addonpayments.com/EPGCheckout/rest/online/pay";
        public const string H2H_PROD = "https://checkout.addonpayments.com/EPGCheckout/rest/online/pay";

        public const string CAPTURE_STG = "https://checkout-stg.addonpayments.com/EPGCheckout/rest/online/capture";
        public const string CAPTURE_PROD = "https://checkout.addonpayments.com/EPGCheckout/rest/online/capture";

        public const string REBATE_STG = "https://checkout-stg.addonpayments.com/EPGCheckout/rest/online/rebate";
        public const string REBATE_PROD = "https://checkout.addonpayments.com/EPGCheckout/rest/online/rebate";

        public const string VOID_STG = "https://checkout-stg.addonpayments.com/EPGCheckout/rest/online/void";
        public const string VOID_PROD = "https://checkout.addonpayments.com/EPGCheckout/rest/online/void";

        public const string JS_AUTH_STG = "https://epgjs-mep-stg.addonpayments.com/auth";
        public const string JS_AUTH_PROD = "https://epgjs-mep.addonpayments.com/auth";

        public const string JS_CHARGE_STG = "https://epgjs-mep-stg.addonpayments.com/charge/v2";
        public const string JS_CHARGE_PROD = "https://epgjs-mep.addonpayments.com/charge/v2";
    }
}
