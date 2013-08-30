//
// Low Lag Moving Average
//
// Based on old NinjaTrader version of another low lag moving average. Original
// version was very difficult to read code, as it appears to be based on an old
// meta trader script. Original version only worked with CalcOnBarClose=true.
//
// This version is restructured so it can be better understood, and it is
// updated so that it now works with either value of CalcOnBarClose. There is a
// embedded helper class to perform the inner average of the original code.
//
// 11/18/2010 - Created by aslan for NT7
//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

namespace NinjaTrader.Indicator
{
    [Description("Low Lag Moving Average")]
    public class LLMA : Indicator
    {
        private int    length = 14;
        private double phase  = 0;   // value between -100.0 & 100.0

		private double phaseParam, logParam, sqrtParam, sqrtDivider, lengthDivider; // calculated const
		
		private int lastBar = -1;
		private MidAvg midAvg;
		private DataSeries paramA, paramB, cycleDelta, avgDelta, fC0, fA8, fC8;

				
        protected override void Initialize()
        {
            Add(new Plot (new Pen(Color.Orange,2), PlotStyle.Line, "LLMA"));
            this.Name = "Low Lag Moving Average";
			midAvg = new MidAvg();
			
			paramA     = new DataSeries(this);
			paramB     = new DataSeries(this);
			cycleDelta = new DataSeries(this);
			avgDelta   = new DataSeries(this);
			fC0        = new DataSeries(this);
			fA8        = new DataSeries(this);
			fC8        = new DataSeries(this);

			double lengthParam = Math.Max(0.0000000001, (Length - 1) / 2.0);

			phaseParam = Phase < -100 ? 0.5 :
						 Phase >  100 ? 2.5 :
						 Phase / 100.0 + 1.5;
			
			logParam = Math.Max(0, Math.Log(Math.Sqrt(lengthParam)) / Math.Log(2.0) + 2.0);

			sqrtParam = Math.Sqrt(lengthParam) * logParam; 
			sqrtDivider = sqrtParam / (sqrtParam + 1.0);
			lengthParam *= 0.9; 
			lengthDivider = lengthParam / (lengthParam + 2.0);
			
            Overlay				= true;
            PriceTypeSupported	= true;	
			CalculateOnBarClose = false;
			BarsRequired        = 30;     // does not show first 30 bars of values
        }
		
		public override string ToString() {	
			return "";		
		}

		protected override void OnBarUpdate()
        {	
			bool commit = false;
			if (CurrentBar != lastBar) {
				commit = true;
				lastBar = CurrentBar;
			}
			
			double sValue = Input[0];

			if (CurrentBar == 0) {
				paramB.Set(sValue);
				paramA.Set(sValue);
				cycleDelta.Set(0);
				avgDelta.Set(0);
				Value.Set(sValue);
			}
			else {	
				double absValue = Math.Max(Math.Abs(sValue - paramA[1]), Math.Abs(sValue - paramB[1]));
				double delta = absValue + 0.0000000001; // 1.0e-10;
				cycleDelta.Set(delta);

				// calc SMA(10) of cycleDelta
				int cycleLen = Math.Min(10, CurrentBar);  // currBar starts at 1 in this leg
				double deltaSum = avgDelta[1] * Math.Min(10, CurrentBar-1);
				deltaSum -= cycleDelta[cycleLen];
				deltaSum += delta;
				double avgD = deltaSum / cycleLen; 
				avgDelta.Set(avgD);
					
				double mAvg = midAvg.Add(avgD, CurrentBar > 127 ? avgDelta[127] : 0, commit);
				
				if (CurrentBar <= 30) {
					// initial bars
					paramA[0] = (sValue - paramA[1]) > 0 ? sValue : sValue - (sValue - paramA[1]) * sqrtDivider; 
					paramB[0] = (sValue - paramB[1]) < 0 ? sValue : sValue - (sValue - paramB[1]) * sqrtDivider; 

					if (CurrentBar == 30) {
						// last init bar, init vars for main code
						fC0[0] = Input[0];
						int intPart = (Math.Ceiling(sqrtParam) >= 1.0) ? (int)Math.Ceiling(sqrtParam) : 1; 
						int dnShift, leftInt = intPart;
						intPart = (Math.Floor(sqrtParam) >= 1.0) ? (int)Math.Floor(sqrtParam) : 1; 
						int upShift, rightPart = intPart;
						double dValue = (leftInt == rightPart) ? 1.0 : (sqrtParam - rightPart) / (leftInt - rightPart);
						upShift = (rightPart <= 29) ? rightPart : 29; 
						dnShift = (leftInt <= 29) ? leftInt : 29; 
						fA8[0] = (Input[0] - Input[upShift]) * (1 - dValue) / rightPart + (Input[0] - Input[dnShift]) * dValue / leftInt;
					} 
					Value.Set(sValue);
				}
				else {
					double powerValue = (0.5 <= logParam - 2.0) ? logParam - 2.0 : 0.5;
					double dValue = logParam >= Math.Pow(absValue/mAvg, powerValue) ? Math.Pow(absValue/mAvg, powerValue) : logParam; 
					if (dValue < 1) dValue = 1;
					powerValue = Math.Pow(sqrtDivider, Math.Sqrt(dValue)); 
					
					paramA[0] = (sValue - paramA[1]) > 0 ? sValue : sValue - (sValue - paramA[1]) * powerValue;
					paramB[0] = (sValue - paramB[1]) < 0 ? sValue : sValue - (sValue - paramB[1]) * powerValue; 

					powerValue   = Math.Pow(lengthDivider, dValue);
					double squareValue  = powerValue * powerValue;
					fC0[0] = (1 - powerValue) * sValue + powerValue * fC0[1];
					fC8[0] = (sValue - fC0[0]) * (1.0 - lengthDivider) + lengthDivider * fC8[1];
					fA8[0] = (phaseParam * fC8[0] + fC0[0] - Value[1]) *
							 (powerValue * (-2.0) + squareValue + 1) + squareValue * fA8[1];  
					Value.Set(Value[1] + fA8[0]);
				}
			}
        }

