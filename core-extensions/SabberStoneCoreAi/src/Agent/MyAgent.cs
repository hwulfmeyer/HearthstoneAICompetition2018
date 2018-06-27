using Newtonsoft.Json;
using SabberStoneCore.Enums;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Model.Zones;
using SabberStoneCore.Tasks;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using SabberStoneCore.Model;

namespace SabberStoneCoreAi.Agent
{
	class MyAgent : AbstractAgent
	{
		private string filename = "";
		private Random Rnd = new Random();
		private List<MyScoreJsonFormat> ListMyScoreJsonFormat = new List<MyScoreJsonFormat>();
		private Queue<PlayerTask> ListPlayerTasksToDo;

		public MyAgent(string fname)
		{
			filename = fname;
		}

		public override void InitializeGame()
		{
		}


		public override void InitializeAgent()
		{
			Rnd = new Random();
			ListMyScoreJsonFormat = new List<MyScoreJsonFormat>();
			ListPlayerTasksToDo = new Queue<PlayerTask>();
		}


		public override void FinalizeAgent()
		{
		}


		public override void FinalizeGame()
		{
			ListPlayerTasksToDo = new Queue<PlayerTask>();
		}


		public override void FinalizeGame(PlayState playState)
		{
			ListPlayerTasksToDo = new Queue<PlayerTask>();
			Console.WriteLine(playState);
			WriteToFile(playState);
		}


		public override PlayerTask GetMove(POGame.POGame poGame)
		{
			//return GetMoveRandom(poGame);
			return GetMoveSearchTree(poGame, 1);
		}


		public PlayerTask GetMoveRandom(POGame.POGame poGame)
		{
			List<PlayerTask> options = poGame.CurrentPlayer.Options();
			var state = new MyScoreJsonFormat(poGame);
			ListMyScoreJsonFormat.Add(state);
			return options[Rnd.Next(options.Count)];
		}


		public PlayerTask GetMoveSearchTree(POGame.POGame poGame, int depth)
		{
			if (ListPlayerTasksToDo.Count == 0)
			{
				var root = new NodeGameState(poGame);
				root.IDDFS(depth);
				ListPlayerTasksToDo = root.GetPlayerTasks();
			}
			return ListPlayerTasksToDo.Dequeue();
		}



