namespace DemoApi.ViewModel
{
    public class AuthModel
    {
    }
    public class DgftAuthRequest
    {
        public string userID { get; set; }
        public string password { get; set; }
    }
    public class DgftAuthResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string userID { get; set; }
    }

    public class ProcessStatusRequest
    {
        public string uniqueTxId { get; set;}
    }
    
}
