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
using MakeItSoNumberOne;
#endregion

namespace MakeItSoNumberOne
{
	public enum DeviceSelection { Both, None, TDI_only, SMI_only }
	public enum DeviceType { Diamonds, Arrows, Dots, Triangles }
}

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    /// <summary>
    /// This indicator combines the SMI5 and the Trader's Dynamic Index into one that will generate an arrow when the two line up.
    /// </summary>
    [Description("This indicator combines the SMI and the Trader's Dynamic Index into one that will generate a user selected device when the two line up.")]
    public class SMTDIndex : Indicator
    {
        #region Variables
		private int				range			= 10;
		private int				emaperiod1		= 17;
		private int				emaperiod2		= 1;
		private int 			smiemaperiod 	= 2;
		private int				rsiPeriod 		= 7;
		private int				pricePeriod		= 2;
		private int				signalPeriod	= 7;
		
		private DeviceSelection deviceSelection = DeviceSelection.Both;
		private DeviceType 		deviceType 		= DeviceType.Diamonds;
		private Color			upDeviceColor 	= Color.Blue;
		private Color			downDeviceColor = Color.Red;
		private bool 			playSoundFile 	= false;
		private string 			soundFileName 	= "Alert1.wav";
		
		private BoolSeries		upDeviceSeries;
		private BoolSeries		downDeviceSeries;

		// SMI dataseries
		private DataSeries 		smis;
		private DataSeries		sms;
		private DataSeries		hls;
		private DataSeries		SMIEMA;
		
		// TDI dataseries
		private DataSeries		PriceLine;
		private DataSeries		SignalLine;
		private RSI 			DYNRSI;
		private SMA 			DYNPrice;
		private SMA 			DYNSignal; 

		#endregion

        /// <summary>
        /// This method is used to configure the indicator and is called once before any bar data is loaded.
        /// </summary>
        protected override void Initialize()
        {
            Overlay	= true;
			
			smis	= new DataSeries(this);
			sms		= new DataSeries(this);
			hls		= new DataSeries(this);
			SMIEMA  = new DataSeries(this);
			
			PriceLine = new DataSeries(this);
			SignalLine = new DataSeries(this);

			upDeviceSeries = new BoolSeries(this);
			downDeviceSeries = new BoolSeries(this);

        }

		protected override void OnStartUp()
		{
			DYNRSI = RSI(Input,RSIPeriod,1);
			DYNPrice = SMA(DYNRSI,PricePeriod);
			DYNSignal = SMA(DYNRSI,SignalPeriod);
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Stochastic Momentum = SM {distance of close - midpoint}
		 	sms.Set(Close[0] - 0.5 * ((MAX(High, Range)[0] + MIN(Low, Range)[0])));
			
			// High low diffs
			hls.Set(MAX(High, Range)[0] - MIN(Low, Range)[0]);

			// Stochastic Momentum Index = SMI
			double denom = 0.5*EMA( EMA(hls, EMAPeriod1),EMAPeriod2 )[0];
 			smis.Set( 100*(EMA( EMA(sms,EMAPeriod1), EMAPeriod2 ))[0] / (denom ==0 ? 1 : denom  ));
			
			// Set the line value for the SMIEMA by taking the EMA of the SMI
			SMIEMA.Set(EMA(smis, SMIEMAPeriod)[0]);

			double priceValue = DYNPrice[0];
			PriceLine.Set(priceValue);
			SignalLine.Set(DYNSignal[0]);

			#region Draw devices and maybe play sounds

			upDeviceSeries.Set(false);
			downDeviceSeries.Set(false);
			switch( PickDeviceSelection )
			{
				case DeviceSelection.SMI_only:
				{
					if( CrossBelow(smis, SMIEMA,1) )
					{
						drawDevice( false, "SMIOnlyUp" );
						downDeviceSeries.Set(0, true);
						if( PlaySoundFile ) {
							PlaySound( SoundFileName );
						}
					}
					else if( CrossAbove(smis, SMIEMA,1) )
					{
						drawDevice( true, "SMIOnlyDown" );
						upDeviceSeries.Set(0, true);
						if( PlaySoundFile ) {
							PlaySound( SoundFileName );
						}
					}
					break;
				}
				case DeviceSelection.TDI_only:
				{
					if( CrossAbove( PriceLine, SignalLine, 1 ) ) // have used the Signal instead of the Average line)
					{
						drawDevice( true, "TDIOnlyUp" );
						upDeviceSeries.Set(0, true);
						if( PlaySoundFile ) {
							PlaySound( SoundFileName );
						}
					}
					else if( CrossBelow( PriceLine, SignalLine, 1 ) )
					{
						drawDevice( false, "TDIOnlyDown" );
						downDeviceSeries.Set(0, true);
						if( PlaySoundFile ) {
							PlaySound( SoundFileName );
						}
					}
					break;
				}
				case DeviceSelection.Both:
				{
					if( CrossBelow(smis, SMIEMA,1)
						&& CrossBelow( PriceLine, SignalLine, 1 ) )
					{
						drawDevice( false, "BothDown" );
						downDeviceSeries.Set(0, true);
						if( PlaySoundFile ) {
							PlaySound( SoundFileName );
						}
					}
					else 
					{
						RemoveDrawObject( "BothDown"+CurrentBar );	
					}
					
					if( CrossAbove(smis, SMIEMA,1)
						&& CrossAbove( PriceLine, SignalLine, 1 ) )
					{
						drawDevice( true, "BothUp" );
						upDeviceSeries.Set(0, true);
						if( PlaySoundFile ) {
							PlaySound( SoundFileName );
						}
					}
					else 
					{
						RemoveDrawObject( "BothUp"+CurrentBar );	
					}
					break;
				}				
			}
			#endregion (Draw devices...)
			
		}
		
		// Pet peeve: I really can't stand all that bullshizzle NT puts on the label...
		public override string ToString()
		{
			string retVal = "SMTDIndex( Device selection = " + PickDeviceSelection.ToString() + " )"; 
			return retVal;
		}

		
		#region Devices
		private void drawDevice( bool upDevice, string tag )
		{
			switch( PickDeviceType )
			{
				case DeviceType.Arrows:
				{
					if( upDevice ) 	{
						DrawArrowUp( tag+CurrentBar.ToString(), AutoScale, 0, Low[0] - 3*TickSize, DeviceUpColor);
					}
					else {
						DrawArrowDown( tag+CurrentBar.ToString(), AutoScale, 0, High[0] + 3*TickSize, DeviceDownColor);
					}						
					break;
				}
				case DeviceType.Diamonds:
				{
					if( upDevice ) 	{
						DrawDiamond( tag+CurrentBar.ToString(), AutoScale, 0, Low[0] - 3*TickSize, DeviceUpColor);
					}
					else {
						DrawDiamond( tag+CurrentBar.ToString(), AutoScale, 0, High[0] + 3*TickSize, DeviceDownColor);
					}						
					break;
				}
				case DeviceType.Dots:
				{
					if( upDevice ) 	{
						DrawDot( tag+CurrentBar.ToString(), AutoScale, 0, Low[0] - 3*TickSize, DeviceUpColor);
					}
					else {
						DrawDot( tag+CurrentBar.ToString(), AutoScale, 0, High[0] + 3*TickSize, DeviceDownColor);
					}						
					break;
				}
				case DeviceType.Triangles:
				{
					if( upDevice ) 	{
						DrawTriangleUp( tag+CurrentBar.ToString(), AutoScale, 0, Low[0] - 3*TickSize, DeviceUpColor);
					}
					else {
						DrawTriangleDown( tag+CurrentBar.ToString(), AutoScale, 0, High[0] + 3*TickSize, DeviceDownColor);
					}						
					break;
				}
			}
		}
		#endregion

        #region Properties
        public BoolSeries UpDeviceSeries
        {
            get 
			{ 
				Update();
				return upDeviceSeries; 
			}
        }
		[Browsable(false)]	
        public BoolSeries DownDeviceSeries
        {
            get 
			{ 
				Update();
				return downDeviceSeries; 
			}
        }
		
		[Description("Range for momentum calculation. ( Q )")]
		[Category("SMI_Parameters")]
		public int Range
		{
			get { return range; }
			set { range = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("1st ema smothing period. ( R )")]
		[Category("SMI_Parameters")]
		public int EMAPeriod1
		{
			get { return emaperiod1; }
			set { emaperiod1 = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("2nd ema smoothing period. ( S )")]
		[Category("SMI_Parameters")]
		public int EMAPeriod2
		{
			get { return emaperiod2; }
			set { emaperiod2 = Math.Max(1, value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("SMI EMA smoothing period.")]
		[Category("SMI_Parameters")]
		public int SMIEMAPeriod
		{
			get { return smiemaperiod; }
			set { smiemaperiod = Math.Max(1, value); }
		}
		
		[Description("Period for RSI")]
		[Category("SMI_Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for RSI")]
		public int RSIPeriod
		{
			get { return rsiPeriod; }
			set { rsiPeriod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for Priceline")]
		[Category("TDI_Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for Priceline")]
		public int PricePeriod
		{
			get { return pricePeriod; }
			set { pricePeriod = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Period for Signalline")]
		[Category("TDI_Parameters")]
		[Gui.Design.DisplayNameAttribute("Period for Signalline")]
		public int SignalPeriod
		{
			get { return signalPeriod; }
			set { signalPeriod = Math.Max(1, value); }
		}

		[Description("Device selection: Both, None, SMI only, TDI only")]
		[Category("Audio-Visual")]
		[Gui.Design.DisplayNameAttribute("\t\t\t\t\tDevice selection")]
		public DeviceSelection PickDeviceSelection
		{
			get { return deviceSelection; }
			set { deviceSelection = value; }
		}
		[Description("Device type: Arrows, diamonds, dots or triangles")]
		[Category("Audio-Visual")]
		[Gui.Design.DisplayNameAttribute("\t\t\t\tDevice type")]
		public DeviceType PickDeviceType
		{
			get { return deviceType; }
			set { deviceType = value; }
		}
		[Description("Device color up selection")]
        [Category("Audio-Visual")]
		[Gui.Design.DisplayNameAttribute ("\t\t\tDevice up color")]
        public Color DeviceUpColor
        {
            get { return upDeviceColor; }
            set { upDeviceColor = value; }
        }
		[Browsable(false)]
		public string UpDeviceColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upDeviceColor); }
			set { upDeviceColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		[Description("Device color down selection")]
        [Category("Audio-Visual")]
		[Gui.Design.DisplayNameAttribute ("\t\tDevice down color")]
        public Color DeviceDownColor
        {
            get { return downDeviceColor; }
            set { downDeviceColor = value; }
        }
		[Browsable(false)]
		public string DownDeviceColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downDeviceColor); }
			set { downDeviceColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		[Description("True to play a sound alert if alert condition is true.")]
        [Category("Audio-Visual")]
        [Gui.Design.DisplayNameAttribute("\tPlay sound alert?")]		
        public bool PlaySoundFile
        {
            get { return playSoundFile; }
            set { playSoundFile = value; }
        }

		[Description("Sound file name if alert condition is true.")]
        [Category("Audio-Visual")]
        [Gui.Design.DisplayNameAttribute("Sound alert file name")]		
        public string SoundFileName
        {
            get { return soundFileName; }
            set { soundFileName = value; }
        }

		#endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private SMTDIndex[] cacheSMTDIndex = null;

        private static SMTDIndex checkSMTDIndex = new SMTDIndex();

        /// <summary>
        /// This indicator combines the SMI and the Trader's Dynamic Index into one that will generate a user selected device when the two line up.
        /// </summary>
        /// <returns></returns>
        public SMTDIndex SMTDIndex()
        {
            return SMTDIndex(Input);
        }

        /// <summary>
        /// This indicator combines the SMI and the Trader's Dynamic Index into one that will generate a user selected device when the two line up.
        /// </summary>
        /// <returns></returns>
        public SMTDIndex SMTDIndex(Data.IDataSeries input)
        {
            if (cacheSMTDIndex != null)
                for (int idx = 0; idx < cacheSMTDIndex.Length; idx++)
                    if (cacheSMTDIndex[idx].EqualsInput(input))
                        return cacheSMTDIndex[idx];

            lock (checkSMTDIndex)
            {
                if (cacheSMTDIndex != null)
                    for (int idx = 0; idx < cacheSMTDIndex.Length; idx++)
                        if (cacheSMTDIndex[idx].EqualsInput(input))
                            return cacheSMTDIndex[idx];

                SMTDIndex indicator = new SMTDIndex();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                Indicators.Add(indicator);
                indicator.SetUp();

                SMTDIndex[] tmp = new SMTDIndex[cacheSMTDIndex == null ? 1 : cacheSMTDIndex.Length + 1];
                if (cacheSMTDIndex != null)
                    cacheSMTDIndex.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheSMTDIndex = tmp;
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
        /// This indicator combines the SMI and the Trader's Dynamic Index into one that will generate a user selected device when the two line up.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMTDIndex SMTDIndex()
        {
            return _indicator.SMTDIndex(Input);
        }

        /// <summary>
        /// This indicator combines the SMI and the Trader's Dynamic Index into one that will generate a user selected device when the two line up.
        /// </summary>
        /// <returns></returns>
        public Indicator.SMTDIndex SMTDIndex(Data.IDataSeries input)
        {
            return _indicator.SMTDIndex(input);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// This indicator combines the SMI and the Trader's Dynamic Index into one that will generate a user selected device when the two line up.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.SMTDIndex SMTDIndex()
        {
            return _indicator.SMTDIndex(Input);
        }

        /// <summary>
        /// This indicator combines the SMI and the Trader's Dynamic Index into one that will generate a user selected device when the two line up.
        /// </summary>
        /// <returns></returns>
        public Indicator.SMTDIndex SMTDIndex(Data.IDataSeries input)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.SMTDIndex(input);
        }
    }
}
#endregion
