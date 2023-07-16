using System;

namespace ConsoleApp
{

    public class User
    {
        public string email { get; set; }
        public List<Expense> expenses { get; set; }

        public User(string email, List<Expense> expenses)
        {
            this.email = email;
            this.expenses = expenses;
        }

         public User()
        {
        }
    }

    public class GetUsers
    {
        public List<User> users { get; set; }
    }
}

