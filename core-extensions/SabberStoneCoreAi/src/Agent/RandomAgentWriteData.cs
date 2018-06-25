using Newtonsoft.Json;
using SabberStoneCore.Enums;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Model.Zones;
using SabberStoneCore.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SabberStoneCoreAi.Agent
{
	class RandomAgentWriteData : AbstractAgent
	{
		private string filename = "";
		private Random Rnd = new Random();
		private List<MyScoreJsonFormat> Statelist = new List<MyScoreJsonFormat>();

		public RandomAgentWriteData(string fname)
		{
			filename = fname;
		}

		public override void InitializeAgent()
		{
			Rnd = new Random();
			Statelist = new List<MyScoreJsonFormat>();
		}

		public override void FinalizeAgent()
		{
			Console.WriteLine("Test");
		}

		public override void FinalizeGame()
		{
		}

		public override PlayerTask GetMove(POGame.POGame poGame)
		{
			List<PlayerTask> options = poGame.CurrentPlayer.Options();
			var state = new MyScoreJsonFormat(poGame);
			Statelist.Add(state);
			return options[Rnd.Next(options.Count)];
		}



		public override void InitializeGame()
		{
			//Nothing to do here
		}

		//is called at the end of a game
		public override void FinalizeGame(PlayState playState)
		{
			if (filename != "")
			{
				foreach (MyScoreJsonFormat x in Statelist)
				{
					x.player_won = (int)playState == 4?1: (int)playState == 5?0:-1;
				}


				foreach (MyScoreJsonFormat item in Statelist)
				{
					using (FileStream fs = File.Open(@"C:\ttt\hai\" + filename, FileMode.Append))
					using (var sw = new StreamWriter(fs))
					using (JsonWriter jw = new JsonTextWriter(sw))
					{
						jw.Formatting = Formatting.None;
						var serializer = new JsonSerializer();
						serializer.Serialize(jw, item);
						sw.Write("\n");
						jw.Close();
						sw.Close();
						fs.Close();
					}
				}

			}
		}
	}

	class MyScoreJsonFormat
	{
		public Dictionary<string,int> player = new Dictionary<string, int>();
		public List<Dictionary<string, int>> player_boardcards = new List<Dictionary<string, int>>();
		public List<List<Dictionary<string, int>>> player_handcards = new List<List<Dictionary<string, int>>>();

		public Dictionary<string, int> opponent = new Dictionary<string, int>();
		public List<Dictionary<string, int>> opponent_boardcards = new List<Dictionary<string, int>>();

		public int player_won;


		public MyScoreJsonFormat(POGame.POGame poGame)
		{
			player = new Dictionary<string, int>();
			player.Add("HeroClass", (int)poGame.CurrentPlayer.HeroClass);
			player.Add("BaseMana", poGame.CurrentPlayer.BaseMana);
			player.Add("DragonInHand", poGame.CurrentPlayer.DragonInHand ? 1 : -1);
			player.Add("NumCardsPlayedThisTurn", poGame.CurrentPlayer.NumCardsPlayedThisTurn);
			player.Add("NumFriendlyMinionsThatAttackedThisTurn", poGame.CurrentPlayer.NumFriendlyMinionsThatAttackedThisTurn);
			player.Add("NumFriendlyMinionsThatDiedThisTurn", poGame.CurrentPlayer.NumFriendlyMinionsThatDiedThisTurn);
			player.Add("NumMinionsPlayedThisTurn", poGame.CurrentPlayer.NumMinionsPlayedThisTurn);
			player.Add("NumMinionsPlayerKilledThisTurn", poGame.CurrentPlayer.NumMinionsPlayerKilledThisTurn);
			player.Add("NumTotemSummonedThisGame", poGame.CurrentPlayer.NumTotemSummonedThisGame);
			player.Add("HandZoneCount", poGame.CurrentPlayer.HandZone.Count);
			player.Add("BoardZoneCount", poGame.CurrentPlayer.BoardZone.Count);
			player.Add("DeckZoneCount", poGame.CurrentPlayer.DeckZone.Count);
			player.Add("GraveyardZoneCount", poGame.CurrentPlayer.GraveyardZone.Count);
			player.Add("HeroArmor", poGame.CurrentPlayer.Hero.Armor);
			player.Add("HeroAttackDamage", poGame.CurrentPlayer.Hero.AttackDamage);
			player.Add("HeroCantBeTargetedByHeroPowers", poGame.CurrentPlayer.Hero.CantBeTargetedByHeroPowers ? 1 : -1);
			player.Add("HeroCantBeTargetedBySpells", poGame.CurrentPlayer.Hero.CantBeTargetedBySpells ? 1 : -1);
			player.Add("HeroHealth", poGame.CurrentPlayer.Hero.Health);
			player.Add("HeroIsFrozen", poGame.CurrentPlayer.Hero.IsFrozen ? 1 : -1);
			player.Add("HeroIsImmune", poGame.CurrentPlayer.Hero.IsImmune ? 1 : -1);
			player.Add("HeroNumAttacksThisTurn", poGame.CurrentPlayer.Hero.NumAttacksThisTurn);
			player.Add("HeroTotalAttackDamage", poGame.CurrentPlayer.Hero.TotalAttackDamage);

			opponent = new Dictionary<string, int>();
			opponent.Add("HeroClass", (int)poGame.CurrentOpponent.HeroClass);
			opponent.Add("BaseMana", poGame.CurrentOpponent.BaseMana);
			opponent.Add("DragonInHand", poGame.CurrentOpponent.DragonInHand ? 1 : -1);
			opponent.Add("NumCardsPlayedThisTurn", poGame.CurrentOpponent.NumCardsPlayedThisTurn);
			opponent.Add("NumFriendlyMinionsThatAttackedThisTurn", poGame.CurrentOpponent.NumFriendlyMinionsThatAttackedThisTurn);
			opponent.Add("NumFriendlyMinionsThatDiedThisTurn", poGame.CurrentOpponent.NumFriendlyMinionsThatDiedThisTurn);
			opponent.Add("NumMinionsPlayedThisTurn", poGame.CurrentOpponent.NumMinionsPlayedThisTurn);
			opponent.Add("NumMinionsPlayerKilledThisTurn", poGame.CurrentOpponent.NumMinionsPlayerKilledThisTurn);
			opponent.Add("NumTotemSummonedThisGame", poGame.CurrentOpponent.NumTotemSummonedThisGame);
			opponent.Add("HandZoneCount", poGame.CurrentOpponent.HandZone.Count);
			opponent.Add("BoardZoneCount", poGame.CurrentOpponent.BoardZone.Count);
			opponent.Add("DeckZoneCount", poGame.CurrentOpponent.DeckZone.Count);
			opponent.Add("GraveyardZoneCount", poGame.CurrentOpponent.GraveyardZone.Count);
			opponent.Add("HeroArmor", poGame.CurrentOpponent.Hero.Armor);
			opponent.Add("HeroAttackDamage", poGame.CurrentOpponent.Hero.AttackDamage);
			opponent.Add("HeroCantBeTargetedByHeroPowers", poGame.CurrentOpponent.Hero.CantBeTargetedByHeroPowers ? 1 : -1);
			opponent.Add("HeroCantBeTargetedBySpells", poGame.CurrentOpponent.Hero.CantBeTargetedBySpells ? 1 : -1);
			opponent.Add("HeroHealth", poGame.CurrentOpponent.Hero.Health);
			opponent.Add("HeroIsFrozen", poGame.CurrentOpponent.Hero.IsFrozen ? 1 : -1);
			opponent.Add("HeroIsImmune", poGame.CurrentOpponent.Hero.IsImmune ? 1 : -1);
			opponent.Add("HeroNumAttacksThisTurn", poGame.CurrentOpponent.Hero.NumAttacksThisTurn);
			opponent.Add("HeroTotalAttackDamage", poGame.CurrentOpponent.Hero.TotalAttackDamage);

			player_boardcards = new List<Dictionary<string, int>>();
			foreach (Minion x in poGame.CurrentPlayer.BoardZone)
			{
				var curcard = new Dictionary<string, int>
				{
					{ "Armor", x.Armor },
					{ "AttackDamage", x.AttackDamage },
					{ "BaseHealth", x.BaseHealth},
					{ "Cost", x.Cost},
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "Health", x.Health},
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 }
				};
				player_boardcards.Add(curcard);
			}

			opponent_boardcards = new List<Dictionary<string, int>>();
			foreach (Minion x in poGame.CurrentOpponent.BoardZone)
			{
				var curcard = new Dictionary<string, int>
				{
					{ "Armor", x.Armor },
					{ "AttackDamage", x.AttackDamage },
					{ "BaseHealth", x.BaseHealth},
					{ "Cost", x.Cost},
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "Health", x.Health},
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 }
				};
				opponent_boardcards.Add(curcard);
			}

			player_handcards = new List<List<Dictionary<string, int>>>();

			var handcards_minions = new List<Dictionary<string, int>>();
			foreach (Minion x in poGame.CurrentPlayer.HandZone.OfType<Minion>())
			{
				var curcard = new Dictionary<string, int>
				{
					{ "Armor", x.Armor },
					{ "AttackDamage", x.AttackDamage },
					{ "BaseHealth", x.BaseHealth},
					{ "Cost", x.Cost},
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "Health", x.Health},
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 }
				};
				handcards_minions.Add(curcard);
			}
			player_handcards.Add(handcards_minions);


			var handcards_spells = new List<Dictionary<string, int>>();
			foreach (Spell x in poGame.CurrentPlayer.HandZone.OfType<Spell>())
			{
				var curcard = new Dictionary<string, int>
				{
					{ "Id", x.Id}
				};
				handcards_minions.Add(curcard);
			}
			player_handcards.Add(handcards_spells);

			var handcards_weapons = new List<Dictionary<string, int>>();
			foreach (Weapon x in poGame.CurrentPlayer.HandZone.OfType<Weapon>())
			{
				var curcard = new Dictionary<string, int>
				{
					{ "Cost", x.Cost},
					{ "AttackDamage", x.AttackDamage },
					{ "Durability", x.Durability},
					{ "Damage", x.Damage}

				};
				handcards_minions.Add(curcard);
			}
			player_handcards.Add(handcards_weapons);

		}

		void SetPlayerWon(PlayState playState)
		{
			player_won = ((int)playState) == 4 ? 1 : ((int)playState) == 5 ? 0 : -1;
		}
	}

}
