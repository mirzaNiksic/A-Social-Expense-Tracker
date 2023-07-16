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
                                    Console.WriteLine("Expense name: " + expense.expenseName);
                                    Console.WriteLine("You own to: " + user.email);

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
            if (totalUnpaid > 0) PayRequest(loggedInUser);
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
            PaymentRequest deletePaymentRequest = new PaymentRequest();
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

                        if (user.expenses.Count == 0)
                        {
                            Console.WriteLine("You do not have any Expense for which you can add payment request.");
                            break;
                        }
                        else
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
                            DateTime time;
                            string input;
                            do
                            {
                                input = Console.ReadLine();
                                if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out time) == false)
                                {
                                    Console.WriteLine("Invalid date format.");
                                    Console.Write("Enter expense date (dd/mm/yyyy): ");
                                }
                            }
                            while (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out time) == false);
                            var dateOnly = DateOnly.FromDateTime(time);
                            newPaymentRequest.dueAt = dateOnly.ToString();

                            int amount;
                            Console.Write("Enter requested amount: ");
                            do
                            {
                                input = Console.ReadLine();
                                if (int.TryParse(input, out amount) == false)
                                {
                                    Console.WriteLine("Please enter a valid number.");
                                    Console.Write("Enter expense amount: ");
                                }
                            }
                            while (int.TryParse(input, out amount) == false || amount <= 0);
                            newPaymentRequest.amount = amount;

                            foreach (var paymentRequests in expenseToEdit.paymentRequests)
                            {
                                if (paymentRequests.who.Equals(who.email))
                                {
                                    deletePaymentRequest = paymentRequests;
                                    newPaymentRequest.amount = paymentRequests.amount + amount;
                                    newPaymentRequest.amountPaid = paymentRequests.amountPaid;
                                }
                            }
                        }
                    }
                }

                if (isValid)
                {
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
                    expenseToEdit.paymentRequests.Remove(deletePaymentRequest);
                    expenseToEdit.paymentRequests.Add(newPaymentRequest);
                    loggedInUser.expenses.Add(expenseToEdit);

                    root.users.Add(loggedInUser);
                    usersList = root.users;
                }
            }

            if (isValid)
            {
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

        public void PayRequest(User loggedInUser)
        {
            PaymentRequest requestToPay = new PaymentRequest();
            Expense expenseToPay = new Expense();
            Expense expenseToAdd = new Expense();
            Expense expenseToRemove = new Expense();
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
                            }
                        }
                        userToDelete2 = root.users.FirstOrDefault(u => u.email == loggedInUser.email);
                    }

                    foreach (User user in root.users)
                    {
                        if (user.email.Equals(loggedInUser.email))
                        {
                            if (user.expenses.Count != 0)
                            {
                                foreach (var expense in user.expenses)
                                {
                                    if (expense.expenseName.Equals(expenseToAdd.expenseName))
                                    {
                                        expenseToRemove = expense;
                                        foreach (var payments in expense.payments)
                                        {
                                            payment = new Payment(requestToPay.amount, payments.amountPaid + amountToPay, userToDelete.email, DateTime.Now.ToString("MM/dd/yyyy"));
                                        }
                                    }
                                    else payment = new Payment(requestToPay.amount, amountToPay, userToDelete.email, DateTime.Now.ToString("MM/dd/yyyy"));
                                }
                            }
                            else payment = new Payment(requestToPay.amount, amountToPay, userToDelete.email, DateTime.Now.ToString("MM/dd/yyyy"));
                        }
                    }

                    root.users.Remove(userToDelete);
                    root.users.Remove(userToDelete2);

                    userToDelete2.expenses.Remove(expenseToRemove);
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

