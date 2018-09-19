using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace LuisBot.Services
{
    public class OrderDetails
    {
        public static Dictionary<string, string> Order(string OrderId,String res)
        {
            var value = new Dictionary<string, string>();
            var AuthValue = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE1MzcyNjIwNDEsImV4cCI6MTg1MjYyMjA0MSwiaXNzIjoiaHR0cDovL2VjMi0xOC0xODgtMzgtMjEwLnVzLWVhc3QtMi5jb21wdXRlLmFtYXpvbmF3cy5jb20iLCJhdWQiOlsiaHR0cDovL2VjMi0xOC0xODgtMzgtMjEwLnVzLWVhc3QtMi5jb21wdXRlLmFtYXpvbmF3cy5jb20vcmVzb3VyY2VzIiwibm9wX2FwaSJdLCJjbGllbnRfaWQiOiI3NTU2NWU5NC1kZjMzLTQ1MjctOTkyMy1iYzI3MzI0M2I0NTQiLCJzdWIiOiI3NTU2NWU5NC1kZjMzLTQ1MjctOTkyMy1iYzI3MzI0M2I0NTQiLCJhdXRoX3RpbWUiOjE1MzcyNjE3OTYsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsibm9wX2FwaSIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.Afqi6ZyjuXml_le22euysvDBxLzQ0I_mX_9PPyawFZQa5dkOcO9ZRqAnKJPYU6w3FSNZuO-DZ6FpAz_KXcOs3iYmJBnDvw0lbGbEXz2pcaxvAQLKsFQ5A8mNkWsjj95Dkg5IBoCvmVHc1xl77BQv4jXckFFb1GG8v4WfDGbi_60kzQy10A8ydAKlmWaQ-YbNMjTicWFhg5nYevOmanoaOFSz3oYbggAMdrFHaOeLxIz3UPJk51FmuJ9di5gzC4O-Yi0NDsZvG3855qD5LqPMHhy_Ej2oQeQQa_GPJI3BWfH3HAqL0x3-FEUHjo1lkaDMGif0K4YgTyjgisxXOrfhbw";
            WebRequest request = WebRequest.Create(@"http://ec2-18-188-38-210.us-east-2.compute.amazonaws.com/api/orders/"+OrderId);
            request.Headers.Add("Authorization", AuthValue);
            WebResponse response = request.GetResponse();
            var streamReader = new StreamReader(response.GetResponseStream());
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            var jsonResult = JObject.Parse(result);
            if(res == "shipStatus")
                value.Add("Status", (string)jsonResult["orders"][0]["shipping_status"]);
            else if(res =="OrderStatus")
                value.Add("Status", (string)jsonResult["orders"][0]["order_status"]);
            return value;
            
        }
    }
}