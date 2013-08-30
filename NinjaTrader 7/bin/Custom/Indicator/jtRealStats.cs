 // v1.5 by John Thom
// credits to the NinjaTrader team member that posted the MarketData routine that I incorporated below.

#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Design;
using System.Collections.Generic;


#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// Enter the description of your new custom indicator here
    /// </summary>
    [Description("jtRealtime Stats version 1.5.3")]
    public class jtRealStats : Indicator
    {
		// user settings
		
		private bool showStats;
		private TextPosition statsPosition;

		private bool showMarketData;
		private bool showVolume;
		private Color higherVolColor = Color.Cyan;
		private Color lowerVolColor = Color.Fuchsia;
		private Color sameVolColor = Color.LightGray;
		private Color outlineColor = Color.Black;
		private Color textColor = Color.Black;
		private Color levelIILevel1Color = Color.Yellow;
		private Color levelIILevel2Color = Color.DarkOrange;
		private Color levelIILevel3Color = Color.DarkRed;
		private Color levelIILevel4Color = Color.Green;
		private Color levelIILevel5Color = Color.Blue;
		private Color levelIILevel6Color = Color.Cyan;
		private Color levelIILevel7Color = Color.Fuchsia;
		private Color levelIILevel8Color = Color.DarkMagenta;
		private Color levelIILevel9Color = Color.DarkGoldenrod;
		private Color levelIILevel10Color = Color.DarkTurquoise;
		private Color priceLineColor = Color.Black;
		private Color bidColor = Color.LimeGreen;
		private Color askColor = Color.DarkRed;
		private int maxRows;

		
		private SolidBrush higherVolBrush;
		private SolidBrush lowerVolBrush;
		private SolidBrush sameVolBrush;
		private Pen outlinePen;
		private Brush textBrush;
		private SolidBrush levelIILevel1Brush;
		private SolidBrush levelIILevel2Brush;
		private SolidBrush levelIILevel3Brush;
		private SolidBrush levelIILevel4Brush;
		private SolidBrush levelIILevel5Brush;
		private SolidBrush levelIILevel6Brush;
		private SolidBrush levelIILevel7Brush;
		private SolidBrush levelIILevel8Brush;
		private SolidBrush levelIILevel9Brush;
		private SolidBrush levelIILevel10Brush;
		private SolidBrush askBrush;
		private SolidBrush bidBrush;
		private Pen priceLinePen;

		
		
		
		private bool showLevelIIHistogram;
		private int histogramOpacity = 100;
		private int levelIIRefreshDelayMillis;
		private bool showHighLowSameVolume;
		private bool showBidAskVolume;
		private bool drawPriceLine;
		private bool outlineHistBars;
		private int askTotY;
		private int bidTotY;
		private int lineStrength;
		private StringFormat stringFormat;
		private StringFormat stringCFormat;
		private Font stringFont;
		private Font stringBFont;
		private int tickVolHigherThanClose;
		private int tickVolLowerThanClose;
		private int tickVolEqualClose;
		private int lastBar;		
		private int lastTickVol;
		private int tickCnt;
		private DateTime lastRefresh;		
		private Brush[] brushes;
		private const int arrowWidth = 14;
		private const int arrowHight = 12;	
		private	List<LadderRow>		askRows	= new List<LadderRow>();
		private	List<LadderRow>		bidRows	= new List<LadderRow>();		

		// ------------- Stats Properties -----------------------------
		
		[Description("Show Volume/Tick statistics on chart.")]	
		[Category("Stats Settings")]
		[NinjaTrader.Gui.Design.DisplayName("1. Show")]		
		public bool ShowStats {
			get{ return showStats;}
			set{ showStats = value;}
		}
		
		[Description("Position on Price panel where statics will be displayed.")]	
		[Category("Stats Settings")]
		[NinjaTrader.Gui.Design.DisplayName("2. Display Position")]		
		public TextPosition StatsPosition {
			get{ return statsPosition;}
			set{ statsPosition = value;}
		}
		
		// ------------- Volume Properties -----------------------------
		
		
		[Description("Show Tick volume indicator.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("1. Show")]		
		public bool ShowTickVolume {
			get{ return showVolume;}
			set{ showVolume = value;}
		}		
		
		[Description("Show volume higher, lower and same as last bar close price.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("2. Show High/Low/Same ")]		
		public bool ShowHighLowSameVolume {
			get{ return showHighLowSameVolume;}
			set{ showHighLowSameVolume = value;}
		}		
		
		[Description("Show current bid/ask volume.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("3. Show Bid/Ask")]		
		public bool ShowBidAskVolume {
			get{ return showBidAskVolume;}
			set{ showBidAskVolume = value;}
		}		
		
		
		
		[Description("Color for the volume higher.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("4. Volume-Higher Color")]		
		public Color HigherVolColor {
			get{ return higherVolColor;}
			set{ higherVolColor = value;}
		}	

		[Browsable(false)]
    	public string _HigherVolColor
    	{
        		get { return SerializableColor.ToString(this.HigherVolColor); }
        		set { this.HigherVolColor = SerializableColor.FromString(value); }
    	}
		
		
		[Description("Color for volume same.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("5. Neutral-Volume Color")]		
		public Color SameVolColor {
			get{ return sameVolColor;}
			set{ sameVolColor = value;}
		}	

		[Browsable(false)]
    	public string _SameVolColor
    	{
        		get { return SerializableColor.ToString(this.SameVolColor); }
        		set { this.SameVolColor = SerializableColor.FromString(value); }
    	}

		
		[Description("Color for volume lower.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("6. Volume-Lower Color")]		
		public Color LowerVolColor {
			get{ return lowerVolColor;}
			set{ lowerVolColor = value;}
		}	
		
		[Browsable(false)]
    	public string _LowerVolColor
    	{
        		get { return SerializableColor.ToString(this.LowerVolColor); }
        		set { this.LowerVolColor = SerializableColor.FromString(value); }
    	}
		

		[Description("Color for Ask Volume.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("7. Ask Volume Color")]		
		public Color AskColor {
			get{ return askColor;}
			set{ askColor = value;}
		}	
		
		[Browsable(false)]
    	public string AskColorSerialized
    	{
        		get { return SerializableColor.ToString(this.AskColor); }
        		set { this.AskColor = SerializableColor.FromString(value); }
    	}

		[Description("Color for Bid Volume.")]	
		[Category("Volume Settings")]
		[NinjaTrader.Gui.Design.DisplayName("8. Bid Volume Color")]		
		public Color BidColor {
			get{ return bidColor;}
			set{ bidColor = value;}
		}	
		
		[Browsable(false)]
    	public string BidColorSerialized
    	{
        		get { return SerializableColor.ToString(this.BidColor); }
        		set { this.BidColor = SerializableColor.FromString(value); }
    	}

		
		// ------------- Level II Properties -----------------------------
		
		
		

		[Description("Show Level II indicator.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("01. Show")]		
		public bool ShowLevelII {
			get{ return showMarketData;}
			set{ showMarketData = value;}
		}

		[Description("Show Level II histogram.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("02. Show Histogram")]		
		public bool ShowLevelIIHistogram {
			get{ return showLevelIIHistogram;}
			set{ showLevelIIHistogram = value;}
		}

		[Description("Histogram opacity.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("03. Histogram Bar Opacity")]		
		public int HistogramOpacity {
			get{ return histogramOpacity;}
			set{ histogramOpacity = Math.Max(Math.Min(value, 255),0);}
		}

		[Description("Level II refresh delay in milliseconds. NOTE: This will be the number of milliseconds that the Level II data will lag real-time.  This may be necessary if your CPU utilization increases too much.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("04. Refresh Delay")]		
		public int LevelIIRefreshDelay {
			get{ return levelIIRefreshDelayMillis;}
			set{ levelIIRefreshDelayMillis = value;}
		}

		[Description("Outline histogram bars for visibility.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("05. Outline bars")]		
		public bool OutlineHistBars {
			get{ return outlineHistBars;}
			set{ outlineHistBars = value;}
		}
		
		
		[Description("Draw a line at the current price level. Makes it easier to see price movement when you have a larger right margin.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("06. Draw price line")]		
		public bool DrawPriceLine {
			get{ return drawPriceLine;}
			set{ drawPriceLine = value;}
		}

		[Description("Price line color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("18. Price line color")]		
		public Color PriceLineColor {
			get{ return priceLineColor;}
			set{ priceLineColor = value;}
		}	
		
		[Description("Max Levels to Display")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("19. Max Levels")]		
		public int MaxLevels {
			get{ return maxRows;}
			set{ maxRows = value;}
		}	
		
		[Browsable(false)]
    	public string _PriceLineColor
    	{
    		get { return SerializableColor.ToString(PriceLineColor); }
    		set { this.PriceLineColor = SerializableColor.FromString(value); }
    	}	
		
		[Description("Level 1 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("08. Level 1 Color")]		
		public Color LevelIILevel1Color {
			get{ return  levelIILevel1Color;}
			set{ levelIILevel1Color = value;}
		}	

		[Browsable(false)]
    	public string LevelIILevel1ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel1Color); }
    		set { this.LevelIILevel1Color = SerializableColor.FromString(value); }
    	}			
		
		[Description("Level 2 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("09. Level 2 Color")]		
		public Color LevelIILevel2Color {
			get{ return levelIILevel2Color;}
			set{ levelIILevel2Color = value;}
		}	

		[Browsable(false)]
    	public string LevelIILevel2ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel2Color); }
    		set { this.LevelIILevel2Color = SerializableColor.FromString(value); }
    	}			
	
		[Description("Level 3 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("10. Level 3 Color")]		
		public Color LevelIILevel3Color {
			get{ return levelIILevel3Color;}
			set{ levelIILevel3Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel3ColorColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel3Color); }
    		set { this.LevelIILevel3Color = SerializableColor.FromString(value); }
    	}	
		
		[Description("Level 4 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("11. Level 4 Color")]		
		public Color LevelIILevel4Color {
			get{ return levelIILevel4Color;}
			set{ levelIILevel4Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel4ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel4Color); }
    		set { this.LevelIILevel4Color = SerializableColor.FromString(value); }
    	}	
		
		[Description("Level 5 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("12. Level 5 Color")]		
		public Color LevelIILevel5Color {
			get{ return levelIILevel5Color;}
			set{ levelIILevel5Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel5ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel5Color); }
    		set { this.LevelIILevel5Color = SerializableColor.FromString(value); }
    	}			
		
		[Description("Level 6 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("13. Level 6 Color")]		
		public Color LevelIILevel6Color {
			get{ return levelIILevel6Color;}
			set{ levelIILevel6Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel6ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel6Color); }
    		set { this.LevelIILevel6Color = SerializableColor.FromString(value); }
    	}	
		
		[Description("Level 7 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("14. Level 7 Color")]		
		public Color LevelIILevel7Color {
			get{ return levelIILevel7Color;}
			set{ levelIILevel7Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel7ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel7Color); }
    		set { this.LevelIILevel7Color = SerializableColor.FromString(value); }
    	}	
		
		
		[Description("Level 8 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("15. Level 8 Color")]		
		public Color LevelIILevel8Color {
			get{ return levelIILevel8Color;}
			set{ levelIILevel8Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel8ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel8Color); }
    		set { this.LevelIILevel8Color = SerializableColor.FromString(value); }
    	}	
		
		[Description("Level 9 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("16. Level 9 Color")]		
		public Color LevelIILevel9Color {
			get{ return levelIILevel9Color;}
			set{ levelIILevel9Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel9ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel9Color); }
    		set { this.LevelIILevel9Color = SerializableColor.FromString(value); }
    	}	
		
		
		[Description("Level 10 Color.")]	
		[Category("Level II Settings")]
		[NinjaTrader.Gui.Design.DisplayName("17. Level 10 Color")]		
		public Color LevelIILevel10Color {
			get{ return levelIILevel10Color;}
			set{ levelIILevel10Color = value;}
		}	
		
		[Browsable(false)]
    	public string LevelIILevel10ColorSerialize
    	{
    		get { return SerializableColor.ToString(LevelIILevel10Color); }
    		set { this.LevelIILevel10Color = SerializableColor.FromString(value); }
    	}	
		
	

	    // ------------- Level II Properties -----------------------------

		
		[Description("Color used to outline bars when outlining is enable.")]	
		[Category("Misc")]
		[NinjaTrader.Gui.Design.DisplayName("1. Outline Color")]		
		public Color OutlineColor {
			get{ return outlineColor;}
			set{ outlineColor = value;}
		}	

		[Browsable(false)]
    	public string outlineColorSerialize
    	{
    		get { return SerializableColor.ToString(OutlineColor); }
    		set { this.OutlineColor = SerializableColor.FromString(value); }
    	}	
		
		[Description("Color used for all text.")]	
		[Category("Misc")]
		[NinjaTrader.Gui.Design.DisplayName("2. Text Color")]		
		public Color TextColor {
			get{ return textColor;}
			set{ textColor = value;}
		}	

		[Browsable(false)]
    	public string textColorSerialize
    	{
    		get { return SerializableColor.ToString(TextColor); }
    		set { this.TextColor = SerializableColor.FromString(value); }
    	}	

		
		
		
        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
			higherVolBrush = new SolidBrush(higherVolColor);
			lowerVolBrush = new SolidBrush(lowerVolColor);
			sameVolBrush = new SolidBrush(sameVolColor);
			outlinePen = new Pen(outlineColor);
			textBrush = new SolidBrush(textColor);
			levelIILevel1Brush = new SolidBrush(LevelIILevel1Color);
			levelIILevel2Brush = new SolidBrush(LevelIILevel2Color);
			levelIILevel3Brush = new SolidBrush(LevelIILevel3Color);
			levelIILevel4Brush = new SolidBrush(LevelIILevel4Color);
			levelIILevel5Brush = new SolidBrush(LevelIILevel5Color);
			levelIILevel6Brush = new SolidBrush(LevelIILevel6Color);
			levelIILevel7Brush = new SolidBrush(LevelIILevel7Color);
			levelIILevel8Brush = new SolidBrush(LevelIILevel8Color);
			levelIILevel9Brush = new SolidBrush(LevelIILevel9Color);
			levelIILevel10Brush = new SolidBrush(LevelIILevel10Color);
			askBrush = new SolidBrush(AskColor);
			bidBrush = new SolidBrush(BidColor);
			priceLinePen = new Pen(PriceLineColor);			
			
			PlotsConfigurable 	= false;
			CalculateOnBarClose = false;		
			DrawOnPricePanel = true;
			
            Overlay				= true;
			PaintPriceMarkers 	= false;
            PriceTypeSupported	= false;
			
			stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Far;
			stringCFormat = new StringFormat();
			stringCFormat.Alignment = StringAlignment.Center;
			stringFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular, GraphicsUnit.Pixel);
			stringBFont = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold, GraphicsUnit.Pixel);
		
			lastBar = -1;
			
			showStats = true;
			statsPosition = TextPosition.TopLeft;
			showMarketData = true;
			showVolume = true;
			levelIIRefreshDelayMillis = 1000;
			showLevelIIHistogram = true;
			drawPriceLine = true;
			outlineHistBars = false;
			
			brushes = new Brush[] {
    		new SolidBrush(Color.FromArgb(HistogramOpacity, levelIILevel1Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel2Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel3Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel4Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel5Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel6Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel7Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel8Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel9Color)),
    		new SolidBrush(Color.FromArgb(HistogramOpacity,levelIILevel10Color))
			};
			
			priceLinePen.DashStyle = DashStyle.Dot;

			showHighLowSameVolume = true;
			showBidAskVolume = true;
			
			maxRows = 10;

			
		}
		
		protected override void OnBarUpdate()
        {
				

			try {
				// -----  accumulate the volume by tick for this bar.
				if (!Historical && CurrentBar > 0)
				{
					if (lastBar == CurrentBar){
						UpdateTickVolume();
						tickCnt++;
					} else {
						// new bar, reset counters.
						lastBar = CurrentBar;
						tickVolHigherThanClose = 0;
						tickVolLowerThanClose = 0;
						tickVolEqualClose = 0;
						lastTickVol = 0;
						tickCnt = 1;
						UpdateTickVolume();
					}

					int ticksRemaining = 0;
					if (Bars.Period.Id == PeriodType.Tick){
						ticksRemaining = Bars.Period.Value - Bars.TickCount;
					}
					
					if (ShowStats){
						string text = string.Format("Current Bar Stats...\r\n\r\n     Tick Count:{0}   Remaining: {1}   %Complete: {2}\r\n     Volume: {3}   Higher: {4}   Same: {5}   Lower: {6}\r\n     Ask: {7}   Bid: {8}",Bars.TickCount,ticksRemaining , (int)(Bars.PercentComplete*100), Volume[0],tickVolHigherThanClose, tickVolEqualClose, tickVolLowerThanClose, GetCurrentAskVolume(), GetCurrentBidVolume() );
						DrawTextFixed("jtRealStats", text,  StatsPosition); 	
					}
				}
			} catch (Exception ex){
				Print("Unhandled exception in jtRealStats.OnBarUpdate(): " + ex.Message);
			}
        }
		
		/*
		*	Accumulate the volume for each tick by vol higher, lower or same as prior bar close price
		*/
		private void UpdateTickVolume(){
			int  vol =  (int)Volume[0]-lastTickVol;
			if (Input[0] > Close[1]){
				tickVolHigherThanClose += vol;
			} else 	if (Input[0] < Close[1]) {
				tickVolLowerThanClose += vol;
			} else {
				tickVolEqualClose += vol;
			}
			lastTickVol = (int)Volume[0];	
		}
		
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max){
	
			try {
				
					int priceY = (bounds.Y + bounds.Height) - ((int) ((( Close[0] - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height))-1;
					
					
					if (ShowTickVolume){
						
						int X = bounds.X+bounds.Width-13;
				
						if (showHighLowSameVolume){
							// Draw the high/low/same volume bars
		
							Rectangle rect = new Rectangle(X, bounds.Y, 13, bounds.Height);
							graphics.FillRectangle(Brushes.White, rect);
	
							double heightPerTick = bounds.Height / Volume[0];
							
							if (heightPerTick > 0){
								
								int volHigherHeight = (int)(heightPerTick * tickVolHigherThanClose);
								int volLowerHeight = (int)(heightPerTick * tickVolLowerThanClose);
								int volSameHeight = (int)(heightPerTick * tickVolEqualClose);
								int offset = (bounds.Height - volLowerHeight - volHigherHeight - volSameHeight)/2;
								
								Rectangle higherRect = new Rectangle(X, offset, 11, volHigherHeight);
								Rectangle sameRect = new Rectangle(X, offset+volHigherHeight, 11, volSameHeight);
								Rectangle lowerRect = new Rectangle(X, offset + volHigherHeight + volSameHeight, 11, volLowerHeight);
			
								graphics.FillRectangle(higherVolBrush, higherRect);
								graphics.FillRectangle(sameVolBrush, sameRect);
								graphics.FillRectangle(lowerVolBrush, lowerRect);
//!!								
								graphics.DrawString(tickVolHigherThanClose.ToString(), stringBFont, textBrush, bounds.X-12 + bounds.Width , bounds.Y, stringFormat);
								graphics.DrawString(tickVolLowerThanClose.ToString(), stringBFont, textBrush, bounds.X-12 + bounds.Width , bounds.Y+bounds.Height-12, stringFormat);
							}
						}
						
						if (showBidAskVolume){
							// draw the bid/ask volume bars
							int askVol = (int) GetCurrentAskVolume();
							int bidVol = (int) GetCurrentBidVolume();
		
							graphics.DrawRectangle(outlinePen, X+3,priceY-askVol, 5, askVol); 
							graphics.DrawRectangle(outlinePen, X+3, priceY, 5, bidVol); 
							graphics.FillRectangle(askBrush, X+4,priceY-askVol+1, 4, askVol-1); 
							graphics.FillRectangle(bidBrush, X+4, priceY+1, 4, bidVol-1); 
						}
					}
	
					// Draw the level II histogram 
					
					int totAVol = 0;
					int totBVol = 0;
				
					if (ShowLevelII){
						int x1 = base.ChartControl.CanvasRight;

						if (ShowTickVolume){
							x1 -= 14;
						}
						int idx = 0;
						int priceOffset = 7;
						int barWidthScaler = 1;
						
						// find longest bar 					
						int largestVol = 0;
						for (int x = 0; x < askRows.Count && x < bidRows.Count; x++){
							if (largestVol < askRows[x].Volume) largestVol  = (int) askRows[x].Volume;
							if (largestVol < bidRows[x].Volume) largestVol  = (int) bidRows[x].Volume;
						}
						
						// scale to less that 1/4 of chart width
						if ((bounds.Width/4) < largestVol){
							barWidthScaler = largestVol / (bounds.Width/4); 
						} else {
							barWidthScaler = 1;
						}
						
						
						
						for (idx = 0; idx < askRows.Count && idx < maxRows; idx++){
							int y1 = (bounds.Y + bounds.Height) - ((int) ((( askRows[idx].Price - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height)) - priceOffset;
							if (ShowLevelIIHistogram) {
								int barWidth = (int) askRows[idx].Volume/barWidthScaler > 0 ? (int) askRows[idx].Volume/barWidthScaler : 1;
								Rectangle rect = new Rectangle(x1-25- (int) askRows[idx].Volume/barWidthScaler, y1+3, barWidth, 6);
								graphics.FillRectangle(GetLadderBrush(idx), rect);
	
								if (outlineHistBars){
									graphics.DrawRectangle(outlinePen, rect);
								}
								
							}
							graphics.DrawString(askRows[idx].Volume.ToString(), stringFont, textBrush, x1, y1+2, stringFormat);
							totAVol += (int) askRows[idx].Volume;
						}
						
						askTotY = 0;
						
						if (idx > 1){
							askTotY = (bounds.Y + bounds.Height) - ((int) ((( askRows[idx-1].Price - min + TickSize*2) / ChartControl.MaxMinusMin(max, min)) * bounds.Height)) - priceOffset;
							graphics.DrawString(totAVol.ToString(), stringBFont, textBrush, x1, askTotY, stringFormat);
						}
						
						for (idx = 0; idx < bidRows.Count && idx < maxRows; idx++){
							int y1 = (bounds.Y + bounds.Height) - ((int) ((( bidRows[idx].Price - min) / ChartControl.MaxMinusMin(max, min)) * bounds.Height)) - priceOffset;
							if (ShowLevelIIHistogram) {
								int barWidth = (int) bidRows[idx].Volume/barWidthScaler > 0 ? (int) bidRows[idx].Volume/barWidthScaler : 1;
								Rectangle rect = new Rectangle(x1-25- (int) bidRows[idx].Volume/barWidthScaler, y1+3, barWidth, 6);
								graphics.FillRectangle(GetLadderBrush(idx), rect);

								if (outlineHistBars){
									graphics.DrawRectangle(outlinePen, rect);
								}
							}
							graphics.DrawString(bidRows[idx].Volume.ToString(), stringFont, textBrush, x1, y1+2, stringFormat);
							totBVol += (int) bidRows[idx].Volume;
						}
						
						bidTotY = 0;
						
						if (idx > 1){
							bidTotY = (bounds.Y + bounds.Height) - ((int) ((( bidRows[idx-1].Price - min - TickSize*2) / ChartControl.MaxMinusMin(max, min)) * bounds.Height)) - priceOffset;
							graphics.DrawString(totBVol.ToString(), stringBFont, textBrush, x1, bidTotY, stringFormat);
						}
						
						
						// draw arrows to indicate price trend based on supply/demand and up/down volume
						
						{ 
							SmoothingMode oldMode = graphics.SmoothingMode;
							try {
								graphics.SmoothingMode = SmoothingMode.AntiAlias;
								int x = x1 - arrowWidth-2;
			
								if (totAVol > totBVol){
									int y = askTotY -3;
									Point[] lines = new Point[]{new Point(x, y), new Point(x + arrowWidth/2 , y - arrowHight), new Point(x + arrowWidth, y),new Point(x, y)};
									if (tickVolHigherThanClose > tickVolLowerThanClose) {
										graphics.FillPolygon(higherVolBrush, lines);
										graphics.DrawPolygon(outlinePen, lines);
									} else {
										graphics.DrawPolygon(outlinePen, lines);
									}
				
									graphics.DrawString(string.Format("{0:F2}", (double)totAVol/(double)totBVol), stringBFont, textBrush, x+4, y-arrowHight-15,stringCFormat);
								} else if (totAVol < totBVol){
									int y = bidTotY + 15;
									Point[] lines = new Point[]{new Point(x, y), new Point(x + arrowWidth/2 , y + arrowHight), new Point(x + arrowWidth, y),new Point(x, y)};
									if (tickVolHigherThanClose < tickVolLowerThanClose) {
										graphics.FillPolygon(lowerVolBrush, lines);
										graphics.DrawPolygon(outlinePen, lines);
									} else {
										graphics.DrawPolygon(outlinePen, lines);
									}
									graphics.DrawString(string.Format("{0:F2}", (double)totBVol/(double)totAVol), stringBFont, textBrush, x+4, y+arrowHight+5,stringCFormat);
								}
							} finally {
								graphics.SmoothingMode = oldMode;
							}
						}
						
						//  draw a price line

						if (drawPriceLine){
							graphics.DrawLine(priceLinePen, bounds.X, priceY, bounds.X+bounds.Width, priceY);
						}

					}
				
				} catch (Exception ex){
					Print("Unhandled exception in jtRealStats.Plot(): " + ex.Message);
				}
		}
		
		private Brush GetLadderBrush(int idx){
			Brush brush = null;
			if (idx < brushes.Length){
				brush = brushes[idx];
			} else {
				brush = textBrush;
			}
			return brush;
		}
		
		protected override void OnMarketDepth(MarketDepthEventArgs e)
        {
			if (ShowLevelII){
				List<LadderRow> rows = null;
	
				// Checks to see if the Market Data is of the Ask type
				if (e.MarketDataType == MarketDataType.Ask)
					rows = askRows;
				
				// Checks to see if the Market Data is of the Bid type
				else if (e.MarketDataType == MarketDataType.Bid)
					rows = bidRows;
	
				if (rows == null)
					return;
	
				// Checks to see if the action taken by the Ask data was an insertion into the ladder
				if (e.Operation == Operation.Insert)
					rows.Insert(e.Position, new LadderRow(e.Price, e.Volume, e.MarketMaker));
				
				/* Checks to see if the action taken by the Ask data was a removal of itself from the ladder
				Note: Due to the multi threaded architecture of the NT core, race conditions could occur
				-> check if e.Position is within valid range */
				else if (e.Operation == Operation.Remove && e.Position < rows.Count)
					rows.RemoveAt(e.Position);
				
				/* Checks to see if the action taken by the Ask data was to update a data already on the ladder
				Note: Due to the multi threaded architecture of the NT core, race conditions could occur
				-> check if e.Position is within valid range */
				else if (e.Operation == Operation.Update && e.Position < rows.Count)
				{
					rows[e.Position].MarketMaker	= e.MarketMaker;
					rows[e.Position].Price			= e.Price;
					rows[e.Position].Volume			= e.Volume;
				}
				
				// Calling ChartControl.Refresh() will cause the Level II data to update real-time.  Adding
				// this delay routine will prevent CPU utilization from going through the roof.  Setting a 
				// really high value will effectly delay the update until the next tick arrives which could
				// be a really long time is the market is really slow.  This may or may not matter to some.
				try {
				if (lastRefresh.AddMilliseconds(LevelIIRefreshDelay) < DateTime.Now){
					ChartControl.Refresh();
					lastRefresh = DateTime.Now;
				}
				} catch {}
			}
        }
		
		private class LadderRow
		{
			public	string	MarketMaker;			// relevant for stocks only
			public	double	Price;
			public	long	Volume;

			public LadderRow(double myPrice, long myVolume, string myMarketMaker)
			{
				MarketMaker	= myMarketMaker;
				Price		= myPrice;
				Volume		= myVolume;
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private jtRealStats[] cachejtRealStats = null;

        private static jtRealStats checkjtRealStats = new jtRealStats();

        /// <summary>
        /// jtRealtime Stats version 1.5.3
        /// </summary>
        /// <returns></returns>
        public jtRealStats jtRealStats()
        {
            return jtRealStats(Input);
        }

        /// <summary>
        /// jtRealtime Stats version 1.5.3
        /// </summary>
        /// <returns></returns>
        public jtRealStats jtRealStats(Data.IDataSeries input)
        {
            if (cachejtRealStats != null)
                for (int idx = 0; idx < cachejtRealStats.Length; idx++)
                    if (cachejtRealStats[idx].EqualsInput(input))
                        return cachejtRealStats[idx];

            lock (checkjtRealStats)
            {
                if (cachejtRealStats != null)
                    for (int idx = 0; idx < cachejtRealStats.Length; idx++)
                        if (cachejtRealStats[idx].EqualsInput(input))
                            return cachejtRealStats[idx];

                jtRealStats indicator = new jtRealStats();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                jtRealStats[] tmp = new jtRealStats[cachejtRealStats == null ? 1 : cachejtRealStats.Length + 1];
                if (cachejtRealStats != null)
                    cachejtRealStats.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cachejtRealStats = tmp;
                return indicator;
            }
        }
    }
}

// This namespace holds all market analyzer column definitions and is required. Do not change it.
namespace NinjaTrader.MarketAnalyzer
{
    public partial class Column : ColumnBase
    {
        /// <summary>
        /// jtRealtime Stats version 1.5.3
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jtRealStats jtRealStats()
        {
            return _indicator.jtRealStats(Input);
        }

        /// <summary>
        /// jtRealtime Stats version 1.5.3
        /// </summary>
        /// <returns></returns>
        public Indicator.jtRealStats jtRealStats(Data.IDataSeries input)
        {
            return _indicator.jtRealStats(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// jtRealtime Stats version 1.5.3
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.jtRealStats jtRealStats()
        {
            return _indicator.jtRealStats(Input);
        }

        /// <summary>
        /// jtRealtime Stats version 1.5.3
        /// </summary>
        /// <returns></returns>
        public Indicator.jtRealStats jtRealStats(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.jtRealStats(input);
        }
    }
}
#endregion
