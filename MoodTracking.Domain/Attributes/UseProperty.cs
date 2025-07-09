namespace Domain.Attributes 
{ 
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)] 
    public class UseProperty : System.Attribute 
    { 
        public bool use {get; set;} 
        public UseProperty(bool use) 
        { 
            this.use = use; 
        } 
    } 
} 
