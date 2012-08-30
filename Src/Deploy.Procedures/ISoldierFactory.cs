using Deploy.Tasks;

namespace Deploy.Procedures
{
    public interface ISoldierFactory
    {
        ISoldier Create(string clientUrl, string authenticationSecretKey, bool allowNonTrustedPawnCertificate = false);
    }
}