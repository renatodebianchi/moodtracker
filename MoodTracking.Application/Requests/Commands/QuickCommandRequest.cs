namespace Application.Requests.Commands { 
    public abstract class QuickCommandRequest<Model> : QuickRequest 
    { 
        public Model model {get; set;} 
        public QuickCommandRequest(): base() 
        {} 
    } 
} 