		public void WriteToFile(PlayState playState)
		{
			if (filename != "")
			{
				foreach (MyScoreJsonFormat x in ListMyScoreJsonFormat)
				{
					x.player_won = (int)playState == 4 ? 1 : (int)playState == 5 ? 0 : -1;
				}


				foreach (MyScoreJsonFormat item in ListMyScoreJsonFormat)
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


	class NodeGameState
	{
		public NodeGameState prt;	// parent
		public List<NodeGameState> chdr; // children
		public POGame.POGame game; //gamestate of the note
		public PlayerTask task;
		public bool isEnemyNode;
		public bool WasExpanded;
		public float value;

		public NodeGameState(POGame.POGame poGame, PlayerTask task = null,  NodeGameState parent = null)
		{
			chdr = new List<NodeGameState>();
			prt = parent;
			game = poGame;
			this.task = task;
			WasExpanded = false;

			if (parent == null) isEnemyNode = false; // root is always MyPlayer
			else if(game.CurrentPlayer.PlayerId != parent.game.CurrentPlayer.PlayerId)
			{
				isEnemyNode = !parent.isEnemyNode;
			}
			else
			{
				isEnemyNode = parent.isEnemyNode;
			}
		}


		public bool IsRoot
		{
			get { return prt.Equals(null); }
		}


		public bool IsLeaf
		{
			get { return chdr.Count == 0; }
		}


		/// <summary>
		/// Iterative deepening depth-first (recursive depth-limited DFS)
		/// </summary>
		public void IDDFS(int maxDepth)
		{
			for (int i=0;i< maxDepth; i++)
			{
				DLS();
			}
		}


		private void DLS()
		{
			if (!WasExpanded)
			{
				//fill HandZone of enemy if it is enemys turn
				if (isEnemyNode)
				{
					int cardsToAdd = game.CurrentPlayer.HandZone.Count;
					//Added while loop because apparently the foreach only removes every 2nd card? :/
					while (game.CurrentPlayer.HandZone.Count > 0)
					{
						foreach (IPlayable x in game.CurrentPlayer.HandZone)
						{
							game.CurrentPlayer.HandZone.Remove(x);
						}
					}
					IEnumerable<Card> cards = game.CurrentPlayer.DeckZone.Controller.Standard;

					while (!game.CurrentPlayer.HandZone.IsFull && cardsToAdd > 0)
					{
						Card card = Util.Choose<Card>(cards.ToList());

						// don't add cards that have already reached max occurence in hand + graveyard + boardzone of current Player
						if (game.CurrentPlayer.HandZone.Count(c => c.Card == card) +
							game.CurrentPlayer.GraveyardZone.Count(c => c.Card == card) +
							game.CurrentPlayer.BoardZone.Count(c => c.Card == card) >= card.MaxAllowedInDeck)
							continue;

						IPlayable entity = Entity.FromCard(game.CurrentPlayer.DeckZone.Controller, card);
						game.CurrentPlayer.HandZone.Add(entity);

						cardsToAdd--;
					}
				}

				Expand();
			}
			else
			{
				foreach (NodeGameState child in chdr)
				{
					child.DLS();
				}
			}

		}


		/// <summary>
		/// MinMax Search
		/// </summary>
		/// <returns></returns>
		internal Queue<PlayerTask> GetPlayerTasks()
		{
			MiniMax();
			Queue<PlayerTask> que = GetPlayerTasksMinimax();
			return que;
		}

		private Queue<PlayerTask> GetPlayerTasksMinimax()
		{
			var que = new Queue<PlayerTask>();
			if(isEnemyNode != prt.isEnemyNode)
			{
				return que;
			}

			que.Enqueue(task);
			if (IsLeaf)
			{
				return que;
			}
			else
			{
				IOrderedEnumerable<NodeGameState> orderedChildren = chdr.OrderByDescending(p => p.value);
				//Min if true, Max if false
				NodeGameState nextNode = isEnemyNode ? orderedChildren.Last() : orderedChildren.First();
				que.Concat(nextNode.GetPlayerTasksMinimax());
			}

			return que;
		}


		/// <summary>
		/// Minimax algorithm
		/// </summary>
		private float MiniMax()
		{
			if (IsLeaf)
			{
				value = SelectionPolicy();
			}
			else
			{
				if (isEnemyNode)
				{
					//Minimize
					value = chdr.Min(chdr => MiniMax());
				}
				else
				{
					//Maximize
					value = chdr.Max(chdr => MiniMax());
				}
			}
			return value;
		}

		private float SelectionPolicy()
		{
			var score = new Score.ControlScore
			{
				Controller = isEnemyNode? game.CurrentOpponent : game.CurrentPlayer
			};
			return score.Rate();
		}

		private void Expand()
		{
			WasExpanded = true;
			Dictionary<PlayerTask, POGame.POGame> dict = game.Simulate(game.CurrentPlayer.Options());

			foreach (KeyValuePair<PlayerTask, POGame.POGame> item in dict)
			{
				//poGame is null if exception happens
				if(item.Value != null)
				{
					chdr.Add(new NodeGameState(item.Value, item.Key, this));
				}
			}
		}
	}


	class GameStateRepresentation
	{
		public Dictionary<string, int> player = new Dictionary<string, int>();
		public List<Dictionary<string, int>> player_boardcards = new List<Dictionary<string, int>>();
		public List<List<Dictionary<string, int>>> player_handcards = new List<List<Dictionary<string, int>>>();

		public Dictionary<string, int> opponent = new Dictionary<string, int>();
		public List<Dictionary<string, int>> opponent_boardcards = new List<Dictionary<string, int>>();

		public int player_won;


		public GameStateRepresentation(POGame.POGame poGame)
		{
			player = new Dictionary<string, int>();
			player.Add("BaseMana", poGame.CurrentPlayer.BaseMana);
			player.Add("HeroArmor", poGame.CurrentPlayer.Hero.Armor);
			player.Add("HeroAttackDamage", poGame.CurrentPlayer.Hero.AttackDamage);
			player.Add("HeroIsFrozen", poGame.CurrentPlayer.Hero.IsFrozen ? 1 : -1);
			player.Add("HeroIsImmune", poGame.CurrentPlayer.Hero.IsImmune ? 1 : -1);
			player.Add("HeroHealth", poGame.CurrentPlayer.Hero.Health);
			//more info on weapon?

			opponent = new Dictionary<string, int>();
			opponent.Add("BaseMana", poGame.CurrentOpponent.BaseMana);
			opponent.Add("HeroArmor", poGame.CurrentOpponent.Hero.Armor);
			opponent.Add("HeroAttackDamage", poGame.CurrentOpponent.Hero.AttackDamage);
			opponent.Add("HeroIsFrozen", poGame.CurrentOpponent.Hero.IsFrozen ? 1 : -1);
			opponent.Add("HeroIsImmune", poGame.CurrentOpponent.Hero.IsImmune ? 1 : -1);
			opponent.Add("HeroHealth", poGame.CurrentOpponent.Hero.Health);

			player_boardcards = new List<Dictionary<string, int>>();
			foreach (Minion x in poGame.CurrentPlayer.BoardZone)
			{
				var curcard = new Dictionary<string, int>
				{
					{ "AttackDamage", x.AttackDamage },
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "Health", x.Health},
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "ZonePosition", x.ZonePosition }
				};
				player_boardcards.Add(curcard);
			}

			opponent_boardcards = new List<Dictionary<string, int>>();
			foreach (Minion x in poGame.CurrentOpponent.BoardZone)
			{
				var curcard = new Dictionary<string, int>
				{
					{ "AttackDamage", x.AttackDamage },
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "Health", x.Health},
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "ZonePosition", x.ZonePosition }
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
					{ "AttackDamage", x.AttackDamage },
					{ "Durability", x.Durability},
					{ "Damage", x.Damage}

				};
				handcards_minions.Add(curcard);
			}
			player_handcards.Add(handcards_weapons);

		}

