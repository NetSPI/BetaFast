using System.Collections.Generic;
using System;

namespace BetaBank.Quotes
{
    public static class Quotes
    {
        private static List<string> _quotes = new List<string>
        {
            @"You're the type of business person who doesn't even need to know how business works. Because you figured it all out.",
            @"Our account balances now support scientific notation.",
            @"You're a world traveler. You're a market mover. You've played tennis in prison. And now, you keep your money in Beta Bank.",
            @"Don't just buy private islands in the Pacific - keep your money there.",
            @"Last week, you thought the GDP of a small European nation was your quarterly bonus. And you wondered why it was so small.",
            @"This is the bank for the person who needs a place to keep their banks.",
            @"The bank for the person who still speaks with a Mid-Atlantic accent.",
            @"Time is money, and your daughter's cello recital would have been worth millions had you gone.",
            @"Believe it or not, some people shop AT department stores, not FOR department stores. And believe it or not, your money is safe with us."
        };

        public static string GetQuote()
        {
            Random random = new Random();
            return _quotes[random.Next(_quotes.Count)];
        }
    }
}
