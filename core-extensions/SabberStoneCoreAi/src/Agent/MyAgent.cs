using System;
using System.Collections.Generic;
using SabberStoneCore.Enums;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Model.Zones;
using SabberStoneCore.Tasks;
using System.Linq;

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
			//MyScore score = new MyScore();
			return options[Rnd.Next(options.Count)];
		}
	}


	public class MyScore : Score.Score
	{
		//Lists of all the Minions and Weapons, maybe its gonna be useful
		public List<Minion> MyHandZoneMinions = new List<Minion>();
		public List<Weapon> MyHandZoneWeapons = new List<Weapon>();
		public List<Minion> MyBoardZoneMinions = new List<Minion>();
		public List<Minion> OppBoardZoneMinions = new List<Minion>();
		public List<Weapon> OppBoardZoneWeapons = new List<Weapon>();
		public bool MyHasWeapon;
		public bool OppHasWeapon;

		public MyScore()
		{
			foreach (Minion p in Hand)
			{
				MyHandZoneMinions.Add(p); //AttackDamage, Health, Cost
			}

			foreach (Weapon p in Hand)
			{
				MyHandZoneWeapons.Add(p); //AttackDamage, Durability, Cost
			}

			foreach (Minion p in BoardZone)
			{
				MyBoardZoneMinions.Add(p); //AttackDamage, Health, Cost
			}

			foreach (Minion p in OpBoardZone)
			{
				OppBoardZoneMinions.Add(p); //AttackDamage, Health, Cost
			}

			MyHasWeapon = Controller.Hero.Weapon != null ? true : false;
			OppHasWeapon = Controller.Opponent.Hero.Weapon != null ? true : false;
		}

	
		public override int Rate()
		{
			return 0;
		}
	}

}
