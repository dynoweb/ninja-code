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
#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// TripleMA written by ThatManFromTexas 31 January 2011 .
    /// </summary>
    [Description("TripleMA written by ThatManFromTexas.")]
    public class TripleMA : Indicator
   	
	
	{
       ///Variables/////////////////////////////////////////////////////////////////////////////////////
		
		private int mAPeriod1 		= 13;
        private int mAPeriod2 		= 21;
		private int mAPeriod3 		= 34; 
		private DMAType mAType1 	= DMAType.LLMA;
		private DMAType mAType2 	= DMAType.LLMA;
		private DMAType mAType3 	= DMAType.LLMA;
       	private bool 	drawdots	=false;
		private bool 	drawline	=true;
    	
		private DataSeries ma1Series;
		private DataSeries ma2Series;
		private DataSeries ma3Series;
		private DataSeries signal;
		
		private Color upColorMA1		= Color.Lime;
		private Color downColorMA1  	= Color.Red;
				
		private Color upColorMA2		= Color.ForestGreen;
		private Color downColorMA2		= Color.Crimson;
				
		private Color upColorMA3		= Color.Green;
		private Color downColorMA3  	= Color.DarkRed;
		private Color flatColorMA3		= Color.Gold;
		
		private Color upColor		= Color.Lime;
		private Color downColor		= Color.Red;
		private Color flatColor		= Color.Yellow;
		
		// Back Color Change
			private bool 		colorbackground = false;
			private bool 		colorbackgroundall	=false;
			private int 		opacity		= 50;
			private Color 		bgColorUp = Color.RoyalBlue;
			private Color 		bgColorDown = Color.DeepPink;
			private int			zopacity	= 1;
			private bool		colorzone	= true;
		
		// Sound Alert
			private bool soundAlert			= true;
			private string upAlertFile		= "Alert3.wav";
			private string downAlertFile	= "Alert4.wav";
		
		public enum DMAType			// A list of the valid types of moving averages that may be selected.
			{
				EMA = 1,					// Exponential Moving Average.
				HMA = 2,					// Hull Moving Average.
				WMA = 3,					// Weighted Moving Average.
				SMA = 4,					// Simple Moving Average.
				MAX = 5,					// Weighted Moving Average.
				MIN = 6,					// Weighted Moving Average.
				Wilder = 7,
				EhlersFilter = 8,
				ADXVMA = 9,
				ZeroLagEMA = 10,
				TMA = 11,
				VWMA = 12,
				SMMA = 13,
				LLMA = 14,
				None = 15,
			}
		
		
        protected override void Initialize()///////////////////////////////////////////////////////////
        {	
			Add(new Plot(Color.Gray, PlotStyle.Dot, "Dot"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "Line"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "Fast"));
            Add(new Plot(Color.Gray, PlotStyle.Line, "Slow"));
			Add(new Plot(Color.Gray, PlotStyle.Line, "Trend"));
			
			Plots[0].Pen.Width = 2;
			Plots[1].Pen.Width = 2;
			Plots[2].Pen.Width = 2;
			Plots[3].Pen.Width = 2;
			Plots[3].Pen.Width = 2;
			Plots[4].Pen.Width = 2;
			
			Plots[2].Pen.DashStyle = DashStyle.Dash;
			Plots[3].Pen.DashStyle = DashStyle.Dash;
			Plots[3].Pen.DashStyle = DashStyle.Dash;
			Plots[4].Pen.DashStyle = DashStyle.Dash;
			
			CalculateOnBarClose	= false;
            Overlay				= true;
            PriceTypeSupported	= true;
			PaintPriceMarkers = false;
			DisplayInDataBox = false;
			DrawOnPricePanel = true;
			
			ma1Series			= new DataSeries(this);
			ma2Series 			= new DataSeries(this);
			ma3Series			= new DataSeries(this);
			signal				= new DataSeries(this);
		}
        protected override void OnBarUpdate()/////////////////////////////////////////////////////////
        {	
			switch(mAType1)
			{
				case DMAType.EMA:
					ma1Series.Set(EMA(Input,mAPeriod1)[0]);
					break;					
				case DMAType.HMA:
					ma1Series.Set(HMA(Input,mAPeriod1)[0]);
					break;					
				case DMAType.WMA:
					ma1Series.Set(WMA(Input,mAPeriod1)[0]);
					break;					
				case DMAType.SMA:
					ma1Series.Set(SMA(Input,mAPeriod1)[0]);
					break;					
				case DMAType.MIN:
					ma1Series.Set(MIN(Input,mAPeriod1)[0]);
					break;					
				case DMAType.MAX:
					ma1Series.Set(MAX(Input,mAPeriod1)[0]);
					break;					
				case DMAType.Wilder:
					ma1Series.Set(Wilder(Input,mAPeriod1)[0]);
					break;					
				case DMAType.EhlersFilter:
					ma1Series.Set(EhlersFilter(Input,mAPeriod1)[0]);
					break;					
				case DMAType.ADXVMA:
					ma1Series.Set(ADXVMA(Input,mAPeriod1)[0]);
					break;					
				case DMAType.ZeroLagEMA:
					ma1Series.Set(ZeroLagEMA(Input,mAPeriod1)[0]);
					break;
				case DMAType.TMA:
					ma1Series.Set(TMA(Input,mAPeriod1)[0]);
					break;
				case DMAType.VWMA:
					ma1Series.Set(VWMA(Input,mAPeriod1)[0]);
					break;
				case DMAType.SMMA:
					ma1Series.Set(SMMA(Input,mAPeriod1)[0]);
					break;
				case DMAType.LLMA:
					ma1Series.Set(LLMA(Input,mAPeriod1,0)[0]);
					break;	
				case DMAType.None:
					ma1Series.Set(Input[0]);
					break;		
			}
			
			switch(mAType2)
			{
				case DMAType.EMA:
					ma2Series.Set(EMA(Input,mAPeriod2)[0]);
					break;					
				case DMAType.HMA:
					ma2Series.Set(HMA(Input,mAPeriod2)[0]);
					break;					
				case DMAType.WMA:
					ma2Series.Set(WMA(Input,mAPeriod2)[0]);
					break;					
				case DMAType.SMA:
					ma2Series.Set(SMA(Input,mAPeriod2)[0]);
					break;					
				case DMAType.MIN:
					ma2Series.Set(MIN(Input,mAPeriod2)[0]);
					break;					
				case DMAType.MAX:
					ma2Series.Set(MAX(Input,mAPeriod2)[0]);
					break;					
				case DMAType.Wilder:
					ma2Series.Set(Wilder(Input,mAPeriod2)[0]);
					break;					
				case DMAType.EhlersFilter:
					ma2Series.Set(EhlersFilter(Input,mAPeriod2)[0]);
					break;					
				case DMAType.ADXVMA:
					ma2Series.Set(ADXVMA(Input,mAPeriod2)[0]);
					break;					
				case DMAType.ZeroLagEMA:
					ma2Series.Set(ZeroLagEMA(Input,mAPeriod2)[0]);
					break;
				case DMAType.TMA:
					ma2Series.Set(TMA(Input,mAPeriod2)[0]);
					break;
				case DMAType.VWMA:
					ma2Series.Set(VWMA(Input,mAPeriod2)[0]);
					break;
				case DMAType.SMMA:
					ma2Series.Set(SMMA(Input,mAPeriod2)[0]);
					break;
				case DMAType.LLMA:
					ma2Series.Set(LLMA(Input,mAPeriod2,0)[0]);
					break;	
				case DMAType.None:
					ma2Series.Set(Input[0]);
					break;		
			}
			
			
			
			switch(mAType3)
			{
				case DMAType.EMA:
					ma3Series.Set(EMA(Input,mAPeriod3)[0]);
					break;					
				case DMAType.HMA:
					ma3Series.Set(HMA(Input,mAPeriod3)[0]);
					break;					
				case DMAType.WMA:
					ma3Series.Set(WMA(Input,mAPeriod3)[0]);
					break;					
				case DMAType.SMA:
					ma3Series.Set(SMA(Input,mAPeriod3)[0]);
					break;					
				case DMAType.MIN:
					ma3Series.Set(MIN(Input,mAPeriod3)[0]);
					break;					
				case DMAType.MAX:
					ma3Series.Set(MAX(Input,mAPeriod3)[0]);
					break;					
				case DMAType.Wilder:
					ma3Series.Set(Wilder(Input,mAPeriod3)[0]);
					break;					
				case DMAType.EhlersFilter:
					ma3Series.Set(EhlersFilter(Input,mAPeriod3)[0]);
					break;					
				case DMAType.ADXVMA:
					ma3Series.Set(ADXVMA(Input,mAPeriod3)[0]);
					break;					
				case DMAType.ZeroLagEMA:
					ma3Series.Set(ZeroLagEMA(Input,mAPeriod3)[0]);
					break;
				case DMAType.TMA:
					ma3Series.Set(TMA(Input,mAPeriod3)[0]);
					break;
				case DMAType.VWMA:
					ma3Series.Set(VWMA(Input,mAPeriod3)[0]);
					break;
				case DMAType.SMMA:
					ma3Series.Set(SMMA(Input,mAPeriod3)[0]);
					break;
				case DMAType.LLMA:
					ma3Series.Set(LLMA(Input,mAPeriod3,0)[0]);
					break;	
				case DMAType.None:
					ma3Series.Set(Input[0]);
					break;		
			}
			
			
			
			if (Drawline)
			{
					BarColor = Color.Transparent;
					CandleOutlineColor = Color.Transparent;	
			}
			
			
			
			Fast.Set(ma1Series[0]);
         	Slow.Set(ma2Series[0]);
			Trend.Set(ma3Series[0]);
			
			if(Drawdots)
			{
			Dot.Set(Close[0]);
           
			if(Input[0]> Fast[0] && Input[0]>Slow[0]&& Input[0]>Trend[0])
			PlotColors[0][0] = UpColor;
			
			else if(Input[0]< Fast[0] && Input[0]<Slow[0]&& Input[0]<Trend[0])
			PlotColors[0][0] = DownColor;
			
			else PlotColors[0][0] = FlatColor;
			}
			
			if(Drawdots==false)
			{
			Dot.Set(Close[0]);
           
			if(Close[0]>Open[0])
			PlotColors[0][0] = UpColor;
			
			else if(Close[0]<Open[0]) 
			PlotColors[0][0] = DownColor;
			
			else  
			PlotColors[0][0] = FlatColor;
			}
			
			if(Drawline)
			{
			
           
			if(Input[0]> Fast[0] && Input[0]>Slow[0]&& Input[0]>Trend[0])
			{
			Line.Set(Close[0]);
			PlotColors[1][0] = UpColor;
			}
			
			else if(Input[0]< Fast[0] && Input[0]<Slow[0]&& Input[0]<Trend[0])
			{
			Line.Set(Close[0]);
			PlotColors[1][0] = DownColor;
			}
			
			else 
			{
				Line.Set(Close[0]);
				PlotColors[1][0] = FlatColor;
			}
			
			
			}
			
			if (colorbackground)
				if (colorbackgroundall)
			
			
			{
			if(Input[0]> Fast[0] && Input[0]>Slow[0]&& Input[0]>Trend[0])
				BackColorAll = Color.FromArgb(Opacity,BgColorUp);
			
			else if(Input[0]< Fast[0] && Input[0]<Slow[0]&& Input[0]<Trend[0])
			BackColorAll = Color.FromArgb(Opacity,BgColorDown);
			
			else BackColorAll=Color.Empty;
			}
			
			else
				{
			if(Input[0]> Fast[0] && Input[0]>Slow[0]&& Input[0]>Trend[0])
				BackColor = Color.FromArgb(Opacity,BgColorUp);
			
			else if(Input[0]< Fast[0] && Input[0]<Slow[0]&& Input[0]<Trend[0])
			BackColor = Color.FromArgb(Opacity,BgColorDown);
			
			else BackColor=Color.Empty;
			}
			
							
				if (Values[2][0]>Values[3][0])
				{
					PlotColors[2][0] = UpColorMA1;
					PlotColors[3][0] = UpColorMA2;
				}
				else 
				{
					PlotColors[2][0] = DownColorMA1;
					PlotColors[3][0] = DownColorMA2;
				}
				
				
				if (Values[2][0]>Values[4][0] && Values[3][0]>Values[4][0])
				{
					PlotColors[4][0] = UpColorMA3;
				}
				
				else if (Values[2][0]<Values[4][0] && Values[3][0]<Values[4][0])
				{
					PlotColors[4][0] = DownColorMA3;
				}
				
				else 
				{
					PlotColors[4][0] = FlatColorMA3;
					
				}
				
				if (soundAlert)
					{
					if (CrossAbove(Fast, Slow,1))
					Alert("UpAlert",NinjaTrader.Cbi.Priority.Medium,"UpAlert",upAlertFile,60,Color.Navy,Color.Magenta);
				
				
					 if (CrossBelow(Fast, Slow,1))
					Alert("DownAlert",NinjaTrader.Cbi.Priority.Medium,"DownAlert",downAlertFile,60,Color.Navy,Color.Yellow);
				
					}
					
				
				if (CurrentBar<2) return;
				
				if(Colorzone)
				{
				if (Fast[0]>Slow[0])
				{
					
					DrawRegion("MABands"+CurrentBar,1, 0, ma1Series, ma2Series, Color.Transparent, UpColor, Zopacity);
				}
				else 
				{
					
					DrawRegion("MABands"+CurrentBar,1, 0, ma1Series, ma2Series, Color.Transparent, DownColor, Zopacity);
				}
				
				
				}		
												
        }

        #region Properties//////////////////////////////////////////////////////////////////////////////
		
		
       [Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Dot
        {
            get { return Values[0]; }
        }

		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Line
        {
            get { return Values[1]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Fast
        {
            get { return Values[2]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Slow
        {
            get { return Values[3]; }
        }
		
		[Browsable(false)]	// this line prevents the data series from being displayed in the indicator properties dialog, do not remove
        [XmlIgnore()]		// this line ensures that the indicator can be saved/recovered as part of a chart template, do not remove
        public DataSeries Trend
        {
            get { return Values[4]; }
        }
			
     	//Coloring
		
		
		[Description("Background Opacity")]
		[GridCategory("Coloring")]
		[Gui.Design.DisplayNameAttribute("03. Background Opacity")]
		public int Opacity
		{
			get { return opacity; }
			set { opacity = value; }
		}
		
		[Description("Zone Opacity")]
		[GridCategory("Coloring")]
		[Gui.Design.DisplayNameAttribute("06. Zone Opacity")]
		public int Zopacity
		{
			get { return zopacity; }
			set { zopacity = value; }
		}
				
		[Description("Color Background?")]  
		[GridCategory("Coloring")]
		[Gui.Design.DisplayNameAttribute("01. Color Background?")]
		public bool ColorBackground
		{
			get { return colorbackground; }
			set { colorbackground = value; }
		}
		
		[Description("Color Zone?")]  
		[GridCategory("Coloring")]
		[Gui.Design.DisplayNameAttribute("07. Color Zone?")]
		public bool Colorzone
		{
			get { return colorzone; }
			set { colorzone = value; }
		}
		
		[Description("Color Background in ALL Panels?")]  
		[GridCategory("Coloring")]
		[Gui.Design.DisplayNameAttribute("02. Color All Panels?")]
		public bool ColorBackgroundAll
		{
			get { return colorbackgroundall; }
			set { colorbackgroundall = value; }
		}
		
		[Description("Draw Dots?")]
		[GridCategory("Coloring")]
		[Gui.Design.DisplayNameAttribute("04. Draw Dots?")]
		public bool Drawdots
		{
			get { return drawdots; }
			set { drawdots = value; }
		}
		
		[Description("Draw Line?")]
		[GridCategory("Coloring")]
		[Gui.Design.DisplayNameAttribute("05. Draw Line?")]
		public bool Drawline
		{
			get { return drawline; }
			set { drawline = value; }
		}
		
       
     	// Colors
		
		[XmlIgnore()]
        [Description("Up Color for Back Ground")]
        [GridCategory("Colors")]
        [Gui.Design.DisplayNameAttribute("04. Up BG color")]
        public Color BgColorUp
        {
            get { return bgColorUp; }
            set { bgColorUp = value; }
        }
		
        [Browsable(false)]
        public string bgColorUpSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bgColorUp); }
            set { bgColorUp = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
        [XmlIgnore()]
        [Description(" Down Color for Back Ground")]
        [GridCategory("Colors")]
        [Gui.Design.DisplayNameAttribute("05. Down BG color")]
        public Color BgColorDown
        {
            get { return bgColorDown; }
            set { bgColorDown = value; }
        }
		
        [Browsable(false)]
        public string bgColorDownSerialize
        {
            get { return NinjaTrader.Gui.Design.SerializableColor.ToString(bgColorDown); }
            set { bgColorDown = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
        }
		
		[XmlIgnore()]
        [Description("Color for Up Trend")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("01. Color for Up Trend?")]
        public Color UpColor
        {
            get { return upColor; }
            set { upColor = value; }
        }
		
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(UpColor); }
			set { UpColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Down Trend")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("02.  Color for Down Trend?")]
        public Color DownColor
        {
            get { return downColor; }
            set { downColor = value; }
        }
		
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(DownColor); }
			set { DownColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Flat Trend")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("03.  Color for Flat Trend?")]
        public Color FlatColor
        {
            get { return flatColor; }
            set { flatColor = value; }
        }
		
		[Browsable(false)]
		public string FlatColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(FlatColor); }
			set { FlatColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Up Trend MA1")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("06. Color for Up MA1?")]
        public Color UpColorMA1
        {
            get { return upColorMA1; }
            set { upColorMA1 = value; }
        }
		
		[Browsable(false)]
		public string UpColorMA1Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(UpColorMA1); }
			set { UpColorMA1 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Up Trend MA2")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("08. Color for Up MA2?")]
        public Color UpColorMA2
        {
            get { return upColorMA2; }
            set { upColorMA2 = value; }
        }
		
		[Browsable(false)]
		public string UpColorMA2Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(UpColorMA2); }
			set { UpColorMA2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Up Trend MA3")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("10. Color for Up MA3?")]
        public Color UpColorMA3
        {
            get { return upColorMA3; }
            set { upColorMA3 = value; }
        }
		
		[Browsable(false)]
		public string UpColorMA3Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(UpColorMA3); }
			set { UpColorMA3 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Down Trend MA1")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("07.  Color for Down MA1?")]
        public Color DownColorMA1
        {
            get { return downColorMA1; }
            set { downColorMA1 = value; }
        }
		
		[Browsable(false)]
		public string DownColorMA1Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(DownColorMA1); }
			set { DownColorMA1 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Down Trend MA2")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("09.  Color for Down MA2?")]
        public Color DownColorMA2
        {
            get { return downColorMA2; }
            set { downColorMA2 = value; }
        }
		
		[Browsable(false)]
		public string DownColorMA2Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(DownColorMA2); }
			set { DownColorMA2 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Down Trend MA3")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("11.  Color for Down MA3?")]
        public Color DownColorMA3
        {
            get { return downColorMA3; }
            set { downColorMA3 = value; }
        }
		
		[Browsable(false)]
		public string DownColorMA3Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(DownColorMA3); }
			set { DownColorMA3 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[XmlIgnore()]
        [Description("Color for Flat Trend MA3")]
        [GridCategory("Colors")]
		[Gui.Design.DisplayNameAttribute("12.  Color for Flat MA3?")]
        public Color FlatColorMA3
        {
            get { return flatColorMA3; }
            set { flatColorMA3 = value; }
        }
		
		[Browsable(false)]
		public string FlatColorMA3Serialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(FlatColorMA3); }
			set { FlatColorMA3 = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		 [Description("Moving Average Type 1")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("01. Moving Average Type 1 ")]
        public  NinjaTrader.Indicator.TripleMA.DMAType MA1Type
        {
            get { return mAType1; }
            set { mAType1 = value; }
        }
		
        [Description("MAPeriod1")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("02. MAPeriod1 ")]
        public int MAPeriod1
        {
            get { return mAPeriod1; }
            set { mAPeriod1 = Math.Max(1, value); }
        }

        [Description("Moving Average Type 2")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("03. Moving Average Type 2 ")]
        public  NinjaTrader.Indicator.TripleMA.DMAType MA2Type
        {
            get { return mAType2; }
            set { mAType2 = value; }
        }
		
		
		[Description("")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("04. MAPeriod2 ")]
        public int MAPeriod2
        {
            get { return mAPeriod2; }
            set { mAPeriod2 = Math.Max(1, value); }
        }
		
		[Description("Moving Average Type 3")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("05. Moving Average Type 3 ")]
        public  NinjaTrader.Indicator.TripleMA.DMAType MA3Type
        {
            get { return mAType3; }
            set { mAType3 = value; }
        }
		
		
		[Description("")]
        [GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("06. MAPeriod3 ")]
        public int MAPeriod3
        {
            get { return mAPeriod3; }
            set { mAPeriod3 = Math.Max(1, value); }
        }
		
		[Description("Sound alerts activated")]
        [Category("Sound Alerts")]
		[Gui.Design.DisplayName("Activated Sounds")]
        public bool SoundAlert
        {
            get { return soundAlert; }
            set { soundAlert = value; }
        }
		
		[Description("Sound File for UpAlert")]
		[Category("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("UpAlert")]
		public string UpAlertFile
		{
		get { return upAlertFile; }
		set { upAlertFile = value; }
		}
		
		[Description("Sound File for DownAlert")]
		[Category("Sound Alerts")]
		[Gui.Design.DisplayNameAttribute("DownAlert")]
		public string DownAlertFile
		{
		get { return downAlertFile; }
		set { downAlertFile = value; }
		}
		
        #endregion/////////////////////////////////////////////////////////////////////////////////////
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private TripleMA[] cacheTripleMA = null;

        private static TripleMA checkTripleMA = new TripleMA();

        /// <summary>
        /// TripleMA written by ThatManFromTexas.
        /// </summary>
        /// <returns></returns>
        public TripleMA TripleMA(Color bgColorDown, Color bgColorUp, bool colorBackground, bool colorBackgroundAll, bool colorzone, Color downColor, Color downColorMA1, Color downColorMA2, Color downColorMA3, bool drawdots, bool drawline, Color flatColor, Color flatColorMA3, NinjaTrader.Indicator.TripleMA.DMAType mA1Type, NinjaTrader.Indicator.TripleMA.DMAType mA2Type, NinjaTrader.Indicator.TripleMA.DMAType mA3Type, int mAPeriod1, int mAPeriod2, int mAPeriod3, int opacity, Color upColor, Color upColorMA1, Color upColorMA2, Color upColorMA3, int zopacity)
        {
            return TripleMA(Input, bgColorDown, bgColorUp, colorBackground, colorBackgroundAll, colorzone, downColor, downColorMA1, downColorMA2, downColorMA3, drawdots, drawline, flatColor, flatColorMA3, mA1Type, mA2Type, mA3Type, mAPeriod1, mAPeriod2, mAPeriod3, opacity, upColor, upColorMA1, upColorMA2, upColorMA3, zopacity);
        }

        /// <summary>
        /// TripleMA written by ThatManFromTexas.
        /// </summary>
        /// <returns></returns>
        public TripleMA TripleMA(Data.IDataSeries input, Color bgColorDown, Color bgColorUp, bool colorBackground, bool colorBackgroundAll, bool colorzone, Color downColor, Color downColorMA1, Color downColorMA2, Color downColorMA3, bool drawdots, bool drawline, Color flatColor, Color flatColorMA3, NinjaTrader.Indicator.TripleMA.DMAType mA1Type, NinjaTrader.Indicator.TripleMA.DMAType mA2Type, NinjaTrader.Indicator.TripleMA.DMAType mA3Type, int mAPeriod1, int mAPeriod2, int mAPeriod3, int opacity, Color upColor, Color upColorMA1, Color upColorMA2, Color upColorMA3, int zopacity)
        {
            if (cacheTripleMA != null)
                for (int idx = 0; idx < cacheTripleMA.Length; idx++)
                    if (cacheTripleMA[idx].BgColorDown == bgColorDown && cacheTripleMA[idx].BgColorUp == bgColorUp && cacheTripleMA[idx].ColorBackground == colorBackground && cacheTripleMA[idx].ColorBackgroundAll == colorBackgroundAll && cacheTripleMA[idx].Colorzone == colorzone && cacheTripleMA[idx].DownColor == downColor && cacheTripleMA[idx].DownColorMA1 == downColorMA1 && cacheTripleMA[idx].DownColorMA2 == downColorMA2 && cacheTripleMA[idx].DownColorMA3 == downColorMA3 && cacheTripleMA[idx].Drawdots == drawdots && cacheTripleMA[idx].Drawline == drawline && cacheTripleMA[idx].FlatColor == flatColor && cacheTripleMA[idx].FlatColorMA3 == flatColorMA3 && cacheTripleMA[idx].MA1Type == mA1Type && cacheTripleMA[idx].MA2Type == mA2Type && cacheTripleMA[idx].MA3Type == mA3Type && cacheTripleMA[idx].MAPeriod1 == mAPeriod1 && cacheTripleMA[idx].MAPeriod2 == mAPeriod2 && cacheTripleMA[idx].MAPeriod3 == mAPeriod3 && cacheTripleMA[idx].Opacity == opacity && cacheTripleMA[idx].UpColor == upColor && cacheTripleMA[idx].UpColorMA1 == upColorMA1 && cacheTripleMA[idx].UpColorMA2 == upColorMA2 && cacheTripleMA[idx].UpColorMA3 == upColorMA3 && cacheTripleMA[idx].Zopacity == zopacity && cacheTripleMA[idx].EqualsInput(input))
                        return cacheTripleMA[idx];

            lock (checkTripleMA)
            {
                checkTripleMA.BgColorDown = bgColorDown;
                bgColorDown = checkTripleMA.BgColorDown;
                checkTripleMA.BgColorUp = bgColorUp;
                bgColorUp = checkTripleMA.BgColorUp;
                checkTripleMA.ColorBackground = colorBackground;
                colorBackground = checkTripleMA.ColorBackground;
                checkTripleMA.ColorBackgroundAll = colorBackgroundAll;
                colorBackgroundAll = checkTripleMA.ColorBackgroundAll;
                checkTripleMA.Colorzone = colorzone;
                colorzone = checkTripleMA.Colorzone;
                checkTripleMA.DownColor = downColor;
                downColor = checkTripleMA.DownColor;
                checkTripleMA.DownColorMA1 = downColorMA1;
                downColorMA1 = checkTripleMA.DownColorMA1;
                checkTripleMA.DownColorMA2 = downColorMA2;
                downColorMA2 = checkTripleMA.DownColorMA2;
                checkTripleMA.DownColorMA3 = downColorMA3;
                downColorMA3 = checkTripleMA.DownColorMA3;
                checkTripleMA.Drawdots = drawdots;
                drawdots = checkTripleMA.Drawdots;
                checkTripleMA.Drawline = drawline;
                drawline = checkTripleMA.Drawline;
                checkTripleMA.FlatColor = flatColor;
                flatColor = checkTripleMA.FlatColor;
                checkTripleMA.FlatColorMA3 = flatColorMA3;
                flatColorMA3 = checkTripleMA.FlatColorMA3;
                checkTripleMA.MA1Type = mA1Type;
                mA1Type = checkTripleMA.MA1Type;
                checkTripleMA.MA2Type = mA2Type;
                mA2Type = checkTripleMA.MA2Type;
                checkTripleMA.MA3Type = mA3Type;
                mA3Type = checkTripleMA.MA3Type;
                checkTripleMA.MAPeriod1 = mAPeriod1;
                mAPeriod1 = checkTripleMA.MAPeriod1;
                checkTripleMA.MAPeriod2 = mAPeriod2;
                mAPeriod2 = checkTripleMA.MAPeriod2;
                checkTripleMA.MAPeriod3 = mAPeriod3;
                mAPeriod3 = checkTripleMA.MAPeriod3;
                checkTripleMA.Opacity = opacity;
                opacity = checkTripleMA.Opacity;
                checkTripleMA.UpColor = upColor;
                upColor = checkTripleMA.UpColor;
                checkTripleMA.UpColorMA1 = upColorMA1;
                upColorMA1 = checkTripleMA.UpColorMA1;
                checkTripleMA.UpColorMA2 = upColorMA2;
                upColorMA2 = checkTripleMA.UpColorMA2;
                checkTripleMA.UpColorMA3 = upColorMA3;
                upColorMA3 = checkTripleMA.UpColorMA3;
                checkTripleMA.Zopacity = zopacity;
                zopacity = checkTripleMA.Zopacity;

                if (cacheTripleMA != null)
                    for (int idx = 0; idx < cacheTripleMA.Length; idx++)
                        if (cacheTripleMA[idx].BgColorDown == bgColorDown && cacheTripleMA[idx].BgColorUp == bgColorUp && cacheTripleMA[idx].ColorBackground == colorBackground && cacheTripleMA[idx].ColorBackgroundAll == colorBackgroundAll && cacheTripleMA[idx].Colorzone == colorzone && cacheTripleMA[idx].DownColor == downColor && cacheTripleMA[idx].DownColorMA1 == downColorMA1 && cacheTripleMA[idx].DownColorMA2 == downColorMA2 && cacheTripleMA[idx].DownColorMA3 == downColorMA3 && cacheTripleMA[idx].Drawdots == drawdots && cacheTripleMA[idx].Drawline == drawline && cacheTripleMA[idx].FlatColor == flatColor && cacheTripleMA[idx].FlatColorMA3 == flatColorMA3 && cacheTripleMA[idx].MA1Type == mA1Type && cacheTripleMA[idx].MA2Type == mA2Type && cacheTripleMA[idx].MA3Type == mA3Type && cacheTripleMA[idx].MAPeriod1 == mAPeriod1 && cacheTripleMA[idx].MAPeriod2 == mAPeriod2 && cacheTripleMA[idx].MAPeriod3 == mAPeriod3 && cacheTripleMA[idx].Opacity == opacity && cacheTripleMA[idx].UpColor == upColor && cacheTripleMA[idx].UpColorMA1 == upColorMA1 && cacheTripleMA[idx].UpColorMA2 == upColorMA2 && cacheTripleMA[idx].UpColorMA3 == upColorMA3 && cacheTripleMA[idx].Zopacity == zopacity && cacheTripleMA[idx].EqualsInput(input))
                            return cacheTripleMA[idx];

                TripleMA indicator = new TripleMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.BgColorDown = bgColorDown;
                indicator.BgColorUp = bgColorUp;
                indicator.ColorBackground = colorBackground;
                indicator.ColorBackgroundAll = colorBackgroundAll;
                indicator.Colorzone = colorzone;
                indicator.DownColor = downColor;
                indicator.DownColorMA1 = downColorMA1;
                indicator.DownColorMA2 = downColorMA2;
                indicator.DownColorMA3 = downColorMA3;
                indicator.Drawdots = drawdots;
                indicator.Drawline = drawline;
                indicator.FlatColor = flatColor;
                indicator.FlatColorMA3 = flatColorMA3;
                indicator.MA1Type = mA1Type;
                indicator.MA2Type = mA2Type;
                indicator.MA3Type = mA3Type;
                indicator.MAPeriod1 = mAPeriod1;
                indicator.MAPeriod2 = mAPeriod2;
                indicator.MAPeriod3 = mAPeriod3;
                indicator.Opacity = opacity;
                indicator.UpColor = upColor;
                indicator.UpColorMA1 = upColorMA1;
                indicator.UpColorMA2 = upColorMA2;
                indicator.UpColorMA3 = upColorMA3;
                indicator.Zopacity = zopacity;
                Indicators.Add(indicator);
                indicator.SetUp();

                TripleMA[] tmp = new TripleMA[cacheTripleMA == null ? 1 : cacheTripleMA.Length + 1];
                if (cacheTripleMA != null)
                    cacheTripleMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheTripleMA = tmp;
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
        /// TripleMA written by ThatManFromTexas.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TripleMA TripleMA(Color bgColorDown, Color bgColorUp, bool colorBackground, bool colorBackgroundAll, bool colorzone, Color downColor, Color downColorMA1, Color downColorMA2, Color downColorMA3, bool drawdots, bool drawline, Color flatColor, Color flatColorMA3, NinjaTrader.Indicator.TripleMA.DMAType mA1Type, NinjaTrader.Indicator.TripleMA.DMAType mA2Type, NinjaTrader.Indicator.TripleMA.DMAType mA3Type, int mAPeriod1, int mAPeriod2, int mAPeriod3, int opacity, Color upColor, Color upColorMA1, Color upColorMA2, Color upColorMA3, int zopacity)
        {
            return _indicator.TripleMA(Input, bgColorDown, bgColorUp, colorBackground, colorBackgroundAll, colorzone, downColor, downColorMA1, downColorMA2, downColorMA3, drawdots, drawline, flatColor, flatColorMA3, mA1Type, mA2Type, mA3Type, mAPeriod1, mAPeriod2, mAPeriod3, opacity, upColor, upColorMA1, upColorMA2, upColorMA3, zopacity);
        }

        /// <summary>
        /// TripleMA written by ThatManFromTexas.
        /// </summary>
        /// <returns></returns>
        public Indicator.TripleMA TripleMA(Data.IDataSeries input, Color bgColorDown, Color bgColorUp, bool colorBackground, bool colorBackgroundAll, bool colorzone, Color downColor, Color downColorMA1, Color downColorMA2, Color downColorMA3, bool drawdots, bool drawline, Color flatColor, Color flatColorMA3, NinjaTrader.Indicator.TripleMA.DMAType mA1Type, NinjaTrader.Indicator.TripleMA.DMAType mA2Type, NinjaTrader.Indicator.TripleMA.DMAType mA3Type, int mAPeriod1, int mAPeriod2, int mAPeriod3, int opacity, Color upColor, Color upColorMA1, Color upColorMA2, Color upColorMA3, int zopacity)
        {
            return _indicator.TripleMA(input, bgColorDown, bgColorUp, colorBackground, colorBackgroundAll, colorzone, downColor, downColorMA1, downColorMA2, downColorMA3, drawdots, drawline, flatColor, flatColorMA3, mA1Type, mA2Type, mA3Type, mAPeriod1, mAPeriod2, mAPeriod3, opacity, upColor, upColorMA1, upColorMA2, upColorMA3, zopacity);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// TripleMA written by ThatManFromTexas.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.TripleMA TripleMA(Color bgColorDown, Color bgColorUp, bool colorBackground, bool colorBackgroundAll, bool colorzone, Color downColor, Color downColorMA1, Color downColorMA2, Color downColorMA3, bool drawdots, bool drawline, Color flatColor, Color flatColorMA3, NinjaTrader.Indicator.TripleMA.DMAType mA1Type, NinjaTrader.Indicator.TripleMA.DMAType mA2Type, NinjaTrader.Indicator.TripleMA.DMAType mA3Type, int mAPeriod1, int mAPeriod2, int mAPeriod3, int opacity, Color upColor, Color upColorMA1, Color upColorMA2, Color upColorMA3, int zopacity)
        {
            return _indicator.TripleMA(Input, bgColorDown, bgColorUp, colorBackground, colorBackgroundAll, colorzone, downColor, downColorMA1, downColorMA2, downColorMA3, drawdots, drawline, flatColor, flatColorMA3, mA1Type, mA2Type, mA3Type, mAPeriod1, mAPeriod2, mAPeriod3, opacity, upColor, upColorMA1, upColorMA2, upColorMA3, zopacity);
        }

        /// <summary>
        /// TripleMA written by ThatManFromTexas.
        /// </summary>
        /// <returns></returns>
        public Indicator.TripleMA TripleMA(Data.IDataSeries input, Color bgColorDown, Color bgColorUp, bool colorBackground, bool colorBackgroundAll, bool colorzone, Color downColor, Color downColorMA1, Color downColorMA2, Color downColorMA3, bool drawdots, bool drawline, Color flatColor, Color flatColorMA3, NinjaTrader.Indicator.TripleMA.DMAType mA1Type, NinjaTrader.Indicator.TripleMA.DMAType mA2Type, NinjaTrader.Indicator.TripleMA.DMAType mA3Type, int mAPeriod1, int mAPeriod2, int mAPeriod3, int opacity, Color upColor, Color upColorMA1, Color upColorMA2, Color upColorMA3, int zopacity)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.TripleMA(input, bgColorDown, bgColorUp, colorBackground, colorBackgroundAll, colorzone, downColor, downColorMA1, downColorMA2, downColorMA3, drawdots, drawline, flatColor, flatColorMA3, mA1Type, mA2Type, mA3Type, mAPeriod1, mAPeriod2, mAPeriod3, opacity, upColor, upColorMA1, upColorMA2, upColorMA3, zopacity);
        }
    }
}
#endregion
