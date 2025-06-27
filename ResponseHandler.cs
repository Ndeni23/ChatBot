using System;
using System.Collections.Generic;
using System.Linq;
using ChatBot;

namespace chatbot
{
    public class ResponseHandler
    {
        private Dictionary<string, List<string>> keywordGroups = new Dictionary<string, List<string>>()
        {
            { "phishing", new List<string> { "phishing", "scam", "fraud", "spoof" } },
            { "passwords", new List<string> { "password", "credentials", "login" } },
            { "privacy", new List<string> { "privacy", "data protection", "personal info" } },
            { "browsing", new List<string> { "browser", "safe surfing", "website", "http" } },
            { "malware", new List<string> { "malware", "virus", "trojan", "spyware" } },
            { "firewall", new List<string> { "firewall", "network security", "port" } }
        };

        private Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>()
        {
            { "phishing", new List<string> {
                "Phishing attacks often come via email. Be cautious of unexpected messages.",
                "Always verify the sender's email address before clicking on links.",
                "If an email seems suspicious, don't click on any links or attachments."
            }},
            { "passwords", new List<string> {
                "Use strong, unique passwords for each of your accounts.",
                "Consider using a password manager to keep track of your credentials.",
                "Avoid using easily guessable information in your passwords."
            }},
            { "privacy", new List<string> {
                "Protecting your privacy online is crucial. Avoid sharing personal info on unsecured sites.",
                "Use privacy settings on your accounts to control who can see your data.",
                "Consider using a VPN to enhance your online privacy."
            }},
            { "browsing", new List<string> {
                "Ensure the website uses HTTPS before entering personal information.",
                "Be cautious when downloading files from unfamiliar websites.",
                "Keep your browser updated to protect against known vulnerabilities."
            }},
            { "malware", new List<string> {
                "Install reputable antivirus software and keep it updated.",
                "Avoid downloading software from untrusted sources.",
                "Regularly scan your system for malware."
            }},
            { "firewall", new List<string> {
                "A firewall helps protect your network from unauthorized access.",
                "Ensure your firewall is enabled and properly configured.",
                "Regularly check your firewall settings to maintain security."
            }},
            { "default", new List<string> {
                "I'm not sure I understand. Can you try rephrasing?",
                "Let's focus on cybersecurity topics. Can you rephrase your question?",
                "I can help with topics like phishing, passwords, and safe browsing."
            }}
        };

        private Dictionary<string, List<string>> usedResponses = new Dictionary<string, List<string>>();

        public delegate string ResponseSelector(List<string> responseList, List<string> usedList);
        private ResponseSelector selectResponse = (responseList, usedList) =>
        {
            var availableResponses = responseList.Where(r => !usedList.Contains(r)).ToList();
            if (!availableResponses.Any())
            {
                usedList.Clear();
                availableResponses = responseList.ToList();
            }
            var response = availableResponses[new Random().Next(availableResponses.Count)];
            usedList.Add(response);
            return response;
        };

        private string DetectTopic(string input, Conversation conversation)
        {
            input = input.ToLower().Trim();

            foreach (var group in keywordGroups)
            {
                foreach (var keyword in group.Value)
                {
                    if (input.Contains(keyword))
                    {
                        conversation.UpdateTopic(group.Key);
                        return group.Key;
                    }
                }
            }

            if (input.Contains("more") || input.Contains("continue") || input.Contains("what else") ||
                input.Contains("go on") || input.Contains("details") || input.Contains("keep going") ||
                input.Contains("tell me more"))
            {
                string currentTopic = conversation.GetTopic();
                if (currentTopic != "general" && responses.ContainsKey(currentTopic))
                {
                    return currentTopic;
                }
            }

            return "default";
        }

        public string GetResponse(string input, ChatMemory memory, string userName, Conversation conversation)
        {
            input = input.ToLower().Trim();
            string topic = DetectTopic(input, conversation);
            string sentinent = SentinentAnalyzer.AnalyzeSentinent(input);

            if (!usedResponses.ContainsKey(topic))
            {
                usedResponses[topic] = new List<string>();
            }

            var responseList = responses[topic];
            string response = selectResponse(responseList, usedResponses[topic]);

            if (sentinent == "worried")
            {
                response = $"I understand you're feeling {sentinent}, {userName}. Here's some advice: {response}";
            }
            else if (sentinent == "positive")
            {
                response = $"I'm glad you're feeling {sentinent}, {userName}! {response}";
            }

            string facts = memory.RecallFacts();
            if (!string.IsNullOrEmpty(facts) && facts != "I don't remember anything yet.")
            {
                foreach (var fact in facts.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (fact.ToLower().Contains("vpn"))
                    {
                        return $"Hey {userName}, you mentioned VPN earlier. Using a VPN can add an extra layer of protection!";
                    }
                    if (fact.ToLower().Contains("privacy") && topic != "privacy")
                    {
                        return $"Hey {userName}, I remember you're interested in privacy. Would you like to know more about protecting your data?";
                    }
                }
            }

            return response;
        }
    }
}