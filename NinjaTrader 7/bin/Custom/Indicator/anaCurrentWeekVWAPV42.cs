#region Using declarations
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;
#endregion

#region Global Enums

public enum anaSessionTypeVWAPW42 {ETH, RTH}
public enum anaSessionCountVWAPW42 {First, Second, Third, Auto}
public enum anaBandTypeVWAPW42 {Variance_Distance, Variance_Price, Session_Range, None} 

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// The indicator plots the volume weighed average price of the selected week and volatility bands.
	/// </summary>
	[Description("anaCurrentWeekVWAPV42.")]
	public class anaCurrentWeekVWAPV42 : Indicator
	{
		#region Variables
		private DateTime				sessionBegin			= Cbi.Globals.MinDate;
		private DateTime				sessionEnd				= Cbi.Globals.MinDate;
		private DateTime				cacheSessionBeginTmp   	= Cbi.Globals.MinDate;
		private DateTime				cacheSessionEndTmp1		= Cbi.Globals.MinDate;
		private DateTime				cacheSessionEndTmp2		= Cbi.Globals.MinDate;
		private DateTime				cacheSessionDate		= Cbi.Globals.MinDate;
		private DateTime				cacheWeeklyEndDate		= Cbi.Globals.MinDate;
		private DateTime				sessionDateTmp			= Cbi.Globals.MinDate;
		private DateTime				currentDate				= Cbi.Globals.MinDate;
		private DateTime				currentWeek				= Cbi.Globals.MinDate;
		private DateTime				currentAnchorDate		= Cbi.Globals.MinDate;
		private DateTime				lastBarTimeStamp1		= Cbi.Globals.MinDate;
		private DateTime				lastBarTimeStamp2		= Cbi.Globals.MinDate;
		private DateTime				anchorTime				= Cbi.Globals.MinDate;
		private TimeSpan				sessionOffset			= new TimeSpan(0,0,0,0);
		private double					multiplier1				= 1.0;
		private double					multiplier2				= 2.0;
		private double					multiplier3				= 3.0;
		private double					firstBarOpen			= 0.0;
		private double					open					= 0.0;
		private double					high					= 0.0;
		private double					low						= 0.0;
		private double					close					= 0.0;
		private double					mean					= 0.0;
		private double					mean1					= 0.0;
		private double					mean2					= 0.0;
		private	double					volSum					= 0.0;
		private	double					squareSum				= 0.0;
		private	double					currentVolSum			= 0.0;
		private	double					currentSquareSum		= 0.0;
		private	double					currentVWAP				= 0.0;
		private double					priorVWAP				= 0.0;
		private double					sessionVWAP				= 0.0;
		private double					currentHigh				= double.MinValue;
		private double					currentLow				= double.MaxValue;
		private double					deviation				= 0.0;
		private double					offset					= 0.0;
		private SolidBrush				textBrush				= new SolidBrush(Color.Red);
		private Font					textFont				= new Font("Arial", 12);
		private string					errorData1				= "The weekly VWAP can only be displayed on intraday charts.";
		private string					errorData2				= "You may not select a negative offset for the weekly VWAP start time.";
		private bool					plotVWAP				= false;
		private bool					calcOpen				= false;
		private bool					initPlot				= false;
		private bool					extendPublicHoliday		= false;
		private bool					isEndOfWeekHoliday		= false; 
		private bool					firstDayOfPeriod		= false;
		private bool					rthVWAP					= false;
		private bool					isCurrency				= false;
		private bool					isGlobex				= false;
		private bool					gap0					= true;
		private bool					gap1					= true;
		private bool					tickBuilt				= false;
		private bool					applySessionOffset		= false;
		private bool					anchorBar				= false;
		private ArrayList				newSessionBarIdxArr2	= new ArrayList();
		private anaSessionTypeVWAPW42	sessionType				= anaSessionTypeVWAPW42.ETH;
		private anaSessionCountVWAPW42 	selectedSession			= anaSessionCountVWAPW42.Auto;
		private anaSessionCountVWAPW42 	activeSession			= anaSessionCountVWAPW42.Second;
		private anaBandTypeVWAPW42		currentBandType			= anaBandTypeVWAPW42.Variance_Price;
		private Data.PivotRange			pivotRangeType1			= PivotRange.Daily;
		private Data.PivotRange			pivotRangeType2			= PivotRange.Weekly;
		private SolidBrush				innerAreaBrush			= new SolidBrush(Color.LimeGreen);
		private SolidBrush				middleAreaBrush			= new SolidBrush(Color.ForestGreen);
		private SolidBrush				outerAreaBrush			= new SolidBrush(Color.DarkGreen);
		private int						shiftAnchor				= 0;
		private int						numberOfSessions		= 1;
		private int						sessionCount			= 0;
		private int						sessionBar				= 1;
		private Color					upColor					= Color.DodgerBlue;
		private Color					downColor				= Color.OrangeRed;
		private Color					outerBandColor			= Color.DarkGreen;
		private Color					middleBandColor			= Color.ForestGreen;
		private Color					innerBandColor			= Color.LimeGreen;
		private Color					outerAreaColor			= Color.DarkGreen;
		private Color					middleAreaColor			= Color.ForestGreen;
		private Color					innerAreaColor			= Color.LimeGreen;
		private int						opacity1				= 3;
		private int						opacity2				= 0;
		private int						opacity3				= 3;
		private int						plot0Width				= 3;
		private PlotStyle				plot0Style				= PlotStyle.Line;
		private DashStyle				dash0Style				= DashStyle.DashDot;
		private int						plot1Width				= 1;
		private PlotStyle				plot1Style				= PlotStyle.Line;
		private DashStyle				dash1Style				= DashStyle.Solid;
		private DateTime				publicHoliday0			= new DateTime (2009,07,03);
		private DateTime				publicHoliday1;
		private DateTime				publicHoliday2;
		private DateTime[]				publicHoliday			= new DateTime [3];

		#endregion

		/// <summary>
		/// This method is used to configure the indicator and is called once before any bar data is loaded.
		/// </summary>
		protected override void Initialize()
		{
			Add(new Plot(new Pen(Color.Gold,1), PlotStyle.Dot, "Session VWAP"));
			Add(new Plot(new Pen(Color.Gold,1), PlotStyle.Line, "Upper Band SD 3"));
			Add(new Plot(new Pen(Color.Gold,1), PlotStyle.Line, "Upper Band SD 2"));
			Add(new Plot(new Pen(Color.Gold,1), PlotStyle.Line, "Upper Band SD 1"));
			Add(new Plot(new Pen(Color.Gold,1), PlotStyle.Line, "Lower Band SD 1"));
			Add(new Plot(new Pen(Color.Gold,1), PlotStyle.Line, "Lower Band SD 2"));
			Add(new Plot(new Pen(Color.Gold,1), PlotStyle.Line, "Lower Band SD 3"));
			
			AutoScale			= false;
			Overlay				= true;
			PlotsConfigurable	= false;
			ZOrder				= -2;
			BarsRequired		= 0;
		}

		/// <summary>
		/// </summary>
		protected override void OnStartUp()
		{
			if (Instrument.MasterInstrument.InstrumentType == InstrumentType.Future &&
				(Instrument.MasterInstrument.Name == "EMD" ||Instrument.MasterInstrument.Name == "ES" || Instrument.MasterInstrument.Name == "NQ" 
				||Instrument.MasterInstrument.Name == "YM"||Instrument.MasterInstrument.Name == "GE" ||Instrument.MasterInstrument.Name == "SR" 
				||Instrument.MasterInstrument.Name == "UB"||Instrument.MasterInstrument.Name == "ZB" ||Instrument.MasterInstrument.Name == "ZF"
				||Instrument.MasterInstrument.Name == "ZN"||Instrument.MasterInstrument.Name == "ZQ" ||Instrument.MasterInstrument.Name == "ZT"
				||Instrument.MasterInstrument.Name == "6A"||Instrument.MasterInstrument.Name == "6B" ||Instrument.MasterInstrument.Name == "6C"
				||Instrument.MasterInstrument.Name == "6E"||Instrument.MasterInstrument.Name == "6J" ||Instrument.MasterInstrument.Name == "6M"
				||Instrument.MasterInstrument.Name == "6N"||Instrument.MasterInstrument.Name == "6S" ||Instrument.MasterInstrument.Name == "E7"
				||Instrument.MasterInstrument.Name == "J7"||Instrument.MasterInstrument.Name == "M6A" ||Instrument.MasterInstrument.Name == "M6B" 
				||Instrument.MasterInstrument.Name == "M6C"||Instrument.MasterInstrument.Name == "M6E" ||Instrument.MasterInstrument.Name == "M6J"
				||Instrument.MasterInstrument.Name == "M6S"||Instrument.MasterInstrument.Name == "CL" ||Instrument.MasterInstrument.Name == "EH"
				||Instrument.MasterInstrument.Name == "GC"||Instrument.MasterInstrument.Name == "HG" ||Instrument.MasterInstrument.Name == "HO"
				||Instrument.MasterInstrument.Name == "NG"||Instrument.MasterInstrument.Name == "QG" ||Instrument.MasterInstrument.Name == "QM"
				||Instrument.MasterInstrument.Name == "RB"||Instrument.MasterInstrument.Name == "SI" ||Instrument.MasterInstrument.Name == "YG" 
				||Instrument.MasterInstrument.Name == "YI"||Instrument.MasterInstrument.Name == "GF" ||Instrument.MasterInstrument.Name == "GPB"
				||Instrument.MasterInstrument.Name == "HE"||Instrument.MasterInstrument.Name == "LE" ||Instrument.MasterInstrument.Name == "YC"
				||Instrument.MasterInstrument.Name == "YK"||Instrument.MasterInstrument.Name == "YW" ||Instrument.MasterInstrument.Name == "ZC"
				||Instrument.MasterInstrument.Name == "ZE"||Instrument.MasterInstrument.Name == "ZL" ||Instrument.MasterInstrument.Name == "ZM"
				||Instrument.MasterInstrument.Name == "ZO"||Instrument.MasterInstrument.Name == "ZR" ||Instrument.MasterInstrument.Name == "ZS"
				||Instrument.MasterInstrument.Name == "ZW"))				
				isGlobex = true;
			else
				isGlobex = false;
			if (isGlobex)
			{
				publicHoliday[0] = publicHoliday0;
				publicHoliday[1] = publicHoliday1;
				publicHoliday[2] = publicHoliday2;
			}
			else for(int i=0; i<3; i++)
				publicHoliday[i] = Cbi.Globals.MinDate;
			if (Instrument.MasterInstrument.InstrumentType == Cbi.InstrumentType.Currency || Instrument.MasterInstrument.Name == "DX" || Instrument.MasterInstrument.Name == "6A"
				|| Instrument.MasterInstrument.Name == "6B" || Instrument.MasterInstrument.Name == "6C" ||Instrument.MasterInstrument.Name == "6E"
				|| Instrument.MasterInstrument.Name == "6J" || Instrument.MasterInstrument.Name == "6M" || Instrument.MasterInstrument.Name == "6S"
				|| Instrument.MasterInstrument.Name == "6N" || Instrument.MasterInstrument.Name == "E7" || Instrument.MasterInstrument.Name == "J7"
				|| Instrument.MasterInstrument.Name == "M6A" || Instrument.MasterInstrument.Name == "M6B" || Instrument.MasterInstrument.Name == "M6C"
				|| Instrument.MasterInstrument.Name == "M6E" || Instrument.MasterInstrument.Name == "M6J" || Instrument.MasterInstrument.Name == "M6S")
				isCurrency = true;
			else
				isCurrency = false;
			
			if (sessionType == anaSessionTypeVWAPW42.ETH) 
			{
				rthVWAP = false;
				shiftAnchor = TimeSpan.Compare(sessionOffset, new TimeSpan(0,0,0));
				if(shiftAnchor > 0)
					applySessionOffset = true;
			}
			else 
			{	
				rthVWAP = true;
				shiftAnchor = 0;
				applySessionOffset = false;
				if (selectedSession == anaSessionCountVWAPW42.Auto)
				{
					if (isCurrency)
						activeSession = anaSessionCountVWAPW42.Third;
					else
						activeSession = anaSessionCountVWAPW42.Second;
				}
				else 
					activeSession = selectedSession; 			
			}			
			innerAreaBrush = new SolidBrush(Color.FromArgb(25*opacity1, innerAreaColor));
			middleAreaBrush = new SolidBrush(Color.FromArgb(25*opacity2, middleAreaColor));
			outerAreaBrush = new SolidBrush(Color.FromArgb(25*opacity3, outerAreaColor));
			Plots[1].Pen.Color		= OuterBandColor;
			Plots[2].Pen.Color		= MiddleBandColor;
			Plots[3].Pen.Color		= InnerBandColor;
			Plots[4].Pen.Color		= InnerBandColor;
			Plots[5].Pen.Color		= MiddleBandColor;
			Plots[6].Pen.Color		= OuterBandColor;
			Plots[0].Pen.Width 		= plot0Width;
			Plots[0].PlotStyle		= plot0Style;
			Plots[0].Pen.DashStyle 	= dash0Style;
			Plots[1].Pen.Width 		= plot1Width;
			Plots[1].PlotStyle		= plot1Style;
			Plots[1].Pen.DashStyle 	= dash1Style;
			Plots[2].Pen.Width 		= plot1Width;
			Plots[2].PlotStyle		= plot1Style;
			Plots[2].Pen.DashStyle 	= dash1Style;
			Plots[3].Pen.Width 		= plot1Width;
			Plots[3].PlotStyle		= plot1Style;
			Plots[3].Pen.DashStyle 	= dash1Style;
			Plots[4].Pen.Width 		= plot1Width;
			Plots[4].PlotStyle		= plot1Style;
			Plots[4].Pen.DashStyle 	= dash1Style;
			Plots[5].Pen.Width 		= plot1Width;
			Plots[5].PlotStyle		= plot1Style;
			Plots[5].Pen.DashStyle 	= dash1Style;
			Plots[6].Pen.Width 		= plot1Width;
			Plots[6].PlotStyle		= plot1Style;
			Plots[6].Pen.DashStyle 	= dash1Style;
			gap0 = (plot0Style == PlotStyle.Line || plot0Style == PlotStyle.Square);
			gap1 = (plot1Style == PlotStyle.Line || plot1Style == PlotStyle.Square);
			if (BarsPeriod.Id == PeriodType.Minute || BarsPeriod.Id == PeriodType.Second)
				tickBuilt = false;
			else
				tickBuilt = true;
			anchorBar = false;
		}

		/// <summary>
		/// Called on each bar update event (incoming tick)
		/// </summary>
		protected override void OnBarUpdate()
		{
			if (Bars == null)
				return; 
			if (!Data.BarsType.GetInstance(Bars.Period.Id).IsIntraday)
			{
				textBrush.Color = ChartControl.AxisColor;
				DrawTextFixed("errortag1", errorData1, TextPosition.Center, textBrush.Color, textFont, Color.Transparent,Color.Transparent,0);
				return;
			}
			if(shiftAnchor < 0)
			{
				DrawTextFixed("errortag2", errorData2, TextPosition.Center, textBrush.Color, textFont, Color.Transparent,Color.Transparent,0);
				return;
			}
			if (CurrentBar == 0)
			{	
				currentDate = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
				currentWeek = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType2);
				currentAnchorDate = currentWeek;
				sessionCount = 1;
				return;
			}
			
				lastBarTimeStamp1 = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType1);
				lastBarTimeStamp2 = GetLastBarSessionDate(Time[0], Bars, 0, pivotRangeType2);
				if (isEndOfWeekHoliday)
					lastBarTimeStamp2 = GetLastBarSessionDate(Time[0].AddDays(3), Bars, 0, pivotRangeType2);
			if(applySessionOffset) 
			{
				if(Bars.FirstBarOfSession && FirstTickOfBar)
					if(applySessionOffset && lastBarTimeStamp2 != currentAnchorDate)
					{
						calcOpen = false;
						initPlot = false;
						anchorTime = cacheSessionBeginTmp.Add(sessionOffset);
						currentAnchorDate = lastBarTimeStamp2;
					}
				if(tickBuilt && Time[0] >= anchorTime && Time[1] < anchorTime)
					anchorBar = true;
				else if(!tickBuilt && Time[0] > anchorTime && Time[1] <= anchorTime)
					anchorBar = true;
				else
					anchorBar = false;
			}
			else
				anchorBar = false;
			
			if ((!applySessionOffset && lastBarTimeStamp2 != currentWeek) || (applySessionOffset && anchorBar))
			{	
				firstDayOfPeriod = true;
				sessionCount 	= 1;
				calcOpen		= false;
				initPlot 		= false;
				if (numberOfSessions == 1 || !rthVWAP || (rthVWAP && activeSession == anaSessionCountVWAPW42.First))
				{
					calcOpen			= true;
					initPlot 			= true;
					sessionBar			= 1; 
					volSum 				= 0.0;
					squareSum			= 0.0;
					firstBarOpen 		= Open[0];
					open				= Open[0] - firstBarOpen;
					high				= High[0] - firstBarOpen;
					low 				= Low[0] - firstBarOpen;
					close				= Close[0] - firstBarOpen;
					mean1				= 0.5*(high + low);
					mean2				= 0.5*(open + close);
					mean				= 0.5*(mean1 + mean2);
					currentVolSum 		= Volume[0];
					currentVWAP			= mean;
					if(currentBandType == anaBandTypeVWAPW42.Variance_Price)
					{	
						currentSquareSum 	= Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum - currentVWAP*currentVWAP) : 0;
					}	
					else if(currentBandType == anaBandTypeVWAPW42.Variance_Distance)
					{	
						open 				= open - currentVWAP;
						high 				= high - currentVWAP;
						low					= low - currentVWAP;
						close 				= close - currentVWAP;
						mean1 				= mean1 - currentVWAP;
						mean2 				= mean2 - currentVWAP;
						currentSquareSum 	= Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum) : 0;
					}
					else if(currentBandType == anaBandTypeVWAPW42.Session_Range)
					{
						currentHigh		= High[0];
						currentLow		= Low[0];
						offset 			= 0.25*(currentHigh - currentLow);
					}						
				}
				currentDate = lastBarTimeStamp1;
				currentWeek = lastBarTimeStamp2;
				plotVWAP = true;
			}
			else if (!applySessionOffset && lastBarTimeStamp1 != currentDate)
			{
				firstDayOfPeriod = false;
				sessionCount 	= 1;
				if (numberOfSessions == 1 || !rthVWAP || (rthVWAP && activeSession == anaSessionCountVWAPW42.First))
				{
					calcOpen			= true;
					initPlot			= true;
					sessionBar			= sessionBar + 1;
					volSum				= currentVolSum;
					squareSum 			= currentSquareSum;
					priorVWAP			= currentVWAP;
					open				= Open[0] - firstBarOpen;
					high				= High[0] - firstBarOpen;
					low 				= Low[0] - firstBarOpen;
					close				= Close[0] - firstBarOpen;
					mean1				= 0.5*(high + low);
					mean2				= 0.5*(open + close);
					mean				= 0.5*(mean1 + mean2);
					currentVolSum		= volSum + Volume[0];
					currentVWAP			= (currentVolSum > 0.5 ) ? (volSum*priorVWAP + Volume[0]*mean)/currentVolSum : mean;
					if(currentBandType == anaBandTypeVWAPW42.Variance_Price)
					{	
						currentSquareSum 	= squareSum + Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum - currentVWAP*currentVWAP) : 0;
					}	
					else if(currentBandType == anaBandTypeVWAPW42.Variance_Distance)
					{	
						open 				= open - currentVWAP;
						high 				= high - currentVWAP;
						low					= low - currentVWAP;
						close 				= close - currentVWAP;
						mean1 				= mean1 - currentVWAP;
						mean2 				= mean2 - currentVWAP;
						currentSquareSum 	= squareSum + Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum) : 0;
					}
					else if(currentBandType == anaBandTypeVWAPW42.Session_Range)
					{
						currentHigh			= Math.Max(currentHigh, High[0]);
						currentLow			= Math.Min(currentLow, Low[0]);
						offset 				= 0.25*(currentHigh - currentLow);
					}					
				}
				else
					calcOpen	= false;
				currentDate = lastBarTimeStamp1;
			}
			else if (!applySessionOffset && Bars.FirstBarOfSession) 
			{
				if (FirstTickOfBar)
				{
					sessionCount = sessionCount + 1;
					numberOfSessions = Math.Min(3, Math.Max(sessionCount, numberOfSessions));
				}
				if (rthVWAP && firstDayOfPeriod && (numberOfSessions == 1 || (sessionCount == 1 && activeSession == anaSessionCountVWAPW42.First) 
					|| (sessionCount == 2 && activeSession == anaSessionCountVWAPW42.Second) || (sessionCount == 3 && activeSession == anaSessionCountVWAPW42.Third)))
				{
					if (FirstTickOfBar)
					{
						calcOpen		= true;
						initPlot 		= true;
						sessionBar		= 1; 
						volSum 			= 0.0;
						squareSum		= 0.0;
					}
					open				= Open[0] - firstBarOpen;
					high				= High[0] - firstBarOpen;
					low 				= Low[0] - firstBarOpen;
					close				= Close[0] - firstBarOpen;
					mean1				= 0.5*(high + low);
					mean2				= 0.5*(open + close);
					mean				= 0.5*(mean1 + mean2);
					currentVolSum 		= Volume[0];
					currentVWAP			= mean;
					if(currentBandType == anaBandTypeVWAPW42.Variance_Price)
					{	
						currentSquareSum 	= Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum - currentVWAP*currentVWAP) : 0;
					}	
					else if(currentBandType == anaBandTypeVWAPW42.Variance_Distance)
					{	
						open 				= open - currentVWAP;
						high 				= high - currentVWAP;
						low					= low - currentVWAP;
						close 				= close - currentVWAP;
						mean1 				= mean1 - currentVWAP;
						mean2 				= mean2 - currentVWAP;
						currentSquareSum 	= Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum) : 0;
					}
					else if(currentBandType == anaBandTypeVWAPW42.Session_Range)
					{
						currentHigh			= High[0];
						currentLow			= Low[0];
						offset 				= 0.25*(currentHigh - currentLow);
					}		
				}
				else if (!rthVWAP || (rthVWAP && !firstDayOfPeriod && (numberOfSessions == 1 || (sessionCount == 1 && activeSession == anaSessionCountVWAPW42.First) 
					 || (sessionCount == 2 && activeSession == anaSessionCountVWAPW42.Second) || (sessionCount == 3 && activeSession == anaSessionCountVWAPW42.Third))))
				{
					if (FirstTickOfBar)
					{
						calcOpen		= true;
						initPlot 		= true;
						sessionBar		= sessionBar + 1;
						volSum			= currentVolSum;
						squareSum 		= currentSquareSum;
						priorVWAP		= currentVWAP;
					}
					open				= Open[0] - firstBarOpen;
					high				= High[0] - firstBarOpen;
					low 				= Low[0] - firstBarOpen;
					close				= Close[0] - firstBarOpen;
					mean1				= 0.5*(high + low);
					mean2				= 0.5*(open + close);
					mean				= 0.5*(mean1 + mean2);
					currentVolSum		= volSum + Volume[0];
					currentVWAP			= (currentVolSum > 0.5 ) ? (volSum*priorVWAP + Volume[0]*mean)/currentVolSum : mean;
					if(currentBandType == anaBandTypeVWAPW42.Variance_Price)
					{	
						currentSquareSum 	= squareSum + Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum - currentVWAP*currentVWAP) : 0;
					}	
					else if(currentBandType == anaBandTypeVWAPW42.Variance_Distance)
					{	
						open 				= open - currentVWAP;
						high 				= high - currentVWAP;
						low					= low - currentVWAP;
						close 				= close - currentVWAP;
						mean1 				= mean1 - currentVWAP;
						mean2 				= mean2 - currentVWAP;
						currentSquareSum 	= squareSum + Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
						offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum) : 0;
					}
					else if(currentBandType == anaBandTypeVWAPW42.Session_Range)
					{
						currentHigh			= Math.Max(currentHigh, High[0]);
						currentLow			= Math.Min(currentLow, Low[0]);
						offset 				= 0.25*(currentHigh - currentLow);
					}					
				}
				else
					calcOpen	= false;
			}
			else if (calcOpen)
			{
				if (FirstTickOfBar)
				{
					sessionBar 		= sessionBar + 1;
					volSum			= currentVolSum;
					squareSum 		= currentSquareSum;
					priorVWAP		= currentVWAP;
				}
				open				= Open[0] - firstBarOpen;
				high				= High[0] - firstBarOpen;
				low 				= Low[0] - firstBarOpen;
				close				= Close[0] - firstBarOpen;
				mean1				= 0.5*(high + low);
				mean2				= 0.5*(open + close);
				mean				= 0.5*(mean1 + mean2);
				currentVolSum		= volSum + Volume[0];
				currentVWAP			= (currentVolSum > 0.5 ) ? (volSum*priorVWAP + Volume[0]*mean)/currentVolSum : mean;
				if(currentBandType == anaBandTypeVWAPW42.Variance_Price)
				{	
					currentSquareSum 	= squareSum + Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
					offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum - currentVWAP*currentVWAP) : 0;
				}	
				else if(currentBandType == anaBandTypeVWAPW42.Variance_Distance)
				{	
					open 				= open - currentVWAP;
					high 				= high - currentVWAP;
					low					= low - currentVWAP;
					close 				= close - currentVWAP;
					mean1 				= mean1 - currentVWAP;
					mean2 				= mean2 - currentVWAP;
					currentSquareSum 	= squareSum + Volume[0]*(open*open + high*high + low*low + close*close + 2*mean2*mean2 + 2*mean1*mean1)/8.0;
					offset				= (currentVolSum > 0.5) ? Math.Sqrt(currentSquareSum/currentVolSum) : 0;
				}
				else if(currentBandType == anaBandTypeVWAPW42.Session_Range)
				{
					currentHigh			= Math.Max(currentHigh, High[0]);
					currentLow			= Math.Min(currentLow, Low[0]);
					offset 				= 0.25*(currentHigh - currentLow);
				}
			}

			if (plotVWAP && initPlot && !(rthVWAP && activeSession == anaSessionCountVWAPW42.Third && numberOfSessions == 2))
			{
				sessionVWAP = currentVWAP + firstBarOpen;
				SessionVWAP.Set(sessionVWAP);
				if (currentBandType == anaBandTypeVWAPW42.None)
				{
					UpperBand3.Reset();
					UpperBand2.Reset();
					UpperBand1.Reset();
					LowerBand1.Reset();
					LowerBand2.Reset();
					LowerBand3.Reset();
				}	
				else
				{
					UpperBand3.Set(sessionVWAP + multiplier3 * offset);
					UpperBand2.Set(sessionVWAP + multiplier2 * offset);
					UpperBand1.Set(sessionVWAP + multiplier1 * offset);
					LowerBand1.Set(sessionVWAP - multiplier1 * offset);
					LowerBand2.Set(sessionVWAP - multiplier2 * offset);
					LowerBand3.Set(sessionVWAP - multiplier3 * offset);
				}
				if (sessionBar == 1 && gap0)
					PlotColors[0][-Displacement] = Color.Transparent;
				else if (SessionVWAP[0] > SessionVWAP[1])
					PlotColors[0][-Displacement] = UpColor;
				else if (SessionVWAP[0] < SessionVWAP[1])
					PlotColors[0][-Displacement] = DownColor;
				else if(sessionBar == 2 && gap0)
					PlotColors[0][-Displacement] = UpColor;
				else
					PlotColors[0][-Displacement] = PlotColors[0][1-Displacement];
				if(sessionBar == 1 && gap1)
				{
					for (int i = 1; i <= 6; i++)
						PlotColors[i][-Displacement] = Color.Transparent;
				}
			}
			else
			{
				SessionVWAP.Reset();
				UpperBand3.Reset();
				UpperBand2.Reset();
				UpperBand1.Reset();
				LowerBand1.Reset();
				LowerBand2.Reset();
				LowerBand3.Reset();
			}	
		}

		#region Properties
		/// <summary>
		/// </summary>
		[Description("Multiplier used for inner VWAP bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Multiplier inner bands")]
		public double Multiplier1
		{
			get { return multiplier1; }
			set { multiplier1 = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Multiplier used for middle VWAP bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Multiplier middle bands")]
		public double Multiplier2
		{
			get { return multiplier2; }
			set { multiplier2 = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Multiplier used for outer VWAP bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Multiplier outer bands")]
		public double Multiplier3
		{
			get { return multiplier3; }
			set { multiplier3 = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("Session - ETH or RTH - used for calculating VWAP and bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Calculate from session")]
		public anaSessionTypeVWAPW42 SessionType
		{
			get { return sessionType; }
			set { sessionType = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Session # used for calculating OHL, noise and target bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Select RTH session")]
		public anaSessionCountVWAPW42 SelectedSession
		{
			get { return selectedSession; }
			set { selectedSession = value; }
		}

		/// <summary>
		/// </summary>
		[Description("Method used for calculating VWAP bands")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Band type")]
		public anaBandTypeVWAPW42 CurrentBandType
		{
			get { return currentBandType; }
			set { currentBandType = value; }
		}
		
		///<summary
		///</summary>
		[Browsable(false)]
		[XmlIgnore]
		public TimeSpan SessionOffset
		{
			get { return sessionOffset;}
			set { sessionOffset = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Enter positive offset for VWAP anchor bar relative to the start of the first session of the current week")]
		[GridCategory("Options")]
		[Gui.Design.DisplayNameAttribute("Offset ETH (+ d:h:min)")]
		public string S_SessionOffset	
		{
			get 
			{ 
				return string.Format("{0:D1}:{1:D2}:{2:D2}", sessionOffset.Days, sessionOffset.Hours, sessionOffset.Minutes);
			}
			set 
			{ 
				char[] delimiters = new char[] {':'};
				string[]values =((string)value).Split(delimiters, StringSplitOptions.None);
				sessionOffset = new TimeSpan(Convert.ToInt16(values[0]),Convert.ToInt16(values[1]),Convert.ToInt16(values[2]),0);
			}
		}
		
		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries SessionVWAP
		{
			get { return Values[0]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UpperBand3
		{
			get { return Values[1]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UpperBand2
		{
			get { return Values[2]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries UpperBand1
		{
			get { return Values[3]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LowerBand1
		{
			get { return Values[4]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LowerBand2
		{
			get { return Values[5]; }
		}

		/// <summary>
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public DataSeries LowerBand3
		{
			get { return Values[6]; }
		}

		/// <summary>
		/// </summary>
		[Description("Opacity for inner band area")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Opacity inner band area")]
		public int Opacity1
		{
			get { return opacity1; }
			set { opacity1 = Math.Min(Math.Max(0, value),10); }
		}

		/// <summary>
		/// </summary>
		[Description("Opacity for middle band area")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Opacity middle band area")]
		public int Opacity2
		{
			get { return opacity2; }
			set { opacity2 = Math.Min(Math.Max(0, value),10); }
		}

		/// <summary>
		/// </summary>
		[Description("Opacity for Outer Band Area")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Opacity outer band area")]
		public int Opacity3
		{
			get { return opacity3; }
			set { opacity3 = Math.Min(Math.Max(0, value),10); }
		}

		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for rising VWAP")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP rising")]
		public Color UpColor
		{
			get { return upColor; }
			set { upColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string UpColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(upColor); }
			set { upColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for falling VWAP")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP falling")]
		public Color DownColor
		{
			get { return downColor; }
			set { downColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string DownColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(downColor); }
			set { downColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for inner bands")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP inner bands")]
		public Color InnerBandColor
		{
			get { return innerBandColor; }
			set { innerBandColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string InnerBandColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(innerBandColor); }
			set { innerBandColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for middle bands")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP middle bands")]
		public Color MiddleBandColor
		{
			get { return middleBandColor; }
			set { middleBandColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string MiddleBandColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(middleBandColor); }
			set { middleBandColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for outer bands")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP outer bands")]
		public Color OuterBandColor
		{
			get { return outerBandColor; }
			set { outerBandColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string OuterBandColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(outerBandColor); }
			set { outerBandColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for inner band areas")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP inner band area")]
		public Color InnerAreaColor
		{
			get { return innerAreaColor; }
			set { innerAreaColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string InnerAreaColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(innerAreaColor); }
			set { innerAreaColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for middle band area")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP middle band area")]
		public Color MiddleAreaColor
		{
			get { return middleAreaColor; }
			set { middleAreaColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string MiddleAreaColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(middleAreaColor); }
			set { middleAreaColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[XmlIgnore]
		[Description("Select color for outer band area")]
		[Category("Plot Colors")]
		[Gui.Design.DisplayName("VWAP outer band area")]
		public Color OuterAreaColor
		{
			get { return outerAreaColor; }
			set { outerAreaColor = value; }
		}
		
		// Serialize Color object
		[Browsable(false)]
		public string OuterAreaColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(outerAreaColor); }
			set { outerAreaColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
		
		/// <summary>
		/// </summary>
		[Description("Width for VWAP Line.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width VWAP")]
		public int Plot0Width
		{
			get { return plot0Width; }
			set { plot0Width = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("PlotStyle for VWAP Line")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Plot style VWAP")]
		public PlotStyle Plot0Style
		{
			get { return plot0Style; }
			set { plot0Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for VWAP Line")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style VWAP")]
		public DashStyle Dash0Style
		{
			get { return dash0Style; }
			set { dash0Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("Line Width for VWAP Bands.")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Line width bands")]
		public int Plot1Width
		{
			get { return plot1Width; }
			set { plot1Width = Math.Max(1, value); }
		}

		/// <summary>
		/// </summary>
		[Description("DashStyle for VWAP Bands")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Plot style bands")]
		public PlotStyle Plot1Style
		{
			get { return plot1Style; }
			set { plot1Style = value; }
		}
		
		/// <summary>
		/// </summary>
		[Description("DashStyle for VWAP Bands")]
		[Category("Plot Parameters")]
		[Gui.Design.DisplayNameAttribute("Dash style bands")]
		public DashStyle Dash1Style
		{
			get { return dash1Style; }
			set { dash1Style = value; }
		}

		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
        [Category("Settlement next day for Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 01 /no trade date")]
		public DateTime PublicHoliday0
		{
			get { return publicHoliday0;}
			set { publicHoliday0 = value;}
		}	
	
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
        [Category("Settlement next day for Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 02 /no trade date")]
		public DateTime PublicHoliday1
		{
			get { return publicHoliday1;}
			set { publicHoliday1 = value;}
		}	
		
		///<summary
		///</summary>
		[Description("Enter dates for public holidays with no session close.")]
		[Category("Settlement next day for Globex")]
		[Gui.Design.DisplayNameAttribute("Holiday 03 /no trade date")]
		public DateTime PublicHoliday2
		{
			get { return publicHoliday2;}
			set { publicHoliday2 = value;}
		}	
		#endregion

		
		#region Miscellaneous
		
		public override string FormatPriceMarker(double price)
		{
			double trunc = Math.Truncate(price);
			int fraction = Convert.ToInt32(320 * Math.Abs(price - trunc) - 0.0001); // rounding down for ZF and ZT
			string priceMarker = "";
			if (TickSize == 0.03125) 
			{
				fraction = fraction/10;
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else 
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else if (TickSize == 0.015625 || TickSize == 0.0078125)
			{
				if (fraction < 10)
					priceMarker = trunc.ToString() + "'00" + fraction.ToString();
				else if (fraction < 100)
					priceMarker = trunc.ToString() + "'0" + fraction.ToString();
				else	
					priceMarker = trunc.ToString() + "'" + fraction.ToString();
			}
			else
				priceMarker = price.ToString(Gui.Globals.GetTickFormatString(TickSize));
			return priceMarker;
		}		
		
		private DateTime RoundUpTimeToPeriodTime(DateTime time, PivotRange pivotRange)
			
		{
			if (pivotRange == PivotRange.Weekly)
				return  time.AddDays(6 - Convert.ToInt16(time.DayOfWeek)).Date;
			else if (pivotRange == PivotRange.Monthly)
			{
				DateTime result = new DateTime(time.Year, time.Month, 1); 
				return result.AddMonths(1).AddDays(-1);
			}
			else
				return time;
		}

		private DateTime GetLastBarSessionDate(DateTime time, Data.Bars bars, int barsAgo, PivotRange pivotRange)
		{
			if (pivotRange == PivotRange.Daily && time > cacheSessionEndTmp1)
			{
				isEndOfWeekHoliday = false;
				if (Bars.BarsType.IsIntraday)
					sessionDateTmp = Bars.GetTradingDayFromLocal(time);
				else
					sessionDateTmp = time.Date;
				for (int i =0; i<3; i++)
				{
					if (publicHoliday[i].Date == sessionDateTmp)
						isEndOfWeekHoliday = true;
				}
				if(cacheSessionDate != sessionDateTmp) 
					cacheSessionDate = sessionDateTmp;
				Bars.Session.GetNextBeginEnd(bars, barsAgo, out cacheSessionBeginTmp, out cacheSessionEndTmp1);
			}
			else if (pivotRange == PivotRange.Weekly && time > cacheSessionEndTmp2) 
			{
				extendPublicHoliday = false;
				for (int i =0; i<3; i++)
				{
					if (publicHoliday[i].Date == sessionDateTmp)
						extendPublicHoliday = true;
				}
				if (Bars.BarsType.IsIntraday)
					sessionDateTmp = Bars.GetTradingDayFromLocal(time);
				else
					sessionDateTmp = time.Date;
				DateTime tmpWeeklyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp, PivotRange.Weekly);
				if(extendPublicHoliday)
					tmpWeeklyEndDate = RoundUpTimeToPeriodTime(sessionDateTmp.AddDays(1), PivotRange.Weekly);
				if (tmpWeeklyEndDate != cacheWeeklyEndDate || (rthVWAP && numberOfSessions > 1 && firstDayOfPeriod && sessionCount == 1 && activeSession == anaSessionCountVWAPW42.Second)
					|| (rthVWAP && numberOfSessions > 2 && firstDayOfPeriod && sessionCount == 2 && activeSession == anaSessionCountVWAPW42.Third)) 
				{
					cacheWeeklyEndDate = tmpWeeklyEndDate;
					if (newSessionBarIdxArr2.Count == 0 || (newSessionBarIdxArr2.Count > 0 && CurrentBar > (int) newSessionBarIdxArr2[newSessionBarIdxArr2.Count - 1]))
						newSessionBarIdxArr2.Add(CurrentBar);
				}
				Bars.Session.GetNextBeginEnd(bars, barsAgo, out cacheSessionBeginTmp, out cacheSessionEndTmp2);
			}	
			if (pivotRange == PivotRange.Daily) 
				return sessionDateTmp;
			else	
				return RoundUpTimeToPeriodTime(sessionDateTmp, PivotRange.Weekly);

		}
		
		/// <summary>
        /// Overload this method to handle the termination of an indicator. Use this method to dispose of any resources vs overloading the Dispose() method.
		/// </summary>
		protected override void OnTermination()
		{
			innerAreaBrush.Dispose();
			middleAreaBrush.Dispose();
			outerAreaBrush.Dispose();
		}
		
		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			if (Bars == null || ChartControl == null || this.LastBarIndexPainted < BarsRequired)
				return;
			
			int lastBarIndex		= this.LastBarIndexPainted;
			int firstBarIndex 		= Math.Max(BarsRequired, this.FirstBarIndexPainted);
			
			do
			{	
				while (lastBarIndex > firstBarIndex && !Values[0].IsValidPlot(lastBarIndex))
					lastBarIndex = lastBarIndex - 1;
				if (currentBandType == anaBandTypeVWAPW42.None || !Values[0].IsValidPlot(lastBarIndex))
					break; 
				int	firstBarIdxToPaint	= -1;
				for (int i = newSessionBarIdxArr2.Count - 1; i >= 0; i--)
				{
					int prevSessionBreakIdx = (int) newSessionBarIdxArr2[i];
					if (prevSessionBreakIdx + Displacement <= lastBarIndex) 
					{
						firstBarIdxToPaint 	= prevSessionBreakIdx + Displacement; 
						break;
					}
				}
				int firstBarPlotIndex = Math.Max(firstBarIndex, firstBarIdxToPaint);
				
				if(opacity1 > 0)
				{
					using (GraphicsPath	path = new GraphicsPath())
					{
						SmoothingMode oldSmoothingMode 	= graphics.SmoothingMode;
						int x 							= 0;
						int y 							= 0;
						int lastX						= -1;
						int lastY						= -1;
						int lastIdx						= 0;
						for (int idx = lastBarIndex; idx >= firstBarPlotIndex; idx--)    
						{
							if (idx - Displacement >= Bars.Count) 
								continue;
							else if (idx - Displacement < 0)
								break;
							if (!Values[3].IsValidPlot(idx))
								break;
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[3].Get(idx));
							if (idx < lastBarIndex)
								path.AddLine(lastX, lastY, x, y);
							lastX = x;
							lastY	= y;
							lastIdx = idx;
						}
						for (int idx = lastIdx; idx <= lastBarIndex; idx++) 
						{
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[4].Get(idx));
							if(idx == lastIdx)
								path.AddLine(x, lastY, x, y);
							else	
								path.AddLine(lastX, lastY, x, y);
							lastX	= x;
							lastY	= y;
						}							
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.FillPath(innerAreaBrush, path);
						graphics.SmoothingMode = oldSmoothingMode;
					}
				}
					
				if(opacity2 > 0)
				{
					using (GraphicsPath	path = new GraphicsPath())
					{
						SmoothingMode oldSmoothingMode 	= graphics.SmoothingMode;
						int x 							= 0;
						int y 							= 0;
						int lastX						= -1;
						int lastY						= -1;
						int lastIdx						= 0;
						for (int idx = lastBarIndex; idx >= firstBarPlotIndex; idx--)    
						{
							if (idx - Displacement >= Bars.Count) 
								continue;
							else if (idx - Displacement < 0)
								break;
							if (!Values[2].IsValidPlot(idx))
								break;
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[2].Get(idx));
							if (idx < lastBarIndex)
								path.AddLine(lastX, lastY, x, y);
							lastX = x;
							lastY	= y;
							lastIdx = idx;
						}
						for (int idx = lastIdx; idx <= lastBarIndex; idx++) 
						{
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[3].Get(idx));
							if(idx == lastIdx)
								path.AddLine(x, lastY, x, y);
							else	
								path.AddLine(lastX, lastY, x, y);
							lastX	= x;
							lastY	= y;
						}							
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.FillPath(middleAreaBrush, path);
						graphics.SmoothingMode = oldSmoothingMode;
					}					
					using (GraphicsPath	path = new GraphicsPath())
					{
						SmoothingMode oldSmoothingMode 	= graphics.SmoothingMode;
						int x 							= 0;
						int y 							= 0;
						int lastX						= -1;
						int lastY						= -1;
						int lastIdx						= 0;
						for (int idx = lastBarIndex; idx >= firstBarPlotIndex; idx--)    
						{
							if (idx - Displacement >= Bars.Count) 
								continue;
							else if (idx - Displacement < 0)
								break;
							if (!Values[4].IsValidPlot(idx))
								break;
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[4].Get(idx));
							if (idx < lastBarIndex)
								path.AddLine(lastX, lastY, x, y);
							lastX = x;
							lastY	= y;
							lastIdx = idx;
						}
						for (int idx = lastIdx; idx <= lastBarIndex; idx++) 
						{
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[5].Get(idx));
							if(idx == lastIdx)
								path.AddLine(x, lastY, x, y);
							else	
								path.AddLine(lastX, lastY, x, y);
							lastX	= x;
							lastY	= y;
						}							
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.FillPath(middleAreaBrush, path);
						graphics.SmoothingMode = oldSmoothingMode;
					}	
				}
				
				if(opacity3 > 0)
				{
					using (GraphicsPath	path = new GraphicsPath())
					{
						SmoothingMode oldSmoothingMode 	= graphics.SmoothingMode;
						int x 							= 0;
						int y 							= 0;
						int lastX						= -1;
						int lastY						= -1;
						int lastIdx						= 0;
						for (int idx = lastBarIndex; idx >= firstBarPlotIndex; idx--)    
						{
							if (idx - Displacement >= Bars.Count) 
								continue;
							else if (idx - Displacement < 0)
								break;
							if (!Values[1].IsValidPlot(idx))
								break;
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[1].Get(idx));
							if (idx < lastBarIndex)
								path.AddLine(lastX, lastY, x, y);
							lastX = x;
							lastY	= y;
							lastIdx = idx;
						}
						for (int idx = lastIdx; idx <= lastBarIndex; idx++) 
						{
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[2].Get(idx));
							if(idx == lastIdx)
								path.AddLine(x, lastY, x, y);
							else	
								path.AddLine(lastX, lastY, x, y);
							lastX	= x;
							lastY	= y;
						}							
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.FillPath(outerAreaBrush, path);
						graphics.SmoothingMode = oldSmoothingMode;
					}					
					using (GraphicsPath	path = new GraphicsPath())
					{
						SmoothingMode oldSmoothingMode 	= graphics.SmoothingMode;
						int x 							= 0;
						int y 							= 0;
						int lastX						= -1;
						int lastY						= -1;
						int lastIdx						= 0;
						for (int idx = lastBarIndex; idx >= firstBarPlotIndex; idx--)    
						{
							if (idx - Displacement >= Bars.Count) 
								continue;
							else if (idx - Displacement < 0)
								break;
							if (!Values[5].IsValidPlot(idx))
								break;
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[5].Get(idx));
							if (idx < lastBarIndex)
								path.AddLine(lastX, lastY, x, y);
							lastX = x;
							lastY	= y;
							lastIdx = idx;
						}
						for (int idx = lastIdx; idx <= lastBarIndex; idx++) 
						{
							x	= ChartControl.GetXByBarIdx(BarsArray[0], idx);
							y 	= ChartControl.GetYByValue(this, Values[6].Get(idx));
							if(idx == lastIdx)
								path.AddLine(x, lastY, x, y);
							else	
								path.AddLine(lastX, lastY, x, y);
							lastX	= x;
							lastY	= y;
						}							
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
						graphics.FillPath(outerAreaBrush, path);
						graphics.SmoothingMode = oldSmoothingMode;
					}	
				}
				lastBarIndex = firstBarIdxToPaint - 1;
			}
			while (lastBarIndex > firstBarIndex);
			
			base.Plot(graphics, bounds, min, max);
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
        private anaCurrentWeekVWAPV42[] cacheanaCurrentWeekVWAPV42 = null;

        private static anaCurrentWeekVWAPV42 checkanaCurrentWeekVWAPV42 = new anaCurrentWeekVWAPV42();

        /// <summary>
        /// anaCurrentWeekVWAPV42.
        /// </summary>
        /// <returns></returns>
        public anaCurrentWeekVWAPV42 anaCurrentWeekVWAPV42(anaBandTypeVWAPW42 currentBandType, double multiplier1, double multiplier2, double multiplier3, string s_SessionOffset, anaSessionCountVWAPW42 selectedSession, anaSessionTypeVWAPW42 sessionType)
        {
            return anaCurrentWeekVWAPV42(Input, currentBandType, multiplier1, multiplier2, multiplier3, s_SessionOffset, selectedSession, sessionType);
        }

        /// <summary>
        /// anaCurrentWeekVWAPV42.
        /// </summary>
        /// <returns></returns>
        public anaCurrentWeekVWAPV42 anaCurrentWeekVWAPV42(Data.IDataSeries input, anaBandTypeVWAPW42 currentBandType, double multiplier1, double multiplier2, double multiplier3, string s_SessionOffset, anaSessionCountVWAPW42 selectedSession, anaSessionTypeVWAPW42 sessionType)
        {
            if (cacheanaCurrentWeekVWAPV42 != null)
                for (int idx = 0; idx < cacheanaCurrentWeekVWAPV42.Length; idx++)
                    if (cacheanaCurrentWeekVWAPV42[idx].CurrentBandType == currentBandType && Math.Abs(cacheanaCurrentWeekVWAPV42[idx].Multiplier1 - multiplier1) <= double.Epsilon && Math.Abs(cacheanaCurrentWeekVWAPV42[idx].Multiplier2 - multiplier2) <= double.Epsilon && Math.Abs(cacheanaCurrentWeekVWAPV42[idx].Multiplier3 - multiplier3) <= double.Epsilon && cacheanaCurrentWeekVWAPV42[idx].S_SessionOffset == s_SessionOffset && cacheanaCurrentWeekVWAPV42[idx].SelectedSession == selectedSession && cacheanaCurrentWeekVWAPV42[idx].SessionType == sessionType && cacheanaCurrentWeekVWAPV42[idx].EqualsInput(input))
                        return cacheanaCurrentWeekVWAPV42[idx];

            lock (checkanaCurrentWeekVWAPV42)
            {
                checkanaCurrentWeekVWAPV42.CurrentBandType = currentBandType;
                currentBandType = checkanaCurrentWeekVWAPV42.CurrentBandType;
                checkanaCurrentWeekVWAPV42.Multiplier1 = multiplier1;
                multiplier1 = checkanaCurrentWeekVWAPV42.Multiplier1;
                checkanaCurrentWeekVWAPV42.Multiplier2 = multiplier2;
                multiplier2 = checkanaCurrentWeekVWAPV42.Multiplier2;
                checkanaCurrentWeekVWAPV42.Multiplier3 = multiplier3;
                multiplier3 = checkanaCurrentWeekVWAPV42.Multiplier3;
                checkanaCurrentWeekVWAPV42.S_SessionOffset = s_SessionOffset;
                s_SessionOffset = checkanaCurrentWeekVWAPV42.S_SessionOffset;
                checkanaCurrentWeekVWAPV42.SelectedSession = selectedSession;
                selectedSession = checkanaCurrentWeekVWAPV42.SelectedSession;
                checkanaCurrentWeekVWAPV42.SessionType = sessionType;
                sessionType = checkanaCurrentWeekVWAPV42.SessionType;

                if (cacheanaCurrentWeekVWAPV42 != null)
                    for (int idx = 0; idx < cacheanaCurrentWeekVWAPV42.Length; idx++)
                        if (cacheanaCurrentWeekVWAPV42[idx].CurrentBandType == currentBandType && Math.Abs(cacheanaCurrentWeekVWAPV42[idx].Multiplier1 - multiplier1) <= double.Epsilon && Math.Abs(cacheanaCurrentWeekVWAPV42[idx].Multiplier2 - multiplier2) <= double.Epsilon && Math.Abs(cacheanaCurrentWeekVWAPV42[idx].Multiplier3 - multiplier3) <= double.Epsilon && cacheanaCurrentWeekVWAPV42[idx].S_SessionOffset == s_SessionOffset && cacheanaCurrentWeekVWAPV42[idx].SelectedSession == selectedSession && cacheanaCurrentWeekVWAPV42[idx].SessionType == sessionType && cacheanaCurrentWeekVWAPV42[idx].EqualsInput(input))
                            return cacheanaCurrentWeekVWAPV42[idx];

                anaCurrentWeekVWAPV42 indicator = new anaCurrentWeekVWAPV42();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.CurrentBandType = currentBandType;
                indicator.Multiplier1 = multiplier1;
                indicator.Multiplier2 = multiplier2;
                indicator.Multiplier3 = multiplier3;
                indicator.S_SessionOffset = s_SessionOffset;
                indicator.SelectedSession = selectedSession;
                indicator.SessionType = sessionType;
                Indicators.Add(indicator);
                indicator.SetUp();

                anaCurrentWeekVWAPV42[] tmp = new anaCurrentWeekVWAPV42[cacheanaCurrentWeekVWAPV42 == null ? 1 : cacheanaCurrentWeekVWAPV42.Length + 1];
                if (cacheanaCurrentWeekVWAPV42 != null)
                    cacheanaCurrentWeekVWAPV42.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheanaCurrentWeekVWAPV42 = tmp;
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
        /// anaCurrentWeekVWAPV42.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaCurrentWeekVWAPV42 anaCurrentWeekVWAPV42(anaBandTypeVWAPW42 currentBandType, double multiplier1, double multiplier2, double multiplier3, string s_SessionOffset, anaSessionCountVWAPW42 selectedSession, anaSessionTypeVWAPW42 sessionType)
        {
            return _indicator.anaCurrentWeekVWAPV42(Input, currentBandType, multiplier1, multiplier2, multiplier3, s_SessionOffset, selectedSession, sessionType);
        }

        /// <summary>
        /// anaCurrentWeekVWAPV42.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaCurrentWeekVWAPV42 anaCurrentWeekVWAPV42(Data.IDataSeries input, anaBandTypeVWAPW42 currentBandType, double multiplier1, double multiplier2, double multiplier3, string s_SessionOffset, anaSessionCountVWAPW42 selectedSession, anaSessionTypeVWAPW42 sessionType)
        {
            return _indicator.anaCurrentWeekVWAPV42(input, currentBandType, multiplier1, multiplier2, multiplier3, s_SessionOffset, selectedSession, sessionType);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// anaCurrentWeekVWAPV42.
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.anaCurrentWeekVWAPV42 anaCurrentWeekVWAPV42(anaBandTypeVWAPW42 currentBandType, double multiplier1, double multiplier2, double multiplier3, string s_SessionOffset, anaSessionCountVWAPW42 selectedSession, anaSessionTypeVWAPW42 sessionType)
        {
            return _indicator.anaCurrentWeekVWAPV42(Input, currentBandType, multiplier1, multiplier2, multiplier3, s_SessionOffset, selectedSession, sessionType);
        }

        /// <summary>
        /// anaCurrentWeekVWAPV42.
        /// </summary>
        /// <returns></returns>
        public Indicator.anaCurrentWeekVWAPV42 anaCurrentWeekVWAPV42(Data.IDataSeries input, anaBandTypeVWAPW42 currentBandType, double multiplier1, double multiplier2, double multiplier3, string s_SessionOffset, anaSessionCountVWAPW42 selectedSession, anaSessionTypeVWAPW42 sessionType)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.anaCurrentWeekVWAPV42(input, currentBandType, multiplier1, multiplier2, multiplier3, s_SessionOffset, selectedSession, sessionType);
        }
    }
}
#endregion
