using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
        }

        public static void Start()
        {
            var login = new Login();
            var register = new Register();
            User loggedInUser = new User();

            Console.WriteLine("\n-------------------");
            Console.WriteLine("Welcome to the Expenses Tracker App!");
            Console.WriteLine("-------------------");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Sing up");
            Console.WriteLine("-----------------");

            string command = Console.ReadLine();

            switch (command)
            {
                case "1":
                    loggedInUser = login.login();
                    ShowOptions(loggedInUser);
                    break;
                case "2":
                    loggedInUser = register.register();
                    ShowOptions(loggedInUser);
                    break;
                default:
                    break;
            }
        }

        public static void ShowOptions(User loggedInUser)
        {
            var login = new Login();
            var expense = new Expense();
            var paymentRequest = new PaymentRequest();
            var payment = new Payment();

            Console.WriteLine("-------------------");
            Console.WriteLine("Choose action");
            Console.WriteLine("-------------------");
            Console.WriteLine("1: Add a new expense");
            Console.WriteLine("2: View expenses");
            Console.WriteLine("3: Add a new payment request");
            Console.WriteLine("4: View payment requests");
            Console.WriteLine("5: View payment requests received");
            Console.WriteLine("6: View payments");
            Console.WriteLine("7: Log out");
            Console.WriteLine("-----------------");

            string command = Console.ReadLine();

            switch (command)
            {
                case "1":
                    expense.AddExpense(loggedInUser);
                    ShowOptions(loggedInUser);
                    break;
                case "2":
                    expense.ViewUsersExpenses(loggedInUser);
                    ShowOptions(loggedInUser);
                    break;
                case "3":
                    paymentRequest.AddNewPaymentRequest(loggedInUser);
                    ShowOptions(loggedInUser);
                    break;
                case "4":
                    paymentRequest.ViewUsersPaymentRequest(loggedInUser);
                    ShowOptions(loggedInUser);
                    break;
                case "5":
                    paymentRequest.ViewUsersPaymentRequestReceived(loggedInUser);
                    ShowOptions(loggedInUser);
                    break;
                case "6":
                    payment.ViewUsersPayments(loggedInUser);
                    ShowOptions(loggedInUser);
                    break;
                case "7":
                    Start();
                    break;
                default:
                    break;
            }
        }
    }
}

