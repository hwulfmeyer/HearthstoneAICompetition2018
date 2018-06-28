# Decks: #
 * Decks.AggroPirateWarrior
 * Decks.RenoKazakusMage
 * Decks.MidrangeJadeShaman
 * 1 Unknown
 
# Boardstate
## Stats to consider
### From https://knowledgepit.fedcsis.org/mod/page/view.php?id=1022

* player:
	* stats:
		* "crystals_all":int
		* "crystals_current":int
		* "deck_count":int
		* "fatigue_damage":int
		* "hand_count":int
		* "played_minions_count":int
		* "spell_dmg_bonus":int
	* hero:
		* "armor":int
		* "attack":int
		* "hero_card_id":int
		* "hp":int
		* "special_skill_used":int
		* "weapon_durability":int
	* played cards:
		* "attack":int
		* "can_attack":-1,1
		* "charge":-1,1
		* "crystals_cost":int
		* "forgetful":-1,1
		* "freezing":-1,1
		* "frozen":-1,1
		* "hp":int
		* "hp_current":int
		* "poisonous":-1,1
		* "shield":-1,1
		* "stealth":-1,1
		* "taunt":-1,1
		* "windfury":-1,1
	* handcards:
		* "attack":int
		* "charge":-1,1
		* "crystals_cost":int
		* "forgetful":-1,1
		* "freezing":-1,1
		* "hp":int
		* "poisonous":-1,1
		* "name":string
		* "shield":-1,1
		* "stealth":-1,1
		* "taunt":-1,1
		* "type":0,1,2 ("MINION", "SPELL", "WEAPON")
		* "windfury":-1,1
* opponent:
	* played cards:
		* ...
	* hero:
		* ...
	* stats:
		* ...
		
		
## Methods for the Prediction Model
### Outline:
* high dimensional data (~40 dimensions/Attributes)
* large data set (~1.000.000 instances)
### Goal:
* broadly: Classification
* specific: Likelihood of winning when in boardstate x

### Methods:
* (Deep) Convolutional Neural Network
* Information: https://annals-csis.org/Volume_11/drp/pdf/573.pdf

## Method for Data Mining
# Random Agents
* let 2 Random agents play against each other
* random heroclass
* random deck
* shuffle deck in the beginning
