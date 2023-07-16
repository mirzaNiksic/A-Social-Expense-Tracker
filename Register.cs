using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ConsoleApp
{
    public class Register
    {
        public User register()
        {
            var users = new User();
            var usersList = new List<User>();
            bool isNew = false;
            string email = "";

            string filepath = @"C:\Users\MirzaNiksic.AzureAD\Desktop\TestStar5\consoleApp\preparationTest.json";

            WebRequest webRequest = WebRequest.Create(filepath);
            WebResponse webResponse = webRequest.GetResponse();

            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseFromServer = reader.ReadToEnd();

                GetUsers root = JsonConvert.DeserializeObject<GetUsers>(responseFromServer);

                while (!isNew)
                {
                    Console.WriteLine("Please enter your email address!");

                    email = Console.ReadLine();

                    foreach (User user in root.users)
                    {
                        if (user.email.Equals(email))
                        {
                            Console.WriteLine($"User with email {email} already exists!");
                            break;
                        }
                    }
                    users.email = email;
                    users.expenses = new List<Expense> { };
                    isNew = true;
                }
                Console.WriteLine($"\nLogged in, welcome {email}!");
                root.users.Add(users);
                usersList = root.users;
                users = root.users.FirstOrDefault(user => user.email == email);
            }
            //https://www.newtonsoft.com/json/help/html/CreatingLINQtoJSON.htm


            JObject text = new JObject(new JProperty("users", new JArray(from u in usersList
                                                                         select new JObject(
                                                                             new JProperty("email", u.email),
                                                                             new JProperty("expenses", new JArray(
                                                                                 from e in u.expenses
                                                                                 select new JObject(
                                                                                     new JProperty("expenseName", e.expenseName),
                                                                                     new JProperty("expenseDate", e.expenseDate),
                                                                                     new JProperty("amountSpent", e.amountSpent),
                                                                                     new JProperty("paymentRequests", new JArray(
                                                                                         from pr in e.paymentRequests
                                                                                         select new JObject(
                                                                                             new JProperty("amount", pr.amount),
                                                                                             new JProperty("amountPaid", pr.amountPaid),
                                                                                             new JProperty("who", pr.who),
                                                                                             new JProperty("dueAt", pr.dueAt)))),
                                                                                    new JProperty("payments", new JArray(
                                                                                        from p in e.payments
                                                                                        select new JObject(
                                                                                            new JProperty("amount", p.amount),
                                                                                            new JProperty("amountPaid", p.amountPaid),
                                                                                            new JProperty("toWhom", p.toWhom),
                                                                                            new JProperty("dueAt", p.dueAt)
                                                                                     ))))))))));

            System.IO.File.WriteAllText(filepath, text.ToString());

            return users;
        }
    }
}

