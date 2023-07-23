using System;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;

namespace ConsoleApp
{
    public class Expense
    {
        public string expenseDate { get; set; }
        public string expenseName { get; set; }
        public string amountSpent { get; set; }
        public List<PaymentRequest> paymentRequests { get; set; }
        public List<Payment> payments { get; set; }

        public Expense() { }

        public Expense(string expenseDate, string expenseName, string amountSpent, List<PaymentRequest> paymentRequests, List<Payment> payments)
        {
            this.expenseDate = expenseDate;
            this.expenseName = expenseName;
            this.amountSpent = amountSpent;
            this.paymentRequests = paymentRequests;
            this.payments = payments;
        }

        public Expense(Expense expense)
        {
            this.expenseDate = expense.expenseDate;
            this.expenseName = expense.expenseName;
            this.amountSpent = expense.amountSpent;
            this.paymentRequests = new List<PaymentRequest>();
            this.payments = expense.payments;
        }

        public static Expense GetInstanceWithoutPaymentReq(Expense expense)
        {
            return new Expense(expense.expenseDate, expense.expenseName, expense.amountSpent, new List<PaymentRequest>(), expense.payments);
        }

        public static Expense GetInstanceWithoutPayment(Expense expense)
        {
            return new Expense(expense.expenseDate, expense.expenseName, expense.amountSpent, expense.paymentRequests, new List<Payment>());
        }

        public void ViewExistingExpense(User loggedInUser)
        {
            string filepath = @"C:\Users\MirzaNiksic.AzureAD\Desktop\TestStar5\consoleApp\preparationTest.json";
            var pR = new PaymentRequest();

            int totalExpenses = 0;
            int expenseAmount = 0;
            int requested = 0;
            int owe = 0;
            int netPrice = 0;
            string epenseNameInput = "";

            WebRequest webRequest = WebRequest.Create(filepath);
            WebResponse webResponse = webRequest.GetResponse();

            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseFromServer = reader.ReadToEnd();

                GetUsers root = JsonConvert.DeserializeObject<GetUsers>(responseFromServer);

                foreach (User user in root.users)
                {
                    foreach (Expense expense in user.expenses)
                    {
                        if (user.email.Equals(loggedInUser.email))
                        {
                            Console.WriteLine("\n-------------------");
                            Console.WriteLine($"Choose which expense you want to open: ");
                            Console.WriteLine($"Expense Name: {expense.expenseName}");
                        }
                    }
                }
                epenseNameInput = Console.ReadLine();

                foreach (User user in root.users)
                {
                    if (user.email.Equals(loggedInUser.email))
                    {
                        foreach (Expense expense in user.expenses)
                        {
                            if (expense.expenseName.Equals(epenseNameInput))
                            {
                                requested = 0;
                                owe = 0;
                                expenseAmount = 0;

                                if (user.email.Equals(loggedInUser.email))
                                {
                                    Console.WriteLine("\n-------------------");
                                    Console.WriteLine($"Expense Name: {expense.expenseName}");
                                    Console.WriteLine($"Expense Date: {expense.expenseDate}");
                                    Console.WriteLine($"Amount spent: {expense.amountSpent}");

                                    expenseAmount = int.Parse(expense.amountSpent);

                                    if (expense.paymentRequests.Count != 0)
                                    {
                                        foreach (var paymentRequest in expense.paymentRequests)
                                        {
                                            int amountLeft = paymentRequest.amount - paymentRequest.amountPaid;
                                            if (amountLeft > 0)
                                            {
                                                Console.WriteLine($"Payment requests: Amount: {paymentRequest.amount}, Paid: {paymentRequest.amountPaid}, dueAt: {paymentRequest.dueAt}, to: {paymentRequest.who}, Need to pay: {amountLeft}");
                                            }
                                            else Console.WriteLine($"Payment requests: Amount: {paymentRequest.amount}, Paid: {paymentRequest.amountPaid}, dueAt: {paymentRequest.dueAt}, to: {paymentRequest.who}");
                                            requested = requested + paymentRequest.amount;
                                        }
                                    }
                                    if (expense.paymentRequests.Count != 0)
                                    {
                                        foreach (var paymentRequests in expense.paymentRequests)
                                        {
                                            if (paymentRequests.who.Equals(loggedInUser.email))
                                            {
                                                Console.WriteLine("\n-------------------");
                                                Console.WriteLine($"Expense Name: {expense.expenseName}");
                                                Console.WriteLine($"Expense Date: {expense.expenseDate}");
                                                Console.WriteLine($"Amount spent: {expense.amountSpent}");
                                                Console.WriteLine($"Requested from me: Amount: {paymentRequests.amount}, Paid: {paymentRequests.amountPaid}, dueAt: {paymentRequests.dueAt}");
                                                owe = owe + paymentRequests.amount;
                                            }
                                        }
                                    }
                                }

                                netPrice = expenseAmount - requested + owe;
                                totalExpenses = totalExpenses + netPrice;

                                if (netPrice != 0)
                                {
                                    Console.WriteLine($"Payment requested: {requested}");
                                    Console.WriteLine($"Net expenses: {netPrice}");
                                }
                            }
                        }
                    }
                }
                Console.WriteLine($"Add payment request for this expense? (y/n)");
                string input = Console.ReadLine();
                if (input.Equals("y")) pR.AddNewPaymentRequest(loggedInUser, epenseNameInput);
            }
        }

