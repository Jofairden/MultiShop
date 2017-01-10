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


namespace MultiShop
{
	internal class ShopGUI : UIState
	{
		public UIPanel shopPanel;
		public UIList leftPanel;
		public UIList rightPanel;
		public UIScrollbar leftScrollbar;
		public UIScrollbar rightScrollbar;
		public List<ShopUIPanel> shopUIPanels;
		public List<CollectibleUIPanel> collectibleUIPanels;
		public bool visible = false; // is

		internal const float vpadding = 10f; // view padding
		internal const float vwidth = 800f; // view width
		internal const float vheight = 600f; // view height

		private bool dragging = false;
		private Vector2 offset = Vector2.Zero;

		public override void OnInitialize()
		{
			shopPanel = new UIPanel();
			shopPanel.SetPadding(vpadding);
			shopPanel.Left.Set(Main.screenWidth/2f - vwidth/2f, 0f);
			shopPanel.Top.Set(Main.screenHeight/2f - vheight/2f, 0f);
			shopPanel.Width.Set(vwidth - 3f * vpadding, 0f);
			shopPanel.Height.Set(vheight, 0f);
			shopPanel.OnMouseDown += ShopPanel_OnMouseDown;
			shopPanel.OnMouseUp += ShopPanel_OnMouseUp;
			base.Append(shopPanel); // append shopPanel to base UIState

			leftPanel = new UIList();
			leftPanel.OverflowHidden = true;
			leftPanel.SetPadding(vpadding);
			leftPanel.Width.Set(vwidth/2f, 0f);
			leftPanel.Height.Set(vheight, 0f);
			shopPanel.Append(leftPanel); // append leftPanel to shopPanel

			leftScrollbar = new UIScrollbar();
			leftScrollbar.Height.Set(vheight - 6f * vpadding, 0f);
			leftScrollbar.Width.Set(22f, 0f);
			leftScrollbar.Left.Set(vwidth/2f - 80f + vpadding, 0f);
			leftScrollbar.Top.Set(vpadding, 0f);
			leftPanel.Append(leftScrollbar);
			leftPanel.SetScrollbar(leftScrollbar);

			rightPanel = new UIList();
			rightPanel.CopyStyle(leftPanel);
			rightPanel.Width.Set(vwidth/2f + vpadding + leftScrollbar.Width.Pixels, 0f);
			rightPanel.Left.Set(vwidth/2f - 58f + vpadding * 3f, 0f);
			shopPanel.Append(rightPanel); // append rightPanel to shopPanel

			rightScrollbar = new UIScrollbar();
			rightScrollbar.CopyStyle(leftScrollbar);
			rightScrollbar.Left.Set(vwidth / 2f - 80f + vpadding * 2f, 0f);
			rightPanel.Append(rightScrollbar);
			rightPanel.SetScrollbar(rightScrollbar);

			shopUIPanels = new List<ShopUIPanel>();
			// Leftpanel filler
			for (int i = 0; i < 7; i++)
			{
				ShopUIPanel shopUIPanel = new ShopUIPanel(320, 200f, "Convert Souls");
				shopUIPanel.Initialize();
				shopUIPanel.resultPanel.item.SetDefaults(ItemID.SoulofFlight);
				shopUIPanel.resultPanel.item.stack = 16000;
				shopUIPanel.Top.Set((shopUIPanel.Height.Pixels + vpadding/2f)*i, 0f);
				shopUIPanels.Add(shopUIPanel);
				leftPanel.Add(shopUIPanel); // Do not append, Add for the scrollbar
			}

			collectibleUIPanels = new List<CollectibleUIPanel>();
			for (int i = 0; i < 18; i++)
			{
				CollectibleUIPanel collectibleUIPanel = new CollectibleUIPanel();
				collectibleUIPanel.Initialize();
				collectibleUIPanel.SetItem(ItemID.Meowmere + i, i);
				collectibleUIPanel.itemPanel.item.stack = 0; // reset so it doesn't show a tooltip
				collectibleUIPanels.Add(collectibleUIPanel);
				rightPanel.Add(collectibleUIPanel);
			}
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
		internal string headerText;
		public UIText headerUIText;
		public ItemUIPanel itemPanel;
		public UIPanel unitPanel;
		public UIText unitUIText;

		public CollectibleUIPanel()
		{
			base.Width.Set(ShopGUI.vwidth/2f - 80f + ShopGUI.vpadding, 0f);
			base.Height.Set(ShopUIPanel.panelheight, 0f);
			base.SetPadding(ShopGUI.vpadding/2f);
		}

		public override void OnInitialize()
		{
			itemPanel = new ItemUIPanel();
			itemPanel.Width.Set(ShopUIPanel.panelwidth, 0f);
			itemPanel.Height.Set(ShopUIPanel.panelheight, 0f);
			base.Append(itemPanel);

			headerText = "??";
			headerUIText = new UIText(headerText);
			headerUIText.Top.Set(Main.fontItemStack.MeasureString(headerUIText.Text).X / 2 + ShopGUI.vpadding / 4f, 0f);
			headerUIText.Left.Set(itemPanel.Width.Pixels + ShopGUI.vpadding, 0f);
			base.Append(headerUIText);

			unitPanel = new UIPanel();
			unitPanel.Height.Set(itemPanel.Height.Pixels, 0f);
			unitPanel.Width.Set(base.Width.Pixels/4f, 0f);
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
				itemPanel.item.stack = stack;
				headerUIText.SetText(itemPanel.item.name);
				// todo: make this better, lol
				float scale = 1f;
				while (Main.fontItemStack.MeasureString(headerUIText.Text).X > 190 && scale > 0f)
				{
					scale -= 0.1f;
					headerUIText.SetText(headerUIText.Text, scale, false);
				}
				//
				unitUIText.SetText(stack + " units");
				unitUIText.Left.Set(unitPanel.Width.Pixels - ShopGUI.vpadding * 3f - Main.fontItemStack.MeasureString(unitUIText.Text).X, 0f);
			}
		}
	}

