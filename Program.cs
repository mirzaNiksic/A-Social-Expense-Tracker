using System;
using System.Net.NetworkInformation;

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
                    ShowMainMenuOptions(loggedInUser);
                    break;
                case "2":
                    loggedInUser = register.register();
                    ShowMainMenuOptions(loggedInUser);
                    break;
                default:
                    break;
            }
        }

        public static void ShowMainMenuOptions(User loggedInUser)
        {
            var payment = new Payment();

            Console.WriteLine("-------------------");
            Console.WriteLine("Choose action");
            Console.WriteLine("-------------------");
            Console.WriteLine("1: My Expenses");
            Console.WriteLine("2: Payment Requests");
            Console.WriteLine("3: View my Payments");
            Console.WriteLine("4: Log out");
            Console.WriteLine("-----------------");

            string command = Console.ReadLine();

            switch (command)
            {
                case "1":
                    ShowMyExpensesOptions(loggedInUser);
                    break;
                case "2":
                    ShowPaymentRequestOptions(loggedInUser);
                    ShowMainMenuOptions(loggedInUser);
                    break;
                case "3":
                    payment.ViewUsersPayments(loggedInUser);
                    ShowMainMenuOptions(loggedInUser);
                    break;
                case "4":
                    Start();
                    break;
                default:
                    break;
            }
        }

        public static void ShowMyExpensesOptions(User loggedInUser)
        {
            var expense = new Expense();

            Console.WriteLine("-------------------");
            Console.WriteLine("Choose action");
            Console.WriteLine("-------------------");
            Console.WriteLine("1: My Expenses details");
            Console.WriteLine("2: Add a new expense");
            Console.WriteLine("3: Open Existing Expense");
            Console.WriteLine("4: Go to Main Menu");
            Console.WriteLine("-------------------");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    expense.ExpensesList(loggedInUser);
                    ShowMyExpensesOptions(loggedInUser);
                    break;
                case "2":
                    expense.AddExpense(loggedInUser);
                    ShowMyExpensesOptions(loggedInUser);
                    break;
                case "3":
                    expense.ViewExistingExpense(loggedInUser);
                    ShowMyExpensesOptions(loggedInUser);
                    break;
                case "4":
                    ShowMainMenuOptions(loggedInUser);
                    break;
                default:
                    break;
            }
        }

        public static void ShowPaymentRequestOptions(User loggedInUser)
        {
            var paymentRequest = new PaymentRequest();

            Console.WriteLine("-------------------");
            Console.WriteLine("Choose action");
            Console.WriteLine("-------------------");
            Console.WriteLine("1: Add a new payment request");
            Console.WriteLine("2: View payment requests");
            Console.WriteLine("3: View payment requests received");
            Console.WriteLine("4: Go to Main Menu");
            Console.WriteLine("-------------------");

            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    paymentRequest.AddNewPaymentRequest(loggedInUser);
                    ShowPaymentRequestOptions(loggedInUser);
                    break;
                case "2":
                    paymentRequest.ViewUsersPaymentRequest(loggedInUser);
                    ShowPaymentRequestOptions(loggedInUser);
                    break;
                case "3":
                    paymentRequest.ViewUsersPaymentRequestReceived(loggedInUser);
                    ShowPaymentRequestOptions(loggedInUser);
                    break;
                case "4":
                    ShowMainMenuOptions(loggedInUser);
                    break;
                default:
                    break;
            }
        }
    }
}
