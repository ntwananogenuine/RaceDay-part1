namespace raceday_api.database
{//start of namespace
    //what every database method hands back to the controller:
    //a status code, a message and (sometimes) the data
    public class db_result
    {//start of class
        public int status { get; set; } = 200;
        public string message { get; set; } = "";
        public object? data { get; set; }

        public static db_result ok(object? data = null, string message = "")
        {
            return new db_result { status = 200, data = data, message = message };
        }

        public static db_result created(object data)
        {
            return new db_result { status = 201, data = data };
        }

        public static db_result fail(int status, string message)
        {
            return new db_result { status = status, message = message };
        }
    }//end of class
}//end of namespace
