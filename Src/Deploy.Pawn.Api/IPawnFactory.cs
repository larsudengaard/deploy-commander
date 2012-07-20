namespace Deploy.Pawn.Api
{
    public interface IPawnFactory
    {
        IPawnClient Create(string clientUrl, string authenticationSecretKey, bool allowNonTrustedPawnCertificate = false);
    }
}