#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
	partial class Strategy
    {
		/*
		public enum BarData {Open, High, Low, Close, Volume}; 
		public enum ExtremaType {Max, Min}; 
		public enum HistoricalData {YdyOpen, YdyHigh, YdyLow, YdyClose, ONHigh, ONLow, IBHigh, IBLow}; 
		public enum ValueAreaData {Y_POC, Y_VAH, Y_VAL, Y_VWAP}; 
		
		#region Variables
        
		private TimeSpan _pitSessionOpenTime	   = new TimeSpan (09,00,00);
		private TimeSpan _pitSessionCloseTime	   = new TimeSpan (14,30,00);
		private TimeSpan _pitSessionEarlyCloseTime = new TimeSpan (13,30,00);
		private TimeSpan _initialBalanceStartTime  = new TimeSpan (09,00,00);
		private TimeSpan _initialBalanceEndTime	   = new TimeSpan (10,00,00);
		
		private double _valueAreaPercent 		   = 0.70d; 
		
		private int _maxTicksFromSrLine			   = 5; 
		private int _minTicksToNextLine			   = 10; 
		
		private DateTime _o_nOpen, _o_nClose, _ibStart, _ibEnd, _currentDayPitOpen;
		
		private int lineWidth = 2, textBars = 100, pixelOffset = 5; 
		
		private bool _showHistoricalPlot  = true; 
		private bool _isTdyPitSessionOpen = false; 
		
		private Color histLineColor      = Color.Cyan;
		private DashStyle dStyle         = DashStyle.Solid;
		private Color _histDataTextColor = Color.Black; 
		
		private Dictionary<HistoricalData, double> _histData   = new Dictionary<HistoricalData, double>(); 
		private Dictionary<ValueAreaData, double> _valAreaData = new Dictionary<ValueAreaData, double>(); 
		
        #endregion
			
		#region dataValidation
		
		public void _validateHistoricalDataInputs()
		{
			if (_pitSessionOpenTime.Seconds != 0)
				Print(String.Format("{0} Pit session open time must be a round number of minutes (seconds must equal zero).",this._StratName));	
			
			if (_pitSessionCloseTime.Seconds != 0)
				Print(String.Format("{0} Pit session close time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
			if (_pitSessionEarlyCloseTime.Seconds != 0)
				Print(String.Format("{0} Pit session early close time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
			if (_initialBalanceStartTime.Seconds != 0)
				Print(String.Format("{0} Initial balance start time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
			if (_initialBalanceEndTime.Seconds != 0)
				Print(String.Format("{0} Initial balance end time must be a round number of minutes (seconds must equal zero).",this._StratName));
	
			if (_pitSessionCloseTime.CompareTo(_pitSessionOpenTime) != 1)
				Print(String.Format("{0} Pit session close time must be strictly greater than pit session open time.",this._StratName));
			
			if (_pitSessionCloseTime.CompareTo(_pitSessionEarlyCloseTime) != 1)
				Print(String.Format("{0} Pit session close time must be strictly greater than pit session early close time.",this._StratName));
			
			if (_initialBalanceEndTime.CompareTo(_initialBalanceStartTime) != 1)
				Print(String.Format("{0} Initial balance start time must be strictly greater than initial balance end time.",this._StratName));
			
			if ((_valueAreaPercent <= 0) || (_valueAreaPercent > 1.0d) )
				Print(String.Format("{0} valueAreaPercent must be greater than zero and less than or equal to 1.0.", this._StratName));
		}
		#endregion //closes dataValidation
		
		#region historicalData
		
		//GetBar returns the number of bars ago the DateTime occurred.  
		//Bars.GetBar returns the number of the bar that occurred at the DateTime.
		//Use of Bars.GetBar necessitates subtracting from CurrentBar to get the number of bars ago. 
		//CaptureTime should be adjusted for the local time zone before being passed to method. 
		public double _getBarDataAtTime(int barInProgress, DateTime captureTime, BarData bd)
		{
			int barNumber  = Bars.GetBar(captureTime);
			int barsAgo    = CurrentBars[barInProgress] - barNumber; 
			
			switch (bd)
			{
				case BarData.Open:
					return(Opens[barInProgress][barsAgo]); 
				case BarData.High:
					return(Highs[barInProgress][barsAgo]); 
				case BarData.Low:
					return(Lows[barInProgress][barsAgo]); 
				case BarData.Close:
					return(Closes[barInProgress][barsAgo]); 
				case BarData.Volume:
					return(Volumes[barInProgress][barsAgo]); 
				default:
					return(0.0); 
			}
		}
		
		//startTime and endTime should be adjusted for the local time zone before being passed to method. 
		public double _getMaxOrMinBetweenTimes(int barInProgress, DateTime startTime, DateTime endTime, ExtremaType et)
		{
			int barNumber    = Bars.GetBar(startTime);
			int barsAgoStart = CurrentBars[barInProgress] - barNumber; 
			
			barNumber        = Bars.GetBar(endTime);
			int barsAgoEnd   = CurrentBars[barInProgress] - barNumber; 
			
			double extremeValue = 0.0d; 
			if (et == ExtremaType.Max)
			{
				extremeValue = Double.MinValue;
				for(int j = barsAgoStart; j >= barsAgoEnd; j--)
				{
					if(extremeValue < Highs[barInProgress][j]) extremeValue = Highs[barInProgress][j];
				}
				
			}
			if (et == ExtremaType.Min)
			{
				extremeValue = Double.MaxValue;
				for(int j = barsAgoStart; j >= barsAgoEnd; j--)
				{
					if(extremeValue > Lows[barInProgress][j])  extremeValue = Lows[barInProgress][j];
				}
			}
	
			return(extremeValue); 
		}
		
		public int _getBarInProgressFromAttributes(PeriodType pt, int value)
		{
			int i = 0; 
			foreach (Bars b in BarsArray)
			{
				if ((b.Period.Id == pt) && (b.Period.Value == value)) return(i); 
				i++; 
			}
			
			return(0); 
		}
		
		//startTime and endTime should be adjusted for the local time zone before being passed to method. 
		public Dictionary<double, double> _getPriceAndVolumeBetweenTimes(DateTime startTime, DateTime endTime)
		{
			int barInProgress = this._getBarInProgressFromAttributes(PeriodType.Tick, 1); 
			
			int barNumber     = BarsArray[barInProgress].GetBar(startTime);
			int barsAgoStart  = CurrentBars[barInProgress] - barNumber; 
			
			barNumber         = BarsArray[barInProgress].GetBar(endTime);
			int barsAgoEnd    = CurrentBars[barInProgress] - barNumber; 
		
			Dictionary<double, double> d = new Dictionary<double, double>(); 
			double volume; 
			for(int j = barsAgoStart; j >= barsAgoEnd; j--)
			{
				if(d.TryGetValue(Closes[barInProgress][j], out volume))
				{
					d[Closes[barInProgress][j]] = volume + Volumes[barInProgress][j]; 
				}
				else
				{
					d.Add(Closes[barInProgress][j], Volumes[barInProgress][j]); 
				}
			}
			
			return(d); 
		}
		
		public double _getPointOfControl(Dictionary<double, double> d)
		{
			if (d.Count() == 0) return(0.0);
			
			double dPoc = 0; 
			double maxVolume = Double.MinValue; 
			foreach (KeyValuePair<double, double> kvp in d)
			{
				if (kvp.Value > maxVolume)
				{
					maxVolume = kvp.Value;
					dPoc      = kvp.Key; 
				}
			}	
			return(dPoc); 
		}
		
		public double _getVWAP(Dictionary<double, double> d, double sumVolume)
		{		
			if (d.Count() == 0) return(0.0); 
			
			double sumPrd = 0;
			foreach (KeyValuePair<double, double> kvp in d)
			{
				sumPrd = sumPrd + (kvp.Key * kvp.Value); 
			}	
			return(sumPrd/sumVolume); 
		}
		
		public KeyValuePair<double, double> _getValueAreaHighAndLow(double poc, double totalVolume, double vaPercent, List<double> prices, List<double> volumes)
		{
			if ((prices.Count == 0) || (volumes.Count == 0) || (prices.Count != volumes.Count) || (poc == 0.0) ) return(new KeyValuePair<double, double>(0.0, 0.0)); 
			
			int numBrackets  = 2; 
			int pocIdx 		 = prices.IndexOf(poc); 
			int upIdx  		 = pocIdx;
			int dnIdx  		 = pocIdx; 
			
			double vaVolume  = volumes[pocIdx]; //start with the volume at POC
			int targetVol    = Convert.ToInt32(totalVolume * vaPercent); 
			double upVol     = 0.0d;
			double dnVol     = 0.0d; 
			
			bool isTargetHit = false; 
			
			while (vaVolume < targetVol)
			{
				if ((!isTargetHit) && (upIdx < (volumes.Count - numBrackets)) && (dnIdx >= numBrackets))
				{
					upVol = volumes[upIdx + 1] + volumes[upIdx + 2]; 
					dnVol = volumes[dnIdx + 1] + volumes[dnIdx + 2]; 
					
					if ( ((vaVolume + upVol) >= targetVol) || ((vaVolume + dnVol) >= targetVol)) 
					{
						isTargetHit = true; 
					}
					else
					{
						if (upVol > dnVol)
						{
							upIdx+=numBrackets; 
							vaVolume+=upVol; 
						}
						else if (upVol < dnVol)
						{
							dnIdx-=numBrackets; 
							vaVolume+=dnVol; 
						}
						else if ((vaVolume + upVol + dnVol) >= targetVol)
						{
							isTargetHit = true; 
						}
						else
						{
							upIdx+=numBrackets; 
							dnIdx-=numBrackets;
							vaVolume+=(upVol + dnVol); 
						}	
					}
				}
				else
				{
					if (upIdx == (prices.Count - (numBrackets - 1)))
					{
						vaVolume+=volumes[dnIdx - (numBrackets - 1)]; 	
						dnIdx-=(numBrackets - 1); 
					}
					else if (dnIdx == 0)
					{
						vaVolume+=volumes[upIdx - (numBrackets - 1)]; 
						upIdx+=(numBrackets - (numBrackets - 1)); 
					}
					else
					{
						upVol = volumes[upIdx + (numBrackets - 1)];
						dnVol = volumes[dnIdx - (numBrackets - 1)];
						
						if (upVol > dnVol)
						{
							upIdx+=(numBrackets - 1); 
							vaVolume+=upVol;
						}
						else if (upVol < dnVol)
						{
							dnIdx-=numBrackets; 
							vaVolume+=dnVol; 
						}
						else
						{
							upIdx+=(numBrackets - 1); 
							dnIdx-=(numBrackets - 1); 
							vaVolume+=(upVol + dnVol);
						}
					}	
				}
			}

			double vaHigh = prices[upIdx];  
			double vaLow  = prices[dnIdx]; 
			
			return(new KeyValuePair<double, double>(vaHigh, vaLow)); 
		}
		
		public void _setHistoricalData()
		{	
			if (this._UseCustomChart) _histDataTextColor = Color.Blue;
			
			DateTime currentTime = this._EventTime.Add(this._TimeZoneOffset); 
			_currentDayPitOpen   = DateTime.Now.Date.Add(_pitSessionOpenTime).Subtract(this._TimeZoneOffset); 
			
			TimeSpan sundayOpen  = new TimeSpan(18,00,00); 
			
			_o_nClose = currentTime.Date.Add(_pitSessionOpenTime).AddSeconds(-1).Subtract(this._TimeZoneOffset); //one minute before today's pit session opens
			_ibStart  = currentTime.Date.Add(_initialBalanceStartTime).Subtract(this._TimeZoneOffset);  
			_ibEnd 	  = currentTime.Date.Add(_initialBalanceEndTime).Subtract(this._TimeZoneOffset);
			
			_histData.Add(HistoricalData.IBHigh, Double.MinValue);
			_histData.Add(HistoricalData.IBLow,  Double.MaxValue);
			
			DateTime priorDay;
			
			if (currentTime.DayOfWeek == DayOfWeek.Monday)
			{
				priorDay = currentTime.AddDays(-3).Date; //if prior day is Sunday, use pit session from Friday 
				_o_nOpen  = priorDay.Add(sundayOpen).Subtract(this._TimeZoneOffset); 
			} 
			else
			{
				priorDay = currentTime.AddDays(-1).Date; 
				_o_nOpen  = priorDay.Date.Add(_pitSessionCloseTime).AddSeconds(1).Subtract(this._TimeZoneOffset);   //one minute after yesterday's pit close
			}
			
			DateTime priorPitOpen = priorDay.Date.Add(_pitSessionOpenTime).AddMinutes(1).Subtract(this._TimeZoneOffset); //add one minute since the timestamp refers to the time a bar closed
			DateTime priorPitCls  = priorDay.Date.Add(_pitSessionCloseTime).Subtract(this._TimeZoneOffset); 
			if (this.isBondMarketEarlyCloseDay(priorDay.Date)) priorPitCls  = priorDay.Date.Add(_pitSessionEarlyCloseTime).Subtract(this._TimeZoneOffset); 
			
			double keyValue       = _getBarDataAtTime(BarsInProgress, priorPitOpen, BarData.Open); 
			_histData.Add(HistoricalData.YdyOpen, keyValue); 
			
			keyValue			  = _getBarDataAtTime(BarsInProgress, priorPitCls, BarData.Close);
			_histData.Add(HistoricalData.YdyClose, keyValue);
			
			keyValue			  = _getMaxOrMinBetweenTimes(BarsInProgress, priorPitOpen, priorPitCls, ExtremaType.Max);
			_histData.Add(HistoricalData.YdyHigh, keyValue);
			
			keyValue			  = _getMaxOrMinBetweenTimes(BarsInProgress, priorPitOpen, priorPitCls, ExtremaType.Min);
			_histData.Add(HistoricalData.YdyLow, keyValue);
			
			keyValue			  = _getMaxOrMinBetweenTimes(BarsInProgress, _o_nOpen, _o_nClose, ExtremaType.Max);
			_histData.Add(HistoricalData.ONHigh, keyValue);
			
			keyValue			  = _getMaxOrMinBetweenTimes(BarsInProgress, _o_nOpen, _o_nClose, ExtremaType.Min);
			_histData.Add(HistoricalData.ONLow, keyValue);
			
			if (currentTime >= _ibStart)
			{
				keyValue = _getMaxOrMinBetweenTimes(BarsInProgress, _ibStart, _ibEnd, ExtremaType.Max);
				_histData[HistoricalData.IBHigh] = keyValue;
			
				keyValue = _getMaxOrMinBetweenTimes(BarsInProgress, _ibStart, _ibEnd, ExtremaType.Min);
				_histData[HistoricalData.IBLow]  = keyValue;
			}
			
			//populate tick and volume data list for ydy pit session.  Get POC, VWAP, VAH, VAL
			Dictionary<double, double> ydyPitData = new Dictionary<double, double>(); 
			ydyPitData = _getPriceAndVolumeBetweenTimes(priorPitOpen, priorPitCls); 
			
			double totalVolume = ydyPitData.Sum(x => x.Value); 
			
			_valAreaData[ValueAreaData.Y_POC]  = _getPointOfControl(ydyPitData); 
			_valAreaData[ValueAreaData.Y_VWAP] = _getVWAP(ydyPitData, totalVolume); 
			
			ydyPitData = ydyPitData.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);   //sort by price
			List<double> pxLevels = ydyPitData.Keys.ToList(); 
			List<double> volumes  = ydyPitData.Values.ToList(); 
			
			KeyValuePair<double, double> vaHighAndLow = _getValueAreaHighAndLow(_valAreaData[ValueAreaData.Y_POC], totalVolume, _valueAreaPercent, pxLevels, volumes); 
			_valAreaData[ValueAreaData.Y_VAH] = vaHighAndLow.Key;
			_valAreaData[ValueAreaData.Y_VAL] = vaHighAndLow.Value;
			 
			if (_showHistoricalPlot)
			{
				_plotHistoricalLines(); 
				_updateValueArea(); 
			}
		}
				
		public void _plotHistoricalLines()
		{		
			_updateHistoricalPlots(HistoricalData.YdyOpen, "Y-Open",  _histData[HistoricalData.YdyOpen]); 
			_updateHistoricalPlots(HistoricalData.YdyHigh, "Y-High",  _histData[HistoricalData.YdyHigh]); 
			_updateHistoricalPlots(HistoricalData.YdyLow,  "Y-Low",   _histData[HistoricalData.YdyLow]); 
			_updateHistoricalPlots(HistoricalData.YdyClose,"Y-Cls",   _histData[HistoricalData.YdyClose]); 
			_updateHistoricalPlots(HistoricalData.ONHigh,  "O/N-High",_histData[HistoricalData.ONHigh]); 
			_updateHistoricalPlots(HistoricalData.ONLow,   "O/N-Low", _histData[HistoricalData.ONLow]); 

			if (this._EventTime >= _ibStart)
			{
				_updateHistoricalPlots(HistoricalData.IBHigh, "IB-High", _histData[HistoricalData.IBHigh]); 
				_updateHistoricalPlots(HistoricalData.IBLow,  "IB-Low",  _histData[HistoricalData.IBLow]); 
			}
		}
		
		public void _updateValueArea()
		{
			_drawValueArea("Y-POC",  _valAreaData[ValueAreaData.Y_POC]);
			_drawValueArea("Y-VAH",  _valAreaData[ValueAreaData.Y_VAH]); 
			_drawValueArea("Y-VAL",  _valAreaData[ValueAreaData.Y_VAL]);  
			_drawValueArea("Y-VWAP", _valAreaData[ValueAreaData.Y_VWAP]);
		}
		
		public void _drawValueArea(string drawTag, double pValue)
		{
			DrawLine(drawTag + "_line", this._AutoScale, textBars, pValue, -1, pValue, Color.DarkSlateBlue, dStyle, lineWidth); 
			//DrawLine(drawTag + "_line", this._AutoScale, _currentDayPitOpen, pValue, this._EventTime, pValue, Color.DarkSlateBlue, dStyle, lineWidth);
			DrawText(drawTag, this._AutoScale, drawTag, 2, pValue, pixelOffset, _histDataTextColor, new Font("Arial", 6), StringAlignment.Near, Color.Aqua, Color.Lime, 5);	
		}
		
		public void _updateHistoricalPlots(HistoricalData hd, string drawTag, double pValue)
		{
			DrawLine(hd.ToString(), this._AutoScale, textBars, pValue, -1, pValue, histLineColor, dStyle, lineWidth); 
			//DrawLine(hd.ToString(), this._AutoScale, _currentDayPitOpen, pValue, this._EventTime, pValue, histLineColor, dStyle, lineWidth); 
			DrawText(drawTag, this._AutoScale, drawTag, 2, pValue, pixelOffset, _histDataTextColor, new Font("Arial", 6), StringAlignment.Near, Color.Aqua, Color.Lime, 5);	
		}
		
		public void _recalculateHistoricalData()
		{
			double keyValue = 0.0d; 
			if (this._EventTime <= this._O_nClose)
			{
				keyValue = _getMaxOrMinBetweenTimes(BarsInProgress, _o_nOpen, _o_nClose, ExtremaType.Max);
				_histData[HistoricalData.ONHigh] = keyValue; 
				
				keyValue = _getMaxOrMinBetweenTimes(BarsInProgress, _o_nOpen, _o_nClose, ExtremaType.Min);
				_histData[HistoricalData.ONLow] = keyValue; 
				
				if (_showHistoricalPlot)
				{
					_updateHistoricalPlots(HistoricalData.ONHigh, "O/N-High", _histData[HistoricalData.ONHigh]); 
					_updateHistoricalPlots(HistoricalData.ONLow, "O/N-Low",   _histData[HistoricalData.ONLow]); 
				}
			}	
			
			if ((this._EventTime >= _ibStart) && (this._EventTime <= this._ibEnd))
			{
				keyValue = _getMaxOrMinBetweenTimes(BarsInProgress, _ibStart, _ibEnd, ExtremaType.Max);
				_histData[HistoricalData.IBHigh] = keyValue; 
				keyValue = _getMaxOrMinBetweenTimes(BarsInProgress, _ibStart, _ibEnd, ExtremaType.Min);
				_histData[HistoricalData.IBLow] = keyValue; 
				
				if (this._ShowHistoricalPlot)
				{
					_updateHistoricalPlots(HistoricalData.IBHigh, "IB-High", _histData[HistoricalData.IBHigh]); 
					_updateHistoricalPlots(HistoricalData.IBLow, "IB-Low",   _histData[HistoricalData.IBLow]);
				}
			}	
		}
		
		#endregion //closes historicalData
		
		#region entryLevelVersusSrLines
		
		public bool _isEntryPriceNearSRLine(double entryPrice)
		{
			foreach (KeyValuePair<HistoricalData, double> kvp in _histData)
			{
				if (Math.Abs(entryPrice - kvp.Value) <= (this._maxTicksFromSrLine * TickSize)) return(true); 
			}
			foreach (KeyValuePair<ValueAreaData, double> kvp in _valAreaData)
			{
				if (Math.Abs(entryPrice - kvp.Value) <= (this._maxTicksFromSrLine * TickSize)) return(true); 
			}
			return (false); 	
		}
		
		public bool _isEntryPriceOutsideSRLevels(OrderAction oa, double entryPrice)
		{
			List<double> hdList = _histData.Values.ToList();
			List<double> vaList = _valAreaData.Values.ToList();
			
			List<double> allLines = hdList.Concat(vaList).ToList();   
			
			allLines.Sort(); 
			
			for (int i = 0; i < allLines.Count; i++)
			{
				if (Math.Abs(entryPrice - allLines[i]) <= (this._maxTicksFromSrLine * TickSize))
				{
					if (oa == OrderAction.Buy)
					{
						if (i == (allLines.Count - 1))
						{
							return(true); 
						}
						else
						{
							if ((allLines[i+1] - entryPrice) >= this._minTicksToNextLine * TickSize) return(true); 
						}
						
					}
					else
					{
						if (i == 0)
						{
							return(true); 
						}
						else
						{
							if ((entryPrice - allLines[i+1]) >= this._minTicksToNextLine * TickSize) return(true); 
						}
					}
				
				}
			}
			
			return (false); 	
		}
		
		#endregion //closes entryLevelVersusSrLines
		
		#region properties
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Is Tdy Pit Session Open")]
		public bool _IsTdyPitSessionOpen
		{
			get { return _isTdyPitSessionOpen; }
			set { _isTdyPitSessionOpen = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Show Historical Plot")]
		public bool _ShowHistoricalPlot
		{
			get { return _showHistoricalPlot; }
			set { _showHistoricalPlot = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Key Historical Data")]
		public Dictionary<HistoricalData, double> _HistData
		{
			get { return _histData; }
			set { _histData = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Value Area Data")]
		public Dictionary<ValueAreaData, double> _ValAreaData
		{
			get { return _valAreaData; }
			set { _valAreaData = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Value Area Percent")]
		public double _ValueAreaPercent
		{
			get { return _valueAreaPercent; }
			set { _valueAreaPercent = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Maximum Ticks from a Line for Entry")]
		public int _MaxTicksFromSrLine
		{
			get { return _maxTicksFromSrLine; }
			set { _maxTicksFromSrLine = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Minimum Ticks to Secondary Line for Entry")]
		public int _MinTicksToNextLine
		{
			get { return _minTicksToNextLine; }
			set { _minTicksToNextLine = value; }
		}
	
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Tdy Session Start Time")]
		public DateTime _CurrentDayPitOpen
		{
			get { return _currentDayPitOpen; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("O/N Session Start Time")]
		public DateTime _O_nOpen
		{
			get { return _o_nOpen; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("O/N Session End Time")]
		public DateTime _O_nClose
		{
			get { return _o_nClose; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Initial Balance Start Time")]
		public DateTime _IbStart
		{
			get { return _ibStart; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Initial Balance End Time")]
		public DateTime _IbEnd
		{
			get { return _ibEnd; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Pit Session Open")]
		[GridCategory("Session Parameters")]
		public TimeSpan _PitSessionOpenTime
		{
			get { return _pitSessionOpenTime; }
			set { _pitSessionOpenTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Pit Session Close")]
		[GridCategory("Session Parameters")]
		public TimeSpan _PitSessionCloseTime
		{
			get { return _pitSessionCloseTime; }
			set { _pitSessionCloseTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Pit Session Early Close")]
		[GridCategory("Session Parameters")]
		public TimeSpan _PitSessionEarlyCloseTime
		{
			get { return _pitSessionEarlyCloseTime; }
			set { _pitSessionEarlyCloseTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Initial Balance Start Time")]
		[GridCategory("Session Parameters")]
		public TimeSpan _InitialBalanceStartTime
		{
			get { return _initialBalanceStartTime; }
			set { _initialBalanceStartTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Initial Balance End Time")]
		[GridCategory("Session Parameters")]
		public TimeSpan _InitialBalanceEndTime
		{
			get { return _initialBalanceEndTime; }
			set { _initialBalanceEndTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Hist Data Text Color")]
		public Color _HistDataTextColor
		{
			get { return _histDataTextColor; }
			set { _histDataTextColor = value; }
		}
		
		[Browsable(false)]
		public string _HistDataTextColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString( _histDataTextColor); }
			set { _histDataTextColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}
			
		[Browsable(false)]
		public string serial_PitSessionOpenTime
		{
			get {return _pitSessionOpenTime.ToString();}	
			set {_pitSessionOpenTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string serial_PitSessionCloseTime
		{
			get {return _pitSessionCloseTime.ToString();}	
			set {_pitSessionCloseTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string serial_PitSessionEarlyCloseTime
		{
			get {return _pitSessionEarlyCloseTime.ToString();}	
			set {_pitSessionEarlyCloseTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string serial_InitialBalanceStartTime
		{
			get {return _initialBalanceStartTime.ToString();}	
			set {_initialBalanceStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string serial_InitialBalanceEndTime
		{
			get {return _initialBalanceEndTime.ToString();}	
			set {_initialBalanceEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		#endregion //closes properties
		*/
    }
}
