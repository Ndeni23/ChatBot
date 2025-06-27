using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chatbot
{
    public partial class MainWindow : Window
    {
        // --- Data Collections ---
        private readonly ObservableCollection<string> _chatHistory = new();
        private readonly ObservableCollection<CyberTask> _tasks = new();
        private readonly ObservableCollection<string> _activityLogForList = new();

        // --- State variables ---
        private string _userName = "You";

        // --- Quiz State ---
        private List<QuizQuestion> _quizQuestions = new();
        private bool _isQuizActive = false;
        private int _currentQuestionIndex = 0;
        private int _score = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ChatHistoryListBox.ItemsSource = _chatHistory;
            TasksListView.ItemsSource = _tasks;
            ActivityLogListBox.ItemsSource = _activityLogForList;
            InitializeQuiz();
        }

        // --- This method handles the login and switches to the tabbed view ---
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter your name to begin.", "Name Required");
                return;
            }

            _userName = name; // Store the user's name

            // Switch the views
            LoginView.Visibility = Visibility.Collapsed;
            MainChatView.Visibility = Visibility.Visible;
            MainTabControl.SelectedIndex = 0; // Start on the Chat tab

            // Post the welcome message to the chat
            AddBotMessage($"Welcome, {_userName}! I'm your Cybersecurity Bot. Ask for 'help' to see what I can do.");
            LogActivity($"User '{_userName}' logged in.");
        }

        #region Core UI and Input Handling
        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessUserInput();
        private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) ProcessUserInput(); }

        private void ProcessUserInput()
        {
            string userInput = UserInputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;
            AddUserMessage(userInput);
            UserInputTextBox.Clear();
            if (_isQuizActive) HandleQuizAnswer(userInput); else ProcessNlp(userInput);
            ChatScrollViewer!.ScrollToBottom();
        }
        #endregion

        #region NLP Simulation and Intent Recognition
        private void ProcessNlp(string userInput)
        {
            var intent = RecognizeIntent(userInput);
            switch (intent)
            {
                case UserIntent.AddTask: HandleAddTask(userInput); break;
                case UserIntent.StartQuiz: StartQuiz(); break;
                case UserIntent.ShowTasks: ShowTasks(); break;
                case UserIntent.ShowActivityLog: ShowActivityLog(); break;
                case UserIntent.Help:
                    AddBotMessage("I can help! Try:\n'start quiz', 'add task to...', 'show my tasks', 'show activity log'.");
                    break;
                default:
                    string lowerInput = userInput.ToLower();
                    if (lowerInput.Contains("how are you")) { AddBotMessage("I'm doing great, thanks for asking!"); }
                    else { AddBotMessage("I'm not sure how to help with that. Try asking for 'help'."); }
                    break;
            }
        }

        private UserIntent RecognizeIntent(string userInput)
        {
            string lowerInput = userInput.ToLower();
            if (lowerInput.Contains("remind me to") || (lowerInput.Contains("add") && lowerInput.Contains("task"))) return UserIntent.AddTask;
            if (lowerInput.Contains("quiz") || lowerInput.Contains("test my knowledge")) return UserIntent.StartQuiz;
            if ((lowerInput.Contains("show") || lowerInput.Contains("list")) && lowerInput.Contains("task")) return UserIntent.ShowTasks;
            if (lowerInput.Contains("log") || lowerInput.Contains("what have you done")) return UserIntent.ShowActivityLog;
            if (lowerInput.Contains("help") || lowerInput.Contains("what can you do")) return UserIntent.Help;
            return UserIntent.Unknown;
        }
        #endregion

        #region Task Assistant Logic (Task 1)
        private void HandleAddTask(string userInput)
        {
            DateTime? reminderDate = ParseReminderDate(userInput);
            string taskDescription = userInput.ToLower().Replace("add task to", "").Replace("remind me to", "").Split(new[] { " on ", " in ", " tomorrow" }, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
            if (string.IsNullOrWhiteSpace(taskDescription)) { AddBotMessage("Please provide a description for the task."); return; }
            var newTask = new CyberTask { Description = taskDescription, IsCompleted = false, ReminderDate = reminderDate };
            _tasks.Add(newTask);
            string confirmation = $"Task added: '{taskDescription}'.";
            if (reminderDate.HasValue) confirmation += $" I'll remind you on {reminderDate.Value:MMMM dd}.";
            AddBotMessage(confirmation);
            LogActivity($"Task added: '{taskDescription}'");
        }

        private DateTime? ParseReminderDate(string userInput)
        {
            string lowerInput = userInput.ToLower();
            if (lowerInput.Contains("tomorrow")) return DateTime.Now.AddDays(1);
            if (lowerInput.Contains(" in ") && lowerInput.Contains(" day"))
            {
                var parts = lowerInput.Split(new[] { " in ", " day" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts) { if (int.TryParse(part.Trim(), out int days)) return DateTime.Now.AddDays(days); }
            }
            return null;
        }

        private void ShowTasks()
        {
            if (!_tasks.Any()) { AddBotMessage("You have no pending tasks."); return; }
            string taskList = "Here are your current tasks:\n" + string.Join("\n", _tasks.Select((t, i) => $"• {t.Description} {(t.ReminderDate.HasValue ? $"(Reminder: {t.ReminderDate:d})" : "")}"));
            AddBotMessage(taskList);
            MainTabControl.SelectedIndex = 1; // Switch to the Tasks tab
        }
        #endregion

        #region Quiz Game Logic (Task 2)
        private void InitializeQuiz()
        {
            _quizQuestions = new List<QuizQuestion>
            {
                new() { QuestionText = "What is 'phishing'?", Options = new() { "A) A type of virus", "B) A fraudulent attempt to obtain sensitive info", "C) A way to speed up your internet" }, CorrectAnswerIndex = 1, Explanation = "Correct! Phishing uses deceptive emails to trick you." },
                new() { QuestionText = "Which is the strongest password?", Options = new() { "A) 12345678", "B) password", "C) MyP@ssw0rd!2024" }, CorrectAnswerIndex = 2, Explanation = "Correct! A strong password mixes cases, numbers, and symbols." },
                new() { QuestionText = "What does '2FA' stand for?", Options = new() { "A) Two-Factor Authentication", "B) Two-Fast-Algorithms" }, CorrectAnswerIndex = 0, Explanation = "Correct! 2FA adds a second layer of security." },
                new() { QuestionText = "True/False: Public Wi-Fi is safe for banking.", Options = new() { "A) True", "B) False" }, CorrectAnswerIndex = 1, Explanation = "Correct! Public Wi-Fi is often unsecured and risky." },
                new() { QuestionText = "If you get an unexpected email attachment, you should:", Options = new() { "A) Open it immediately", "B) Forward it to friends", "C) Delete it without opening" }, CorrectAnswerIndex = 2, Explanation = "Correct! Unexpected attachments can contain malware." },
                new() { QuestionText = "What does a firewall do?", Options = new() { "A) Cleans viruses", "B) Acts as a barrier for your network", "C) Makes your PC faster" }, CorrectAnswerIndex = 1, Explanation = "Correct! A firewall monitors network traffic to protect you." },
                new() { QuestionText = "What is 'social engineering'?", Options = new() { "A) Designing websites", "B) A computer part", "C) Manipulating people to give up info" }, CorrectAnswerIndex = 2, Explanation = "Correct! It relies on tricking humans, not hacking systems." },
                new() { QuestionText = "A secure website URL starts with 'https://' and shows a:", Options = new() { "A) Green light", "B) Padlock icon", "C) Happy face" }, CorrectAnswerIndex = 1, Explanation = "Correct! The padlock and 'https://' indicate an encrypted connection." },
                new() { QuestionText = "A common sign of a phishing email is:", Options = new() { "A) Perfect grammar", "B) A sense of urgency and suspicious links", "C) It is from your boss" }, CorrectAnswerIndex = 1, Explanation = "Correct! Phishing emails often try to scare you into acting quickly." },
                new() { QuestionText = "The best way to protect your important files is to:", Options = new() { "A) Keep them on your desktop", "B) Regularly back them up", "C) Print them all" }, CorrectAnswerIndex = 1, Explanation = "Correct! Regular backups protect against data loss." }
            };
        }
        private void StartQuiz()
        {
            MainTabControl.SelectedIndex = 0; // Ensure user is on the chat tab
            _isQuizActive = true; _currentQuestionIndex = 0; _score = 0;
            LogActivity("Quiz started.");
            AddBotMessage("Quiz time! I'll ask 10 questions. Reply with the letter of your choice (e.g., 'A').");
            AskNextQuestion();
        }
        private void AskNextQuestion()
        {
            if (_currentQuestionIndex < _quizQuestions.Count)
            {
                QuizQuestion q = _quizQuestions[_currentQuestionIndex];
                string questionText = $"{_currentQuestionIndex + 1}. {q.QuestionText}\n" + string.Join("\n", q.Options);
                AddBotMessage(questionText);
            }
            else EndQuiz();
        }
        private void HandleQuizAnswer(string userAnswer)
        {
            if (string.IsNullOrEmpty(userAnswer)) { AddBotMessage("Please provide an answer."); return; }
            char answerChar = userAnswer.Trim().ToUpper()[0];
            int selectedOptionIndex = answerChar - 'A';
            QuizQuestion q = _quizQuestions[_currentQuestionIndex];
            if (selectedOptionIndex >= 0 && selectedOptionIndex < q.Options.Count)
            {
                if (selectedOptionIndex == q.CorrectAnswerIndex) { AddBotMessage($"Correct! {q.Explanation}"); _score++; }
                else AddBotMessage($"Incorrect. The answer was {q.Options[q.CorrectAnswerIndex][0]}. {q.Explanation}");
                _currentQuestionIndex++; AskNextQuestion();
            }
            else AddBotMessage("Invalid answer. Please enter a valid letter (A, B, C, etc.).");
        }
        private void EndQuiz()
        {
            _isQuizActive = false;
            string feedback = _score switch { >= 8 => "Great job! You're a cybersecurity pro!", >= 5 => "Good effort! You know the basics.", _ => "Keep learning!" };
            AddBotMessage($"Quiz finished! Your final score is {_score}/{_quizQuestions.Count}. {feedback}");
            LogActivity($"Quiz completed with score {_score}/{_quizQuestions.Count}.");
        }
        #endregion

        #region Activity Log (Task 4) and Helper Methods
        private void LogActivity(string action) => _activityLogForList.Add($"{DateTime.Now:g} - {action}");

        private void ShowActivityLog()
        {
            if (!_activityLogForList.Any()) { AddBotMessage("No activity has been logged yet."); return; }
            string logText = "Here's a summary of recent actions:\n" + string.Join("\n", _activityLogForList.AsEnumerable().Reverse().Take(10));
            AddBotMessage(logText);
            MainTabControl.SelectedIndex = 2; // Switch to the Activity Log tab
        }
        private void AddBotMessage(string message)
        {
            _chatHistory.Add($"Bot: {message}");
            ApplyStylesToLastMessage();
        }
        private void AddUserMessage(string message)
        {
            _chatHistory.Add($"{_userName}: {message}");
            ApplyStylesToLastMessage();
        }
        private void ApplyStylesToLastMessage()
        {
            if (!ChatHistoryListBox.Items.IsEmpty)
            {
                var item = ChatHistoryListBox.Items[^1];
                ChatHistoryListBox.UpdateLayout();
                var listBoxItem = ChatHistoryListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (listBoxItem != null)
                {
                    string content = item.ToString()!;
                    if (content.StartsWith("You:") || content.StartsWith(_userName + ":"))
                    {
                        listBoxItem.Style = (Style)FindResource("UserMessageStyle");
                        listBoxItem.Content = content[(content.IndexOf(':') + 2)..];
                    }
                    else
                    {
                        listBoxItem.Style = (Style)FindResource("BotMessageStyle");
                        listBoxItem.Content = content["Bot: ".Length..];
                    }
                }
            }
        }
        #endregion
    }
}