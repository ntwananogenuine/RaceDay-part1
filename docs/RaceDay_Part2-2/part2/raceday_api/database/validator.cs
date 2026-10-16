namespace raceday_api.database
{//start of namespace
    //checks the values sent by the user, returns null when everything is fine
    public static class validator
    {//start of class

        public static string? check_event(string name, string? description, string location,
                                          string event_type, decimal? distance_km)
        {//start of method
            if (string.IsNullOrWhiteSpace(name) || name.Length > 150)
                return "Event name is required (max 150 characters).";
            if (description != null && description.Length > 1000)
                return "Description is too long (max 1000 characters).";
            if (string.IsNullOrWhiteSpace(location) || location.Length > 150)
                return "Location is required (max 150 characters).";
            if (event_type != "Run" && event_type != "Walk" && event_type != "Cycle")
                return "event_type must be Run, Walk or Cycle.";
            if (distance_km != null && (distance_km <= 0 || distance_km > 999.99m))
                return "distance_km must be between 0.01 and 999.99.";
            return null;
        }//end of method

        public static string? check_category(string name, decimal distance_km, int max_participants)
        {//start of method
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                return "Category name is required (max 100 characters).";
            if (distance_km <= 0 || distance_km > 999.99m)
                return "distance_km must be between 0.01 and 999.99.";
            if (max_participants < 1 || max_participants > 100000)
                return "max_participants must be at least 1.";
            return null;
        }//end of method

        public static string? check_result(int position)
        {//start of method
            if (position < 1 || position > 100000)
                return "position must be 1 or higher.";
            return null;
        }//end of method

    }//end of class
}//end of namespace
