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
			int numberOfGamesGames = 5;

			for(int i = 0; i< numberOfGamesGames; i++)
			{
				//Console.WriteLine("Setup gameConfig");

				GameConfig gameConfig = new GameConfig
				{
					StartPlayer = 1,
					Player1HeroClass = CardClass.MAGE,
					Player2HeroClass = CardClass.MAGE,
					//Player1Deck = Decks.MidrangeJadeShaman,
					//Player2Deck = Decks.MidrangeJadeShaman,
					FillDecks = true,
					Shuffle = true,
					Logging = false
				};

				//Console.WriteLine("Setup POGameHandler");
				AbstractAgent player1 = new RandomAgent(); gameConfig.Player1Name = "Player1";
				AbstractAgent player2 = new RandomAgent(); gameConfig.Player2Name = "Player2";
				var gameHandler = new POGameHandler(gameConfig, player1, player2, debug: false);
				//Console.WriteLine("PlayGame");
				var watch = System.Diagnostics.Stopwatch.StartNew();
				gameHandler.PlayGames(300);
				watch.Stop();
				GameStats gameStats = gameHandler.getGameStats();

				gameStats.printResults();

				//Console.WriteLine("Test successful: It took " + ((float)watch.Elapsed.TotalSeconds).ToString("F2") + " Seconds");
			}

			Console.ReadLine();
		}
	}
}
