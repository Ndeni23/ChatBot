using System;
using System.IO;
using System.Media;
using ChatBot;

namespace chatbot
{
    public class Program
    {
        private static ChatMemory memory = new ChatMemory();
        private static string lastUserInput = ""; // Track last user input

        public static void Main(string[] args)
        {
            
            HandleWelcome();

            string userName = GetUserName();
            UserData.SaveUserDetails(userName);
            RunChatbot(userName);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void HandleWelcome()
        {
            welcome_message welcome = new welcome_message();
            welcome.Play();
            Console.WriteLine("\nWelcome to the Cybersecurity Awareness Chatbot!\n");
        }

        private static string GetUserName()
        {
            Console.Write("Chatbot: Hello! What's your name and surname? ");
            string userName = Console.ReadLine()?.Trim() ?? "User";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nChatbot: Welcome, {userName}! I'm here to help you learn about cybersecurity.");
            Console.WriteLine("Just type 'exit' to quit.\n");
            Console.ResetColor();

            return userName;
        }

        private static void RunChatbot(string userName)
        {
            ResponseHandler handler = new ResponseHandler();
            Conversation conversation = new Conversation();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{userName}: ");
                Console.ResetColor();

                string userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Chatbot: Please ask a question.");
                    Console.ResetColor();
                    continue;
                }

                userInput = userInput.Trim();
                lastUserInput = userInput;
                UserData.SaveUserQuestion(userInput);

                if (userInput.ToLowerInvariant() == "exit")
                {
                    Console.WriteLine($"Chatbot: Goodbye, {userName}! Stay safe online.");
                    break;
                }

                if (userInput.ToLower().StartsWith("remember"))
                {
                    if (userInput.Length <= "remember".Length || userInput.Substring("remember".Length).Trim().Length == 0)
                    {
                        Console.WriteLine("\nChatbot: Please tell me what to remember. For example, 'remember I use a VPN'.\n");
                        continue;
                    }
                    string fact = userInput.Substring("remember".Length).Trim();
                    memory.StoreFact(fact);
                    UserData.SaveUserFact(fact);
                    Console.WriteLine("\nChatbot: Got it! I’ll remember that.\n");
                    continue;
                }

                if (userInput.ToLower().Contains("what do you remember"))
                {
                    string recall = memory.RecallFacts();
                    Console.WriteLine($"\nChatbot: {recall}\n");
                    continue;
                }

                if (userInput.ToLower().Contains("how are you"))
                {
                    Console.WriteLine("\nChatbot: I'm doing great, thanks for asking! How can I help you with cybersecurity today?\n");
                    continue;
                }

                if (userInput.ToLower().Contains("what is my name"))
                {
                    Console.WriteLine($"\nChatbot: Your name is {userName}! How can I assist you today?\n");
                    continue;
                }

                if (userInput.ToLower().Contains("what did i ask"))
                {
                    if (string.IsNullOrEmpty(lastUserInput))
                    {
                        Console.WriteLine("\nChatbot: You haven't asked anything yet!\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nChatbot: You last asked: '{lastUserInput}'. How can I help you now?\n");
                    }
                    continue;
                }

                string response = handler.GetResponse(userInput, memory, userName, conversation);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nChatbot: {response}\n");
                Console.ResetColor();
            }
        }
    }
}