	internal class ShopUIPanel : UIPanel
	{
		internal string headerText;
		public UIText headerUIText;
		public ItemUIPanel resultPanel;
		public ItemUIPanel specialPanel;
		public ItemUIPanel[] materialPanels;
		public ItemUIPanel[] currencyPanels;
		public UIText buyUIText;

		internal const float panelwidth = 50;
		internal const float panelheight = 50;

		public ShopUIPanel(float width, float height, string headerText = "", float precent = 0f)
		{
			base.Width.Set(width, precent);
			base.Height.Set(height + ShopGUI.vpadding, precent);
			this.headerText = headerText;
		}

		public override void OnInitialize()
		{
			// Header
			headerUIText = new UIText(headerText);
			base.Append(headerUIText);

			// Result
			resultPanel = new ItemUIPanel();
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
				currentPanel.Width.Set(panelwidth, 0f);
				currentPanel.Height.Set(panelheight, 0f);
				currentPanel.Top.Set(materialPanels[materialPanels.Length - 1].Top.Pixels + ShopGUI.vpadding/2f + panelheight, 0f);
				currentPanel.Left.Set(resultPanel.Width.Pixels + ShopGUI.vpadding/2f + (panelwidth + ShopGUI.vpadding/2f) * i, 0f);
				base.Append(currentPanel);
			}

			// Special slot
			specialPanel = new ItemUIPanel();
			specialPanel.Top.Set(resultPanel.Top.Pixels + resultPanel.Height.Pixels + ShopGUI.vpadding/2f, 0f);
			specialPanel.Width.Set(panelwidth, 0f);
			specialPanel.Height.Set(panelheight, 0f);
			specialPanel.Left.Set(specialPanel.Width.Pixels/4f, 0f);
			base.Append(specialPanel);

			buyUIText = new UIText("BUY");
			var stringSize = Main.fontItemStack.MeasureString(buyUIText.Text);
			buyUIText.Top.Set(specialPanel.Top.Pixels + specialPanel.Height.Pixels + ShopGUI.vpadding/2f, 0f);
			buyUIText.Left.Set(stringSize.X/2f + ShopGUI.vpadding/4f, 0f);
			base.Append(buyUIText);
		}
	}

	internal class ItemUIPanel : UIPanel
	{
		public Item item;
		internal UIText stackUIText;

		public ItemUIPanel(int itemType = 0, int stack = 1)
		{
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
}
