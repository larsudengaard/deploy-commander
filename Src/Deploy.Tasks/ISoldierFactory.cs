namespace Deploy.Tasks
{
    public interface ISoldierFactory
    {
        ISoldier Create(string clientUrl, string authenticationSecretKey, bool allowNonTrustedPawnCertificate = false);
    }
}