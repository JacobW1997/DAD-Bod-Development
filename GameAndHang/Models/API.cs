using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameAndHang.Models
{
    public class API
    {
        public class GameContainerRoot
        {
            public Games[] gamesList { get; set; }
        }

        public class Games
        {
            public string id { get; set; }
            public string name { get; set; }
            public int year_published { get; set; }
            public int min_players { get; set; }
            public int max_players { get; set; }
            public int min_playtime { get; set; }
            public int max_playtime { get; set; }
            public int min_age { get; set; }
            public string description { get; set; }
            public string description_preview { get; set; }
            public string image_url { get; set; }
            public string thumb_url { get; set; }
            public Images images { get; set; }
            public string url { get; set; }
            public string price { get; set; }
            public string msrp { get; set; }
            public string discount { get; set; }
            public string primary_publisher { get; set; }
            public string[] publishers { get; set; }
            public Mechanic[] mechanics { get; set; }
            public Category[] categories { get; set; }
            public string[] designers { get; set; }
            public object[] developers { get; set; }
            public object[] artists { get; set; }
            public string[] names { get; set; }
            public int num_user_ratings { get; set; }
            public float average_user_rating { get; set; }
            public string official_url { get; set; }
            public string rules_url { get; set; }
            public float weight_amount { get; set; }
            public string weight_units { get; set; }
            public int size_height { get; set; }
            public int size_width { get; set; }
            public float size_depth { get; set; }
            public string size_units { get; set; }
            public object matches_specs { get; set; }
            public Spec[] spec { get; set; }
            public int reddit_all_time_count { get; set; }
            public int reddit_week_count { get; set; }
            public int reddit_day_count { get; set; }
            public float historical_low_price { get; set; }
            public DateTime historical_low_date { get; set; }
        }

        public class Images
        {
            public string thumb { get; set; }
            public string small { get; set; }
            public string medium { get; set; }
            public string large { get; set; }
            public string original { get; set; }
        }

        public class Mechanic
        {
            public string id { get; set; }
        }

        public class Category
        {
            public string id { get; set; }
        }

        public class Spec
        {
            public string id { get; set; }
        }

    }
}
