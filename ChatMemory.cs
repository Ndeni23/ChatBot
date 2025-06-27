using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChatBot
{
    public class ChatMemory
    {
        private List<string> facts = new List<string>();

        public void StoreFact(string fact)
        {
            facts.Add(fact);
        }

        public string RecallFacts()
        {
            return facts.Count == 0 ? "I don't remember anything yet." : string.Join("; ", facts);
        }
    }
}