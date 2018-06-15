using System;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.Agent.ExampleAgents;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.Meta;
using SabberStoneCore.Model;
using System.Linq;

namespace SabberStoneCoreAi
{
	internal class Program
	{
		/// <summary>
		/// Decks to choose from:
		///		Decks.AggroPirateWarrior
		///		Decks.RenoKazakusMage
		///		Decks.MidrangeJadeShaman
		/// Rules:
		///		- an Agent class that inherist from Abstract Agent
		///		- supporting files of up to 1GB(please contact us in case you need to load a bigger database or something like an extremely large Neural Network)
		///		- the agent needs to finish the whole turn with a time limit of 75 seconds.Execution times for returned moves are removed from the time being used by the agent.In case the submitted agent exceeds this limit, it will lose the game.
		/// 
		/// </summary>
		/// <param name="args"></param>
		private static void Main(string[] args)
		{
			int numberOfGamesGames = 1000;
			int filesToWrite = 30;
			var Rnd = new Random();
			var watch = System.Diagnostics.Stopwatch.StartNew();
			int exceptioncounter = 0;
			int turnsPlayed = 0;
			for (int k = 0; k < filesToWrite; k++)
			{
				for (int i = 0; i < numberOfGamesGames; i++)
				{

					//Console.WriteLine("Setup gameConfig");
					try
					{
						GameConfig gameConfig = new GameConfig
						{
							StartPlayer = -1,
							Player1HeroClass = (CardClass)Rnd.Next(3, 11),
							Player2HeroClass = (CardClass)Rnd.Next(3, 11),
							//Player1Deck = Decks.MidrangeJadeShaman,
							//Player2Deck = Decks.MidrangeJadeShaman,
							FillDecks = true,
							Shuffle = true,
							FillDecksPredictably = true,
							Logging = false
						};

						AbstractAgent player1 = new RandomAgentWriteData(""); /*("trainingdata_chunk_"+k+".json");*/ gameConfig.Player1Name = "Player1";
						AbstractAgent player2 = new RandomAgentWriteData(""); gameConfig.Player2Name = "Player2";
						var gameHandler = new POGameHandler(gameConfig, player1, player2, debug: false);
						gameHandler.PlayGames(1, true);

						turnsPlayed += gameHandler.getGameStats().TurnsPlayed;
						
					}
					catch
					{
						exceptioncounter++;
						Console.Write("Ex:" + exceptioncounter + " ");
						i--;
					}
					if (i % 100 == 0)
					{
						Console.WriteLine("\t" + watch.Elapsed.TotalMinutes.ToString("F2") + "min \t\tCount:" + i);
					}
				}
				Console.WriteLine("Done  Turns:" + turnsPlayed);
			}
			Console.ReadLine();
		}
	}
}
