using Terraria.ModLoader;

namespace NoHotbarPickup
{
	public class NoHotbarPickup : Mod
	{
		public override void Load()
		{
			NoHotbarPickupGlobal.Load();
		}

		public override void Unload()
		{
			NoHotbarPickupGlobal.Unload();
		}
	}
}
