using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace MultiShop
{
    public class MultiShop : Mod
    {
	    public MultiShop()
	    {
		    Properties = new ModProperties()
		    {
			    Autoload = true
		    };
	    }

	    internal static MultiShop instance; // saves our instance of the MultiShop mod
	    internal static ModHotKey toggleShopHotkey; // the ModHotKey used to toggle the GUI
	    internal UserInterface shopInterface; // the UI (User interface) instance for our shop
	    internal ShopGUI shopGUI; // shop Graphical UI

		public override void Load()
		{
			instance = this; // save our mod instance
			toggleShopHotkey = RegisterHotKey("Toggle MultiShop GUI", "P"); // register a new hotkey
			if (!Main.dedServ)
			{
				// Setup UI if not running from a server
				shopGUI = new ShopGUI();
				shopGUI.Activate();

				shopInterface = new UserInterface();
				shopInterface.SetState(shopGUI);
			}
		}

		public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			// Insert our new shop GUI layer so it updates in this layer
			int mouseTextLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Item / NPC Head")); // do not use Mouse Text, to prevent npc icons going over the gui
			if (mouseTextLayerIndex != -1)
			{
				layers.Insert(mouseTextLayerIndex, new MethodSequenceListItem(
						"MultiShop: Shop GUI",
						delegate
						{
							// 
							if (instance.shopGUI.visible)
							{
								shopInterface.Update(Main._drawInterfaceGameTime);
								shopGUI.Draw(Main.spriteBatch);
							}
							return true;
						},
						null)
				);
			}
		}

		internal class ShopPlayer : ModPlayer
	    {
			public override void ProcessTriggers(TriggersSet triggersSet)
			{
				// If we press the hotkey, toggle the GUI on or off
				if (toggleShopHotkey.JustPressed)
				{
					// Update the GUI before showing it
					if (!instance.shopGUI.visible)
					{
						instance.shopGUI.Update(Main._drawInterfaceGameTime);
					}
					instance.shopGUI.visible = !instance.shopGUI.visible;
				}
			}
		}
	}
}
