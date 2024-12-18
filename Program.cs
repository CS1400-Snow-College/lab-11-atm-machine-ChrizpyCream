﻿//Christopher Hercules, 12/2/24, lab 11
string[] DataBase = File.ReadAllLines("bank.txt"); // changed from Bank.txt to bank.txt|stores them in an array called DataBase


Console.WriteLine("Please enter your username.");
string getUsername = Console.ReadLine(); //user input for username
string name = " "; //placeholder for user's name
string saved_data = "";// String variables for saving transaction data and other user details
string sub_data = "";

// user options displayed
string welcomeMenu = @$"
Welcome {name} what would you like to do?
1)Check Balance
2)Withdraw
3)Deposit
4)Display last transactions
5)Quick Withdraw $40
6)Quick Withdraw $100
7)End current session
Please pick the number corresponding to the option.
";

// loop through each line in the database to find the corresponding username, money and validate the PIN
for (int i = 0; i < DataBase.Length; i++)
{
    // split the current line into separate values 
    string[] splitLines = DataBase[i].Split(",");

    // iterate through the fields of the current line to find the username
    for (int j = 0; j < splitLines.Length; j++)
    {
        // when the username matches, prompt for PIN
        if (getUsername == splitLines[j])
        {
            Console.WriteLine("ENTER YOUR PIN");
            string getPin = Console.ReadLine(); // User input for PIN

            // checks if the entered PIN is correct
            if (getPin == splitLines[j + 1])
            {
                // it clears console after successful PIN verification
                Console.Clear();
                name = splitLines[j]; // sets the name to the username from the database
                // updated welcome menu with the correct user name
                welcomeMenu = @$"
Welcome {name} what would you like to do?
1)Check Balance
2)Withdraw
3)Deposit
4)Display last transactions
5)Quick Withdraw $40
6)Quick Withdraw $100
7)End current session
Please pick the number corresponding to the option.
";
                Console.WriteLine(welcomeMenu);

                // creates a queue to store last transactions (up to 5)
                Queue<string> lastTransactions = new Queue<string>();
                bool exit = false; // control flag for exit loop
                
                // starts the menu loop for user interactions
                do
                {
                    // makes sure only the last 5 transactions are kept in the queue
                    if (lastTransactions.Count > 5)
                    {
                        lastTransactions.Dequeue(); // Remove the oldest transaction
                    }

                    try
                    {
                        // read the user's menu choice
                        int choice = Convert.ToInt32(Console.ReadLine());
                        
                        // switch statement for handling different options
                        switch (choice)
                        {
                            case 1:
                                
                                Console.WriteLine($"YOUR CURRENT BALANCE STANDS AT {splitLines[j + 2]}");
                                break;
                            case 2:
                                
                                Console.WriteLine("HOW MUCH WOULD YOU LIKE TO WITHDRAW?");
                                string dollarSign = splitLines[j + 2].Substring(0, 1); // Get the currency symbol
                                string stringBalance = splitLines[j + 2].Substring(1); // Get the numeric balance
                                decimal getCurrentBalance = ReadDecimal(stringBalance); // Convert string balance to decimal
                                splitLines[j + 2] = dollarSign + decimalToString(Withdraw(ReadDecimal(Console.ReadLine()), getCurrentBalance)); // Update balance after withdrawal
                                lastTransactions.Enqueue($"{dollarSign}{stringBalance} to {splitLines[j + 2]}"); // Log the transaction
                                Console.WriteLine("ACCOUNT UPDATED. WHAT IS NEXT?");
                                break;
                            case 3:
                               
                                Console.WriteLine("HOW MUCH WOULD YOU LIKE TO DEPOSIT?");
                                stringBalance = splitLines[j + 2].Substring(1);
                                dollarSign = splitLines[j + 2].Substring(0, 1);
                                getCurrentBalance = ReadDecimal(stringBalance);
                                splitLines[j + 2] = dollarSign + decimalToString(Deposit(ReadDecimal(Console.ReadLine()), getCurrentBalance));
                                lastTransactions.Enqueue($"{dollarSign}{stringBalance} to {splitLines[j + 2]}");
                                Console.WriteLine("ACCOUNT UPDATED. WHAT IS NEXT?");
                                break;
                            case 4:
                               
                                foreach (string transaction in lastTransactions)
                                {
                                    Console.WriteLine(transaction);
                                }
                                break;
                            case 5:
                                // Quick withdraw of $40
                                stringBalance = splitLines[j + 2].Substring(1);
                                dollarSign = splitLines[j + 2].Substring(0, 1);
                                getCurrentBalance = ReadDecimal(stringBalance);
                                splitLines[j + 2] = dollarSign + decimalToString(Withdraw(40, getCurrentBalance));
                                lastTransactions.Enqueue($"{dollarSign}{stringBalance} to {splitLines[j + 2]}");
                                Console.WriteLine("ACCOUNT UPDATED. WHAT IS NEXT?");
                                break;
                            case 6:
                                // Quick withdraw of $100
                                stringBalance = splitLines[j + 2].Substring(1);
                                dollarSign = splitLines[j + 2].Substring(0, 1);
                                getCurrentBalance = ReadDecimal(stringBalance);
                                splitLines[j + 2] = dollarSign + decimalToString(Withdraw(100, getCurrentBalance));
                                lastTransactions.Enqueue($"{dollarSign}{stringBalance} to {splitLines[j + 2]}");
                                Console.WriteLine("ACCOUNT UPDATED. WHAT IS NEXT?");
                                break;
                            case 7:
                                // End the session
                                exit = true;
                                break;
                            default:
                                // Invalid option handling
                                Console.WriteLine("NOT AN OPTION");
                                break;
                        }
                    }
                    catch (FormatException)
                    {
                        // non-numeric input
                        Console.WriteLine("DID NOT REGISTER OR OPTION INVALID");
                        Console.WriteLine(welcomeMenu);
                    }
                } while (exit == false); // keeps the loop going until user chooses to exit
            }
            else
            {
                
                Console.WriteLine("INCORRECT PIN, TRY AGAIN");
            }
        }
  // rebuild the data string for saving back to the file
  sub_data = sub_data + "," + splitLines[j];
    }

    // Remove the leading comma and add the updated data to saved_data
    sub_data = sub_data.Substring(1);
    saved_data = saved_data + sub_data + '\n';
    sub_data = "";
}

