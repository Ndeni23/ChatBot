using System;
using System.IO;
using System.Media;

namespace chatbot
{
    public class welcome_message
    {
        private readonly string[] errorMessages = {
            "Warning: Greeting audio file not found. Continuing without audio.",
            "Error playing voice greeting"
        };

        public void Play()
        {
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "welcome.wav");

            try
            {
                if (File.Exists(audioPath))
                {
                    using (var player = new SoundPlayer(audioPath))
                    {
                        player.Play();
                    }
                }
                else
                {
                    Console.WriteLine(errorMessages[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{errorMessages[1]}: {ex.Message}");
            }
        }
    }
}