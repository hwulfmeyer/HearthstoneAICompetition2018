# Decks: #
 * Decks.AggroPirateWarrior
 * Decks.RenoKazakusMage
 * Decks.MidrangeJadeShaman
 * 1 Unknown
 
# Boardstate
## Stats to consider
### From https://knowledgepit.fedcsis.org/mod/page/view.php?id=1022
For Both:
 * armor
 * attack
 * heroclass
 * hp
 * special_skill_used
 * weapon_durability
 * crystals_all
 * crystals_current 
 * deck_count
 * nOfRemainingCardsInDeck
 * hand_count
 * fatigue_damage
 * played_minions_count
 * played.nOfCards
 * played.attack
 * played.crystals_cost
 * played.hp_current
 * played.hp_max
 
For Player only:
 * hand.nOfMinions
 * hand.nOfSpells
 * hand.nOfWeapons
 * hand.nOfCards
 * hand.nOfPlayable
 * hand.attack
 * hand.crystals_cost
 * hand.hp
	
	## Methods for the Prediction Model
	### Outline:
	* high dimensional data (~40 dimensions/Attributes)
	* large data set (~1.000.000 instances)
	### Goal:
	* broadly: Classification
	* specific: Likelihood of winning when in boardstate x
	
	### Methods:
	* Convolutional Neural Network
	* Random Forests
	* Ensemble Methods (Train each Method with a random sample of 100.000 instances)
	* Information: https://annals-csis.org/Volume_11/drp/pdf/573.pdf
