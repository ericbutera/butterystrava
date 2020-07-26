using System;
using System.Collections.Generic;

namespace butterystrava.Strava.Responses {
    /*
    https://developers.strava.com/docs/

    https://developers.strava.com/swagger/swagger.json

    200 Successful request
    201 Your activity/etc. was successfully created
    401 Unauthorized
    403 Forbidden; you cannot access
    404 Not found; the requested asset does not exist, or you are not authorized to see it
    429 Too Many Requests; you have exceeded rate limits
    500 Strava is having issues, please check https://status.strava.com
    */

    public class RefreshToken : Token, IToken { }

    public class AuthorizationCode : Token, IToken {
        public Athlete athlete {get;set;}

        public class Athlete {
            public long id {get;set;}
            public string username {get;set;}
            public string firstname {get;set;}
            public string lastname {get;set;}
            /*
            "resource_state": 2,
            "city": "",
            "state": "",
            "country": "",
            "sex": "M",
            "premium": true,
            "summit": true,
            "created_at": "2018-10-25T14:17:34Z",
            "updated_at": "2020-07-25T13:03:32Z",
            "badge_type_id": 1,
            "profile_medium": "avatar/athlete/medium.png",
            "profile": "avatar/athlete/large.png",
            "friend": null,
            "follower": null
            */
        }
    }

    public class Athlete {
        public enum ResourceState {
            TodoDefault = 0,
            Todo1 = 1,
            Todo2 = 2,
            Todo3 = 3
        }

        public enum BadgeType {
            TodoDefault = 0,
            Todo1 = 1,
            Todo2 = 2
        }

        public long id {get;set;}
        public string username {get;set;}
        public ResourceState resource_state {get;set;} // = 2 ?? TODO
        public string firstname {get;set;}
        public string lastname {get;set;}
        public string city {get;set;}
        public string state {get;set;}
        public string country {get;set;}
        public string sex {get;set;} // = "M" TODO
        public bool premium {get;set;}
        public DateTime created_at {get;set;}
        public DateTime? updated_at {get;set;}
        public BadgeType badge_type_id {get;set;} // TODO
        public string profile_medium {get;set;} // avatar/athlete/medium.png
        public string profile {get;set;} // avatar/athlete/large.png
        public string friend {get;set;} // = null ??
        public string follower {get;set;} // = null ??

    }

    public class Activities : List<Activity> {};

    public class Activity {
        public string name {get;set;}
        public double distance {get;set;}
        public int elapsed_time {get;set;}
        public long id {get;set;}
        public Athlete athlete {get;set;}

        public class Athlete {
            public string id {get;set;}
        }
    }
}