		public void SetPlayerWon(PlayState playState)
		{
			player_won = ((int)playState) == 4 ? 1 : ((int)playState) == 5 ? 0 : -1;
		}
	}


	class MyScoreJsonFormat
	{
		public Dictionary<string, int> player = new Dictionary<string, int>();
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
					{ "AttackDamage", x.AttackDamage },
					{ "Health", x.Health},
					{ "BaseHealth", x.BaseHealth},
					{ "Cost", x.Cost},
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "ZonePosition", x.ZonePosition }
				};
				player_boardcards.Add(curcard);
			}

			opponent_boardcards = new List<Dictionary<string, int>>();
			foreach (Minion x in poGame.CurrentOpponent.BoardZone)
			{
				var curcard = new Dictionary<string, int>
				{
					{ "AttackDamage", x.AttackDamage },
					{ "Health", x.Health},
					{ "BaseHealth", x.BaseHealth},								
					{ "Cost", x.Cost},
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "ZonePosition", x.ZonePosition }
				};
				opponent_boardcards.Add(curcard);
			}

			player_handcards = new List<List<Dictionary<string, int>>>();

			var handcards_minions = new List<Dictionary<string, int>>();
			foreach (Minion x in poGame.CurrentPlayer.HandZone.OfType<Minion>())
			{
				var curcard = new Dictionary<string, int>
				{
					{ "AttackDamage", x.AttackDamage },
					{ "BaseHealth", x.BaseHealth},
					{ "Cost", x.Cost},
					{ "HasCharge", x.HasCharge ? 1 : -1 },
					{ "HasTaunt", x.HasTaunt ? 1 : -1 },
					{ "HasBattleCry", x.HasBattleCry ? 1 : -1 },
					{ "HasDeathrattle", x.HasDeathrattle ? 1 : -1 },
					{ "ZonePosition", x.ZonePosition }
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

		public void SetPlayerWon(PlayState playState)
		{
			player_won = ((int)playState) == 4 ? 1 : ((int)playState) == 5 ? 0 : -1;
		}
	}


}
