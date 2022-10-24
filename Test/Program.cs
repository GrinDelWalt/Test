using System;
using System.CodeDom;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace Test
{

    static class Program 
    {
        static void Main(string[] args)
        {
            Game.task1();
        }
    }

    public struct Score
    {
        public int home;
        public int away;

        public Score(int home, int away)
        {
            this.home = home;
            this.away = away;
        }
    }

    public struct GameStamp
    {
        public int offset;
        public Score score;
        public GameStamp(int offset, int home, int away)
        {
            this.offset = offset;
            this.score = new Score(home, away);
        }
    }

    public class Game
    {
        const int TIMESTAMPS_COUNT = 50000;

        const double PROBABILITY_SCORE_CHANGED = 0.0001;

        const double PROBABILITY_HOME_SCORE = 0.45;

        const int OFFSET_MAX_STEP = 3;

        GameStamp[] gameStamps;
        Random rand;

        public Game()
        {
            this.gameStamps = new GameStamp[] { };
            rand = new Random();
        }

        public Game(GameStamp[] gameStamps)
        {
            this.gameStamps = gameStamps;
        }

        GameStamp generateGameStamp(GameStamp previousValue, int position)
        {


            bool scoreChanged = rand.NextDouble() > 1 - PROBABILITY_SCORE_CHANGED;
            int homeScoreChange = scoreChanged && rand.NextDouble() > 1 - PROBABILITY_HOME_SCORE ? 1 : 0;
            int awayScoreChange = scoreChanged && homeScoreChange == 0 ? 1 : 0;
            int offsetChange = (int)(Math.Floor(rand.NextDouble() * OFFSET_MAX_STEP)) + 1;


            return new GameStamp(
                previousValue.offset + offsetChange,
                previousValue.score.home + homeScoreChange,
                previousValue.score.away + awayScoreChange
                );
        }

        static Game generateGame()
        {
            Game game = new Game();
            game.gameStamps = new GameStamp[TIMESTAMPS_COUNT];

            GameStamp currentStamp = new GameStamp(0, 0, 0);
            for (int i = 0; i < TIMESTAMPS_COUNT; i++)
            {
                game.gameStamps[i] = currentStamp;
                currentStamp = game.generateGameStamp(currentStamp, i);
            }

            return game;
        }

        public static void task1()
        {
            Game game = generateGame();
            game.PrintBound();
            //game.printGameStamps();
            while (true)
            {
                int offset = game.ReturnOffset();
                game.PrintChange(game.getScore(offset));
            }
        }
        public void PrintBound()
        {
            Console.WriteLine($"диапозон выбора : {gameStamps[0].offset} - {gameStamps[TIMESTAMPS_COUNT - 1].offset}");
        }
        private int ReturnOffset()
        {

            do
            {
                try
                {
                    Console.Write("Введите offset ---->");
                    int offset = Convert.ToInt32(Console.ReadLine());
                    return offset;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"error {e.Message}");
                }
            } while (true);
        }

        void printGameStamps()
        {
            foreach (GameStamp stamp in this.gameStamps)
            {
                Console.WriteLine($"{stamp.offset}: {stamp.score.home}-{stamp.score.away}");
            }
        }


        public Score getScore(int offset)
        {
            Score score;
            int position;

            BinarySearch(out position, offset);

            return score = gameStamps[position].score;
        }
        private void BinarySearch(out int position, int offset, int upperBound = TIMESTAMPS_COUNT - 1, int lowerBound = 0)
        {
            int sum = upperBound + lowerBound;
            int middle = sum / 2;
            if (gameStamps[middle].offset == offset)
            {
                position = middle;
                return;
            }
            if (upperBound == lowerBound + 1)
            {
                if (gameStamps[upperBound].offset == offset)
                {
                    position = middle;
                    return;
                }
            }
            if (upperBound == lowerBound || upperBound == lowerBound + 1)
            {
                Console.WriteLine("Offset не найден");
                ReturnNearestValue(upperBound);
                position = upperBound;
                return;
            }
            if (gameStamps[middle].offset < offset)
            {
                BinarySearch(out position, offset, upperBound, middle);
            }
            else
            {
                BinarySearch(out position, offset, middle, lowerBound);
            }
        }
        private void ReturnNearestValue(int position)
        {
            Console.WriteLine($"Ближащий Offset : {gameStamps[position].offset}");
        }
        private void PrintChange(Score score)
        {
            Console.WriteLine($"Счет : {score.home} - {score.away}");
        }
    }



}

