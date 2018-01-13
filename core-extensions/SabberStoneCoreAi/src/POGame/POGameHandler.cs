﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.src.Agent;
using SabberStoneCoreAi.src.Agent.ExampleAgents;
using SabberStoneCoreAi.src.POGame;

namespace SabberStoneCoreAi.POGame
{
	class POGameHandler
	{
		private bool debug;

		private AbstractAgent player1;
		private AbstractAgent player2;

		private GameConfig gameConfig;
		private bool setupHeroes = true;

		private GameStats gameStats;
		private static readonly Random Rnd = new Random();


		public POGameHandler(GameConfig gameConfig, bool setupHeroes = true, bool debug=false)
		{
			this.gameConfig = gameConfig;
			this.setupHeroes = setupHeroes;
			player1 = new RandomAgent();
			player2 = new RandomAgent();
			gameStats = new GameStats();
			this.debug = debug;
		}

		public POGameHandler(GameConfig gameConfig, AbstractAgent player1, AbstractAgent player2, bool setupHeroes = true, bool debug =false)
		{
			this.gameConfig = gameConfig;
			this.setupHeroes = setupHeroes;
			this.player1 = player1;
			this.player2 = player2;
			gameStats = new GameStats();
			this.debug = debug;
		}


		public void PlayGame(bool addToGameStats=true)
		{
			Game game = new Game(gameConfig, setupHeroes);
			AbstractAgent currentAgent;
			Stopwatch currentStopwatch;
			PartialObservationGame poGame;
			Stopwatch[] watches = new[] {new Stopwatch(), new Stopwatch()};
			

			game.StartGame();
			while (game.State != State.COMPLETE)
			{
				if (debug)
					Console.WriteLine("Turn " + game.Turn);
				
				currentAgent = game.CurrentPlayer == game.Player1 ? player1 : player2;
				Controller currentPlayer = game.CurrentPlayer;
				currentStopwatch = game.CurrentPlayer == game.Player1 ? watches[0] : watches[1];
				poGame = new PartialObservationGame(game);

				currentStopwatch.Start();
				List<PlayerTask> playertasks = currentAgent.GetMove(poGame);
				currentStopwatch.Stop();

				game.CurrentPlayer.Game = game;
				game.CurrentOpponent.Game = game;

				while (playertasks.Count > 0 && game.CurrentPlayer == currentPlayer)
				{
					if (debug)
						Console.WriteLine(playertasks[0]);
					game.Process(playertasks[0]);
					playertasks.RemoveAt(0);
				}
			}

			if (addToGameStats)
				gameStats.addGame(game, watches);
		}

		public void PlayGames(int nr_of_games, bool addToGameStats=true)
		{
			for (int i = 0; i < 0; i++)
			{
				PlayGame(addToGameStats);
			}
		}

		public GameStats getGameStats()
		{
			return gameStats;
		}
	}

}