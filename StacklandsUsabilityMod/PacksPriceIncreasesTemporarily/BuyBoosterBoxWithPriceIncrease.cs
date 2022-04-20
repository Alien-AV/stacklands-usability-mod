using System.Collections;
using TMPro;
using UnityEngine;

namespace StacklandsUsabilityMod.PacksPriceIncreasesTemporarily
{
    class BuyBoosterBoxWithPriceIncrease : CardTarget
    {
		public Boosterpack Booster
		{
			get
			{
				return WorldManager.instance.GetBoosterPrefab(this.BoosterId);
			}
		}

		public override bool CanHaveCard(GameCard card)
		{
			return this.Booster.IsUnlocked && !WorldManager.instance.RemovingCards && (this.BoughtWithGold(card) || this.BoughtWithChest(card));
		}

		private bool BoughtWithChest(GameCard card)
		{
			Chest chest = card.CardData as Chest;
			return chest != null && chest.CoinCount >= this.AdjustedCost;
		}

		private bool BoughtWithGold(GameCard card)
		{
			return card.CardData.Id == "gold" && WorldManager.instance.StackAllSame(card) && card.GetAllCardsInStack().Count >= this.AdjustedCost;
		}

		public override void CardDropped(GameCard card)
		{
			if (this.BoughtWithGold(card))
			{
				for (int i = 0; i < this.AdjustedCost; i++)
				{
					card.GetLeafCard().DestroyCard(false, true);
				}
			}
			else if (this.BoughtWithChest(card))
			{
				(card.CardData as Chest).CoinCount -= this.AdjustedCost;
			}
			this.BuyPack();
			base.CardDropped(card);
		}

		protected override void Update()
		{
			if (this.Booster.IsUnlocked)
			{
				base.gameObject.name = this.Booster.Name;
				this.BuyText.text = string.Format("{0}{1}", this.AdjustedCost, Icons.Gold);
				this.NameText.text = this.Booster.Name + " Extended";
			}
			else
			{
				base.gameObject.name = "???";
				this.NameText.text = "???";
				this.BuyText.text = "";
			}
			base.Update();
		}

		public override string GetTooltipText()
		{
			if (this.Booster.IsUnlocked)
			{
				return string.Format("Drag {0}{1} here to buy the {2} Pack.\n\n{3}", new object[]
				{
				this.AdjustedCost,
				Icons.Gold,
				this.Booster.Name,
				this.Booster.GetSummary()
				});
			}
			return string.Format("Complete {0} more quests to unlock this Pack", this.Booster.RemainingAchievementCountToUnlock);
		}

		private void BuyPack()
		{
			WorldManager.instance.CreateSmoke(base.transform.position);
			if (this.BoosterId == "basic")
			{
				AchievementManager.instance.SpecialActionComplete("buy_basic_pack", null);
			}
			WorldManager.instance.BoughtBoosterIds.Add(this.BoosterId);
			WorldManager.instance.CreateBoosterpack(base.transform.position, this.BoosterId).Velocity = new Vector3?(new Vector3(0f, 8f, -this.PushDir.Value.z * 4.5f));
			this.CostPenalty += 0.5f;
			base.StartCoroutine(this.LowerCostPenalty());
		}


		private IEnumerator LowerCostPenalty()
		{
			float CurrentTimerTime = 0f;
			yield return new WaitUntil(delegate ()
			{
				CurrentTimerTime += Time.deltaTime * WorldManager.instance.TimeScale;
				return CurrentTimerTime >= 5f;
			});
			this.CostPenalty -= 0.5f;
			yield break;
		}



		private int AdjustedCost
		{
			get
			{
				return (int)((float)this.Cost + this.CostPenalty);
			}
		}

		public int Cost;
		public string BoosterId;
		public Transform SpawnTarget;
		public TextMeshPro BuyText;
		public TextMeshPro NameText;
		public float CostPenalty;

	}
}