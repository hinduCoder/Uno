using System;
using Uno.Model;

namespace Uno
{
    class Program
    {
        static void PrintPlayerCards(Player player)
        {
            Console.WriteLine();
            var i = 1;
            foreach (var card in player.Cards)
            {
                Console.WriteLine("{2}: {0} {1}", card.Type == CardType.Number ? card.Number.ToString() : Enum.GetName(typeof(CardType), card.Type),
                card.Color == CardColor.Black ? "" : Enum.GetName(typeof(CardColor), card.Color), i++);
            }
            Console.WriteLine();
        }
        static void Main()
        {
            Console.WriteLine('⊘');
            var session = new GameSession("Anton", "Nastya");
            session.Deal();
            session.WildCardDiscarded += (o, e) =>
            {
                Console.Write("(R, Y, G, B): ");
                var enter = Console.ReadLine().ToUpper()[0];
                e.Color = enter == 'R' ? CardColor.Red : enter == 'Y' ? CardColor.Yellow : enter == 'G' ? CardColor.Green : CardColor.Blue;
            };
            session.GameFinished += (o, e) =>
            {
                Console.WriteLine("{0} WIN!!!");
            };
            while (true)
            {
                Console.WriteLine("============={0}============", session.CurrentPlayer.Name);
                var topCard = session.Game.DiscardPileTop;
                Console.WriteLine("Top: {0} {1}", topCard.Type == CardType.Number ? topCard.Number.ToString() : Enum.GetName(typeof(CardType), topCard.Type),
                    topCard.Color == CardColor.Black ? "" : Enum.GetName(typeof(CardColor), topCard.Color));
                PrintPlayerCards(session.CurrentPlayer);
                Console.Write("Number of card (0 to draw): ");
                session.Uno();
                var enter = int.Parse(Console.ReadLine());
                if (enter == 0)
                {
                    session.Draw();
                    PrintPlayerCards(session.CurrentPlayer);
                    Console.Write("Number of card (0 to draw): ");
                    enter = int.Parse(Console.ReadLine());
                    if (enter == 0)
                        session.Pass();
                    else
                        session.Discard(enter - 1);
                }
                else
                    session.Discard(enter - 1);
            }
        }

    }
}
