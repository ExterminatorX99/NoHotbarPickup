using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

#pragma warning disable 649

namespace NoHotbarPickup;

public class NoHotbarPickupConfig : ModConfig
{
	[Label("Items go into hotbar when inventory is full")]
	[Tooltip("If this is disabled and inventory is full, items can't be picked up, even if hotbar has room for them")]
	[DefaultValue(false)]
	public bool hotbarWhenFull;

	[Label("Items should go into the last available slot of the inventory")]
	[Tooltip("If off, items fill inventory from beginning")]
	[DefaultValue(true)]
	public bool pickupDirection;

	public override ConfigScope Mode => ConfigScope.ClientSide;
	private static NoHotbarPickupConfig Instance => ModContent.GetInstance<NoHotbarPickupConfig>();
	public static bool HotbarWhenFull => Instance.hotbarWhenFull;
	public static bool PickupDirection => Instance.pickupDirection;
}