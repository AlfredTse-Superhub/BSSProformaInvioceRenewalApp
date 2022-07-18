// See https://aka.ms/new-console-template for more information
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("You are using BSS Proforma Invoice console application V1.0!");
            Console.WriteLine();
            Console.WriteLine("Rules:");
            Console.WriteLine("Step1: Enter 1 or more subscription ID from the same cusomter");
            Console.WriteLine("Step2: A PDF copy of invoice will be saved in the directory of this app.");
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            do
            {
                Console.WriteLine("Please input subscription ID, use spacing for multiple IDs:id1 id2 id3");
                string subscriptionIDInput = Console.ReadLine();
                IEnumerable<string> subscriptionIDList = subscriptionIDInput
                    .Split()
                    .Where(s => s != string.Empty);

                if (subscriptionIDList.Count() == 0)
                {
                    Console.WriteLine("Error: Empty input");
                    continue;
                }
                    
                foreach (string subscriptionID in subscriptionIDList)
                {
                    Console.WriteLine(subscriptionID);
                }
                Console.WriteLine("Checking subscriptions...");
                Console.WriteLine("Generating PDF...");
                Console.WriteLine("Completed!");
                Console.WriteLine();

            } while (true);

        }
    }
}
