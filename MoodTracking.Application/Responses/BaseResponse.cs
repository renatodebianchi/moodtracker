namespace Application.Responses { 
    public class BaseResponse 
    { 
        public bool IsSuccessful { get; set; } 
        public string Message { get; set; } 
        public object Data { get; set; } 
    } 
} 