		#region MidAvg Helper Class
		private class MidAvg {
			// this class tracks a list of sorted values and maintains an average
			// of the middle of the sorted range (throws the tails out)
			private const int size = 128;            // must be multiple of 4
			private const int sizeM1 = size - 1;
			private const int uMid = size / 2;       // i.e. size==128 -> uMid=64
			private const int lMid = uMid - 1;       // i.e. size==128 -> lMid=63
			private const int uQuad = size / 4 * 3;  // i.e. size==128 -> uQuad=96
			private const int lQuad = size / 4;      // i.e. size==128 -> lQuad=32
			// Note: in the above, uQuad should be decremented by -1 if the intent is
			// to have half of the values in the avg, as the algo includes start..end inclusive,
			// however, the algo was left as is to match the original code
			
			private int       num;                   // number of items in list
			private double [] list;                  // storage for values
			private int       left, right;           // left and right edges
			private int       winStart, winEnd;      // begin and end of window on list
			
			private double sum;                            
			private double Avg { get { return _num == 0 ? 0 : _sum / (_winEnd - _winStart + 1); } }

			// the following are temp working vars between commits
			private int _num, _left, _right, _winStart, _winEnd, _insIndex, _rmIndex;
			private double _sum, _newValue;
		
			public MidAvg() {
				sum = 0; _sum = sum;
				num = 0; _num = num;
				left  = uMid; _left  = left;
				right = lMid; _right = right;
				list = new double[size];
				for (int i=0; i<size; i++) list[i] = (i <= lMid) ? -1000000 : 1000000; 
			}
		
			private void Commit() {
				if (_num != 0) {
					// perform the insertion
					if (_rmIndex == _insIndex)
						list[_insIndex] = _newValue; 
					else if (_rmIndex < _insIndex) {
						for (int j=_rmIndex+1; j<=(_insIndex-1); j++) list[j-1] = list[j]; 
						list[_insIndex-1] = _newValue;			
					} 
					else {
						for (int j=_rmIndex-1; j>=_insIndex; j--) list[j+1] = list[j]; 
						list[_insIndex] = _newValue; 
					}
					
					// update state
					num = _num;
					sum = _sum;
					left = _left;
					right = _right;
					winStart = _winStart;
					winEnd = _winEnd;
				}
			}
			
