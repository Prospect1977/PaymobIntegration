namespace APIs.Models
{
    public class ResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public dynamic? AdditionalData { get; set; }
        public dynamic data { get; set; }
        
    }
}
