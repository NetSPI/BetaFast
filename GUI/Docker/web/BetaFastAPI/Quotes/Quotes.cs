using System.Collections.Generic;
using System;

namespace BetaFastAPI.Quotes
{
    public static class Quotes
    {
        private static List<string> _quotes = new List<string>
        {
            "Renting the latest Betamax has never been so easy! I picked up a copy of Superman II as I was leaving the grocery store.",
            "Sure, I can pick up some VHS tapes on my way home. Are they over by the triceratopses?",
            "I don't mind that I have to get up halfway through my movie to change tapes. Sitting is unhealthy!",
            "I thought about buying some movies that I'd only watch once. Then I realized I could rent them!",
            "There's a reason it's not called Betamin!",
            "I'm having a BetaBlast with BetaFast!"
        };

        private static List<string> _speakers = new List<string>
        {
            "Eric Gruber",
            "Eric Gruber, local celebrity",
            "Eric Gruber, known shoplifter",
            "Eric Gruber, champion thumb wrestler",
            "Eric \"Eye in the Sky\" Gruber",
            "Eric Gruber, man of the people",
            "Eric Gruber, Security Consultant of the Stars"
        };

        public static string GetTestimonial()
        {
            return WrapInQuotes(GetQuote()) + " - " + GetSpeaker();
        }

        private static string WrapInQuotes(string quote)
        {
            return "\"" + quote + "\"";
        }

        private static string GetQuote()
        {
            Random random = new Random();
            return _quotes[random.Next(_quotes.Count)];
        }

        private static string GetSpeaker()
        {
            Random random = new Random();
            return _speakers[random.Next(_speakers.Count)];
        }
    }
}
