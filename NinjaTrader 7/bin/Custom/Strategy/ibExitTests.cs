#region Using declarations
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
using System.Drawing.Drawing2D;
using NinjaTrader.Gui.Chart;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    partial class Strategy
    {
		public enum ExitReason {EndOfFirstSession, EndOfSession, MaxTradeDuration, MaxSessionLoss, StopLoss, TakeProfit, TrailExit, None};
		public enum CancelReason {MaxTime, OppSideFill, OppSideSwing, SrCxlPrice, TradingHalt}; 
		
		#region variables
		
		private double _percentTakeProfit_Trigger1	= 0.40d; 
		private double _percentTakeProfit_Trigger2	= 0.50d;
		private double _percentTakeProfit_Trigger3	= 0.90d;
		
		private double _percentTakeProfit_Exit1		= 0.00d;
		private double _percentTakeProfit_Exit2		= 0.25d;
		private double _percentTakeProfit_Exit3		= 0.80d;

		private double _stepInterval				= 0.25d; 
		
		private double _takeProfit 					= 0.0d;
		private double _stopLoss					= 0.0d; 
		
		private int _minTarget_InTicks				= 10;
		private int _maxTarget_InTicks				= 1000;
		
		// Rick added these
		private string _StratName = "RICK DID THIS";
		private int _TradeNumber = 0;
		private int _PrevOpenCont = 0;
		private int _OpenContracts = 0;
		
		
		private bool _useTrailingStops 				= true;
		private bool _exitAtMarketClose				= true; 
		private bool _exitOrderSent					= false; 
		private bool _isTradingHalted				= false; 
		private bool _isPositionClosed              = false; 
		
		private double _positionPnl 				= 0.0d; //pnl in ticks
		private double _dollarPnl 					= 0.0d; //pnl in dollars (adjusted for position size)
		private double _sessionPnl 					= 0.0d; 
		
		private int _consecutiveWLForTradingHalt    = 3; 
		private int _consecutiveWinners			    = 0;
		private int _consecutiveLosers              = 0; 
		private int _numTrail  						= 0;
		private int _exitLevel 						= 0; 
		private bool _hitHighestTrail				= false;
		private ExitReason _exitReason 				= ExitReason.None; 
		
		private List<double> _positionPnlList       = new List<double>(); 
		private List<double> _dollarPnlList         = new List<double>(); 
		private List<double> _sessionPnlList        = new List<double>(); 
		private List<double> _trailTriggerList      = new List<double>();
		private List<double> _trailExitList         = new List<double>();
		//private Dictionary<double, double> _trailStopLevels = new Dictionary<double, double>();
		
		private double _trailExit1, _trailExit2, _trailExit3; 
		
		private ILine _takeProfitLine, _stopExitLine; 
		
		#endregion //closes variables
		
		/*
		#region dataValidation
		
		public void _validateExitParameters()
		{
			if ((_percentTakeProfit_Trigger1 <= 0) || (_percentTakeProfit_Trigger1 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Trigger1 must be greater than zero and less than or equal to 1.0.", this._StratName));

			if ((_percentTakeProfit_Trigger2 <= 0) || (_percentTakeProfit_Trigger2 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Trigger2 must be greater than zero and less than or equal to 1.0.", this._StratName));
			
			if ((_percentTakeProfit_Trigger3 <= 0) || (_percentTakeProfit_Trigger3 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Trigger3 must be greater than zero and less than or equal to 1.0.", this._StratName));
			
			if ((_percentTakeProfit_Exit1 < 0) || (_percentTakeProfit_Exit1 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Exit1 must be greater than or equal to zero and less than or equal to 1.0.", this._StratName));

			if ((_percentTakeProfit_Exit2 <= 0) || (_percentTakeProfit_Exit2 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Exit2 must be greater than zero and less than or equal to 1.0.", this._StratName));
			
			if ((_percentTakeProfit_Exit3 <= 0) || (_percentTakeProfit_Exit2 > 1.0d) )
				Print(String.Format("{0} percentTakeProfit_Exit3 must be greater than zero and less than or equal to 1.0.", this._StratName));
			
			if ((_stepInterval <= 0) || (_stepInterval > 1.0d) )
				Print(String.Format("{0} stepInterval must be greater than zero and less than or equal to 1.0.", this._StratName));
			
			if (_percentTakeProfit_Trigger2 <= _percentTakeProfit_Trigger1)
				Print(String.Format("{0} percentTakeProfit_Trigger2 ({1:N2}) must be strictly greater than percentTakeProfit_Trigger1 ({2:N2}).",
					this._StratName, _percentTakeProfit_Trigger2, _percentTakeProfit_Trigger1));
			
			if (_percentTakeProfit_Trigger3 <= _percentTakeProfit_Trigger2)
				Print(String.Format("{0} percentTakeProfit_Trigger3 ({1:N2}) must be strictly greater than percentTakeProfit_Trigger2 ({2:N2}).",
					this._StratName, _percentTakeProfit_Trigger3, _percentTakeProfit_Trigger2));
			
			if (_percentTakeProfit_Exit2 <= _percentTakeProfit_Exit1)
				Print(String.Format("{0} percentTakeProfit_Exit2 ({1:N2}) must be strictly greater than percentTakeProfit_Exit1 ({2:N2}).",
					this._StratName, _percentTakeProfit_Exit2, _percentTakeProfit_Exit1));
			
			if (_percentTakeProfit_Exit3 <= _percentTakeProfit_Exit2)
				Print(String.Format("{0} percentTakeProfit_Exit3 ({1:N2}) must be strictly greater than percentTakeProfit_Exit2 ({2:N2}).",
					this._StratName, _percentTakeProfit_Exit3, _percentTakeProfit_Exit2));
			
			if (_percentTakeProfit_Trigger1 <= _percentTakeProfit_Exit1)
				Print(String.Format("{0} percentTakeProfit_Trigger1 ({1:N2}) must be strictly greater than percentTakeProfit_Exit1 ({2:N2}).",
					this._StratName, _percentTakeProfit_Trigger1, _percentTakeProfit_Exit1));
			
			if (_percentTakeProfit_Trigger2 <= _percentTakeProfit_Exit2)
				Print(String.Format("{0} percentTakeProfit_Trigger2 ({1:N2}) must be strictly greater than percentTakeProfit_Exit2 ({2:N2}).",
					this._StratName, _percentTakeProfit_Trigger2, _percentTakeProfit_Exit2));
			
			if (_percentTakeProfit_Trigger3 <= _percentTakeProfit_Exit3)
				Print(String.Format("{0} percentTakeProfit_Trigger3 ({1:N2}) must be strictly greater than percentTakeProfit_Exit3 ({2:N2}).",
					this._StratName, _percentTakeProfit_Trigger3, _percentTakeProfit_Exit3));		
		}
		
		#endregion //closes dataValidation
		
		#region exitTests
		
		public double _getLastTradePnl(List<double> tradePnlList)
		{
			double pnl = 0.0d; 
			
			if (tradePnlList.Count == 1) return(Instrument.MasterInstrument.Round2TickSize(tradePnlList[0]));
			else if (tradePnlList.Count > 1)  
			{
				int idx = this._TradeNumber - 1; 
				return(Instrument.MasterInstrument.Round2TickSize(tradePnlList[idx] - tradePnlList[idx - 1])); 
			}	
			
			return(0.0d);
		}
		
		public void _handleExitFills(IExecution ex, ref IOrder orderToClose)
		{
			this._PrevOpenCont  = this._OpenContracts; 
			this._OpenContracts = this._adjustOpenContracts(this._OpenContracts, ex.Quantity, ex.Order.OrderAction);
			
			this._printOrderStateString(ex);  
			
			_dollarPnl += ( -1 * (this._OpenContracts - this._PrevOpenCont) * (ex.Price - this._PositionAvgPrice) * ex.Order.Instrument.MasterInstrument.PointValue); 
			_dollarPnlList.Add(_dollarPnl);
			
			if ((this._OpenContracts == 0) || (Position.MarketPosition == MarketPosition.Flat))
			{	
				_sessionPnl += _dollarPnl;
				_sessionPnlList.Add(_sessionPnl);
				_exitOrderSent    = false; 
				_isPositionClosed = true;
				this._printPositionClosingMsg(ex); 
				this._resetFlags();  
				
				if ((orderToClose != null) && (ex.Order == orderToClose)) orderToClose = null;
			}
		}
		
		public void _resetFlags()
		{	
			this._CanSendNewEntry = true;
			_exitOrderSent  	  = false; 
			_exitReason		      = ExitReason.None;
			
			_dollarPnl 	          = 0; 
			_positionPnl 	      = 0;
			this._FillCount 	  = 0;
			this._OpenContracts   = 0; 
			this._PrevOpenCont 	  = 0;
			
			_dollarPnlList.Clear();
			_positionPnlList.Clear();
			
			this._PendingEntry = PendingEntryType.None;

			_numTrail 			  = 0; 
			_exitLevel 		      = 0; 
			_hitHighestTrail      = false; 
			
			this.RemoveDrawObject(_takeProfitLine); 
			this.RemoveDrawObject(_stopExitLine); 
			this.RemoveDrawObject(this._EntryPointLine); 
		}
		
		public void _setTrailExitLevels()
		{
			Dictionary<double, double> trailStopLevels = new Dictionary<double, double>(); 
			
			_trailTriggerList.Clear(); 
			_trailExitList.Clear(); 
			
			double trailTrigger1   = Instrument.MasterInstrument.Round2TickSize(this._percentTakeProfit_Trigger1 * this._takeProfit);
			double trailTrigger2   = Instrument.MasterInstrument.Round2TickSize(this._percentTakeProfit_Trigger2 * this._takeProfit);
			double trailTrigger3   = Instrument.MasterInstrument.Round2TickSize(this._percentTakeProfit_Trigger3 * this._takeProfit);
			
			_trailExit1            = Instrument.MasterInstrument.Round2TickSize(this._percentTakeProfit_Exit1 * this._takeProfit);
			_trailExit2            = Instrument.MasterInstrument.Round2TickSize(this._percentTakeProfit_Exit2 * this._takeProfit);
			_trailExit3            = Instrument.MasterInstrument.Round2TickSize(this._percentTakeProfit_Exit3 * this._takeProfit);
			
			double distToTrigger3  = this._percentTakeProfit_Trigger3 - this._percentTakeProfit_Trigger2; 
			double distToExit3     = this._percentTakeProfit_Exit3 - this._percentTakeProfit_Exit2; 
			
			double pnlToTrigger3   = Instrument.MasterInstrument.Round2TickSize(this._takeProfit * distToTrigger3); 
			double pnlToExit3      = Instrument.MasterInstrument.Round2TickSize(this._takeProfit * distToExit3); 
			
			double stepInPnl	   = Instrument.MasterInstrument.Round2TickSize(Math.Max(TickSize, (this._stepInterval * pnlToTrigger3))); 
			
			int numIntervals       = Math.Max(1, (int)(pnlToTrigger3/stepInPnl) - 1);
			
			_trailTriggerList.Add(trailTrigger1);
			_trailTriggerList.Add(trailTrigger2);
				
			_trailExitList.Add(_trailExit1);
			_trailExitList.Add(_trailExit2);
			
			double trigg, exit; 
			for (int i = 0; i < numIntervals; i++)
			{
				trigg = trailTrigger2 + (stepInPnl * (i + 1));
				exit  = _trailExit2 + (stepInPnl * (i + 1));
				_trailTriggerList.Add(Instrument.MasterInstrument.Round2TickSize(trigg)); 
				_trailExitList.Add(Instrument.MasterInstrument.Round2TickSize(exit)); 
			}
			
			_trailTriggerList.Add(trailTrigger3);
			_trailExitList.Add(_trailExit3);

//			//_trailStopLevels.Add(trailTrigger1, _trailExit1); 
//			//_trailStopLevels.Add(trailTrigger2, _trailExit2);
//			
//			double trigg, exit; 
//			for (int i = 0; i < numIntervals; i++)
//			{
//				trigg = Instrument.MasterInstrument.Round2TickSize(trailTrigger2 + (stepInPnl * (i + 1)));
//				exit  = Instrument.MasterInstrument.Round2TickSize(_trailExit2    + (stepInPnl * (i + 1)));
//				
//				_trailStopLevels.Add(trigg, exit); 
//			}
			
			//_trailStopLevels.Add(trailTrigger3, _trailExit3);	
			
			//sort dictionary to ensure triggers are in order
			//_trailStopLevels = _trailStopLevels.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
		}
		
		public void _updateExitLevels(IExecution ex)
		{
			double tpLevel 			= ex.Price + (this._SignOfContracts * this._takeProfit);
			double stopLevel		= ex.Price - (this._SignOfContracts * this._stopLoss);
			double trExit1 			= ex.Price + (this._SignOfContracts * this._trailExit1);
			double trExit2 			= ex.Price + (this._SignOfContracts * this._trailExit2);
			double trExit3 			= ex.Price + (this._SignOfContracts * this._trailExit3);
			
			DateTime adjustedTime   = ex.Time; //.Subtract(this._TimeZoneOffset);
			
			_takeProfitLine  		= DrawLine("takeProfitLine", this._AutoScale, adjustedTime, tpLevel, 
										adjustedTime.Add(this._LineDuration), tpLevel, Color.Green, DashStyle.Solid, 2); 
			_stopExitLine			= DrawLine("stopExitLine", this._AutoScale, adjustedTime, stopLevel, 
										adjustedTime.Add(this._LineDuration), stopLevel, Color.Red, DashStyle.Solid, 2);  
			
			int tCnt   = this._TradeNumber;
	
			if (this._useTrailingStops)
			{
				Print(String.Format("{0} TrdNum: {1}. Direction: {2}. Take profit at {3:N2}. Stop loss at {4:N2}. Trail Exit1 at {5:N2}. Trail Exit2 at {6:N2}. Trail Exit3 at {7:N2}", 
					this._StratName, tCnt, ex.MarketPosition, tpLevel, stopLevel, trExit1, trExit2, trExit3)); 
			}
			else
			{
				Print(String.Format("{0} Trade number: {1}. Direction: {2}. Take profit at {3:N2}. Stop loss at {4:N2}. Trailing stops not used.", 
					this._StratName, tCnt, ex.MarketPosition, tpLevel, stopLevel)); 	
			} 	
		}
		
		private void _calculatePnl(IPosition position)
		{
			int positionSign = 1;
			if (position.MarketPosition == MarketPosition.Short) positionSign = -1; 
			
			_positionPnl = Instrument.MasterInstrument.Round2TickSize(Close[0] - position.AvgPrice) * positionSign;
			
			_positionPnlList.Add(_positionPnl); 
		}
		
		public void _setExitReason(IPosition position)
		{	
			this._calculatePnl(position); 
			
			if (_positionPnl >= this._takeProfit)	    _exitReason = ExitReason.TakeProfit;
			if (_positionPnl <= -this._stopLoss) 	    _exitReason = ExitReason.StopLoss;
			
			if ((this._UseTrailingStops) && (_positionPnl <= this._getTrailStopExit(position)))   _exitReason = ExitReason.TrailExit;
			
			if ((this._EventTime > this._Session1EndTime) && (this._EventTime < this._Session2StartTime )) _exitReason = ExitReason.EndOfFirstSession; 
			
			if ((this._exitAtMarketClose) && (this._EventTime >= this._Session2EndTime)) _exitReason = ExitReason.EndOfSession;  
		}
		
		private double _getTrailStopExit(IPosition position)
		{
			double tExit = Double.MinValue; 
		
			if ((!_hitHighestTrail) && (_numTrail == (_trailTriggerList.Count - 1)))
			{
				if (_positionPnl >= _trailTriggerList[_numTrail])
				{
					_hitHighestTrail = true; 
					_adjustStopForTrail(position, _numTrail); 
					_exitLevel = _numTrail; 
				}
			}
			else
			{
				for (int i= _numTrail; i < _trailTriggerList.Count - 1; i++)
				{
					if ( (_positionPnl >= _trailTriggerList[i]) && (_positionPnl < _trailTriggerList[i+1]))
					{
						_adjustStopForTrail(position, i); 
						_exitLevel = i; 
						_numTrail  = i+1; 
						break; 
					}
				}
			}

			if (_numTrail != 0) tExit = _trailExitList[_exitLevel];
			
			return(tExit);
		}
		
		private void _adjustStopForTrail(IPosition position, int trailLevel)
		{
			this.RemoveDrawObject(_stopExitLine);
			this.RemoveDrawObject(_takeProfitLine);
			
			double stopLevel 	= position.AvgPrice + (Math.Sign(this._OpenContracts) * _trailExitList[trailLevel]);
			
			int tickPnl 		= (int)(_trailTriggerList[trailLevel]/position.Instrument.MasterInstrument.TickSize);
			Print(String.Format("{0} {1} position in {2} has hit trigger level {3} of {4} ticks. Stop moved to {5:N2} at {6:T}.", 
				this._StratName, position.MarketPosition, this._FuturesCode, (trailLevel + 1), tickPnl, stopLevel, this._EventTime )); 
			
			_stopExitLine		= DrawLine("stopExitLine", this._AutoScale, this._EventTime.Subtract(this._TimeZoneOffset), stopLevel, 
									this._EventTime.Subtract(this._TimeZoneOffset).Add(this._LineDuration), stopLevel, Color.Red, DashStyle.Solid, 2); 	
			
			_takeProfitLine  	= DrawLine("takeProfitLine", this._AutoScale, this._EventTime.Subtract(this._TimeZoneOffset), _takeProfitLine.StartY, 
									this._EventTime.Subtract(this._TimeZoneOffset).Add(this._LineDuration), _takeProfitLine.EndY, Color.Green, DashStyle.Solid, 2); 
		}
		
		public void _checkForConsecutiveWinsOrLosses()
		{
			double pnl = _getLastTradePnl(_sessionPnlList); 
			
			if (pnl > 0) 
			{
				_consecutiveWinners++;	
				_consecutiveLosers = 0;
			}
			else if (pnl < 0) 
			{
				_consecutiveWinners = 0;	
				_consecutiveLosers++;
			}
			else
			{
				_consecutiveWinners = 0;	
				_consecutiveLosers  = 0;
			}
			Print(String.Format("{0} Consecutive winners: {1}. Consecutive losers: {2}.", this._StratName, _consecutiveWinners, _consecutiveLosers));
			if ((_consecutiveWinners >= _consecutiveWLForTradingHalt) || (_consecutiveLosers >= _consecutiveWLForTradingHalt)) _isTradingHalted = true; 
		}

		#endregion //close exitTests
		
		#region properties
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Percent of TakeProfit - Trigger 1")]
		public double _PercentTakeProfit_Trigger1
		{
			get { return _percentTakeProfit_Trigger1; }
			set { _percentTakeProfit_Trigger1 = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Percent of TakeProfit - Trigger 2")]
		public double _PercentTakeProfit_Trigger2
		{
			get { return _percentTakeProfit_Trigger2; }
			set { _percentTakeProfit_Trigger2 = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Percent of TakeProfit - Trigger 3 ")]
		public double _PercentTakeProfit_Trigger3
		{
			get { return _percentTakeProfit_Trigger3; }
			set { _percentTakeProfit_Trigger3 = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]	
		[Description("Percent of TakeProfit - Exit 1")]
		public double _PercentTakeProfit_Exit1
		{
			get { return _percentTakeProfit_Exit1; }
			set { _percentTakeProfit_Exit1 = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Percent of TakeProfit - Exit 2")]
		public double _PercentTakeProfit_Exit2
		{
			get { return _percentTakeProfit_Exit2; }
			set { _percentTakeProfit_Exit2 = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Percent of TakeProfit - Exit 3")]
		public double _PercentTakeProfit_Exit3
		{
			get { return _percentTakeProfit_Exit3; }
			set { _percentTakeProfit_Exit3 = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Trail Stop - Step Interval")]
		public double _StepInterval
		{
			get { return _stepInterval; }
			set { _stepInterval = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Min Take Profit or Stop Loss (in Ticks)")]
		public int _MinTarget_InTicks
		{
			get { return _minTarget_InTicks; }
			set { _minTarget_InTicks = Math.Max(0, value); }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Max Take Profit or Stop Loss (in Ticks)")]
		public int _MaxTarget_InTicks
		{
			get { return _maxTarget_InTicks; }
			set { _maxTarget_InTicks = Math.Max(0, value); }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Use Trailing Stops")]
		public bool _UseTrailingStops
		{
			get { return _useTrailingStops; }
			set { _useTrailingStops = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Exit at Market Close")]
		public bool _ExitAtMarketClose
		{
			get { return _exitAtMarketClose; }
			set { _exitAtMarketClose = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Position Pnl List")]
		public List<double> _PositionPnlList
		{
			get { return _positionPnlList; }
			set { _positionPnlList = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Current Position Pnl")]
		public double _PositionPnl
		{
			get { return _positionPnl; }
			set { _positionPnl = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Dollar Pnl List")]
		public List<double> _DollarPnlList
		{
			get { return _dollarPnlList; }
			//set { _dollarPnlList = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Current Dollar Pnl")]
		public double _DollarPnl
		{
			get { return _dollarPnl; }
			//set { _dollarPnl = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Session Pnl List")]
		public List<double> _SessionPnlList
		{
			get { return _sessionPnlList; }
			//set { _sessionPnl = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Take Profit Level")]
		public double _TakeProfit
		{
			get { return _takeProfit; }
			set { _takeProfit = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Stop Loss Level")]
		public double _StopLoss
		{
			get { return _stopLoss; }
			set { _stopLoss = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Exit Reason")]
		public ExitReason _ExitReason
		{
			get { return _exitReason; }
			set { _exitReason = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Exit Order Sent")]
		public bool _ExitOrderSent
		{
			get { return _exitOrderSent; }
			set { _exitOrderSent = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Is Trading Halted")]
		public bool _IsTradingHalted
		{
			get { return _isTradingHalted; }
			set { _isTradingHalted = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Is Position Closed")]
		public bool _IsPositionClosed
		{
			get { return _isPositionClosed; }
			set { _isPositionClosed = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Consecutive Winning Trades")]
		public int _ConsecutiveWinners
		{
			get { return _consecutiveWinners; }
			//set { _consecutiveWinners = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Consecutive Losing Trades")]
		public int _ConsecutiveLosers
		{
			get { return _consecutiveLosers; }
			//set { _consecutiveLosers = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Consecutive W/L for Trd Halt")]
		public int _ConsecutiveWLForTradingHalt
		{
			get { return _consecutiveWLForTradingHalt; }
			set { _consecutiveWLForTradingHalt = Math.Max(1, value); }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Take Profit Line")]
		public ILine _TakeProfitLine
		{
			get { return _takeProfitLine; }
			set { _takeProfitLine = value; }
		}
		
		[XmlIgnore()]
		[Browsable(false)]
		[Description("Stop Exit Line")]
		public ILine _StopExitLine
		{
			get { return _stopExitLine; }
			set { _stopExitLine = value; }
		}

		#endregion //closes properties
		*/
    }
}