        public void ExpensesList(User loggedInUser)
        {
            string filepath = @"C:\Users\MirzaNiksic.AzureAD\Desktop\TestStar5\consoleApp\preparationTest.json";

            int totalExpenses = 0;
            int expenseAmount = 0;
            int requested = 0;
            int owe = 0;
            int netPrice = 0;

            WebRequest webRequest = WebRequest.Create(filepath);
            WebResponse webResponse = webRequest.GetResponse();

            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseFromServer = reader.ReadToEnd();

                GetUsers root = JsonConvert.DeserializeObject<GetUsers>(responseFromServer);

                foreach (User user in root.users)
                {
                    foreach (Expense expense in user.expenses)
                    {
                        requested = 0;
                        owe = 0;
                        expenseAmount = 0;

                        if (user.email.Equals(loggedInUser.email))
                        {
                            if (expense.payments.Count == 0)
                            {
                                Console.WriteLine("\n-------------------");
                                Console.WriteLine($"Expense Name: {expense.expenseName}");
                                Console.WriteLine($"Expense Date: {expense.expenseDate}");
                                Console.WriteLine($"Amount spent: {expense.amountSpent}");

                                expenseAmount = int.Parse(expense.amountSpent);

                                if (expense.paymentRequests.Count != 0)
                                {
                                    foreach (var paymentRequest in expense.paymentRequests)
                                    {
                                        int amountLeft = paymentRequest.amount - paymentRequest.amountPaid;
                                        requested = requested + paymentRequest.amount;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (expense.paymentRequests.Count != 0)
                            {
                                foreach (var paymentRequests in expense.paymentRequests)
                                {
                                    if (paymentRequests.who.Equals(loggedInUser.email))
                                    {
                                        owe = owe + paymentRequests.amount;
                                    }
                                }
                            }
                        }

                        netPrice = expenseAmount - requested + owe;
                        totalExpenses = totalExpenses + netPrice;

                        if (netPrice != 0)
                        {
                            Console.WriteLine($"Net expenses: {netPrice}");
                            Console.WriteLine($"Payment requested: {requested}");
                            Console.WriteLine($"Payment received : {owe}");
                        }
                    }
                }
            }
            Console.WriteLine($"\nTotal expenses: {totalExpenses}");
        }

        public void AddExpense(User loggedInUser)
        {
            var newExpense = new Expense();
            var usersList = new List<User>();

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
                        string input;
                        Console.Write("Enter expense name: ");
                        string expenseName = Console.ReadLine();
                        newExpense.expenseName = expenseName;

                        Console.Write("Enter expense date (dd/mm/yyyy): ");
                        DateTime time;
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
                        newExpense.expenseDate = dateOnly.ToString();

                        int amount;
                        Console.Write("Enter expense amount: ");
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
                        newExpense.amountSpent = amount.ToString();

                        newExpense.paymentRequests = new List<PaymentRequest> { };
                        newExpense.payments = new List<Payment> { };
                    }
                }
                var userToRemove = root.users.Single(r => r.email == loggedInUser.email);
                root.users.Remove(userToRemove);
                loggedInUser.expenses.Add(newExpense);
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
    }

}

