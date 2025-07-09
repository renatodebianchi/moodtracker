namespace Domain.ValueObjects 
{ 
    public class EmailSettings 
    { 
        public string Mail { get; set; } 
        public string DisplayName { get; set; } 
        public string User { get; set; } 
        public string Password { get; set; } 
        public string Host { get; set; } 
        public int Port { get; set; } 
        public bool EnableSSL { get; set; } 
        public bool UseAutentication { get; set; } 
    } 
} 
