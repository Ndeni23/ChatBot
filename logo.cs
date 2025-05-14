using System;

namespace voice_greeting
{
    public class Logo
    {
        private readonly string _logoArt;

        public Logo(string logoArt)
        {
            _logoArt = logoArt;
        }

        public void Display()
        {
            Console.WriteLine(_logoArt);
        }
    }
}