using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NoHotbarPickup
{
	internal class NoHotbarPickupGlobal : GlobalItem
	{

		private static readonly short[] ammos = {
			ItemID.Gel, // Gel
			ItemID.WoodenArrow, // Arrow
			ItemID.CopperCoin, // Coin
			ItemID.FallenStar, // FallenStar
			ItemID.MusketBall, // Bullet
			ItemID.SandBlock, // Sand
			ItemID.Seed, // Dart
			ItemID.RocketI, // Rocket
			ItemID.GreenSolution, // Solution
			ItemID.Flare, // Flare
			ItemID.Snowball, // Snowball
			ItemID.StyngerBolt, // StyngerBolt
			ItemID.CandyCorn, // CandyCorn
			ItemID.ExplosiveJackOLantern, // JackOLantern
			ItemID.Stake, // Stake
			ItemID.Nail // NailFriendly
		};

		public override bool OnPickup(Item item, Player player) {
			switch (item.type) {
				// Coins
				case ItemID.CopperCoin:
				case ItemID.SilverCoin:
				case ItemID.GoldCoin:
				case ItemID.PlatinumCoin:
				// Hearts
				case ItemID.Heart:
				case ItemID.CandyApple:
				case ItemID.CandyCane:
				// Mana stars
				case ItemID.Star:
				case ItemID.SoulCake:
				case ItemID.SugarPlum:
				// Nebula buffs
				case ItemID.NebulaPickup1:
				case ItemID.NebulaPickup2:
				case ItemID.NebulaPickup3:
					return true;
			}

			bool hotbarWhenFull = ModContent.GetInstance<NoHotbarPickupConfig>().hotbarWhenFull;
			bool pickupDirection = ModContent.GetInstance<NoHotbarPickupConfig>().pickupDirection;
			// player.inventory [59]
			// 0-9 hotbar
			// 10-49 are actual inventory
			// 50-53 coins
			// 54-57 ammo
			// 58 mouse ???

			bool canPickUp(int from, int to, int emptySlot) {
				for (int i = from; i < to; i++) {
					// check inventory and hotbar for item
					Item invItem = player.inventory[i];
					if (invItem.type == item.type) {
						// item already in inventory
						if (invItem.stack == invItem.maxStack)
							continue;

						if (invItem.maxStack == 1) {
							// item can't stack
							if (emptySlot != -1) {
								player.inventory[emptySlot] = item;
								ItemText.NewText(item, item.stack);
							}
						}
						else if (invItem.stack + item.stack > invItem.maxStack) {
							// item can stack but stack + existing.stack > maxStack
							//	add what can be added to existing
							int origStack = item.stack;
							item.stack -= invItem.maxStack - invItem.stack;
							invItem.stack = invItem.maxStack;
							//	add new item using remainder (if there's a spot for it)
							if (emptySlot != -1) {
								player.inventory[emptySlot] = item;
								ItemText.NewText(item, origStack);
							}
							else {
								player.QuickSpawnClonedItem(item, item.stack);
								ItemText.NewText(item, origStack - item.stack);
							}
						}
						else {
							// item can stack
							invItem.stack += item.stack;
							ItemText.NewText(item, item.stack);
						}
						// item has been picked up
						return true;
					}
				}
				// item not in inventory
				return false;
			}

			if (ammos.Any(type => type == item.ammo)) {
				int firstEmptyAmmoSlot = -1;

				for (int i = 54; i < 58; i++)
					if (player.inventory[i].type == ItemID.None) {
						firstEmptyAmmoSlot = i;
						break;
					}

				if (canPickUp(54, 58, firstEmptyAmmoSlot))
					return false;
			}

			// find open slot based on config option `pickupDirection`
			int start = pickupDirection ? 49 : 10;
			int end = pickupDirection ? 10 : 49;
			int step = pickupDirection ? -1 : 1;
			int firstEmptySlot = -1;

			for (int i = start; i != end; i += step)
				if (player.inventory[i].type == ItemID.None) {
					firstEmptySlot = i;
					break;
				}

			if (firstEmptySlot == -1 && hotbarWhenFull)
				for (int i = 0; i < 10; i++)
					if (player.inventory[i].type == ItemID.None) {
						firstEmptySlot = i;
						break;
					}

			if (canPickUp(0, 50, firstEmptySlot))
				return false;

			if (firstEmptySlot != -1) {
				player.inventory[firstEmptySlot] = item;
				ItemText.NewText(item, item.stack);
			}

			return false;
		}

		public override bool CanPickup(Item item, Player player) {
			switch (item.type) {
				// Coins
				case ItemID.CopperCoin:
				case ItemID.SilverCoin:
				case ItemID.GoldCoin:
				case ItemID.PlatinumCoin:
				// Hearts
				case ItemID.Heart:
				case ItemID.CandyApple:
				case ItemID.CandyCane:
				// Mana stars
				case ItemID.Star:
				case ItemID.SoulCake:
				case ItemID.SugarPlum:
				// Nebula buffs
				case ItemID.NebulaPickup1:
				case ItemID.NebulaPickup2:
				case ItemID.NebulaPickup3:
					return true;
			}

			bool hotbarWhenFull = ModContent.GetInstance<NoHotbarPickupConfig>().hotbarWhenFull;
			bool pickupDirection = ModContent.GetInstance<NoHotbarPickupConfig>().pickupDirection;
			// player.inventory [59]
			// 0-9 hotbar
			// 10-49 are actual inventory
			// 50-53 coins
			// 54-57 ammo
			// 58 mouse ???

			bool canPickUp(int from, int to) {
				for (int i = from; i < to; i++) {
					// check inventory and hotbar for item
					ref Item invItem = ref player.inventory[i];
					if (invItem.type == item.type) {
						// item already in inventory
						if (invItem.stack == invItem.maxStack)
							continue;

						return true;
					}
				}
				return false;
			}

			if (ammos.Any(type => type == item.ammo) && canPickUp(54, 58))
				return true;

			// find open slot based on config option `pickupDirection`
			int start = pickupDirection ? 49 : 10;
			int end = pickupDirection ? 10 : 49;
			int step = pickupDirection ? -1 : 1;
			int firstEmptySlot = -1;

			for (int i = start; i != end; i += step)
				if (player.inventory[i].type == ItemID.None) {
					firstEmptySlot = i;
					break;
				}

			if (firstEmptySlot == -1 && hotbarWhenFull)
				for (int i = 0; i < 10; i++)
					if (player.inventory[i].type == ItemID.None) {
						firstEmptySlot = i;
						break;
					}

			return canPickUp(0, 50) || firstEmptySlot != -1;
		}
	}
}
