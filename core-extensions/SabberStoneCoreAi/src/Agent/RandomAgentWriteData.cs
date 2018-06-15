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


				foreach (MyScoreJsonFormat x in Statelist)
				{
					using (FileStream fs = File.Open(@"C:\ttt\hai\" + filename, FileMode.Append))
					using (var sw = new StreamWriter(fs))
					using (JsonWriter jw = new JsonTextWriter(sw))
					{
						jw.Formatting = Formatting.None;
						var serializer = new JsonSerializer();
						serializer.Serialize(jw, x);
					}
				}
				string json = JsonConvert.SerializeObject(Statelist[5], Formatting.Indented);
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
			player.Add("CurrentSpellPower", poGame.CurrentPlayer.CurrentSpellPower);
			player.Add("DragonInHand", poGame.CurrentPlayer.DragonInHand ? 1 : -1);
			player.Add("HeroPowerDisabled", poGame.CurrentPlayer.HeroPowerDisabled ? 1 : -1);
			player.Add("MaxResources", poGame.CurrentPlayer.MaxResources);
			player.Add("NumAttacksThisTurn", poGame.CurrentPlayer.NumAttacksThisTurn);
			player.Add("NumCardsDrawnThisTurn", poGame.CurrentPlayer.NumCardsDrawnThisTurn);
			player.Add("NumCardsPlayedThisTurn", poGame.CurrentPlayer.NumCardsPlayedThisTurn);
			player.Add("NumElementalsPlayedLastTurn", poGame.CurrentPlayer.NumElementalsPlayedLastTurn);
			player.Add("NumElementalsPlayedThisTurn", poGame.CurrentPlayer.NumElementalsPlayedThisTurn);
			player.Add("NumFriendlyMinionsThatAttackedThisTurn", poGame.CurrentPlayer.NumFriendlyMinionsThatAttackedThisTurn);
			player.Add("NumFriendlyMinionsThatDiedThisGame", poGame.CurrentPlayer.NumFriendlyMinionsThatDiedThisGame);
			player.Add("NumFriendlyMinionsThatDiedThisTurn", poGame.CurrentPlayer.NumFriendlyMinionsThatDiedThisTurn);
			player.Add("NumMinionsPlayedThisTurn", poGame.CurrentPlayer.NumMinionsPlayedThisTurn);
			player.Add("NumMinionsPlayerKilledThisTurn", poGame.CurrentPlayer.NumMinionsPlayerKilledThisTurn);
			player.Add("NumMurlocsPlayedThisGame", poGame.CurrentPlayer.NumMurlocsPlayedThisGame);
			player.Add("NumOptionsPlayedThisTurn", poGame.CurrentPlayer.NumOptionsPlayedThisTurn);
			player.Add("NumSecretsPlayedThisGame", poGame.CurrentPlayer.NumSecretsPlayedThisGame);
			player.Add("NumSpellsPlayedThisGame", poGame.CurrentPlayer.NumSpellsPlayedThisGame);
			player.Add("NumTimesHeroPowerUsedThisGame", poGame.CurrentPlayer.NumTimesHeroPowerUsedThisGame);
			player.Add("NumTotemSummonedThisGame", poGame.CurrentPlayer.NumTotemSummonedThisGame);
			player.Add("NumTurnsLeft", poGame.CurrentPlayer.NumTurnsLeft);
			player.Add("RemainingMana", poGame.CurrentPlayer.RemainingMana);
			player.Add("UsedMana", poGame.CurrentPlayer.UsedMana);
			player.Add("HandZoneCount", poGame.CurrentPlayer.HandZone.Count);
			player.Add("BoardZoneCount", poGame.CurrentPlayer.BoardZone.Count);
			player.Add("DeckZoneCount", poGame.CurrentPlayer.DeckZone.Count);
			player.Add("GraveyardZoneCount", poGame.CurrentPlayer.GraveyardZone.Count);
			player.Add("SecretZoneCount", poGame.CurrentPlayer.SecretZone.Count);
			player.Add("HeroArmor", poGame.CurrentPlayer.Hero.Armor);
			player.Add("HeroAttackDamage", poGame.CurrentPlayer.Hero.AttackDamage);
			player.Add("HeroAurasCount", poGame.CurrentPlayer.Hero.Auras.Count);
			player.Add("HeroCanAttack", poGame.CurrentPlayer.Hero.CanAttack ? 1 : -1);
			player.Add("HeroCantBeTargetedByHeroPowers", poGame.CurrentPlayer.Hero.CantBeTargetedByHeroPowers ? 1 : -1);
			player.Add("HeroCantBeTargetedByOpponents", poGame.CurrentPlayer.Hero.CantBeTargetedByOpponents ? 1 : -1);
			player.Add("HeroCantBeTargetedBySpells", poGame.CurrentPlayer.Hero.CantBeTargetedBySpells ? 1 : -1);
			player.Add("HeroHasDeathrattle", poGame.CurrentPlayer.Hero.HasDeathrattle ? 1 : -1);
			player.Add("HeroHasLifeSteal", poGame.CurrentPlayer.Hero.HasLifeSteal ? 1 : -1);
			player.Add("HeroHasStealth", poGame.CurrentPlayer.Hero.HasStealth ? 1 : -1);
			player.Add("HeroHasTaunt", poGame.CurrentPlayer.Hero.HasTaunt ? 1 : -1);
			player.Add("HeroHasWindfury", poGame.CurrentPlayer.Hero.HasWindfury ? 1 : -1);
			player.Add("HeroHealth", poGame.CurrentPlayer.Hero.Health);
			player.Add("HeroHeroPowerDamage", poGame.CurrentPlayer.Hero.HeroPowerDamage);
			player.Add("HeroIsFrozen", poGame.CurrentPlayer.Hero.IsFrozen ? 1 : -1);
			player.Add("HeroIsImmune", poGame.CurrentPlayer.Hero.IsImmune ? 1 : -1);
			player.Add("HeroIsSilenced", poGame.CurrentPlayer.Hero.IsSilenced ? 1 : -1);
			player.Add("HeroNumAttacksThisTurn", poGame.CurrentPlayer.Hero.NumAttacksThisTurn);
			player.Add("HeroTotalAttackDamage", poGame.CurrentPlayer.Hero.TotalAttackDamage);

			opponent = new Dictionary<string, int>();
			opponent.Add("HeroClass", (int)poGame.CurrentOpponent.HeroClass);
			opponent.Add("BaseMana", poGame.CurrentOpponent.BaseMana);
			opponent.Add("CurrentSpellPower", poGame.CurrentOpponent.CurrentSpellPower);
			opponent.Add("DragonInHand", poGame.CurrentOpponent.DragonInHand ? 1 : -1);
			opponent.Add("HeroPowerDisabled", poGame.CurrentOpponent.HeroPowerDisabled ? 1 : -1);
			opponent.Add("MaxResources", poGame.CurrentOpponent.MaxResources);
			opponent.Add("NumAttacksThisTurn", poGame.CurrentOpponent.NumAttacksThisTurn);
			opponent.Add("NumCardsDrawnThisTurn", poGame.CurrentOpponent.NumCardsDrawnThisTurn);
			opponent.Add("NumCardsPlayedThisTurn", poGame.CurrentOpponent.NumCardsPlayedThisTurn);
			opponent.Add("NumElementalsPlayedLastTurn", poGame.CurrentOpponent.NumElementalsPlayedLastTurn);
			opponent.Add("NumElementalsPlayedThisTurn", poGame.CurrentOpponent.NumElementalsPlayedThisTurn);
			opponent.Add("NumFriendlyMinionsThatAttackedThisTurn", poGame.CurrentOpponent.NumFriendlyMinionsThatAttackedThisTurn);
			opponent.Add("NumFriendlyMinionsThatDiedThisGame", poGame.CurrentOpponent.NumFriendlyMinionsThatDiedThisGame);
			opponent.Add("NumFriendlyMinionsThatDiedThisTurn", poGame.CurrentOpponent.NumFriendlyMinionsThatDiedThisTurn);
			opponent.Add("NumMinionsPlayedThisTurn", poGame.CurrentOpponent.NumMinionsPlayedThisTurn);
			opponent.Add("NumMinionsopponentKilledThisTurn", poGame.CurrentOpponent.NumMinionsPlayerKilledThisTurn);
			opponent.Add("NumMurlocsPlayedThisGame", poGame.CurrentOpponent.NumMurlocsPlayedThisGame);
			opponent.Add("NumOptionsPlayedThisTurn", poGame.CurrentOpponent.NumOptionsPlayedThisTurn);
			opponent.Add("NumSecretsPlayedThisGame", poGame.CurrentOpponent.NumSecretsPlayedThisGame);
			opponent.Add("NumSpellsPlayedThisGame", poGame.CurrentOpponent.NumSpellsPlayedThisGame);
			opponent.Add("NumTimesHeroPowerUsedThisGame", poGame.CurrentOpponent.NumTimesHeroPowerUsedThisGame);
			opponent.Add("NumTotemSummonedThisGame", poGame.CurrentOpponent.NumTotemSummonedThisGame);
			opponent.Add("NumTurnsLeft", poGame.CurrentOpponent.NumTurnsLeft);
			opponent.Add("RemainingMana", poGame.CurrentOpponent.RemainingMana);
			opponent.Add("UsedMana", poGame.CurrentOpponent.UsedMana);
			opponent.Add("HandZoneCount", poGame.CurrentOpponent.HandZone.Count);
			opponent.Add("BoardZoneCount", poGame.CurrentOpponent.BoardZone.Count);
			opponent.Add("DeckZoneCount", poGame.CurrentOpponent.DeckZone.Count);
			opponent.Add("GraveyardZoneCount", poGame.CurrentOpponent.GraveyardZone.Count);
			opponent.Add("SecretZoneCount", poGame.CurrentOpponent.SecretZone.Count);
			opponent.Add("HeroArmor", poGame.CurrentOpponent.Hero.Armor);
			opponent.Add("HeroAttackDamage", poGame.CurrentOpponent.Hero.AttackDamage);
			opponent.Add("HeroAurasCount", poGame.CurrentOpponent.Hero.Auras.Count);
			opponent.Add("HeroCanAttack", poGame.CurrentOpponent.Hero.CanAttack ? 1 : -1);
			opponent.Add("HeroCantBeTargetedByHeroPowers", poGame.CurrentOpponent.Hero.CantBeTargetedByHeroPowers ? 1 : -1);
			opponent.Add("HeroCantBeTargetedByOpponents", poGame.CurrentOpponent.Hero.CantBeTargetedByOpponents ? 1 : -1);
			opponent.Add("HeroCantBeTargetedBySpells", poGame.CurrentOpponent.Hero.CantBeTargetedBySpells ? 1 : -1);
			opponent.Add("HeroHasDeathrattle", poGame.CurrentOpponent.Hero.HasDeathrattle ? 1 : -1);
			opponent.Add("HeroHasLifeSteal", poGame.CurrentOpponent.Hero.HasLifeSteal ? 1 : -1);
			opponent.Add("HeroHasStealth", poGame.CurrentOpponent.Hero.HasStealth ? 1 : -1);
			opponent.Add("HeroHasTaunt", poGame.CurrentOpponent.Hero.HasTaunt ? 1 : -1);
			opponent.Add("HeroHasWindfury", poGame.CurrentOpponent.Hero.HasWindfury ? 1 : -1);
			opponent.Add("HeroHealth", poGame.CurrentOpponent.Hero.Health);
			opponent.Add("HeroHeroPowerDamage", poGame.CurrentOpponent.Hero.HeroPowerDamage);
			opponent.Add("HeroIsFrozen", poGame.CurrentOpponent.Hero.IsFrozen ? 1 : -1);
			opponent.Add("HeroIsImmune", poGame.CurrentOpponent.Hero.IsImmune ? 1 : -1);
			opponent.Add("HeroIsSilenced", poGame.CurrentOpponent.Hero.IsSilenced ? 1 : -1);
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
					{ "CanAttack", x.CanAttack ? 1 : -1 },
					{ "CantAttackHeroes", x.CantAttackHeroes ? 1 : -1 },
					{ "CantBeTargetedByHeroPowers", x.CantBeTargetedByHeroPowers ? 1 : -1 },
					{ "CantBeTargetedByOpponents", x.CantBeTargetedByOpponents ? 1 : -1 },
					{ "CantBeTargetedBySpells", x.CantBeTargetedBySpells ? 1 : -1 },
					{ "Cost", x.Cost},
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "HasDivineShield", x.HasDivineShield ? 1 : -1 },
					{ "HasLifeSteal", x.HasLifeSteal ? 1 : -1 },
					{ "HasStealth", x.HasStealth ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasWindfury", x.HasWindfury ? 1 : -1 },
					{ "Health", x.Health},
					{ "IsEnraged", x.IsEnraged ? 1 : -1 },
					{ "IsFrozen", x.IsFrozen ? 1 : -1 },
					{ "IsImmune", x.IsImmune ? 1 : -1 },
					{ "IsSilenced", x.IsSilenced ? 1 : -1 },
					{ "JustPlayed", x.JustPlayed ? 1 : -1 },
					{ "LastBoardPosition", x.LastBoardPosition},
					{ "Poisonous", x.Poisonous ? 1 : -1 },
					{ "ZonePosition", x.ZonePosition}
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
					{ "CanAttack", x.CanAttack ? 1 : -1 },
					{ "CantAttackHeroes", x.CantAttackHeroes ? 1 : -1 },
					{ "CantBeTargetedByHeroPowers", x.CantBeTargetedByHeroPowers ? 1 : -1 },
					{ "CantBeTargetedByOpponents", x.CantBeTargetedByOpponents ? 1 : -1 },
					{ "CantBeTargetedBySpells", x.CantBeTargetedBySpells ? 1 : -1 },
					{ "Cost", x.Cost},
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "HasDivineShield", x.HasDivineShield ? 1 : -1 },
					{ "HasLifeSteal", x.HasLifeSteal ? 1 : -1 },
					{ "HasStealth", x.HasStealth ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasWindfury", x.HasWindfury ? 1 : -1 },
					{ "Health", x.Health},
					{ "IsEnraged", x.IsEnraged ? 1 : -1 },
					{ "IsFrozen", x.IsFrozen ? 1 : -1 },
					{ "IsImmune", x.IsImmune ? 1 : -1 },
					{ "IsSilenced", x.IsSilenced ? 1 : -1 },
					{ "JustPlayed", x.JustPlayed ? 1 : -1 },
					{ "LastBoardPosition", x.LastBoardPosition},
					{ "Poisonous", x.Poisonous ? 1 : -1 },
					{ "ZonePosition", x.ZonePosition}
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
					{ "CanAttack", x.CanAttack ? 1 : -1 },
					{ "Cost", x.Cost},
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "HasLifeSteal", x.HasLifeSteal ? 1 : -1 },
					{ "HasStealth", x.HasStealth ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasWindfury", x.HasWindfury ? 1 : -1 },
					{ "JustPlayed", x.JustPlayed ? 1 : -1 },
					{ "Poisonous", x.Poisonous ? 1 : -1 },
				};
				handcards_minions.Add(curcard);
			}
			player_handcards.Add(handcards_minions);


			var handcards_spells = new List<Dictionary<string, int>>();
			foreach (Spell x in poGame.CurrentPlayer.HandZone.OfType<Spell>())
			{
				var curcard = new Dictionary<string, int>
				{
					{ "Cost", x.Cost},
					{ "Id", x.Id},
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
