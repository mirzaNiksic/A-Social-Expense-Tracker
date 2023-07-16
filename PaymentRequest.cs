using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ConsoleApp
{
    public class PaymentRequest
    {
        public int amount { get; set; }
        public int amountPaid { get; set; }
        public string who { get; set; }
        public string dueAt { get; set; }

        public void ViewUsersPaymentRequestReceived(User loggedInUser)
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
                    foreach (var expense in user.expenses)
                    {
                        if (expense.paymentRequests != null)
                        {
                            foreach (var paymentRequests in expense.paymentRequests)
                            {
                                if (paymentRequests.who.Equals(loggedInUser.email))
                                {
                                    Console.WriteLine("\n-------------------");
                                    Console.WriteLine("Name: " + expense.expenseName);
                                    Console.WriteLine("You own to: " + "User who sent payment request");

                                    string dueDate = paymentRequests.dueAt;
                                    Console.WriteLine("Due to: " + dueDate);

                                    Console.WriteLine("Total amount: " + expense.amountSpent);
                                    Console.WriteLine("Amount requested: " + paymentRequests.amount);
                                    int amountLeft = paymentRequests.amount - paymentRequests.amountPaid;
                                    Console.WriteLine("Left to pay: " + amountLeft);

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

        public void ViewUsersPaymentRequest(User loggedInUser)
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
                            if (expense.paymentRequests != null)
                            {
                                foreach (var paymentRequests in expense.paymentRequests)
                                {
                                    Console.WriteLine("\n-------------------");
                                    Console.WriteLine("Name: " + expense.expenseName);
                                    Console.WriteLine("Requested from: " + paymentRequests.who);

                                    string dueDate = paymentRequests.dueAt;
                                    Console.WriteLine("Due to: " + dueDate);

                                    Console.WriteLine("Total amount: " + expense.amountSpent);
                                    Console.WriteLine("Amount requested: " + paymentRequests.amount);
                                    int amountLeft = paymentRequests.amount - paymentRequests.amountPaid;
                                    Console.WriteLine("Left to pay: " + amountLeft);

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

        public void AddNewPaymentRequest(User loggedInUser)
        {
            PaymentRequest newPaymentRequest = new PaymentRequest();
            Expense expenseToEdit = new Expense();
            User who = new User();
            var usersList = new List<User>();
            string filepath = @"C:\Users\MirzaNiksic.AzureAD\Desktop\TestStar5\consoleApp\preparationTest.json";
            bool isValid = false;
            var expenseName = "";

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
                        while (!isValid)
                        {
                            Console.WriteLine("\nChoose the expense for which you want to request payment: ");

                            foreach (var expense in user.expenses)
                            {
                                Console.WriteLine(expense.expenseName);
                            }
                            expenseName = Console.ReadLine();

                            foreach (var expense in user.expenses)
                            {
                                if (expense.expenseName.Equals(expenseName))
                                {
                                    expenseToEdit = user.expenses.FirstOrDefault(r => r.expenseName == expenseName);
                                    isValid = true;
                                }
                            }
                            if (!isValid)
                                Console.WriteLine("Wrong expense name, please try again");
                        }

                        isValid = false;
                        while (!isValid)
                        {
                            Console.Write("Enter email: ");
                            string whoEmail = Console.ReadLine();

                            foreach (User user2 in root.users)
                            {
                                if (user2.email.Equals(whoEmail) && !user2.email.Equals(loggedInUser.email))
                                {
                                    isValid = true;
                                    who = root.users.FirstOrDefault(r => r.email == whoEmail);
                                }
                            }
                        }

                        newPaymentRequest.who = who.email;
                        Console.Write("Enter payment request last pay day (dd/mm/yyyy): ");
                        string date = Console.ReadLine();
                        newPaymentRequest.dueAt = date;
                        Console.Write("Enter requested amount: ");
                        var amount = Console.ReadLine();
                        newPaymentRequest.amount = int.Parse(amount);
                        newPaymentRequest.amountPaid = 0;
                    }
                }

                var userToRemove = root.users.Single(r => r.email == loggedInUser.email);


                foreach (User a in root.users)
                {
                    if (a.email.Equals(loggedInUser.email))
                    {
                        loggedInUser.expenses.RemoveAll(x => x.expenseName == expenseToEdit.expenseName);
                        break;
                    }
                }

                root.users.Remove(userToRemove);
                expenseToEdit.paymentRequests.Add(newPaymentRequest);
                loggedInUser.expenses.Add(expenseToEdit);

                root.users.Add(loggedInUser);
                usersList = root.users;
            }
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
        }

        public void PayRequest(User loggedInUser)
        {
            PaymentRequest requestToPay = new PaymentRequest();
            Expense expenseToPay = new Expense();
            Expense expenseToAdd = new Expense();
            Payment payment = new Payment();
            User who = new User();
            User userToDelete = new User();
            User userToDelete2 = new User();
            var usersList = new List<User>();
            string filepath = @"C:\Users\MirzaNiksic.AzureAD\Desktop\TestStar5\consoleApp\preparationTest.json";
            bool isValid = false;
            var expenseName = "";
            var amountToPay = 0;
            int newAmountPaid = 0;
            Dictionary<string, Expense> mergedExpenses = new Dictionary<string, Expense>();

            Console.WriteLine("Pay a payment request? (y/n)");
            string input = Console.ReadLine();

            if (input == "y")
            {
                Console.WriteLine("\nChoose the expense for which you want to request payment: ");

                WebRequest webRequest = WebRequest.Create(filepath);
                WebResponse webResponse = webRequest.GetResponse();

                using (Stream stream = webResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string responseFromServer = reader.ReadToEnd();

                    GetUsers root = JsonConvert.DeserializeObject<GetUsers>(responseFromServer);

                    foreach (User user in root.users)
                    {
                        foreach (var expense in user.expenses)
                        {
                            if (expense.paymentRequests != null)
                            {
                                foreach (var paymentRequests in expense.paymentRequests)
                                {
                                    if (paymentRequests.who.Equals(loggedInUser.email))
                                    {
                                        Console.WriteLine("Expense name: " + expense.expenseName);
                                    }
                                }
                            }
                        }
                    }

                    var toPay = Console.ReadLine();

                    foreach (User user in root.users)
                    {
                        foreach (var expense in user.expenses)
                        {
                            if (expense.expenseName.Equals(toPay))
                            {
                                if (expense.paymentRequests.Count != 0)
                                {
                                    foreach (var paymentRequests in expense.paymentRequests)
                                    {
                                        if (paymentRequests.who.Equals(loggedInUser.email))
                                        {
                                            expenseToPay = expense;
                                            expenseToAdd = Expense.GetInstanceWithoutPaymentReq(expense);
                                            Console.WriteLine("How much do you want to pay?");
                                            amountToPay = int.Parse(Console.ReadLine());
                                            paymentRequests.amountPaid = paymentRequests.amountPaid + amountToPay;
                                            newAmountPaid = paymentRequests.amountPaid;
                                            requestToPay = paymentRequests;
                                            requestToPay.amountPaid = 0;
                                            requestToPay.amountPaid = newAmountPaid;
                                            userToDelete = user;
                                            break;
                                        }
                                    }
                                }
                                payment = new Payment(requestToPay.amount - amountToPay, amountToPay, user.email, DateTime.Now.ToString("MM/dd/yyyy"));
                            }
                        }
                        userToDelete2 = root.users.FirstOrDefault(u => u.email == loggedInUser.email);
                    }

                    root.users.Remove(userToDelete);
                    root.users.Remove(userToDelete2);

                    expenseToAdd.payments.Add(payment);
                    userToDelete2.expenses.Add(expenseToAdd);

                    userToDelete.expenses.Remove(expenseToPay);
                    expenseToPay = Expense.GetInstanceWithoutPayment(expenseToPay);

                    userToDelete.expenses.Add(expenseToPay);

                    root.users.Add(userToDelete);
                    root.users.Add(userToDelete2);

                    usersList = root.users;
                }

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
            }
        }
    }
}

