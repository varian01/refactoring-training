using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        private static List<User> listOfUsers = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(@"Data/Users.json"));
        private static List<Product> listOfProducts = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText(@"Data/Products.json"));

        public static void Start()
        {
            DisplayWelcomeMessage();

            // Login
            Login:
            // Prompt for input of username
            string enteredUsername = EnterUserName();

            // Validate Username
            if (!string.IsNullOrEmpty(enteredUsername))
            {
                if (EnteredUserNameIsValid(enteredUsername))
                {
                    // Prompt for input of password
                    string enteredPassword = EnterPassword();

                    if (EnteredPasswordIsValid(enteredUsername, enteredPassword))
                    {
                        DisplayWelcomeMessage(enteredUsername);

                        double balance = EnterRemainingBalance(enteredUsername);

                        DisplayRemainingBalance(balance);

                        // Show product list
                        while (true)
                        {
                            DisplayProducts();

                            int itemSelected = EnterProductSelection();

                            // Check if user entered number that equals product count
                            if (itemSelected == listOfProducts.Count())
                            {
                                UpdateBalance(enteredUsername, balance);

                                WriteOutNewBalance();

                                WriteOutNewQuantities();
                                
                                PromptToExitConsole();
                                return;
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine("You want to buy: " + listOfProducts[itemSelected].Name);
                                Console.WriteLine("Your balance is " + balance.ToString("C"));

                                int quantity = EnterAmountToPurchase();

                                if (HasSufficientFunds(balance, itemSelected, quantity))
                                {
                                    continue;
                                }

                                if (QuantityIsInStock(itemSelected, quantity))
                                {
                                    continue;
                                }

                                // Check if quantity is greater than zero
                                if (quantity > 0)
                                {
                                    // Balance = Balance - Price * Quantity
                                    balance = balance - listOfProducts[itemSelected].Price * quantity;

                                    // Quanity = Quantity - Quantity
                                    listOfProducts[itemSelected].Qty = listOfProducts[itemSelected].Qty - quantity;

                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("You bought " + quantity + " " + listOfProducts[itemSelected].Name);
                                    Console.WriteLine("Your new balance is " + balance.ToString("C"));
                                    Console.ResetColor();
                                }
                                else
                                {
                                    // Quantity is less than zero
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine();
                                    Console.WriteLine("Purchase cancelled");
                                    Console.ResetColor();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Invalid Password
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine();
                        Console.WriteLine("You entered an invalid password.");
                        Console.ResetColor();

                        goto Login;
                    }
                }
                else
                {
                    // Invalid User
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("You entered an invalid user.");
                    Console.ResetColor();

                    goto Login;
                }
            }

            PromptToExitConsole();
        }

        public static void DisplayWelcomeMessage()
        {
            // Write welcome message
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");
        }

        public static string EnterUserName()
        {
            // Prompt for user input
            Console.WriteLine();
            Console.WriteLine("Enter Username:");
            return Console.ReadLine();
        }

        public static string EnterPassword()
        {
            // Prompt for user input
            Console.WriteLine("Enter Password:");
            return Console.ReadLine();
        }

        public static bool EnteredUserNameIsValid(string enteredUserName)
        {
            if (!string.IsNullOrEmpty(enteredUserName))
            {
                foreach (var user in listOfUsers)
                { 
                    // Check if entered username matches a name in the user list
                    if (user.Name == enteredUserName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EnteredPasswordIsValid(string username, string password)
        {
            // Validate Password
            foreach (var user in listOfUsers)
            { 
                // Check that name and password match
                if (user.Name == username && user.Pwd == password)
                {
                    return true;
                }
            }

            return false;
        }

        public static void DisplayWelcomeMessage(string enteredUserName)
        {
            // Show welcome message
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Login successful! Welcome " + enteredUserName + "!");
            Console.ResetColor();
        }

        public static double EnterRemainingBalance(string username)
        {
            double balance = 0;

            foreach (var user in listOfUsers)
            { 
                // Check that name and password match
                if (user.Name == username)
                {
                    return user.Bal;
                }
            }

            return balance;
        }

        public static void DisplayRemainingBalance(double bal)
        {
            // Show balance 
            Console.WriteLine();
            Console.WriteLine("Your balance is " + bal.ToString("C"));
        }

        public static void DisplayProducts()
        {

            // Prompt for user input
            Console.WriteLine();
            Console.WriteLine("What would you like to buy?");
            foreach (var product in listOfProducts)
            {
                Console.WriteLine(listOfProducts.IndexOf(product) + 1 + ": " + product.Name + " (" + product.Price.ToString("C") + ")");
            }
            Console.WriteLine(listOfProducts.Count + 1 + ": Exit");
        }

        public static int EnterProductSelection()
        {
            // Prompt for user input
            Console.WriteLine("Enter a number:");
            string answer = Console.ReadLine();
            int num = Convert.ToInt32(answer);
            num = num - 1; /* Subtract 1 from number
                            num = num + 1 // Add 1 to number */

            return num;
        }

        public static void UpdateBalance(string username,double balance)
        {
            // Update balance
            foreach (var user in listOfUsers)
            {
                // Check that name and password match
                if (user.Name == username)
                {
                    user.Bal = balance;
                }
            }
        }

        public static void WriteOutNewBalance()
        {
            // Write out new balance
            string json = JsonConvert.SerializeObject(listOfUsers, Formatting.Indented);
            File.WriteAllText(@"Data/Users.json", json);
        }

        public static void WriteOutNewQuantities()
        {
            // Write out new quantities
            string json2 = JsonConvert.SerializeObject(listOfProducts, Formatting.Indented);
            File.WriteAllText(@"Data/Products.json", json2);
        }

        public static int EnterAmountToPurchase()
        {
            // Prompt for user input
            Console.WriteLine("Enter amount to purchase:");
            string answer = Console.ReadLine();
            int quantity = Convert.ToInt32(answer);

            return quantity;
        }

        public static bool HasSufficientFunds(double balance, int itemSelected, int quantity)
        {
            // Check if balance - quantity * price is less than 0
            if (balance - listOfProducts[itemSelected].Price * quantity < 0)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("You do not have enough money to buy that.");
                Console.ResetColor();

                return false;
            }

            return true;
        }

        public static bool QuantityIsInStock(int itemSelected, int quantity)
        {
            // Check if quantity is less than quantity
            if (listOfProducts[itemSelected].Qty <= quantity)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("Sorry, " + listOfProducts[itemSelected].Name + " is out of stock");
                Console.ResetColor();

                return false;
            }

            return true;
        }

        public static void PromptToExitConsole()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit");
            Console.ReadLine();
        }

    }
}