			public double Add(double newValue, double removeValue, bool commit) {
				int	sIndex;    // search index

				if (commit) Commit();
				
				_newValue = newValue;
				
				if (num >= sizeM1) {
					// buffer is full, so look for entry to be removed to make room
					sIndex = uMid; _rmIndex = sIndex;
					while (sIndex > 1) { 
						if (list[_rmIndex] < removeValue)		{ sIndex /= 2; _rmIndex += sIndex; }
						else if (list[_rmIndex] <= removeValue)	{ sIndex = 1; } 
						else 									{ sIndex /= 2; _rmIndex -= sIndex; }
					}
				} 
				else {
					// not full yet, insert entry, list grows out from middle, _rmIndex is next entry to use
					if ((right + left) > sizeM1) {
						_left = left - 1; _rmIndex = _left; 
					} 
					else {
						_right = right + 1; _rmIndex = _right; 
					}
					// keep track of window that we sum over
					_winEnd = (_right > uQuad)  ? uQuad : _right;
					_winStart = (_left < lQuad) ? lQuad : _left;
				}
				
				// find insertion point in sorted list
				sIndex = uMid; _insIndex = sIndex;         
				while (sIndex > 1) {
					if (list[_insIndex] >= _newValue) {
						if (list[_insIndex - 1] <= _newValue) { sIndex = 1; }
						else { sIndex /= 2; _insIndex -= sIndex; }
					} else { sIndex /= 2; _insIndex += sIndex; }
					if ((_insIndex == sizeM1) && (_newValue > list[sizeM1])) _insIndex = size; 
				}
				
				// adjust sum based on insertion/deletion points
				_sum = sum;
				
				// list additions
				if (_insIndex <= _rmIndex) {
					if (_winStart <= _insIndex && _insIndex <= _winEnd) 
						_sum += _newValue;           // new value in the window
					else if (_insIndex < _winStart && _winStart <= _rmIndex) 
						_sum += list[_winStart - 1]; // push new entry into list 
				}
				else if (_insIndex <= _winStart) {
					if (_rmIndex <= _winEnd && (_winEnd + 1) < _insIndex) 
						_sum += list[_winEnd + 1];   // push entry down
				}
				else if (_insIndex <= (_winEnd + 1)) 
					_sum += _newValue;               // insert into list or end slot when moved
				else if (_rmIndex <= _winEnd && (_winEnd + 1) < _insIndex) 
					_sum += list[_winEnd + 1];       // push entry down
								
				// list removals
				if (num > sizeM1 && _winStart <= _rmIndex && _rmIndex <= _winEnd) 
					_sum -= list[_rmIndex];          // remove entry from list
				else if (_insIndex <= _winEnd && _winEnd < _rmIndex ) 
					_sum -= list[_winEnd];           // push entry off end of list
				else if (_rmIndex < _winStart && _winStart < _insIndex) 
					_sum -= list[_winStart];         // push entry off start of list
						
				// do not perform the insertion, done in commit

				if (num <= sizeM1) {
					_num = num + 1;
				}
				
				return Avg;
			}			
		}
		#endregion

        #region Properties
		
        [Description("Lookback interval")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayNameAttribute("\t\tLength")]
        public int Length
        {
            get { return length; }
            set { length = Math.Max(1, value); }
        }

        [Description("Phase (similar to offset), -100 to 100")]
        [GridCategory("Parameters")]
        [Gui.Design.DisplayNameAttribute("\tPhase")]
        public double Phase
        {
            get { return phase; }
            set { phase = Math.Min(Math.Max(-100, value), 100); }
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
        private LLMA[] cacheLLMA = null;

        private static LLMA checkLLMA = new LLMA();

        /// <summary>
        /// Low Lag Moving Average
        /// </summary>
        /// <returns></returns>
        public LLMA LLMA(int length, double phase)
        {
            return LLMA(Input, length, phase);
        }

        /// <summary>
        /// Low Lag Moving Average
        /// </summary>
        /// <returns></returns>
        public LLMA LLMA(Data.IDataSeries input, int length, double phase)
        {
            if (cacheLLMA != null)
                for (int idx = 0; idx < cacheLLMA.Length; idx++)
                    if (cacheLLMA[idx].Length == length && Math.Abs(cacheLLMA[idx].Phase - phase) <= double.Epsilon && cacheLLMA[idx].EqualsInput(input))
                        return cacheLLMA[idx];

            lock (checkLLMA)
            {
                checkLLMA.Length = length;
                length = checkLLMA.Length;
                checkLLMA.Phase = phase;
                phase = checkLLMA.Phase;

                if (cacheLLMA != null)
                    for (int idx = 0; idx < cacheLLMA.Length; idx++)
                        if (cacheLLMA[idx].Length == length && Math.Abs(cacheLLMA[idx].Phase - phase) <= double.Epsilon && cacheLLMA[idx].EqualsInput(input))
                            return cacheLLMA[idx];

                LLMA indicator = new LLMA();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.Length = length;
                indicator.Phase = phase;
                Indicators.Add(indicator);
                indicator.SetUp();

                LLMA[] tmp = new LLMA[cacheLLMA == null ? 1 : cacheLLMA.Length + 1];
                if (cacheLLMA != null)
                    cacheLLMA.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheLLMA = tmp;
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
        /// Low Lag Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LLMA LLMA(int length, double phase)
        {
            return _indicator.LLMA(Input, length, phase);
        }

        /// <summary>
        /// Low Lag Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.LLMA LLMA(Data.IDataSeries input, int length, double phase)
        {
            return _indicator.LLMA(input, length, phase);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Low Lag Moving Average
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.LLMA LLMA(int length, double phase)
        {
            return _indicator.LLMA(Input, length, phase);
        }

        /// <summary>
        /// Low Lag Moving Average
        /// </summary>
        /// <returns></returns>
        public Indicator.LLMA LLMA(Data.IDataSeries input, int length, double phase)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.LLMA(input, length, phase);
        }
    }
}
#endregion
