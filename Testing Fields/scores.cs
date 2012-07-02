using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Testing_Fields
{
    /// <summary>
    /// Container for the scores records
    /// </summary>
    public class scoreRecord
    {
        public string name;
        public int score;
        public scoreRecord(string Name, int Score)
        {
            name = Name;
            score = Score;
        }
    }

    /// <summary>
    /// Class that contains static methods to handle the high scores file
    /// </summary>
    public static class scores
    {
        /// <summary>
        /// Creates the scores file in the directory if the file doesn't exist
        /// </summary>
        public static void initializeHighScoresFile()
        {
            if (!Directory.Exists("stats"))
            {
                Directory.CreateDirectory("stats");
            }

            if (!File.Exists(@"stats\scores.hs"))
            {
                String[] scores={"Leafburn Scores", "---------------"};
                File.WriteAllLines(@"stats\scores.hs", scores);
            }
        }

        /// <summary>
        /// Append the high scores to the scores.hs file
        /// </summary>
        /// <param name="p1Name">Player 1's name</param>
        /// <param name="p2Name">Player 2's name</param>
        /// <param name="p1Score">Player 1's end game score</param>
        /// <param name="p2Score">Player 2's end game score</param>
        public static void recordScores(string p1Name, string p2Name, int p1Score, int p2Score)
        {
            int count = 0;
            StreamReader highScoreFile = new StreamReader(@"stats\scores.hs");
            string inputLine = "";
            LinkedList<scoreRecord> highscores = new LinkedList<scoreRecord>();
            while ((inputLine = highScoreFile.ReadLine()) != null)
            {
                if (count > 1)
                {
                    string[] tmp = inputLine.Split(';');
                    highscores.AddLast(new scoreRecord(tmp[0], int.Parse(tmp[1])));
                }
                count++;
            }
            highScoreFile.Close();
            if (highscores.ToArray().Length <= 500)
            {
                string[] record = { p1Name + ";" + p1Score.ToString(), p2Name + ";" + p2Score.ToString() };
                File.AppendAllLines(@"stats\scores.hs", record);
            }
            else
            {
                highscores = removeOldLowScores(highscores, new scoreRecord(p1Name, p1Score));
                highscores = removeOldLowScores(highscores, new scoreRecord(p2Name, p2Score));
                List<string> toFile = new List<string>();
                toFile.Add("Leafburn Scores");
                toFile.Add("---------------");
                foreach (scoreRecord i in highscores)
                {
                    toFile.Add(i.name + ";" + i.score);
                }
                File.WriteAllLines(@"stats\scores.hs", toFile);
            }
        }

        /// <summary>
        /// Checks if the new score is higher than the lowest high score. If so it replaces it.
        /// </summary>
        private static LinkedList<scoreRecord> removeOldLowScores(LinkedList<scoreRecord> highscores, scoreRecord newScore)
        {
            highscores = new LinkedList<scoreRecord>(highscores.OrderByDescending(x => x.score));
            if (newScore.score > highscores.Last.Value.score)
            {
                highscores.RemoveLast();
                highscores.AddLast(newScore);
            }
            return highscores;
        }
    }
}
