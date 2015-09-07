using System;
using System.Collections.Generic;
using Uno;

namespace WebClient.Controllers
{
    public static class Game
    {
        public static GameSession Session;
        private static List<string> _queue = new List<string>();


        public static void AddPlayer(String name)
        {
            _queue.Add(name);
            if (_queue.Count == 2)
            {
                Session = new GameSession(_queue.ToArray());
                Session.Deal();
            }
        }
    }
}