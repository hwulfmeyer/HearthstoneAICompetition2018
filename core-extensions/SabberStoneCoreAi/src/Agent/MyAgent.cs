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
			poGame.Simulate(options);
			return options[Rnd.Next(options.Count)];
		}
	}


	class NodeMCTS
	{
		public int N;   // Number of visits
		public int Q;   // Number of victories
		public NodeMCTS prt;	// parent
		public List<NodeMCTS> chdr; // children


		public NodeMCTS(NodeMCTS parent = null)
		{
			prt = parent;
			chdr = new List<NodeMCTS>();
		}


		public bool IsRoot
		{
			get { return prt.Equals(null); }
		}


		public bool IsLeaf
		{
			get { return chdr.Count == 0; }
		}
	}

}
