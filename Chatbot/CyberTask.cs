using System;
namespace Chatbot
{
    public class CyberTask
    {
        public string Description { get; set; } = string.Empty;
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public override string ToString() => $"{(IsCompleted ? "[Completed]" : "[Pending]")} {Description}{(ReminderDate.HasValue ? $" (Reminder: {ReminderDate.Value:yyyy-MM-dd})" : "")}";
    }
}
