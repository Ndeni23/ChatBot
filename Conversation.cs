using System.Collections.Generic;

namespace ChatBot
{
    public class Conversation
    {
        public string LastSentinent { get; set; } // Note: Keeping as "Sentinent" to match your naming convention

        private string currentTopic = "general";

        public void UpdateTopic(string newTopic)
        {
            if (!string.IsNullOrEmpty(newTopic))
                currentTopic = newTopic;
        }

        public string GetTopic()
        {
            return currentTopic;
        }
    }
}