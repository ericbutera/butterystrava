namespace butterystrava.Models {
    public class IndexModel {
        public string ClientId {get;set;}
        public string RedirectUri {get;set;} = "https://localhost:5001/home/code";
        public string Scope {get;set;} = "activity:read_all,activity:write";
        public Account Account {get;set;}
    }
}