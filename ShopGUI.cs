using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;

namespace MultiShop
{
	//todo: remove duplicate entries for material panels when possible

	//internal class ShopSortButton : UIElement
	//{
	//	public UIPanel _panel;
	//	public float offsetY = 0f;

	//	public ShopSortButton()
	//	{
	//		base.Width.Set(40, 0f);
	//		base.Height.Set(40, 0f);
	//	}

	//	public override void OnInitialize()
	//	{
	//		_panel = new UIPanel();
	//		_panel.OnClick += ShopSortButton_OnClick;
	//		_panel.BackgroundColor = Color.Yellow;
	//		_panel.Width.Set(40f, 0f);
	//		_panel.Height.Set(40f, 0f);
	//		_panel.Left.Set(5f, 0f);
	//		_panel.Top.Set(5f + offsetY, 0f);
	//		base.Append(_panel);
	//	}

	//	internal void ResetTop()
	//	{
	//		_panel?.Top.Set(5f + offsetY, 0f);
	//	}

	//	private void ShopSortButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
	//	{
	//		Main.NewText("Clicked on button!");
	//		MultiShop.instance.shopGUI.rightPanel._items.Sort((x, y) => (x as CollectibleUIPanel).itemPanel.item.name.CompareTo((y as CollectibleUIPanel).itemPanel.item.name));
	//		MultiShop.instance.shopGUI.rightPanel.RecalculateChildren();
	//	}
	//}

	//internal class ShopSortPanel : UIPanel
	//{
	//	internal List<ShopSortButton> _buttons;

	//	public ShopSortPanel()
	//	{
	//		_buttons = new List<ShopSortButton>();
	//	}

	//	public override void OnInitialize()
	//	{
	//		base.SetPadding(0);
	//		base.Width.Set(50f, 0f);
	//		base.Height.Set(ShopGUI.vheight / 2f - ShopGUI.vpadding, 0f);
	//		base.Top.Set(ShopGUI.vheight / 10f - 22f, 0f);
	//	}

	//	public void AddButton(ShopSortButton button)
	//	{
	//		button.Initialize();
	//		button.offsetY = _buttons.Count * button.Width.Pixels + 2.5f * _buttons.Count;
	//		button.ResetTop();
	//		_buttons.Add(button);
	//	}

	//	internal void AppendButtons()
	//	{
	//		foreach (var button in _buttons)
	//		{
	//			if (button != null)
	//				base.Append(button);
	//		}
	//	}
	//}

	internal class ShopGUI : UIState
	{
		//public ShopSortPanel leftSortPanel;
		//public ShopSortPanel rightSortPanel;

		public UIPanel shopPanel;
		public UIList leftPanel;
		public UIList rightPanel;
		public UIScrollbar leftScrollbar;
		public UIScrollbar rightScrollbar;
		public List<ShopUIPanel> shopUIPanels;
		public List<CollectibleUIPanel> collectibleUIPanels;
		public bool visible = false; // is

		internal List<Tuple<int, int>> TEMPCollectibles;

		internal const float vpadding = 10f; // view padding
		internal const float vwidth = 800f; // view width
		internal const float vheight = 600f; // view height

		private bool dragging = false;
		private Vector2 offset = Vector2.Zero;

