namespace SmartHomeApp
{
    public static class AuthConfig
    {
        public const string Domain = "sofusskovgaard.eu.auth0.com";
        public const string ClientId = "oIV20lSAkkNCxwXlR56OgVA0tsaPvhmD";
        public const string Audience = "http://localhost:4001";
        public const string Scopes = "openid profile offline_access read:data-period read:latest-data";
        public const string PackageName = "com.companyname.SmartHomeApp";
    }
}