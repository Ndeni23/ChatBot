Features
This application successfully integrates and demonstrates all required features from Parts 1, 2, and 3 of the POE.
Part 3: Cybersecurity Chatbot (Core Application)
Task Assistant: Users can add cybersecurity-related tasks (e.g., "add task to update my antivirus").
Reminders: Tasks can be created with simple, natural language reminders (e.g., "...tomorrow" or "...in 5 days").
Cybersecurity Quiz: A 10-question quiz with multiple-choice and true/false questions tests the user's knowledge on topics like phishing, passwords, and safe browsing. The chatbot provides immediate feedback and explanations for each answer.
Natural Language Processing (NLP) Simulation: The chatbot uses keyword matching and intent recognition (UserIntent enum) to understand a variety of user commands, even when phrased differently.
Humanizing Features: The chatbot can handle simple conversational greetings and questions (e.g., "how are you?", "thanks"), making the interaction feel more natural.
Activity Log: All significant actions (tasks added, quizzes started, facts remembered) are logged with timestamps and can be viewed via the "Activity Log" tab or a chat command.
Chat Memory: The bot can remember and recall facts provided by the user (e.g., "remember that...", "what do you remember?").
Professional GUI: The application is built with WPF/XAML and features:
A modern dark theme.
A dedicated login screen to personalize the experience.
A multi-tabbed interface for organized navigation.
Custom-styled chat bubbles for clear conversation flow.
Graphics Rendering & Sound:
The application plays a welcome sound on startup.
It dynamically generates ASCII art from an image file and displays it on the login screen.