		public override void OnInitialize()
		{
			shopPanel = new UIPanel();
			shopPanel.SetPadding(vpadding);
			shopPanel.OverflowHidden = false;
			shopPanel.Left.Set(Main.screenWidth/2f - vwidth/2f, 0f);
			shopPanel.Top.Set(Main.screenHeight/2f - vheight/2f, 0f);
			shopPanel.Width.Set(vwidth - 3f * vpadding, 0f);
			shopPanel.Height.Set(vheight, 0f);
			shopPanel.OnMouseDown += ShopPanel_OnMouseDown;
			shopPanel.OnMouseUp += ShopPanel_OnMouseUp;
			base.Append(shopPanel); // append shopPanel to base UIState

			//leftSortPanel = new ShopSortPanel();
			//leftSortPanel.Left.Set(-44f - vpadding * 2f, 0f);
			//shopPanel.Append(leftSortPanel);

			//rightSortPanel = new ShopSortPanel();
			//rightSortPanel.Left.Set(vwidth - 3 * vpadding, 0f);

			//shopPanel.Append(rightSortPanel);

			UIPanel leftPanelBG = new UIPanel();
			leftPanelBG.OverflowHidden = true;
			leftPanelBG.SetPadding(0);
			leftPanelBG.Width.Set(vwidth / 2f - 40f + vpadding, 0f);
			leftPanelBG.Height.Set(vheight, 0f);
			shopPanel.Append(leftPanelBG);

			leftPanel = new UIList();
			leftPanel.OverflowHidden = true;
			leftPanel.SetPadding(vpadding);
			leftPanel.Width.Set(vwidth/2f, 0f);
			leftPanel.Height.Set(vheight, 0f);
			leftPanelBG.Append(leftPanel); // append leftPanel to shopPanel

			leftScrollbar = new UIScrollbar();
			leftScrollbar.Height.Set(vheight - 6f * vpadding, 0f);
			leftScrollbar.Width.Set(22f, 0f);
			leftScrollbar.Left.Set(vwidth/2f - 80f + vpadding, 0f);
			leftScrollbar.Top.Set(vpadding, 0f);
			leftPanel.Append(leftScrollbar);
			leftPanel.SetScrollbar(leftScrollbar);

			UIPanel rightPanelBG = new UIPanel();
			rightPanelBG.CopyStyle(leftPanelBG);
			rightPanelBG.Left.Set(vwidth / 2f - 58f + vpadding * 4f, 0f);
			shopPanel.Append(rightPanelBG);

			rightPanel = new UIList();
			rightPanel.CopyStyle(leftPanel);
			//rightPanel.Width.Set(vwidth/2f + vpadding + leftScrollbar.Width.Pixels, 0f);
			//rightPanel.Left.Set(vwidth/2f - 58f + vpadding * 3f, 0f);
			rightPanelBG.Append(rightPanel); // append rightPanel to shopPanel

			rightScrollbar = new UIScrollbar();
			rightScrollbar.CopyStyle(leftScrollbar);
			rightScrollbar.Left.Set(vwidth / 2f - 80f + vpadding, 0f);
			rightPanel.Append(rightScrollbar);
			rightPanel.SetScrollbar(rightScrollbar);

			// Temp fill of collectibles
			TEMPCollectibles = new List<Tuple<int, int>>();
			var tmpC = new Tuple<int, int>[]
			{
				new Tuple<int, int>(ItemID.Bone, 500),
				new Tuple<int, int>(ItemID.DemoniteOre, 450),
			};

			for (int i = 0; i < tmpC.Length; i++)
			{
				TEMPCollectibles.Add(tmpC[i]);
			}

			for (int i = ItemID.Sapphire; i < ItemID.Diamond + 1; i++)
			{
				TEMPCollectibles.Add(new Tuple<int, int>(i, 50));
			}

			for (int i = ItemID.LivingCursedFireBlock; i < ItemID.LivingUltrabrightFireBlock + 1; i++)
			{
				TEMPCollectibles.Add(new Tuple<int, int>(i, 30));
			}

			collectibleUIPanels = new List<CollectibleUIPanel>();
			for (int i = 0; i < TEMPCollectibles.Count; i++)
			{
				CollectibleUIPanel collectibleUIPanel = new CollectibleUIPanel();
				collectibleUIPanel.Initialize();
				collectibleUIPanel.SetItem(TEMPCollectibles[i].Item1, TEMPCollectibles[i].Item2);
				collectibleUIPanels.Add(collectibleUIPanel);
				rightPanel.Add(collectibleUIPanel);
			}
			SortingMode.ApplySort(ref rightPanel, SortingMode.CollectibleSortingMode.UnitDesc);

			//var tmp = new ShopSortButton();
			//rightSortPanel.AddButton(tmp);
			//rightSortPanel.AppendButtons();

			shopUIPanels = new List<ShopUIPanel>();
			// Leftpanel filler
			for (int i = 0; i < 10; i++)
			{
				ShopUIPanel shopUIPanel = new ShopUIPanel();
				shopUIPanel.Initialize();
				shopUIPanel.SetResult(ItemID.SoulofFlight + Main.rand.Next(50), Main.rand.Next(50));
				for (int j = 0; j < shopUIPanel.materialPanels.Length; j++)
				{
					var currentPanel = shopUIPanel.materialPanels[j];
					currentPanel.item.SetDefaults(collectibleUIPanels[Main.rand.Next(collectibleUIPanels.Count - 1)].itemPanel.item.type);
					currentPanel.item.stack = Main.rand.Next(6);
				}
				for (int j = 0; j < shopUIPanel.currencyPanels.Length; j++)
				{
					var currentPanel = shopUIPanel.currencyPanels[j];
					currentPanel.item.stack = Main.rand.Next(20 * (j + 1));
				}
				shopUIPanel.Top.Set((shopUIPanel.Height.Pixels + vpadding / 2f) * i, 0f);
				shopUIPanels.Add(shopUIPanel);
				leftPanel.Add(shopUIPanels[shopUIPanels.Count - 1]); // Do not append, Add for the scrollbar
			}
			SortingMode.ApplySort(ref leftPanel, SortingMode.ShopSortingMode.Normal);
		}

