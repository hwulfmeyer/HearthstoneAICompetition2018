using SabberStoneCore.Enums;
using SabberStoneCore.Tasks;
using System;
using System.Collections.Generic;

namespace SabberStoneCoreAi.Agent
{
	class MyAgent : AbstractAgent
	{
		private Random Rnd;

		public override void InitializeGame()
		{
		}

		public override void InitializeAgent()
		{
			Rnd = new Random();
		}

		public override void FinalizeAgent()
		{
		}

		public override void FinalizeGame()
		{
		}

		public override void FinalizeGame(PlayState playState)
		{
			Console.WriteLine("MyAgent: " + playState.ToString());
		}

		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{
			List<PlayerTask> options = poGame.CurrentPlayer.Options();
			return options[Rnd.Next(options.Count)];
		}
	}

}
