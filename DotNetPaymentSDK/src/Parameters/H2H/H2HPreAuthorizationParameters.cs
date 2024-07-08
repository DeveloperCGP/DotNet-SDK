namespace DotNetPaymentSDK.src.Parameters.H2H
{
    public class H2HPreAuthorizationParameters : H2HRedirectionParameters
    {
        public H2HPreAuthorizationParameters() : base()
        {
            SetAutoCapture(false);
        }

        public override Tuple<bool, string> IsMissingField()
        {
            if (IsAutoCapture())
            {
                return new(true, "autoCapture");
            }
            return base.IsMissingField();
        }
    }
}