		// Panel dragging
		private void _Recalculate(Vector2 mousePos, float precent = 0f)
		{
			shopPanel.Left.Set(mousePos.X - offset.X, precent);
			shopPanel.Top.Set(mousePos.Y - offset.Y, precent);
			Recalculate();
		}

		private void ShopPanel_OnMouseUp(UIMouseEvent evt, UIElement listeningElement)
		{
			_Recalculate(evt.MousePosition);
			dragging = false;
		}

		private void ShopPanel_OnMouseDown(UIMouseEvent evt, UIElement listeningElement)
		{
			offset = new Vector2(evt.MousePosition.X - shopPanel.Left.Pixels, evt.MousePosition.Y - shopPanel.Top.Pixels);
			dragging = true;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Vector2 mousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);

			if (shopPanel.ContainsPoint(mousePosition)) 
			{
				Main.LocalPlayer.mouseInterface = true;
			}

			if (dragging)
			{
				_Recalculate(mousePosition);
			}
		}
	}

	internal class CollectibleUIPanel : UIPanel
	{
		internal int _stack;
		internal string headerText;
		public UIText headerUIText;
		public ItemUIPanel itemPanel;
		public UIPanel unitPanel;
		public UIText unitUIText;

		public CollectibleUIPanel()
		{
			base.Width.Set(ShopGUI.vwidth/2f - 80f, 0f);
			base.Height.Set(ShopUIPanel.panelheight, 0f);
			base.SetPadding(ShopGUI.vpadding/2f);
		}

		public override void OnInitialize()
		{
			itemPanel = new ItemUIPanel();
			itemPanel.OnClick += (s, e) =>
			{
				var sortMode = SortingMode.CurrentCollectibleSortMode == SortingMode.CollectibleSortingMode.NamesDesc ? SortingMode.CollectibleSortingMode.NamesAsc
							   : SortingMode.CurrentCollectibleSortMode == SortingMode.CollectibleSortingMode.NamesAsc ?
							   SortingMode.CollectibleSortingMode.Normal : SortingMode.CollectibleSortingMode.NamesDesc;

				SortingMode.ApplySort(ref MultiShop.instance.shopGUI.rightPanel, sortMode);
			};
			itemPanel.Width.Set(ShopUIPanel.panelwidth, 0f);
			itemPanel.Height.Set(ShopUIPanel.panelheight, 0f);
			base.Append(itemPanel);

			headerText = "??";
			headerUIText = new UIText(headerText);
			headerUIText.Top.Set(Main.fontItemStack.MeasureString(headerUIText.Text).X / 2 + ShopGUI.vpadding / 4f, 0f);
			headerUIText.Left.Set(itemPanel.Width.Pixels + ShopGUI.vpadding, 0f);
			base.Append(headerUIText);

			unitPanel = new UIPanel();
			unitPanel.OnClick += (s, e) =>
			{
				var sortMode = SortingMode.CurrentCollectibleSortMode == SortingMode.CollectibleSortingMode.UnitDesc ?
							   SortingMode.CollectibleSortingMode.UnitAsc
							   : SortingMode.CollectibleSortingMode.UnitDesc;

				SortingMode.ApplySort(ref MultiShop.instance.shopGUI.rightPanel, sortMode);
			};
			unitPanel.OnMouseOver += (s, e) =>
			{
				(e as UIPanel).PanelUIHover();
			};
			unitPanel.OnMouseOut += (s, e) =>
			{
				(e as UIPanel).PanelUIHover(false);
			};
			unitPanel.Height.Set(itemPanel.Height.Pixels, 0f);
			unitPanel.Width.Set(base.Width.Pixels/4f + ShopGUI.vpadding, 0f);
			unitPanel.Left.Set(base.Width.Pixels - unitPanel.Width.Pixels - ShopGUI.vpadding, 0f);
			base.Append(unitPanel);

			string text = itemPanel.item.stack + " units";
			unitUIText = new UIText(text);
			unitUIText.Left.Set(unitPanel.Width.Pixels - ShopGUI.vpadding * 3f - Main.fontItemStack.MeasureString(text).X, 0f);
			unitPanel.Append(unitUIText);
		}

		internal void SetItem(int itemType, int stack = 0)
		{
			if (itemPanel != null)
			{
				itemPanel.item.SetDefaults(itemType);
				headerText = itemPanel.item.name;
				itemPanel.item.stack = 0;
				SetItemName(itemPanel.item.name);
				this._stack = stack;
				unitUIText.SetText(stack + " units");
				unitUIText.Left.Set(unitPanel.Width.Pixels - ShopGUI.vpadding * 3f - Main.fontItemStack.MeasureString(unitUIText.Text).X, 0f);
			}
		}

		internal void SetItemName(string text)
		{
			headerUIText.SetText(text);
			string newText = text;
			while (Main.fontItemStack.MeasureString(newText).X > 130)
			{
				newText = newText.Substring(0, newText.Length - 1);
				headerUIText.SetText(newText + "...");
			}
		}
	}

	internal class ShopUIPanel : UIPanel
	{
		internal int totalGoldValue = 0;
		internal string headerText = "";
		public UIText headerUIText;
		public ItemUIPanel resultPanel;
		public ItemUIPanel specialPanel;
		public ItemUIPanel[] materialPanels;
		public ItemUIPanel[] currencyPanels;
		public UIText buyUIText;

		internal const float panelwidth = 50;
		internal const float panelheight = 50;

		public ShopUIPanel()
		{
			base.Width.Set(320, 0f);
			base.Height.Set(ShopGUI.vheight/3f - ShopGUI.vpadding * 1.5f, 0f);
		}

		internal void SetResult(int itemType, int stack = 1)
		{
			resultPanel.item.ResetStats(itemType);
			resultPanel.item.SetDefaults(itemType);
			resultPanel.item.stack = stack;
			SetHeader(resultPanel.item.name);
		}

		internal void SetHeader(string text)
		{
			this.headerText = text;
		}

		public override void OnInitialize()
		{
			// Header
			headerUIText = new UIText(headerText);
			base.Append(headerUIText);

			// Result
			resultPanel = new ItemUIPanel();
			resultPanel.OnClick += (s, e) =>
			{
				var sortMode = SortingMode.CurrentShopSortMode == SortingMode.ShopSortingMode.ResultNameDesc ? SortingMode.ShopSortingMode.ResultNameAsc
							   : SortingMode.CurrentShopSortMode == SortingMode.ShopSortingMode.ResultNameAsc ?
							   SortingMode.ShopSortingMode.Normal : SortingMode.ShopSortingMode.ResultNameDesc;

				SortingMode.ApplySort(ref MultiShop.instance.shopGUI.leftPanel, sortMode);
			};
			resultPanel.Width.Set(panelwidth*1.5f, 0f);
			resultPanel.Height.Set(panelheight*1.5f, 0f);
			resultPanel.Top.Set(Main.fontItemStack.MeasureString(headerText).Y, 0f);
			base.Append(resultPanel);

			// Materialpanels
			materialPanels = new ItemUIPanel[8];
			for (int i = 0; i < materialPanels.Length; i++)
			{
				materialPanels[i] = new ItemUIPanel();
				var currentPanel = materialPanels[i];
				currentPanel.Width.Set(panelwidth, 0f);
				currentPanel.Height.Set(panelheight, 0f);
				currentPanel.Top.Set(i < 4 
					? resultPanel.Top.Pixels 
					: resultPanel.Top.Pixels + panelheight + ShopGUI.vpadding/2f, 0f);
				currentPanel.Left.Set(resultPanel.Width.Pixels + ShopGUI.vpadding/2f + (panelwidth + ShopGUI.vpadding/2f ) * (i%(materialPanels.Length/2f)), 0f);
				base.Append(currentPanel);
			}

			// Currencypanels
			currencyPanels = new ItemUIPanel[4];
			for (int i = 0; i < currencyPanels.Length; i++)
			{
				currencyPanels[i] = new ItemUIPanel(ItemID.CopperCoin + i, 0);
				var currentPanel = currencyPanels[i];
				currentPanel.OnClick += (s, e) =>
				{
					var sortMode = SortingMode.CurrentShopSortMode == SortingMode.ShopSortingMode.GoldDesc ? SortingMode.ShopSortingMode.GoldAsc
								   : SortingMode.ShopSortingMode.GoldDesc;

					SortingMode.ApplySort(ref MultiShop.instance.shopGUI.leftPanel, sortMode);
				};
				currentPanel.Width.Set(panelwidth, 0f);
				currentPanel.Height.Set(panelheight, 0f);
				currentPanel.Top.Set(materialPanels[materialPanels.Length - 1].Top.Pixels + ShopGUI.vpadding/2f + panelheight, 0f);
				currentPanel.Left.Set(resultPanel.Width.Pixels + ShopGUI.vpadding/2f + (panelwidth + ShopGUI.vpadding/2f) * i, 0f);
				base.Append(currentPanel);
			}

			// Special slot
			specialPanel = new ItemUIPanel();
			specialPanel.item.SetDefaults(ItemID.CrystalShard);
			specialPanel.item.stack = Main.rand.Next(50);
			specialPanel.OnClick += (s, e) =>
			{
				var sortMode = SortingMode.CurrentShopSortMode == SortingMode.ShopSortingMode.ArtifactDesc ? SortingMode.ShopSortingMode.ArtifactAsc
								   : SortingMode.ShopSortingMode.ArtifactDesc;

				SortingMode.ApplySort(ref MultiShop.instance.shopGUI.leftPanel, sortMode);
			};
			specialPanel.Top.Set(resultPanel.Top.Pixels + resultPanel.Height.Pixels + ShopGUI.vpadding/2f, 0f);
			specialPanel.Width.Set(panelwidth, 0f);
			specialPanel.Height.Set(panelheight, 0f);
			specialPanel.Left.Set(specialPanel.Width.Pixels/4f, 0f);
			base.Append(specialPanel);

			buyUIText = new UIText("BUY");
			buyUIText.OnMouseOver += (s, e) =>
			{
				(e as UIText).TextUIHover();
			};
			buyUIText.OnMouseOut += (s, e) =>
			{
				(e as UIText).TextUIHover(false);
			};
			var stringSize = Main.fontItemStack.MeasureString(buyUIText.Text);
			buyUIText.Top.Set(specialPanel.Top.Pixels + specialPanel.Height.Pixels + ShopGUI.vpadding/2f, 0f);
			buyUIText.Left.Set(stringSize.X/2f + ShopGUI.vpadding/4f, 0f);
			base.Append(buyUIText);
		}

		internal void UpdateValue()
		{
			totalGoldValue = Item.buyPrice(currencyPanels[3].item.stack, currencyPanels[2].item.stack, currencyPanels[1].item.stack, currencyPanels[0].item.stack);
		}
	}

	internal class ItemUIPanel : UIPanel
	{
		public Item item;
		internal UIText stackUIText;

		public ItemUIPanel(int itemType = 0, int stack = 1)
		{
			base.OnMouseOver += (s, e) =>
			{
				(e as UIPanel).PanelUIHover();
			};
			base.OnMouseOut += (s, e) =>
			{
				(e as UIPanel).PanelUIHover(false);
			};
			item = new Item();
			item.SetDefaults(itemType);
			item.stack = stack;
		}

		public override void OnInitialize()
		{
			if (item.stack > 1)
			{
				string stackText = item.stack.ToString();
				stackUIText = new UIText(stackText);
				Vector2 stringSize = Main.fontItemStack.MeasureString(stackText);
				stackUIText.Top.Set(base.Height.Pixels/2f - stringSize.Y/4f, 0f);
				//stackUIText.Left.Set(stringSize.X/4f, 0f);
				base.Append(stackUIText);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (item != null && item.type != 0)
			{
				if (base.IsMouseHovering)
				{
					Item mouseItem = new Item();
					mouseItem.SetDefaults(0);
					Main.mouseItem = mouseItem;
					Main.hoverItemName = item.name;
					Main.toolTip = item.Clone();
					Main.toolTip.name = Main.toolTip.name + (Main.toolTip.modItem != null ? $"[{Main.toolTip.modItem.mod.Name}]" : "");
				}

				CalculatedStyle innerDimensions = base.GetInnerDimensions();
				Texture2D texture2D = Main.itemTexture[this.item.type];
				Rectangle frame;
				if (Main.itemAnimations[item.type] != null)
				{
					frame = Main.itemAnimations[item.type].GetFrame(texture2D);
				}
				else
				{
					frame = texture2D.Frame(1, 1, 0, 0);
				}
				float drawScale = 1f;
				float num2 = (float)innerDimensions.Width * 1f;
				if ((float)frame.Width > num2 || (float)frame.Height > num2)
				{
					if (frame.Width > frame.Height)
					{
						drawScale = num2 / (float)frame.Width;
					}
					else
					{
						drawScale = num2 / (float)frame.Height;
					}
				}
				Vector2 drawPosition = new Vector2(innerDimensions.X, innerDimensions.Y);
				drawPosition.X += (float)innerDimensions.Width * 1f / 2f - (float)frame.Width * drawScale / 2f;
				drawPosition.Y += (float)innerDimensions.Height * 1f / 2f - (float)frame.Height * drawScale / 2f;

				this.item.GetColor(Color.White);
				spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(frame), this.item.GetAlpha(Color.White), 0f,
					Vector2.Zero, drawScale, SpriteEffects.None, 0f);
				if (this.item.color != default(Color))
				{
					spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(frame), this.item.GetColor(Color.White), 0f,
						Vector2.Zero, drawScale, SpriteEffects.None, 0f);
				}
			}
		}
	}

	internal static class SortingMode
	{
		public static CollectibleSortingMode CurrentCollectibleSortMode = CollectibleSortingMode.Normal;
		public static int CollectibleSortCount => Enum.GetNames(typeof(CollectibleSortingMode)).Length;

		public static ShopSortingMode CurrentShopSortMode = ShopSortingMode.Normal;

		public static int ShopSortCount => Enum.GetNames(typeof(ShopSortingMode)).Length;


		public enum CollectibleSortingMode
		{
			Normal = 1,
			NamesAsc,
			NamesDesc,
			UnitAsc,
			UnitDesc,
		}

		public enum ShopSortingMode
		{
			Normal = 1,
			ResultNameAsc,
			ResultNameDesc,
			GoldAsc,
			GoldDesc,
			ArtifactAsc,
			ArtifactDesc,
		}

		public static void ApplySort(ref UIList list, CollectibleSortingMode mode)
		{
			switch (mode)
			{
				default:
				case CollectibleSortingMode.Normal:
					list.UpdateOrder();
					break;
				case CollectibleSortingMode.NamesAsc:
					list._items.Sort((x, y) => (x as CollectibleUIPanel).itemPanel.item.name.CompareTo((y as CollectibleUIPanel).itemPanel.item.name));
					break;
				case CollectibleSortingMode.NamesDesc:
					list._items.Sort((x, y) => (y as CollectibleUIPanel).itemPanel.item.name.CompareTo((x as CollectibleUIPanel).itemPanel.item.name));
					break;
				case CollectibleSortingMode.UnitAsc:
					list._items.Sort((x, y) => (x as CollectibleUIPanel)._stack.CompareTo((y as CollectibleUIPanel)._stack));
					break;
				case CollectibleSortingMode.UnitDesc:
					list._items.Sort((x, y) => (y as CollectibleUIPanel)._stack.CompareTo((x as CollectibleUIPanel)._stack));
					break;
			}
			CurrentCollectibleSortMode = mode;
		}

		public static void ApplySort(ref UIList list, ShopSortingMode mode)
		{
			switch (mode)
			{
				default:
				case ShopSortingMode.Normal:
					list.UpdateOrder();
					break;
				case ShopSortingMode.ResultNameAsc:
					list._items.Sort((x, y) => (x as ShopUIPanel).resultPanel.item.name.CompareTo((y as ShopUIPanel).resultPanel.item.name));
					break;
				case ShopSortingMode.ResultNameDesc:
					list._items.Sort((x, y) => (y as ShopUIPanel).resultPanel.item.name.CompareTo((x as ShopUIPanel).resultPanel.item.name));
					break;
				case ShopSortingMode.GoldAsc:
					list._items.ForEach(x => (x as ShopUIPanel).UpdateValue());
					list._items.Sort((x, y) => (x as ShopUIPanel).totalGoldValue.CompareTo((y as ShopUIPanel).totalGoldValue));
					break;
				case ShopSortingMode.GoldDesc:
					list._items.ForEach(x => (x as ShopUIPanel).UpdateValue());
					list._items.Sort((x, y) => (y as ShopUIPanel).totalGoldValue.CompareTo((x as ShopUIPanel).totalGoldValue));
					break;
				case ShopSortingMode.ArtifactAsc:
					list._items.Sort((x, y) => (x as ShopUIPanel).specialPanel.item.stack.CompareTo((y as ShopUIPanel).specialPanel.item.stack));
					break;
				case ShopSortingMode.ArtifactDesc:
					list._items.Sort((x, y) => (y as ShopUIPanel).specialPanel.item.stack.CompareTo((x as ShopUIPanel).specialPanel.item.stack));
					break;
			}
			CurrentShopSortMode = mode;
		}
	}

	internal static class EffectHelper
	{
		public static void PanelUIHover(this UIPanel panel, bool hover = true)
		{
			panel.BackgroundColor = hover ? new Color(panel.BackgroundColor.R, panel.BackgroundColor.G, panel.BackgroundColor.B + 45, panel.BackgroundColor.A + 45)
				: new Color(panel.BackgroundColor.R, panel.BackgroundColor.G, panel.BackgroundColor.B - 45, panel.BackgroundColor.A - 45);
		}

		public static void TextUIHover(this UIText text, bool hover = true)
		{
			text.TextColor = hover ? Color.Yellow : Color.White;
		}
	}
}
