using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ConsoleApp
{
    public class Payment
    {
        public int amount { get; set; }
        public int amountPaid { get; set; }
        public string toWhom { get; set; }
        public string dueAt { get; set; }

        public Payment()
        { }
        public Payment(int amount, int amountPaid, string toWhom, string dueAt)
        {
            this.amount = amount;
            this.amountPaid = amountPaid;
            this.toWhom = toWhom;
            this.dueAt = dueAt;
        }

        public void ViewUsersPayments(User loggedInUser)
        {
            int totalUnpaid = 0;
            string filepath = @"C:\Users\MirzaNiksic.AzureAD\Desktop\TestStar5\consoleApp\preparationTest.json";

            WebRequest webRequest = WebRequest.Create(filepath);
            WebResponse webResponse = webRequest.GetResponse();

            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseFromServer = reader.ReadToEnd();

                GetUsers root = JsonConvert.DeserializeObject<GetUsers>(responseFromServer);

                foreach (User user in root.users)
                {
                    if (user.email.Equals(loggedInUser.email))
                    {
                        foreach (var expense in user.expenses)
                        {
                            if (expense.payments != null && expense.payments.Count != 0)
                            {
                                foreach (var payments in expense.payments)
                                {
                                    Console.WriteLine("\n-------------------");
                                    Console.WriteLine("Name: " + expense.expenseName);
                                    Console.WriteLine("Who: " + payments.toWhom);
                                    Console.WriteLine("Amount paid: " + payments.amountPaid);

                                    int amountLeft = payments.amount - payments.amountPaid;
                                    Console.WriteLine("Left to pay: " + amountLeft);

                                    DateTime dueDate = DateTime.ParseExact(payments.dueAt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    int daysLeft = (int)dueDate.Subtract(DateTime.Now).TotalDays;
                                    if (daysLeft < 0)
                                    {
                                        daysLeft = 0;
                                    }
                                    Console.WriteLine("Days left: " + daysLeft);

                                    string isPaid = (amountLeft == 0) ? "Yes" : "No";
                                    Console.WriteLine("Paid: " + isPaid);

                                    totalUnpaid += amountLeft;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("\nTotal unpaid: " + totalUnpaid);
            }
        }
    }
}