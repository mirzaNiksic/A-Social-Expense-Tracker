using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ConsoleApp
{
    public class Login
    {
        public User login()
        {
            bool isLogged = false;
            var users = new User();

            string filepath = @"C:\Users\MirzaNiksic.AzureAD\Desktop\TestStar5\consoleApp\preparationTest.json";

            WebRequest webRequest = WebRequest.Create(filepath);
            WebResponse webResponse = webRequest.GetResponse();

            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseFromServer = reader.ReadToEnd();

                GetUsers root = JsonConvert.DeserializeObject<GetUsers>(responseFromServer);

                while(!isLogged)
                {
                    Console.WriteLine("Please enter your email address");

                    string email = Console.ReadLine();

                    foreach (User user in root.users)
                    {
                        if(user.email.Equals(email))
                        {
                            Console.WriteLine($"\nLogged in, welcome back {user.email}!");
                            isLogged = true;
                            return root.users.FirstOrDefault(user => user.email == email);
                        }
                    }
                }
            }
            return users;
        }
    }
}

