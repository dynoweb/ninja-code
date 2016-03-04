#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This file holds all user defined strategy methods.
    /// </summary>
    partial class Strategy
    {
		public enum StrategyTimeZone {Pacific, Mountain, Central, Eastern, UTC};
		
		#region variables
			
		private TimeSpan _firstSessionStartTime    = new TimeSpan (09,30,00); 
		private TimeSpan _firstSessionEndTime 	   = new TimeSpan (12,00,00);
		private TimeSpan _secondSessionStartTime   = new TimeSpan (12,00,00); 
		private TimeSpan _secondSessionEndTime 	   = new TimeSpan (23,30,00);
		private TimeSpan _prdBeforeEndToFlatten	   = new TimeSpan (00,15,00);
		
		private TimeSpan _timeZoneOffset 		   = new TimeSpan (00,00,00); 
		private StrategyTimeZone _strategyTimeZone = StrategyTimeZone.Eastern; 
		private int _numBizDaysToRollBeforeExpiry = 1; 
		
		private bool _hasSession1Ended;
		private bool _hasSession2Ended;
		
		private DateTime _session1StartTime, _session1EndTime, _session2StartTime, _session2EndTime, _lastNewEntryTime;
		
		#endregion //closes variables
		
		#region dataValidation
		
		public void _validateTimeParameters()
		{
			if (Instrument.MasterInstrument.InstrumentType == InstrumentType.Future)
			{
				string codePrefix    = Instrument.MasterInstrument.Name; //get first two letters of code (i.e. CLM5 returns CL)
				char monthCode       = this.futureTickerMonthCode(Instrument.Expiry.Month); 
				int mnth             = this.futuresCodeToMonthNumber(monthCode);
				int yr               = Instrument.Expiry.Year; 
				
				AssetClass assetCls  = this.getInstrumentAssetClass(codePrefix);
				MarketName mktName   = this.getInstrumentMarketName(codePrefix); 
				
				DateTime anchorDate  = new DateTime(yr, mnth, 1); 
				DateTime lastTrdDate = this.futuresLastTradingDate(anchorDate, codePrefix, mktName, assetCls); 
				DateTime dateToRoll  = this.addBusinessDaysToDate(lastTrdDate, -1 * _numBizDaysToRollBeforeExpiry, mktName, assetCls);  
				
				Print(String.Format("{0} Last trading date for {1} futures contract is {2:MMMM d yyyy}.", this._StratName, codePrefix, lastTrdDate)); 
				Print(String.Format("{0} Contract should be rolled forward on or before {1:MMMM d yyyy}.", this._StratName, dateToRoll)); 
				Print(String.Empty);
	
				if (DateTime.Now.Date >= dateToRoll)
				{
					string dataMsg = String.Format("{0} Contract is either unsupported or within {1} days of expiration.  Roll to next contract.", 
						this._StratName, _numBizDaysToRollBeforeExpiry); 
					Print(dataMsg); 	
					DrawTextFixed("dataMsg", dataMsg, TextPosition.BottomRight, Color.Navy, new Font("Arial", 12), Color.LightGray, Color.LightGreen, 6);
				}
				
				int daysLoaded = Bars.BarsData.DaysBack;
				if (daysLoaded < 3) 
				{
					string dataMsg = String.Format("{0} Chart loads {1} days of data.  Needs {2} days of history to calculate ATR factors.",
						this._StratName, daysLoaded, 3);
					Print(dataMsg);
					DrawTextFixed("dataMsg", dataMsg, TextPosition.BottomRight, Color.Navy, new Font("Arial", 12), Color.LightGray, Color.LightGreen, 6); 
				}
			}
			
			if (_session1StartTime.Second != 0)
				Print(String.Format("{0} Session1 start time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
			if (_firstSessionStartTime.Seconds != 0)
				Print(String.Format("{0} Session1 end time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
			if (_secondSessionStartTime.Seconds != 0)
				Print(String.Format("{0} Session2 start time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
			if (_secondSessionEndTime.Seconds != 0)
				Print(String.Format("{0} Session2 end time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
//			if (_endOfNewEntriesTime.Seconds != 0)
//				Print(String.Format("{0} Last new entry time must be a round number of minutes (seconds must equal zero).",this._StratName));
			
			if (_firstSessionEndTime.CompareTo(_firstSessionStartTime) != 1)
				Print(String.Format("{0} Session1 start time must be strictly less than Session1 end time.",this._StratName));
			
			if (_secondSessionEndTime.CompareTo(_secondSessionStartTime) != 1)
				Print(String.Format("{0} Session2 start time must be strictly less than Session2 end time.",this._StratName));
			
			if (_secondSessionStartTime.CompareTo(_firstSessionEndTime) == -1)
				Print(String.Format("{0} Session2 start time must be greater than or equal to Session1 end time.",this._StratName));
			
//			if (_endOfNewEntriesTime.CompareTo(_secondSessionEndTime) == 1)
//				Print(String.Format("{0} Last new entry time must be less than or equal to Session2 end time.",this._StratName));
		}
		
		#endregion //closes dataValidation 
		
		#region timeZoneAdjustments
		
		private TimeSpan _getOffsetBetweenTimeZones(DateTime timeToConvert, string fromZone, string toZone )
		{
			DateTime specTime = DateTime.SpecifyKind(timeToConvert.Date, DateTimeKind.Unspecified);
			DateTime destTime = _convertSourceTimeToDestinationTime(specTime, fromZone, toZone);
			return (destTime - specTime);
		}

		private DateTime _convertSourceTimeToDestinationTime(DateTime timeToConvert, 
			string sourceTimeZoneID, string destinationTimeZoneID)
		{
			try
			{
				DateTime destinationTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(timeToConvert, 
					sourceTimeZoneID, destinationTimeZoneID);
				return (destinationTime);
			}
			catch
			{
				return (timeToConvert);
			}
		}
		
		private string _mapStrategyTimeZoneToWindowsID(StrategyTimeZone stz)
        {
            switch (stz)
            {
                case StrategyTimeZone.Pacific: 		return ("Pacific Standard Time");
          		case StrategyTimeZone.Mountain: 	return ("Mountain Standard Time");
          		case StrategyTimeZone.Central: 		return ("Central Standard Time");
          		case StrategyTimeZone.Eastern: 		return ("Eastern Standard Time");
          		case StrategyTimeZone.UTC: 			return ("UTC");
          		default: 							return ("UTC");
            }
        }
		
		public void _setLocalTimeZoneOffset()
		{
			TimeSpan ts           = new TimeSpan(0,0,0); 
			
			StrategyTimeZone myTZ = _strategyTimeZone;
			string stratTzString  = _mapStrategyTimeZoneToWindowsID(myTZ);
			_timeZoneOffset 	  = _getOffsetBetweenTimeZones(DateTime.Now, TimeZoneInfo.Local.Id.ToString(), stratTzString); 
		}
		
		public void _setValidTradingTimes(DateTime currentTime)
		{	
			_session1StartTime 	= currentTime.Date.Add(_firstSessionStartTime); 
			_session1EndTime    = currentTime.Date.Add(_firstSessionEndTime); 
			_session2StartTime 	= currentTime.Date.Add(_secondSessionStartTime); 
			_session2EndTime    = currentTime.Date.Add(_secondSessionEndTime); 
			_lastNewEntryTime   = _session2EndTime.Subtract(_prdBeforeEndToFlatten); 
			
			Print(String.Format("{0} Local time zone: {1}", this._StratName, TimeZoneInfo.Local.Id ));
			Print(String.Format("{0} Strategy time zone: {1}", this._StratName, _strategyTimeZone));
			
			Print(String.Format("{0} Session 1 start time: {1:MMM d yyyy h:mm}. Session 1 end time: {2:T}", this._StratName, _session1StartTime, _session1EndTime ));
			Print(String.Format("{0} Session 2 start time: {1:T}. Last entry allowed at: {2:T}. Session 2 end time: {3:T}", 
				this._StratName, _session2StartTime, _lastNewEntryTime, _session2EndTime));
		}
		
		#endregion //closes timeZoneAdjustments	
		
		#region properties
		
		[Browsable(false)]
		[Description("Session1 Start Time")]
		public DateTime _Session1StartTime
		{
			get { return _session1StartTime; }
		}
		
		[Browsable(false)]
		[Description("Session1 End Time")]
		public DateTime _Session1EndTime
		{
			get { return _session1EndTime; }
		}
		
		[Browsable(false)]
		[Description("Session2 Start Time")]
		public DateTime _Session2StartTime
		{
			get { return _session2StartTime; }
		}
		
		[Browsable(false)]
		[Description("Session2 End Time")]
		public DateTime _Session2EndTime
		{
			get { return _session2EndTime; }
		}
		
		[Browsable(false)]
		[Description("Last New Entry Time")]
		public DateTime _LastNewEntryTime
		{
			get { return _lastNewEntryTime; }
		}
		
		[Browsable(false)]
		[Description("Time Zone Offset")]
		public TimeSpan _TimeZoneOffset
		{
			get { return _timeZoneOffset; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Strategy Time Zone")]
		public StrategyTimeZone _StrategyTimeZone
		{
			get { return _strategyTimeZone; }
			set { _strategyTimeZone = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Number of Days Early to Roll")]
		public int _NumBizDaysToRollBeforeExpiry
		{
			get { return _numBizDaysToRollBeforeExpiry; }
			set { _numBizDaysToRollBeforeExpiry = Math.Max(1, value); }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Has First Session Ended")]
		public bool _HasSession1Ended
		{
			get { return _hasSession1Ended; }
			set { _hasSession1Ended = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Has Second Session Ended")]
		public bool _HasSession2Ended
		{
			get { return _hasSession2Ended; }
			set { _hasSession2Ended = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("First Session Start Time")]
		public TimeSpan _FirstSessionStartTime
		{
			get { return _firstSessionStartTime; }
			set { _firstSessionStartTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("First Session End Time")]
		public TimeSpan _FirstSessionEndTime
		{
			get { return _firstSessionEndTime; }
			set { _firstSessionEndTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Second Session Start Time")]
		public TimeSpan _SecondSessionStartTime
		{
			get { return _secondSessionStartTime; }
			set { _secondSessionStartTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Second Session End Time")]
		public TimeSpan _SecondSessionEndTime
		{
			get { return _secondSessionEndTime; }
			set { _secondSessionEndTime = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Time before End of Session to Stop New Entries")]
		public TimeSpan _PrdBeforeEndToFlatten
		{
			get { return _prdBeforeEndToFlatten; }
			set { _prdBeforeEndToFlatten = value; }
		}	
		
		[Browsable(false)]
		public string serial_FirstSessionStartTime
		{
			get {return _firstSessionStartTime.ToString();}	
			set {_firstSessionStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string serial_FirstSessionEndTime
		{
			get {return _firstSessionEndTime.ToString();}	
			set {_firstSessionEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string serial_SecondSessionStartTime
		{
			get {return _secondSessionStartTime.ToString();}	
			set {_secondSessionStartTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		[Browsable(false)]
		public string serial_SecondSessionEndTime
		{
			get {return _secondSessionEndTime.ToString();}	
			set {_secondSessionEndTime = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}

		[Browsable(false)]
		public string serial_PrdBeforeEndToFlatten
		{
			get {return _prdBeforeEndToFlatten.ToString();}	
			set {_prdBeforeEndToFlatten = string.IsNullOrEmpty(value) ? TimeSpan.Zero : TimeSpan.Parse(value);}
		}
		
		#endregion //closes properties
    }
}
