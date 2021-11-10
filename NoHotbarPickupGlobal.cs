using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace NoHotbarPickup;

public class NoHotbarPickupGlobal : GlobalItem
{
	private static List<int> _ignored;

	private static List<int> _ammo;

	public override void Load()
	{
		LoadIgnored();
		LoadAmmos();
	}

	private static void LoadIgnored()
	{
		_ignored = new List<int>
		{
			// Coins
			ItemID.CopperCoin,
			ItemID.SilverCoin,
			ItemID.GoldCoin,
			ItemID.PlatinumCoin,
			// Hearts
			ItemID.Heart,
			ItemID.CandyApple,
			ItemID.CandyCane,
			// Mana stars
			ItemID.Star,
			ItemID.SoulCake,
			ItemID.SugarPlum,
			// Nebula buffs
			ItemID.NebulaPickup1,
			ItemID.NebulaPickup2,
			ItemID.NebulaPickup3
		};
		//_ignored = _ignored.Concat(ItemID.Sets.NebulaPickup.Where(b => b).Select((_, i) => i)); // TODO why doesn't this work?
	}

	private static void LoadAmmos()
	{
		_ammo = new List<int>
		{
			AmmoID.Gel,
			AmmoID.Arrow,
			AmmoID.Coin,
			AmmoID.FallenStar,
			AmmoID.Bullet,
			AmmoID.Sand,
			AmmoID.Dart,
			AmmoID.Rocket,
			AmmoID.Solution,
			AmmoID.Flare,
			AmmoID.Snowball,
			AmmoID.StyngerBolt,
			AmmoID.CandyCorn,
			AmmoID.JackOLantern,
			AmmoID.Stake,
			AmmoID.NailFriendly
		};
	}

	public override void Unload()
	{
		_ignored = null;
		_ammo = null;
	}

	public override bool OnPickup(Item item, Player player)
	{
		if (_ignored.Contains(item.type))
			return true;

		if (item.ModItem?.ItemSpace(player) == true)
			return true;

		// player.inventory [59]
		// 0-9 hotbar
		// 10-49 are actual inventory
		// 50-53 coins
		// 54-57 ammo
		// 58 mouse ???
		void TextAndSound(int stack)
		{
			PopupText.NewText(PopupTextContext.RegularItemPickup, item, stack);
			SoundEngine.PlaySound(SoundID.Grab, player.position);
		}

		bool Pickup(int from, int to, int emptySlot)
		{
			// check inventory and hotbar for item to stack into
			for (int i = from; i < to; i++)
			{
				Item invItem = player.inventory[i];

				if (invItem.type != item.type)
					continue;

				if (invItem.stack == invItem.maxStack)
					continue;

				if (invItem.maxStack == 1) // item doesn't stack
				{
					if (emptySlot == -1)
						return true;

					player.inventory[emptySlot] = item;
					TextAndSound(item.stack);
				}
				else if (invItem.stack + item.stack > invItem.maxStack)
				{
					// item can stack, but stack + existing.stack > maxStack
					// add what can be added to existing
					int origStack = item.stack;
					item.stack -= invItem.maxStack - invItem.stack;
					invItem.stack = invItem.maxStack;
					// add new item using remainder (if there's a spot for it)
					if (emptySlot != -1)
					{
						player.inventory[emptySlot] = item;
						TextAndSound(origStack);
					}
					else
					{
						player.QuickSpawnClonedItem(item, item.stack);
						TextAndSound(origStack - item.stack);
					}
				}
				else
				{
					// item can stack
					invItem.stack += item.stack;
					TextAndSound(item.stack);
				}

				// item has been picked up
				return true;
			}

			// item not in inventory
			return false;
		}

		if (_ammo.Contains(item.ammo) || item.shoot != ItemID.None)
		{
			int firstEmptyAmmoSlot = -1;

			for (int i = 54; i < 58; i++)
				if (player.inventory[i].type == ItemID.None)
				{
					firstEmptyAmmoSlot = i;
					break;
				}

			if (Pickup(54, 58, firstEmptyAmmoSlot))
				return false;
		}

		// find open slot based on config option 'PickupDirection'
		(int start, int end, int step) = NoHotbarPickupConfig.PickupDirection ? (49, 10, -1) : (10, 49, 1);
		int firstEmptySlot = -1;

		for (int i = start; i != end; i += step)
			if (player.inventory[i].type == ItemID.None)
			{
				firstEmptySlot = i;
				break;
			}

		if (firstEmptySlot == -1 && NoHotbarPickupConfig.HotbarWhenFull)
			for (int i = 0; i < 10; i++)
				if (player.inventory[i].type == ItemID.None)
				{
					firstEmptySlot = i;
					break;
				}

		if (Pickup(0, 50, firstEmptySlot))
			return false;

		if (firstEmptySlot == -1)
			return false;

		player.inventory[firstEmptySlot] = item;
		TextAndSound(item.stack);

		return false;
	}

	public override bool CanPickup(Item item, Player player)
	{
		if (_ignored.Contains(item.type))
			return true;

		if (item.ModItem?.ItemSpace(player) == true)
			return true;

		Item[] voidVault = player.bank4.item;

		// player.inventory [59]
		// 0-9 hotbar
		// 10-49 are actual inventory
		// 50-53 coins
		// 54-57 ammo
		// 58 mouse ???

		bool HasRoom(int from, int to)
		{
			for (int i = from; i < to; i++)
			{
				// check inventory and hotbar for item
				Item invItem = player.inventory[i];
				if (invItem.type != item.type)
					continue;

				// item already in inventory
				if (invItem.stack == invItem.maxStack)
					continue;

				return true;
			}

			return false;
		}

		if (_ammo.Any(type => type == item.ammo) && HasRoom(54, 58))
			return true;

		// find open slot based on config option 'PickupDirection'
		(int start, int end, int step) = NoHotbarPickupConfig.PickupDirection ? (49, 10, -1) : (10, 49, 1);
		int firstEmptySlot = -1;

		for (int i = start; i != end; i += step)
			if (player.inventory[i].type == ItemID.None)
			{
				firstEmptySlot = i;
				break;
			}

		if (firstEmptySlot != -1)
			return true;

		if (!NoHotbarPickupConfig.HotbarWhenFull)
			return HasRoom(0, 50);

		for (int i = 0; i < 10; i++)
			if (player.inventory[i].type == ItemID.None)
			{
				firstEmptySlot = i;
				break;
			}

		return HasRoom(0, 50) || firstEmptySlot != -1;
	}
}