using System.ComponentModel;
using Terraria.ModLoader.Config;

#pragma warning disable 649

namespace NoHotbarPickup
{
	internal class NoHotbarPickupConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Label("Items go into hotbar when inventory is full")]
		[Tooltip("If disabled and inventory full, items can't be picked up, even if hotbar has room for them")]
		[DefaultValue(true)]
		public bool hotbarWhenFull;

		[Label("Items should go into the last available slot of the inventory")]
		[Tooltip("If off, items fill inventory from beginning")]
		[DefaultValue(true)]
		public bool pickupDirection;
	}
}
