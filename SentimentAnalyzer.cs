using System;
using System.Linq;

namespace ChatBot
{
    public static class SentimentAnalyzer
    {
        public static string AnalyzeSentinent(string input)
        {
            input = input.ToLower();
            if (input.Contains("worried") || input.Contains("scared")) return "worried";
            if (input.Contains("happy") || input.Contains("great")) return "positive";
            return "neutral";
        }
    }
}