// write the updated bank data back to the "bank.txt" file
File.WriteAllText("bank.txt", saved_data);

//  withdrawing money from an account
decimal Withdraw(decimal amountWithdraw, decimal currentBalance)
{
    try
    {
        // checks if the withdrawal amount is greater than the current balance
        if (currentBalance < amountWithdraw)
        {
            throw new ArgumentException("NOT ENOUGH FUNDS TO PROCEED");
        }

        // (checks if the withdrawal amount is negative) important
        if (amountWithdraw < 0)
        {
            throw new FormatException();
        }

        // calculate and return the new balance after withdrawal
        decimal newBalance = currentBalance - amountWithdraw;
        return newBalance;
    }
    catch (ArgumentException e)
    {
        // Handle insufficient funds error
        Console.WriteLine(e.Message);
        Console.WriteLine(welcomeMenu);
        return currentBalance; // Return the original balance if withdrawal failed
    }
}

// depositing money into an account
decimal Deposit(decimal amountDeposit, decimal currentBalance)
{
    try
    {
        // checks if the deposit amount is negative
        if (amountDeposit < 0)
        {
            throw new ArgumentException("THATS A WITHDRAWAL NOT A DEPOSIT");
        }

        // calculate and return the new balance after deposit
        decimal newBalance = currentBalance + amountDeposit;
        return newBalance;
    }
    catch (ArgumentException e)
    {
        // handlea negative deposit error
        Console.WriteLine(e.Message);
        Console.WriteLine(welcomeMenu);
        return currentBalance; // returns the original balance if deposit failed
    }
}
// converting string input to decimal safely
decimal ReadDecimal(string getInput)
{
    decimal number = -1;
    do
    {
        // Convert the input string to decimal
        number = Convert.ToDecimal(getInput);
    } while (number == -1); // keeps retrying if conversion failed
    return number;
}

// converting decimal value to string
string decimalToString(decimal Input)
{
    string Output = "";
    try
    {
        // convert decimal to string
        Output = Input.ToString();
        return Output;
    }
    catch (Exception)
    {
        // handle exception if conversion fails
        Console.WriteLine("INVALID STRING");
    }
    return Output; // Return the string representation of the decimal
}
