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
using System.Collections.Generic;
using System.Windows.Forms;
using NinjaTrader.Gui.Design;
using System.Drawing.Design;
using Gom;

#endregion

// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
	/// <summary>
	/// Enter the description of your new custom indicator here
	/// </summary>
	[Description("Gom Market Profile")]

	public class GomMP : GomDeltaIndicator
	{

		const int PRE_RTH = -1;
		const int POST_RTH = -2;

		class GomRangeArray<T>
		{
			const int initsize = 8;

			private int _initPrice = 0;
			private int _maxpositive = 0;
			private int _maxnegative = 0;
			private int _size = initsize;

			bool notinited = true;

			public int MinRange { get { return _initPrice + _maxnegative; } }
			public int MaxRange { get { return _initPrice + _maxpositive; } }
			public bool IsEmpty { get { return notinited; } }

			private T[] _arr = new T[initsize];

			private void Resize()
			{
				int _newsize = 2 * _size;
				T[] newarr = new T[_newsize];

				Array.Copy(_arr, 0, newarr, 0, _maxpositive + 1);
				if (_maxnegative < 0)
					Array.Copy(_arr, _maxpositive + 1, newarr, _maxpositive + 1 + _size, _size - _maxpositive - 1);

				_size = _newsize;
				_arr = newarr;
			}


			public T this[int price]   // Indexer declaration
			{
				get
				{
					if (notinited) return default(T);

					int delta = price - _initPrice;

					if (delta >= 0)
					{
						if (delta > _maxpositive)
							return default(T);
						else
							return _arr[delta];
					}
					else
					{
						if (delta < _maxnegative)
							return default(T);
						else
							return _arr[_size + delta];
					}
				}

				set
				{
					if (notinited)
					{
						_initPrice = price;
						notinited = false;
					}

					int delta = price - _initPrice;

					if (delta >= 0)
					{
						while ((delta - _maxnegative) >= _size)
							Resize();
						_arr[delta] = value;
						_maxpositive = Math.Max(_maxpositive, delta);
					}
					else
					{
						while ((_maxpositive - delta) >= _size)
							Resize();
						_arr[_size + delta] = value;
						_maxnegative = Math.Min(_maxnegative, delta);
					}
				}
			}
		}

		class VolAndDelta
		{

			private float _delta;
			private float _totalvolume;

			public float Delta { get { return _delta; } }
			public float TotalVolume { get { return _totalvolume; } }

			public VolAndDelta(float delta, float volume)
			{
				_delta = delta;
				_totalvolume = volume;
			}

			public void AddVolume(float delta, float volume)
			{
				_delta += delta;
				_totalvolume += volume;
			}

		}


		class VolAtPrice : GomRangeArray<VolAndDelta>
		{

			public VolAtPrice(int price, float delta, float volume)
			{
				this[price] = new VolAndDelta(delta, volume);
			}

			public void AddVolume(int price, float delta, float volume)
			{
				VolAndDelta curVolAtPrice = this[price];
				if (curVolAtPrice == null)
					this[price] = new VolAndDelta(delta, volume);
				else
					curVolAtPrice.AddVolume(delta, volume);
			}

			//returns a smoothed volatprice object.
			public VolAtPrice(VolAtPrice initVol, int smooth)
			{
				float vol2add;
				float divisor = 1.0f / ((smooth + 1) * (smooth + 1));
				float curVol;

				if (initVol.IsEmpty) return;

				for (int curPrice = initVol.MinRange; curPrice <= initVol.MaxRange; curPrice++)
				{
					if (initVol[curPrice] != null)
					{
						curVol = initVol[curPrice].TotalVolume;

						for (int i = 1; i <= smooth; i++)
						{
							vol2add = curVol * (smooth + 1 - i) * divisor;
							this.AddVolume(curPrice + i, 0, vol2add);
							this.AddVolume(curPrice - i, 0, vol2add);
						}

						vol2add = curVol * (smooth + 1) * divisor;
						this.AddVolume(curPrice, 0, vol2add);

					}
				}

			}

			public void Draw(GraphicalHelper helper, bool isRTH, int curTime, int x0, GomRangeArray<float> lastVolumes, GomRangeArray<float> newVolumes)
			{
				float curVol;
				float curDelta;
				float lastVol;

				VolAtPrice toDraw;

				if (this.IsEmpty) return;

				if ((helper.colorMode != Gom.MPColorMode.Delta) && (helper.nbSmooth > 0))
					toDraw = new VolAtPrice(this, helper.nbSmooth);
				else
					toDraw = this;

				for (int curPrice = toDraw.MinRange; curPrice <= toDraw.MaxRange; curPrice++)
				{
					if (toDraw[curPrice] != null)
					{
						curVol = toDraw[curPrice].TotalVolume;
						lastVol = lastVolumes[curPrice];

						if (helper.colorMode == Gom.MPColorMode.Delta)
						{
							curDelta = toDraw[curPrice].Delta;

							helper.FillRectangle(curPrice, (curDelta > 0) ? helper.chartcontrol.UpColor : helper.chartcontrol.DownColor, x0, lastVol, Math.Abs(curDelta));
							helper.FillRectangle(curPrice, helper.neutralColor, x0, lastVol + Math.Abs(curDelta), curVol - Math.Abs(curDelta));
						}
						else if (helper.colorMode == Gom.MPColorMode.TimeColor)
							helper.FillRectangle(curPrice, isRTH ? helper.ColorMapper[curTime] : helper.neutralColor, x0, lastVol, curVol);

						lastVolumes[curPrice] = lastVol + curVol;
						newVolumes[curPrice] += curVol;
					}
				}
			}

		}


		class VolAtTimeBar : GomRangeArray<VolAtPrice>
		{

			public void AddVolume(int time, int price, float delta, float volume)
			{

				VolAtPrice curVolAtPrice = this[time];

				if (curVolAtPrice == null)
					this[time] = new VolAtPrice(price, delta, volume);
				else
					curVolAtPrice.AddVolume(price, delta, volume);

			}
		}

		class PriceVol
		{
			private int _price;
			private float _vol;

			public int Price { get { return _price; } }
			public float Vol { get { return _vol; } }

			public PriceVol(int price, float vol)
			{
				_price = price;
				_vol = vol;
			}
		}

		class Profile : GomRangeArray<float>
		{
			public PriceVol POC, VWAP, VAH, VAL;
			private float totvol = 0.0f;
			private GraphicalHelper helper;

			float maxvol = 0;
			double weightedsum = 0;
			int maxprice = 0;

			public Profile(GraphicalHelper param)
				: base()
			{
				helper = param;
			}

			public void AddVolumes(GomRangeArray<float> lastVolumes)
			{
				if (lastVolumes.IsEmpty) return;

				float curVol, readVol, newVol;

				for (int curPrice = lastVolumes.MinRange; curPrice <= lastVolumes.MaxRange; curPrice++)
				{
					readVol = lastVolumes[curPrice];
					totvol += readVol;
					weightedsum += curPrice * readVol;

					curVol = this[curPrice];
					newVol = curVol + readVol;
					this[curPrice] = newVol;

					if (newVol > maxvol)
					{
						maxvol = newVol;
						maxprice = curPrice;
					}
				}

				if (totvol > 0)
				{
					int vwapprice = Convert.ToInt32(weightedsum / totvol);
					VWAP = new PriceVol(vwapprice, lastVolumes[vwapprice]);
					POC = new PriceVol(maxprice, maxvol);
				}
			}

			public void ComputeVA()
			{
				if (totvol < 1.0f) return;

				int startindex = POC.Price;
				int lastCommittedUpPrice = startindex;
				int lastCommittedDownPrice = startindex;
				float CommittedVolume = POC.Vol;
				float Vol_70Pcent = 0.7f * totvol;

				bool overflowwith2 = false;

				float upvol, downvol;

				while (CommittedVolume < Vol_70Pcent)
				{
					// 2 bars
					if ((!overflowwith2) && (lastCommittedUpPrice <= MaxRange - 2) && (lastCommittedDownPrice >= MinRange + 2))
					{
						upvol = this[lastCommittedUpPrice + 1] + this[lastCommittedUpPrice + 2];
						downvol = this[lastCommittedDownPrice - 1] + this[lastCommittedDownPrice - 2];

						if (((CommittedVolume + upvol) > Vol_70Pcent) || ((CommittedVolume + downvol) > Vol_70Pcent))
							overflowwith2 = true;
						else
						{
							if (upvol > downvol)
							{
								lastCommittedUpPrice += 2;
								CommittedVolume += upvol;
							}
							else if (upvol < downvol)
							{
								lastCommittedDownPrice -= 2;
								CommittedVolume += downvol;
							}
							else
							{
								if ((CommittedVolume + upvol + downvol) > Vol_70Pcent)
									overflowwith2 = true;
								else
								{
									lastCommittedUpPrice += 2;
									lastCommittedDownPrice -= 2;
									CommittedVolume += upvol + downvol;
								}
							}
						}
					}
					else
					{
						if (lastCommittedUpPrice == MaxRange)
						{
							CommittedVolume += this[lastCommittedDownPrice - 1];
							lastCommittedDownPrice -= 1;
						}
						else if (lastCommittedDownPrice == MinRange)
						{
							CommittedVolume += this[lastCommittedUpPrice + 1];
							lastCommittedUpPrice += 1;
						}
						else
						{
							upvol = this[lastCommittedUpPrice + 1];
							downvol = this[lastCommittedDownPrice - 1];

							if (upvol > downvol)
							{
								lastCommittedUpPrice += 1;
								CommittedVolume += upvol;
							}
							else if (upvol < downvol)
							{
								lastCommittedDownPrice -= 1;
								CommittedVolume += downvol;
							}
							else
							{
								lastCommittedUpPrice += 1;
								lastCommittedDownPrice -= 1;
								CommittedVolume += upvol + downvol;

							}
						}
					}
				}

				VAH = new PriceVol(lastCommittedUpPrice, this[lastCommittedUpPrice]);
				VAL = new PriceVol(lastCommittedDownPrice, this[lastCommittedDownPrice]);
			}

			private void DrawLevel(int x0, PriceVol param, Color color)
			{
				if (param != null)
					helper.FillRectangleSpecialBars(param.Price, color, x0, (helper.allPocVolume) ? POC.Vol : param.Vol);
			}

			public void DrawLevels(int x0)
			{
				if (totvol > 0)
				{
					if (helper.showVa)
					{
						if (!helper.showContinuous)
							ComputeVA();
						DrawLevel(x0, VAH, helper.vaPen.Color);
						DrawLevel(x0, VAL, helper.vaPen.Color);
					}

					if (helper.showVwap)
						DrawLevel(x0, VWAP, helper.vwapPen.Color);

					if (helper.showPoc)
						DrawLevel(x0, POC, helper.pocPen.Color);

				}
			}

			public void RepaintHeat(int x0)
			{
				if (totvol > 0)
				{
					for (int i = this.MinRange; i <= this.MaxRange; i++)
						helper.FillRectangle(i, GraphicalHelper.ColorFromHSV((POC.Vol - this[i]) / POC.Vol * 240.0f), x0, 0, this[i]);

				}
			}

			public void RepaintNoColor(int x0)
			{
				if (totvol > 0)
				{
					for (int i = this.MinRange; i <= this.MaxRange; i++)
						helper.FillRectangle(i, helper.neutralColor, x0, 0, this[i]);
				}
			}


			public void ShowOutline(int x0)
			{
				if (totvol > 0)
					helper.PlotProfileOutline(x0, this);
			}

			public void DrawVN(int x0, int maxx)
			{
				//peakdet by Eli Billauer http://www.billauer.co.il/peakdet.html

				float mn = float.MaxValue;
				float mx = float.MinValue;

				int mnpos = -1;
				int mxpos = -1;

				bool lookformax = true;

				float delta = POC.Vol * helper.vnSize / 100f;

				float curVol;

				for (int i = this.MinRange; i <= this.MaxRange; i++)
				{
					curVol = this[i];

					if (curVol > mx)
					{
						mx = curVol;
						mxpos = i;
					}

					if (curVol < mn)
					{
						mn = curVol;
						mnpos = i;
					}

					if (lookformax)
					{
						if (curVol < mx - delta)
						{
							helper.DrawLine(mxpos, helper.hvnPen, x0, mx, maxx);
							mn = curVol; mnpos = i;
							lookformax = false;
						}
					}
					else
					{
						if (curVol > mn + delta)
						{
							helper.DrawLine(mnpos, helper.lvnPen, x0, mn, maxx);
							mx = curVol; mxpos = i;
							lookformax = true;
						}
					}
				}
			}

		}



		private class MultiLine
		{
			private Pen _pen;
			List<int> _prices;

			public Pen Pen { get { return _pen; } set { _pen = value; } }
			public List<int> Prices { get { return _prices; } set { _prices = value; } }
		}


		class VolAtTimeBars : GomRangeArray<VolAtTimeBar>
		{

			private GraphicalHelper helper;

			public VolAtTimeBars(GraphicalHelper param)
				: base()
			{
				helper = param;
			}


			public void DrawBars(int firstBar, int lastBar, int x0, int maxx)
			{

				Profile prof = new Profile(helper);
				GomRangeArray<float> lastVolumes = new GomRangeArray<float>();
				GomRangeArray<float> newVolumes;

				int curTime;
				bool isRTH;
				VolAtTimeBar curVolAtTimeBar;

				List<int> barnums = new List<int>();
				List<int> pocprices = new List<int>();
				List<int> vwapprices = new List<int>();
				List<int> vahprices = new List<int>();
				List<int> valprices = new List<int>();

				List<MultiLine> MLs = new List<MultiLine>();

				for (int i = firstBar; i <= lastBar; i++)
				{
					curVolAtTimeBar = this[i];
					if (!curVolAtTimeBar.IsEmpty)
					{
						newVolumes = new GomRangeArray<float>();
						for (curTime = curVolAtTimeBar.MinRange; curTime <= curVolAtTimeBar.MaxRange; curTime++)
						{
							if (curVolAtTimeBar[curTime] != null)
							{
								isRTH = ((curTime != PRE_RTH) && (curTime != POST_RTH));

								if ((helper.showAH == Gom.MPAHMode.All) ||
									 (isRTH && (helper.showAH == Gom.MPAHMode.RTH)) ||
									 (!isRTH && (helper.showAH == Gom.MPAHMode.AH)))
									curVolAtTimeBar[curTime].Draw(helper, isRTH, curTime, x0, lastVolumes, newVolumes);
							}
						}

						prof.AddVolumes(newVolumes);

						if ((prof.POC != null) && (helper.showContinuous))
						{
							barnums.Add(i);

							pocprices.Add(prof.POC.Price);
							vwapprices.Add(prof.VWAP.Price);

							if (helper.showVa)
							{
								prof.ComputeVA();

								vahprices.Add(prof.VAH.Price);
								valprices.Add(prof.VAL.Price);
							}
						}
					}
				}

				if (prof.POC != null)
				{
					prof.DrawLevels(x0);

					if (helper.colorMode == Gom.MPColorMode.Heat)
						prof.RepaintHeat(x0);
					else if (helper.colorMode == Gom.MPColorMode.NoColor)
						prof.RepaintNoColor(x0);

					if (helper.showVn && (helper.plotMode != Gom.MPPlotMode.Bar))
						prof.DrawVN(x0, maxx);

					if ((helper.showContinuous) && (barnums.Count > 1))
					{
						if (helper.showPoc)
							MLs.Add(new MultiLine { Pen = helper.pocPen, Prices = pocprices });

						if (helper.showVwap)
							MLs.Add(new MultiLine { Pen = helper.vwapPen, Prices = vwapprices });

						if (helper.showVa)
						{
							MLs.Add(new MultiLine { Pen = helper.vaPen, Prices = vahprices });
							MLs.Add(new MultiLine { Pen = helper.vaPen, Prices = valprices });
						}

						helper.DrawContinuous(barnums, MLs);
					}

					if (helper.showOutline)
						prof.ShowOutline(x0);
				}
			}
		}


		sealed class GraphicalHelper
		{
			public Graphics graphics;
			public ChartControl chartcontrol;
			public double TickSize;
			public int BarSpacing = 0;
			public Color[] ColorMapper = new Color[1440];
			public Gom.MPAHMode showAH = Gom.MPAHMode.All;
			public Color neutralColor = Color.Black;
			public short alpha = 128;
			public int minutestep = 30;
			public double starthue = 0;
			public double endhue = 360;
			public Gom.MPColorMode colorMode = Gom.MPColorMode.Heat;
			public int rthlengthminutes;
			public bool rtl = false;
			public bool showPoc = true;
			public bool showVa = true;
			public bool showVwap = true;
			public bool showOutline = true;
			public int pocSize = 3;
			public bool allPocVolume = true;
			public Pen pocPen = new Pen(Color.Red, 1);
			public Pen vaPen = new Pen(Color.Blue, 1);
			public Pen vwapPen = new Pen(Color.Green, 1);
			public Pen hvnPen = new Pen(Color.Green);
			public Pen lvnPen = new Pen(Color.Red);
			public Pen outlinePen = new Pen(Color.Red);
			public bool showVn = false;
			public int vnSize = 5;
			public int accCount = 1;
			public int nbSmooth = 0;
			public float scale = 0.005F;
			public Gom.MPPlotMode plotMode = Gom.MPPlotMode.Daily;
			public Indicator indic;
			public DateTime startDate = new DateTime();
			public double offset;
			public bool showContinuous = true;


			static public Color ColorFromHSV(double hue)
			{
				int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
				double f = hue / 60 - Math.Floor(hue / 60);

				int v = 255;
				int p = 0;
				int q = Convert.ToInt32(255 * (1 - f));
				int t = Convert.ToInt32(255 * f);

				if (hi == 0) return Color.FromArgb(255, v, t, p);
				else if (hi == 1) return Color.FromArgb(255, q, v, p);
				else if (hi == 2) return Color.FromArgb(255, p, v, t);
				else if (hi == 3) return Color.FromArgb(255, p, q, v);
				else if (hi == 4) return Color.FromArgb(255, t, p, v);
				else return Color.FromArgb(255, v, p, q);
			}


			public void FillRectangle(int priceTick, Color color, float x0, float initialvolume, float volume)
			{
				double priceLower = ((double)priceTick - 0.5) * TickSize; //;- GHelper.TickSize / 2;
				float yLower = GetYPos(priceLower);
				float yUpper = GetYPos(priceLower + TickSize);
				float height = Math.Max(1, Math.Abs(yUpper - yLower) - BarSpacing);

				using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, color)))
				{
					if (!rtl)
						graphics.FillRectangle(brush, x0 + initialvolume * scale, yUpper, volume * scale, height);
					else
						graphics.FillRectangle(brush, x0 - (initialvolume + volume) * scale, yUpper, volume * scale, height);
				}
			}

			public void PlotProfileOutline(float x0, Profile prof)
			{
				PointF[] pts = new PointF[(prof.MaxRange - prof.MinRange + 1) * 2];
				for (int i = prof.MinRange; i <= prof.MaxRange; i++)
				{
					double priceLower = ((double)i - 0.5) * TickSize; //;- GHelper.TickSize / 2;
					float yLower = GetYPos(priceLower);
					float yUpper = GetYPos(priceLower + TickSize);
					pts[(i - prof.MinRange) * 2].Y = yLower;
					pts[(i - prof.MinRange) * 2 + 1].Y = yUpper;
					float x;
					if (!rtl)
						x = x0 + prof[i] * scale;
					else
						x = x0 - prof[i] * scale;

					pts[(i - prof.MinRange) * 2].X = x;
					pts[(i - prof.MinRange) * 2 + 1].X = x;
				}

				graphics.DrawLines(outlinePen, pts);


			}


			public void FillRectangleSpecialBars(int priceTick, Color color, float x0, float volume)
			{
				double priceLower = ((double)priceTick - 0.5) * TickSize;
				float yLower = GetYPos(priceLower);
				float yUpper = GetYPos(priceLower + TickSize);
				float height = Math.Max(1, Math.Abs(yUpper - yLower) - BarSpacing);


				if (height < pocSize)
				{
					yUpper = GetYPos(priceTick * TickSize) - (float)Math.Ceiling(pocSize / 2.0F);
					height = pocSize;
				}

				using (SolidBrush brush = new SolidBrush(color))
				{
					if (!rtl)
						graphics.FillRectangle(brush, x0, yUpper, volume * scale, height);
					else
						graphics.FillRectangle(brush, x0 - volume * scale, yUpper, volume * scale, height);
				}
			}

			public void DrawLine(int priceTick, Pen pen, float x0, float initialvolume, float maxx)
			{
				double price = priceTick * TickSize;
				float y = GetYPos(price);


				if (!rtl)
					graphics.DrawLine(pen, x0 + initialvolume * scale, y, maxx, y);
				else
					graphics.DrawLine(pen, maxx, y, x0 - initialvolume * scale, y);

			}

			public void DrawContinuous(List<int> barnum, List<MultiLine> prices)
			{
				int nbpoints = barnum.Count;
				int nblines = prices.Count;

				Point[][] points = new Point[nblines][];

				int curx;
				int i, j;

				curx = chartcontrol.GetXByBarIdx(indic.Bars, barnum[0]);
				for (i = 0; i < nblines; i++)
				{
					points[i] = new Point[nbpoints * 2 - 1];
					points[i][0].X = curx;
					points[i][0].Y = (int)GetYPos(prices[i].Prices[0] * TickSize);
				}

				int index;

				for (j = 1; j < nbpoints; j++)
				{
					curx = chartcontrol.GetXByBarIdx(indic.Bars, barnum[j]);
					for (i = 0; i < nblines; i++)
					{
						index = 2 * j - 1;
						points[i][index].X = curx;
						points[i][index].Y = points[i][index - 1].Y;

						index = 2 * j;
						points[i][index].X = curx;
						points[i][index].Y = (int)GetYPos(prices[i].Prices[j] * TickSize);
					}
				}

				for (i = 0; i < nblines; i++)
					graphics.DrawLines(prices[i].Pen, points[i]);

			}

			public float GetYPos(double price)
			{
				return chartcontrol.GetYByValue(indic, price + offset);

			}

			public void ComputeColorMapper()
			{
				double hue;

				int nbclasses = rthlengthminutes / minutestep;

				if (nbclasses * minutestep < rthlengthminutes)
					nbclasses++;

				double step = (endhue - starthue) / (nbclasses);

				for (int j = 0; j < nbclasses; j++)
				{
					hue = starthue + j * step;

					if (hue > 359) hue -= 360;
					if (hue < 0) hue += 360;

					ColorMapper[j] = ColorFromHSV(hue);
				}
			}


		}

		int startbar = -1;
		int lastcalcbar = -1;

		System.Windows.Forms.MouseEventHandler mouseUpEvtH;
		System.Windows.Forms.MouseEventHandler mouseMoveEvtH;

		VolAtTimeBars bartimevols;
		VolAtTimeBar curvolattime;

		List<int> sessions = new List<int>();

		TimeSpan startTime = new TimeSpan(15, 30, 00);
		int starttimeminutes;

		TimeSpan rthLength = new TimeSpan(6, 45, 00);

		GraphicalHelper GHelper = new GraphicalHelper();

		double invTickSize = 0.0f;

		Gom.MPDataSource dataSource = Gom.MPDataSource.File;

		bool premiumCorrection = false;
		string underlyingInstr = "^SP500";

		int sum;
		int nsum;
		long lastixtime, lastfuttime;
		List<double> offsets = new List<double>();
		List<int> vols = new List<int>();
		double offset = 0;

		Dictionary<int, double> baroffsets = new Dictionary<int, double>();

		DateTime lastOffsetDate = new DateTime(0L);

		bool correctedCloses;

		enum CustomProfileStateEnum { None, WaitingStartBar, WaitingCloseBar, Delete };

		CustomProfileStateEnum customProfileState = CustomProfileStateEnum.None;

		int selectedStartBar;
		int selectedEndBar;

		string customPlots = "";

		DateTime lastDateTime = Gom.Utils.nullDT;
		double lastPrice;
		Gom.MPProfileData vt = Gom.MPProfileData.Volume;

		Gom.HotKeyClass GomHK;
		Gom.MPMsgStyle hkMsgStyle = Gom.MPMsgStyle.Full;

		int tickFilterMin = 1;
		int tickFilterMax = 1000000;


		[XmlIgnore()]
		[Browsable(false)]
		public string GomHKMessage
		{
			get
			{
				if (hkMsgStyle == Gom.MPMsgStyle.None)
					return ",";
				else if (hkMsgStyle == Gom.MPMsgStyle.NumberOnly)
					return ".";
				else if (hkMsgStyle == Gom.MPMsgStyle.NameOnly)
					return "";
				else
				{
					string Msg = " - " + vt + " - mode:" + GHelper.plotMode + ((GHelper.plotMode == Gom.MPPlotMode.FromDate) ? (" " + GHelper.startDate.ToString()) : "") + "  - color:" + GHelper.colorMode + " - nb bars/days:" + GHelper.accCount +
					" - smoothing:" + GHelper.nbSmooth + " - min peak size:" + GHelper.vnSize + "% of POC";

					if (customProfileState == CustomProfileStateEnum.Delete)
						Msg += " - Click to Delete";
					else if (customProfileState == CustomProfileStateEnum.WaitingCloseBar)
						Msg += " - Click to select End Bar";
					else if (customProfileState == CustomProfileStateEnum.WaitingStartBar)
						Msg += " - Click to select Start Bar";

					return Msg;
				}
			}
		}

		//declaring a delegate to functions we will dichotomize. 
		delegate int Function(int x);

		private int GetX(int x)
		{
			return ChartControl.GetXByBarIdx(BarsArray[0], x);
		}

		//the dichotomy function, we look for yToFind using paramFunction, starting at xL and ending at xR/
		private int IntegerDichotomy(int xL, int xR, Function paramFunction, int yToFind)
		{

			int yL = paramFunction(xL);
			//xl matches
			if (yL == yToFind)
				return (xL);

			int yR = paramFunction(xR);
			//xR matches
			if (yR == yToFind)
				return (xR);

			if (xL == xR)
				return xL;

			// if xR=xL+1, we're between 2 points. we choose the closest.
			if ((xR - xL) == 1)
				if (Math.Abs((yToFind - yL)) > Math.Abs((yR - yToFind)))
					return xR;
				else
					return xL;

			//else we split interval in 2 and start again.
			int xM = (xL + xR) / 2;
			int yM = paramFunction(xM);

			if (Math.Sign(yL - yToFind) == Math.Sign((yM - yToFind)))
				return IntegerDichotomy(xM, xR, paramFunction, yToFind);
			else
				return IntegerDichotomy(xL, xM, paramFunction, yToFind);

		}


		private void chart_MouseMove(object sender, MouseEventArgs e)
		{

			int lastBar = Math.Min(Math.Max(this.LastBarIndexPainted, 0), BarsArray[0].Count - 1);
			int firstBar = Math.Max(this.FirstBarIndexPainted, 0);

			firstBar = Math.Max(startbar, firstBar);
			lastBar = Math.Min(lastcalcbar, lastBar);

			int barnum = IntegerDichotomy(Math.Max(firstBar - 1, 0), Math.Min(lastBar + 1, CurrentBar), GetX, e.X);

			if ((barnum >= firstBar) && barnum <= lastBar)
			{
				selectedEndBar = barnum;
				ChartControl.ChartPanel.Invalidate();
			}
		}


		private void chart_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				int lastBar = Math.Min(Math.Max(this.LastBarIndexPainted, 0), BarsArray[0].Count - 1);
				int firstBar = Math.Max(this.FirstBarIndexPainted, 0);

				firstBar = Math.Max(startbar, firstBar);
				lastBar = Math.Min(lastcalcbar, lastBar);

				int barnum = IntegerDichotomy(Math.Max(firstBar - 1, 0), Math.Min(lastBar + 1, CurrentBar), GetX, e.X);

				if ((barnum >= firstBar) && barnum <= lastBar)
				{
					if (customProfileState == CustomProfileStateEnum.WaitingStartBar)
					{
						selectedStartBar = barnum;
						customProfileState = CustomProfileStateEnum.WaitingCloseBar;
						this.ChartControl.ChartPanel.MouseMove += mouseMoveEvtH;
					}
					else if (customProfileState == CustomProfileStateEnum.WaitingCloseBar)
					{
						customProfileState = CustomProfileStateEnum.None;
						this.ChartControl.ChartPanel.MouseMove -= mouseMoveEvtH;
						this.ChartControl.ChartPanel.MouseUp -= mouseUpEvtH;

						if (((selectedStartBar > selectedEndBar) == GHelper.rtl) || (selectedStartBar == selectedEndBar))
						{
							if (!String.IsNullOrEmpty(customPlots))
								customPlots += ";";

							customPlots += Math.Min(selectedStartBar, selectedEndBar) + "," + Math.Max(selectedStartBar, selectedEndBar);
						}
					}

					else if (customProfileState == CustomProfileStateEnum.Delete)
					{
						if (!String.IsNullOrEmpty(customPlots))
						{
							string[] list = customPlots.Split(';');

							foreach (string pair in list)
							{
								string[] barnums = pair.Split(',');

								int bar0 = Int32.Parse(barnums[0]);
								int bar1 = Int32.Parse(barnums[1]);

								if (bar1 == -1)
									bar1 = CurrentBar;

								if ((barnum >= bar0) && (barnum <= bar1))
								{
									customPlots = customPlots.Replace(pair + ";", "").Replace(";" + pair, "").Replace(pair, "");
									customProfileState = CustomProfileStateEnum.None;
									this.ChartControl.ChartPanel.MouseMove -= mouseMoveEvtH;
									this.ChartControl.ChartPanel.MouseUp -= mouseUpEvtH;
									break;
								}
							}
						}
					}
				}
			}
		}

		//HotKeys

		[Gom.HotKey(Keys.Control | Keys.Multiply)]
		private void HK_Rtl()
		{
			GHelper.rtl = !GHelper.rtl;
		}

		[Gom.HotKey(Keys.Multiply)]
		private void HK_ShowVN()
		{
			GHelper.showVn = !GHelper.showVn;
		}

		[Gom.HotKey(Keys.Divide)]
		private void HK_PlotMode()
		{
			GHelper.plotMode = (Gom.MPPlotMode)(((int)GHelper.plotMode + 1) % Enum.GetValues(typeof(Gom.MPPlotMode)).Length);
		}

		[Gom.HotKey(Keys.Control | Keys.Alt | Keys.Add)]
		private void HK_AccUp()
		{
			GHelper.accCount++;
		}

		[Gom.HotKey(Keys.Control | Keys.Add)]
		private void HK_SmoothUp()
		{
			GHelper.nbSmooth++;
		}

		[Gom.HotKey(Keys.Alt | Keys.Add)]
		private void HK_VNSizeUp()
		{
			GHelper.vnSize = Math.Min(100, GHelper.vnSize + 1);
		}

		[Gom.HotKey(Keys.Add)]
		private void HK_ScaleUp()
		{
			GHelper.scale *= 1.1F;
		}

		[Gom.HotKey(Keys.Control | Keys.Alt | Keys.Subtract)]
		private void HK_AccDown()
		{
			GHelper.accCount = Math.Max(1, GHelper.accCount - 1);
		}

		[Gom.HotKey(Keys.Control | Keys.Subtract)]
		private void HK_SmoothDown()
		{
			GHelper.nbSmooth = Math.Max(0, GHelper.nbSmooth - 1);
		}

		[Gom.HotKey(Keys.Alt | Keys.Subtract)]
		private void HK_VNSizeDown()
		{
			GHelper.vnSize = Math.Max(0, GHelper.vnSize - 1);
		}

		[Gom.HotKey(Keys.Subtract)]
		private void HK_ScaleDown()
		{
			GHelper.scale *= 0.9F;
		}

		[Gom.HotKey(Keys.Decimal)]
		private void HK_ColorMode()
		{
			GHelper.colorMode = (Gom.MPColorMode)(((int)GHelper.colorMode + 1) % Enum.GetValues(typeof(Gom.MPColorMode)).Length);
		}

		[Gom.HotKey(Keys.Insert)]
		private void HK_Insert()
		{
			if ((customProfileState == CustomProfileStateEnum.None))
			{
				customProfileState = CustomProfileStateEnum.WaitingStartBar;
				this.ChartControl.ChartPanel.MouseUp += mouseUpEvtH;
			}
			else if ((customProfileState == CustomProfileStateEnum.WaitingCloseBar) && !GHelper.rtl)
			{
				customProfileState = CustomProfileStateEnum.None;
				this.ChartControl.ChartPanel.MouseMove -= mouseMoveEvtH;
				this.ChartControl.ChartPanel.MouseUp -= mouseUpEvtH;

				if (!String.IsNullOrEmpty(customPlots))
					customPlots += ";";

				customPlots += selectedStartBar + "," + "-1";
			}
		}

		[Gom.HotKey(Keys.Escape)]
		private void HK_Escape()
		{
			if (customProfileState != CustomProfileStateEnum.None)
			{
				customProfileState = CustomProfileStateEnum.None; ;
				this.ChartControl.ChartPanel.MouseUp -= mouseUpEvtH;
				this.ChartControl.ChartPanel.MouseMove -= mouseMoveEvtH;

			}
		}

		[Gom.HotKey(Keys.Delete)]
		private void HK_Delete()
		{
			if (customProfileState == CustomProfileStateEnum.None)
			{
				customProfileState = CustomProfileStateEnum.Delete;
				this.ChartControl.ChartPanel.MouseUp += mouseUpEvtH;
			}
		}

		[Gom.HotKey(Keys.Space)]
		private void HK_ShowAH()
		{
			GHelper.showAH = (Gom.MPAHMode)(((int)GHelper.showAH + 1) % Enum.GetValues(typeof(Gom.MPAHMode)).Length);
		}


		public void chart_KeyDown(object sender, KeyEventArgs e)
		{
			//	GomHK.ManageHotKeys(e);
		}


		protected override void GomOnStartUp()
		{
			invTickSize = 1.0f / Bars.Instrument.MasterInstrument.TickSize;

			mouseUpEvtH = new System.Windows.Forms.MouseEventHandler(this.chart_MouseUp);
			mouseMoveEvtH = new System.Windows.Forms.MouseEventHandler(this.chart_MouseMove);

			GHelper.TickSize = Bars.Instrument.MasterInstrument.TickSize;
			GHelper.indic = this;

			GomHK = new Gom.HotKeyClass(this);
		}

		protected override void GomInitialize()
		{
			CalculateOnBarClose = false;
			Overlay = true;
			PriceTypeSupported = false;

			this.FileFormat = "NinjaTickFile";

			starttimeminutes = (int)startTime.TotalMinutes;
			GHelper.rthlengthminutes = (int)rthLength.TotalMinutes;

			GHelper.ComputeColorMapper();

			bartimevols = new VolAtTimeBars(GHelper);

			sessions.Add(0);

			GHelper.startDate = DateTime.Now.Date.Add(startTime);

			if ((dataSource == Gom.MPDataSource.MinuteData) || premiumCorrection)
				Add(PeriodType.Minute, 1);
			else if (dataSource == Gom.MPDataSource.TickData)
				Add(PeriodType.Tick, 1);

			if (premiumCorrection)
				Add(underlyingInstr, PeriodType.Minute, 1);

		}



		private int CalcTime(DateTime time0)
		{
			int minute = Convert.ToInt16(Math.Floor((time0 - time0.Date).TotalMinutes) - starttimeminutes);

			int time;

			if (minute < 0)
				time = PRE_RTH;
			else if (minute >= GHelper.rthlengthminutes)
				time = POST_RTH;
			else
				time = minute / GHelper.minutestep;

			return time;

		}


		private void ProcessPricesForOffsetCalculation()
		{
			offsets.Add((
				Opens[1][0] - Opens[2][0] +
				Closes[1][0] - Closes[2][0] +
				Highs[1][0] - Highs[2][0] +
				Lows[1][0] - Lows[2][0]) * 0.25);

			nsum++;

			sum += (int)Volumes[1][0];

			vols.Add((int)Volumes[1][0]);

		}

		private void CalcOffset()
		{
			if (BarsInProgress == 1)
				lastfuttime = Times[1][0].Ticks;

			else if (BarsInProgress == 2)
				lastixtime = Times[2][0].Ticks;

			if (lastixtime == lastfuttime)
				ProcessPricesForOffsetCalculation();
		}

		private void CreateNewBar(int barnum)
		{
			bartimevols[barnum] = new VolAtTimeBar();
			curvolattime = bartimevols[barnum];
			lastcalcbar = barnum;
			baroffsets.Add(barnum, offset);
		}


		protected override void GomOnBarUpdate()
		{

			if ((Historical) && premiumCorrection)
			{
				if (BarsInProgress != 0)
					CalcOffset();

				if (Bars.FirstBarOfSession && (Time[0].Date > lastOffsetDate))
				{
					lastOffsetDate = Time[0].Date;
					double med = sum / (double)nsum;
					double newsum = 0.0f;
					int newnsum = 0;

					for (int i = 0; i < nsum; i++)
					{
						if (vols[i] < med)
						{
							newsum += offsets[i];
							newnsum++;
						}
					}
					if (newnsum > 0)
						offset = newsum / (double)newnsum;

					sum = 0;
					nsum = 0;
					vols.Clear();
					offsets.Clear();

					if (BarsArray[0].Instrument.MasterInstrument.MergePolicy == MergePolicy.MergeNonBackAdjusted)
					{
						DateTime zonedate = TimeZoneInfo.ConvertTime(Time[0], Bars.Session.TimeZoneInfo);
						foreach (RollOver RO in Instrument.MasterInstrument.RollOverCollection)
							if (zonedate.Date == RO.Date)
								offset += RO.Offset;
					}

					Print(Time[0].Date + " " + offset);
				}
			}

			if ((BarsInProgress == 1) && Historical && dataSource != Gom.MPDataSource.File)
			{
				if (Bars.SessionBreak)
					lastDateTime = Gom.Utils.nullDT;

				if ((CurrentBars[0] == -1) || Times[1][0] > Times[0][0])
				{
					curvolattime = bartimevols[CurrentBars[0] + 1];

					if (curvolattime == null)
						CreateNewBar(CurrentBars[0] + 1);
				}


				int vol = 0;

				if (vt == Gom.MPProfileData.Volume)
				{
					vol = (int)Volumes[1][0];

					if (vol < tickFilterMin || vol > tickFilterMax)
						vol = 0;

				}
				else if (vt == Gom.MPProfileData.Time)
				{
					if (lastDateTime != Gom.Utils.nullDT)
						vol = (int)Time[0].Subtract(lastDateTime).TotalSeconds;

				}
				else if ((vt == Gom.MPProfileData.Tick) && (dataSource == Gom.MPDataSource.TickData))
					vol = 1;


				if (vol > 0)
				{
					if (dataSource == Gom.MPDataSource.MinuteData)
					{
						int vol1 = vol / 2;
						int vol2 = vol - vol1;

						//manual dithering to avoid combing effects (if between 2 ticks, even tick is favored)

						double pr = (Highs[1][0] + Lows[1][0]) * 0.5;

						if (vol1 > 0)
							curvolattime.AddVolume(CalcTime(Times[1][0]), Convert.ToInt32((pr - offset + 0.0001f) * invTickSize), 0, vol1);
						if (vol2 > 0)
							curvolattime.AddVolume(CalcTime(Times[1][0]), Convert.ToInt32((pr - offset - 0.0001f) * invTickSize), 0, vol2);
					}
					else
						curvolattime.AddVolume(CalcTime(Times[1][0]), Convert.ToInt32((lastPrice - offset) * invTickSize), 0, vol);
				}

				lastDateTime = Time[0];
				lastPrice = Close[0];
			}


			if ((BarsInProgress == 0))
			{
				if ((lastcalcbar < CurrentBar))
				{
					CreateNewBar(CurrentBar);
				}

				if (Bars.SessionBreak)
				{
					if (!sessions.Exists(element => element == CurrentBar))
						sessions.Add(CurrentBar);

					lastDateTime = Gom.Utils.nullDT;
				}

				if (startbar == -1)
					startbar = CurrentBar;
			}
		}


		protected override void GomOnMarketData(Gom.MarketDataType e)
		{
			if (dataSource == Gom.MPDataSource.File || !Historical)
			{
				if (vt == Gom.MPProfileData.Time)
				{
					if (lastDateTime != Gom.Utils.nullDT)
					{
						int deltatime = (int)e.Time.Subtract(lastDateTime).TotalSeconds;
						if (deltatime != 0)
							curvolattime.AddVolume(CalcTime(e.Time), Convert.ToInt32((lastPrice - offset) * invTickSize), 0, deltatime);
					}

					lastDateTime = e.Time;
					lastPrice = e.Price;
				}
				else if (vt == Gom.MPProfileData.Volume)
				{
					if (e.Volume >= tickFilterMin && e.Volume <= tickFilterMax)
						curvolattime.AddVolume(CalcTime(e.Time), Convert.ToInt32((e.Price - offset) * invTickSize), CalcDelta(e.TickType, e.Price, e.Volume), e.Volume);
				}
				else if (vt == Gom.MPProfileData.Tick)
					curvolattime.AddVolume(CalcTime(e.Time), Convert.ToInt32((e.Price - offset) * invTickSize), CalcDelta(e.TickType, e.Price, 1), 1);


			}
		}


		private int GetXPos(int barNum)
		{
			return ChartControl.GetXByBarIdx(BarsArray[0], barNum);
		}

		private void PlotCustomProfile(Graphics graphics, int startBar, int endBar)
		{
			double minval = Double.MaxValue, maxval = Double.MinValue;
			bartimevols.DrawBars(startBar, endBar, GetXPos((GHelper.rtl) ? endBar : startBar), GetXPos((GHelper.rtl) ? startBar : endBar));

			if (endBar < CurrentBar)
			{
				for (int i = startBar; i <= endBar; i++)
				{
					if (BarsArray[0].GetHigh(i) > maxval)
						maxval = BarsArray[0].GetHigh(i);

					if (BarsArray[0].GetLow(i) < minval)
						minval = BarsArray[0].GetLow(i);
				}
				using (Pen newPen = (Pen)ChartControl.ChartStyle.Pen.Clone())
				{
					newPen.DashStyle = DashStyle.Dash;
					graphics.DrawRectangle(newPen, GetXPos(startBar), ChartControl.GetYByValue(BarsArray[0], maxval), GetXPos(endBar) - GetXPos(startBar), ChartControl.GetYByValue(BarsArray[0], minval) - ChartControl.GetYByValue(BarsArray[0], maxval));
				}
			}

		}


		public override void Plot(Graphics graphics, Rectangle bounds, double min, double max)
		{
			GHelper.chartcontrol = ChartControl;
			GHelper.graphics = graphics;

			if (Bars == null) return;

			int nbars = BarsArray[0].Count;
			if (nbars == 0) return;

			int lastBar = Math.Min(Math.Max(this.LastBarIndexPainted, 0), nbars - 1);
			int firstBar = Math.Max(this.FirstBarIndexPainted, 0);

			firstBar = Math.Max(startbar, firstBar);
			lastBar = Math.Min(lastcalcbar, lastBar);

			int sideProfile = (GHelper.rtl) ? bounds.Right : bounds.Left;
			int sideOpposite = (GHelper.rtl) ? bounds.Left : bounds.Right;

			int x0;

			GHelper.offset = baroffsets[lastBar];

			if ((correctedCloses) && (lastBar > firstBar))
			{
				PointF[] points = new PointF[lastBar - firstBar + 1];
				for (int i = firstBar; i <= lastBar; i++)
				{
					points[i - firstBar].X = GetXPos(i);
					points[i - firstBar].Y = GHelper.GetYPos(Closes[0][CurrentBars[0] - i] - baroffsets[i]);
				}

				using (Pen pen = new Pen(Color.Orange))
					graphics.DrawLines(pen, points);
			}


			if (GHelper.plotMode == Gom.MPPlotMode.Daily)
			{
				int sessnum = -1;

				do
				{
					sessnum++;
				} while ((sessions[sessnum] < firstBar) && (sessnum < (sessions.Count - 1)));

				if ((sessnum > 0) && ((sessions[sessnum] > lastBar) || GHelper.rtl))
					sessnum--;

				do
				{
					if (sessions[sessnum] <= lastBar)
					{
						if (!GHelper.rtl)
							x0 = Math.Max(GetXPos(sessions[sessnum]), sideProfile);
						else
						{
							if ((sessnum == (sessions.Count - 1)) || (sessions[sessnum + 1] > lastBar))
								x0 = sideProfile;
							else
								x0 = GetXPos(sessions[sessnum + 1]);
						}

						int fb = sessions[Math.Max(0, sessnum - (GHelper.accCount - 1))];
						int lb;
						int maxy;

						if (sessnum < (sessions.Count - 1))
						{
							lb = Math.Max(Math.Min(sessions[sessnum + 1] - 1, lastBar), 0);
							maxy = (GHelper.rtl) ? GetXPos(sessions[sessnum]) : GetXPos(lb);
						}

						else
						{
							lb = lastBar;
							maxy = (GHelper.rtl) ? GetXPos(sessions[sessnum]) : sideOpposite;
						}

						bartimevols.DrawBars(fb, lb, x0, maxy);
					}
					sessnum++;
				} while (sessnum < sessions.Count);
			}


			if (GHelper.plotMode == Gom.MPPlotMode.Bar)
			{
				for (int j = firstBar; j <= lastBar; j++)
				{
					x0 = GetXPos(j) + ((GHelper.rtl) ? -1 : 1) * 3;
					bartimevols.DrawBars(Math.Max(0, j - (GHelper.accCount - 1)), j, x0, sideOpposite);
				}

			}

			if (GHelper.plotMode == Gom.MPPlotMode.All)
				bartimevols.DrawBars(startbar, lastBar, sideProfile, sideOpposite);


			if (GHelper.plotMode == Gom.MPPlotMode.OnScreen)
				bartimevols.DrawBars(firstBar, lastBar, sideProfile, sideOpposite);

			if (GHelper.plotMode == Gom.MPPlotMode.FromDate)
			{
				int fb = Bars.GetBar(GHelper.startDate);

				if (fb < lastBar)
					bartimevols.DrawBars(fb, lastBar, GHelper.rtl ? sideProfile : ((fb < firstBar) ? sideProfile : GetXPos(fb)), sideOpposite);
			}

			if (GHelper.plotMode == Gom.MPPlotMode.LastNDays)
			{
				int fb = sessions[Math.Max(0, sessions.Count - GHelper.accCount)];

				if (fb < lastBar)
					bartimevols.DrawBars(fb, lastBar, GHelper.rtl ? sideProfile : ((fb < firstBar) ? sideProfile : GetXPos(fb)), sideOpposite);
			}

			if (customProfileState == CustomProfileStateEnum.WaitingCloseBar)
			{
				if (((selectedStartBar > selectedEndBar) == GHelper.rtl) || (selectedStartBar == selectedEndBar))
					PlotCustomProfile(graphics, Math.Min(selectedStartBar, selectedEndBar), Math.Max(selectedStartBar, selectedEndBar));
			}

			if (!String.IsNullOrEmpty(customPlots))
			{
				string[] list = customPlots.Split(';');

				foreach (string pair in list)
				{
					string[] barnums = pair.Split(',');

					int bar0 = Int32.Parse(barnums[0]);
					int bar1 = Int32.Parse(barnums[1]);

					if (bar1 == -1)
						PlotCustomProfile(graphics, bar0, Math.Max(bar0, lastBar));
					else
						PlotCustomProfile(graphics, bar0, bar1);
				}
			}

		}


		protected override void GomOnTermination()
		{
			if (mouseUpEvtH != null)
			{
				this.ChartControl.ChartPanel.MouseUp -= mouseUpEvtH;
				mouseUpEvtH = null;
			}

			if (mouseMoveEvtH != null)
			{
				this.ChartControl.ChartPanel.MouseMove -= mouseMoveEvtH;
				mouseMoveEvtH = null;
			}


			GomHK.Dispose();

			base.Dispose();

			bartimevols = null;
			curvolattime = null;
		}

		#region Properties


		[Description("Min Tick Size")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Global Volume Filter: 1.MinSize")]
		public int TickFilterMin
		{
			get { return tickFilterMin; }
			set { tickFilterMin = value; }
		}


		[Description("Max Tick Size")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Global Volume Filter: 2.MaxSize")]
		public int TickFilterMax
		{
			get { return tickFilterMax; }
			set { tickFilterMax = value; }
		}

		[Description("Plotting mode.")]
		[Category("0. Main Settings")]
		[Gui.Design.DisplayNameAttribute("2. Plot Mode")]
		public Gom.MPPlotMode PlotMode
		{
			get { return GHelper.plotMode; }
			set { GHelper.plotMode = value; }
		}

		[Description("Date Start. Only used if PlotMode=FromDate")]
		[Category("0. Main Settings")]
		[Gui.Design.DisplayNameAttribute("3. Start Date")]
		public DateTime StartDate
		{
			get { return GHelper.startDate; }
			set { GHelper.startDate = value; }
		}

		[Description("Color Mode. Time Based color change, Delta and NoColor. Can be toggled using . Key")]
		[Category("0. Main Settings")]
		[Gui.Design.DisplayNameAttribute("1. Color Bar Mode")]
		public Gom.MPColorMode ColorMode
		{
			get { return GHelper.colorMode; }
			set { GHelper.colorMode = value; }
		}

		[Description("Days/Bars to accumulate. Can be incremented/decremented using Ctrl Alt + and Ctrl Alt - keys")]
		[Category("0. Main Settings")]
		[Gui.Design.DisplayNameAttribute("4. Days/Bars to accumulate")]
		public int AccCount
		{
			get { return GHelper.accCount; }
			set { if (value > 0) GHelper.accCount = value; }
		}

		[Description("Number of smoothing bars. Can be toggled using Ctrl + and Ctrl -")]
		[Category("0. Main Settings")]
		[Gui.Design.DisplayNameAttribute("5. Nb smoothing bars")]
		public int NbSmooth
		{
			get { return GHelper.nbSmooth; }
			set { if (value >= 0) GHelper.nbSmooth = value; }
		}

		[Description("Defines plotted data : volume, time or ticks")]
		[Category("0. Main Settings")]
		[Gui.Design.DisplayNameAttribute("6. Plotted Data")]
		public Gom.MPProfileData VT
		{
			get { return vt; }
			set { vt = value; }
		}


		[Description("RTH Start Time")]
		[Category("1. Timing Settings")]
		[Gui.Design.DisplayNameAttribute("1. RTH Start Time")]
		[XmlIgnore()]
		public TimeSpan StartTime
		{
			get { return startTime; }
			set
			{
				startTime = value;
				starttimeminutes = (int)startTime.TotalMinutes;
			}
		}

		[Browsable(false)]
		public string StartTimeSerialize
		{
			get { return startTime.ToString(); }
			set { startTime = TimeSpan.Parse(value); }
		}

		[Description("RTH Session Length")]
		[Category("1. Timing Settings")]
		[Gui.Design.DisplayNameAttribute("2. RTH Session Length")]
		[XmlIgnore()]
		public TimeSpan RTHLength
		{
			get { return rthLength; }
			set
			{
				rthLength = value;
				GHelper.rthlengthminutes = (int)rthLength.TotalMinutes;
				GHelper.ComputeColorMapper();
			}
		}

		[Browsable(false)]
		public string RTHLengthSerialize
		{
			get { return rthLength.ToString(); }
			set { rthLength = TimeSpan.Parse(value); }
		}

		[Description("Show AH trades. Can be toggled using space bar")]
		[Category("1. Timing Settings")]
		[Gui.Design.DisplayNameAttribute("3. Show AH Trades")]
		public Gom.MPAHMode ShowAH
		{
			get { return GHelper.showAH; }
			set { GHelper.showAH = value; }
		}

		[XmlIgnore()]
		[Description("Neutral Color. Used as a clor for AH and if fancy colors is disabled")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayName("1. Neutral Color")]

		public Color NeutralColor
		{
			get { return GHelper.neutralColor; }
			set { GHelper.neutralColor = value; }
		}

		[Browsable(false)]
		public string NeutralColorSerialize
		{
			get { return NinjaTrader.Gui.Design.SerializableColor.ToString(GHelper.neutralColor); }
			set { GHelper.neutralColor = NinjaTrader.Gui.Design.SerializableColor.FromString(value); }
		}

		[Description("Alpha Channel for the color. 0-255 (0= full transparent, 255= full opacity")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayNameAttribute("2. Alpha")]
		public short Alpha
		{
			get { return GHelper.alpha; }
			set
			{
				GHelper.alpha = value;
				GHelper.ComputeColorMapper();
			}
		}

		[Description("Spacing between the volume bars.")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayNameAttribute("3. Bar spacing")]
		public int BarSpacing
		{
			get { return GHelper.BarSpacing; }
			set
			{
				GHelper.BarSpacing = Math.Min(5, Math.Max(0, value));
			}
		}


		[Description("RTL Mode : If true Right to Left, if false Left To Right. Can be toggles using Ctrl * key")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayNameAttribute("4. Right To Left")]
		public bool RTL
		{
			get { return GHelper.rtl; }
			set { GHelper.rtl = value; }
		}

		[Description("Plotting Scale. Determines lengths of bars. Can be modified using + and - keys")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayNameAttribute("5. Plotting Scale")]
		public float Scale
		{
			get { return GHelper.scale; }
			set { GHelper.scale = value; }
		}

		[Description("Show Outline")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayNameAttribute("6. Show Outline")]
		public bool ShowsOutline
		{
			get { return GHelper.showOutline; }
			set { GHelper.showOutline = value; }
		}

		[Editor((typeof(PenEditor)), (typeof(UITypeEditor)))]
		[TypeConverter((typeof(PenConverter)))]
		[XmlIgnore()]
		[Description("Outline pen")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayNameAttribute("7. Outline pen")]
		public Pen OutlinePen
		{
			get { return GHelper.outlinePen; }
			set { GHelper.outlinePen = value; }
		}

		[Browsable(false)]
		public SerializablePen OutlinePenSerialize
		{
			get { return SerializablePen.FromPen(GHelper.outlinePen); }
			set { GHelper.outlinePen = SerializablePen.ToPen(value); }
		}


		[Description("Message Style")]
		[Category("2. Display Settings")]
		[Gui.Design.DisplayNameAttribute("8. Message Style")]
		public Gom.MPMsgStyle HKMsgStyle
		{
			get { return hkMsgStyle; }
			set { hkMsgStyle = value; }
		}


		[Description("Show POC")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("1. Show POC")]
		public bool ShowsPOC
		{
			get { return GHelper.showPoc; }
			set { GHelper.showPoc = value; }
		}

		[Description("Show VA")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("2. Show VA")]
		public bool ShowsVA
		{
			get { return GHelper.showVa; }
			set { GHelper.showVa = value; }
		}

		[Description("Show VWAP")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("3. Show VWAP")]
		public bool ShowsVWAP
		{
			get { return GHelper.showVwap; }
			set { GHelper.showVwap = value; }
		}

		[Description("Minimum size of POC bar and others (vwap etc)")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("4. POC etc bar min height")]
		public int PocSize
		{
			get { return GHelper.pocSize; }
			set
			{
				if (value > 0)
					GHelper.pocSize = value;
			}
		}


		[Description("Make all special bars POC size")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("5. All bars POC size")]
		public bool AllPocVolume
		{
			get { return GHelper.allPocVolume; }
			set { GHelper.allPocVolume = value; }
		}


		[Description("Show continuous POC/VA/VWAP")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("6. Show continuous POC/VWAP/VA lines")]
		public bool ShowContinuous
		{
			get { return GHelper.showContinuous; }
			set { GHelper.showContinuous = value; }
		}





		[Editor((typeof(PenEditor)), (typeof(UITypeEditor)))]
		[TypeConverter((typeof(PenConverter)))]
		[XmlIgnore()]
		[Description("POC continuous line style")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("7. POC Style")]
		public Pen PocPen
		{
			get { return GHelper.pocPen; }
			set { GHelper.pocPen = value; }
		}

		[Browsable(false)]
		public SerializablePen PocPenSerialize
		{
			get { return SerializablePen.FromPen(GHelper.pocPen); }
			set { GHelper.pocPen = SerializablePen.ToPen(value); }
		}

		[Editor((typeof(PenEditor)), (typeof(UITypeEditor)))]
		[TypeConverter((typeof(PenConverter)))]
		[XmlIgnore()]
		[Description("VA continuous line style")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("8. VA Style")]
		public Pen VAPen
		{
			get { return GHelper.vaPen; }
			set { GHelper.vaPen = value; }
		}

		[Browsable(false)]
		public SerializablePen VAPenSerialize
		{
			get { return SerializablePen.FromPen(GHelper.vaPen); }
			set { GHelper.vaPen = SerializablePen.ToPen(value); }
		}

		[Editor((typeof(PenEditor)), (typeof(UITypeEditor)))]
		[TypeConverter((typeof(PenConverter)))]
		[XmlIgnore()]
		[Description("VWAP continuous line style")]
		[Category("3. POC/VA/VWAP")]
		[Gui.Design.DisplayNameAttribute("9. VWAP Style")]
		public Pen VWAPPen
		{
			get { return GHelper.vwapPen; }
			set { GHelper.vwapPen = value; }
		}

		[Browsable(false)]
		public SerializablePen VWAPPenSerialize
		{
			get { return SerializablePen.FromPen(GHelper.vwapPen); }
			set { GHelper.vwapPen = SerializablePen.ToPen(value); }
		}


		[Description("Show HVN/LVN. Can be toggled using * key")]
		[Category("4. HVN/LVN")]
		[Gui.Design.DisplayNameAttribute("1. Show HVN/LVN")]
		public bool ShowVN
		{
			get { return GHelper.showVn; }
			set { GHelper.showVn = value; }
		}

		[Description("Minimum size of peaks, expressed as percentage of the POC. Can be incremented decremented using Alt + and Alt -")]
		[Category("4. HVN/LVN")]
		[Gui.Design.DisplayNameAttribute("2. HVN/LVN Min Size")]
		public int VNSize
		{
			get { return GHelper.vnSize; }
			set { GHelper.vnSize = value; }
		}


		[Editor((typeof(PenEditor)), (typeof(UITypeEditor)))]
		[TypeConverter((typeof(PenConverter)))]
		[XmlIgnore()]
		[Description("HVN Color")]
		[Category("4. HVN/LVN")]
		[Gui.Design.DisplayNameAttribute("3. HVN Style")]
		public Pen HVNPen
		{
			get { return GHelper.hvnPen; }
			set { GHelper.hvnPen = value; }
		}

		[Browsable(false)]
		public SerializablePen HVNPenSerialize
		{
			get { return SerializablePen.FromPen(GHelper.hvnPen); }
			set { GHelper.hvnPen = SerializablePen.ToPen(value); }
		}

		[Editor((typeof(PenEditor)), (typeof(UITypeEditor)))]
		[TypeConverter((typeof(PenConverter)))]
		[XmlIgnore()]
		[Description("LVN Color")]
		[Category("4. HVN/LVN")]
		[Gui.Design.DisplayNameAttribute("4. LVN Style")]
		public Pen LVNPen
		{
			get { return GHelper.lvnPen; }
			set { GHelper.lvnPen = value; }
		}

		[Browsable(false)]
		public SerializablePen LVNPenSerialize
		{
			get { return SerializablePen.FromPen(GHelper.lvnPen); }
			set { GHelper.lvnPen = SerializablePen.ToPen(value); }
		}



		[Description("Color Resolution in minutes (must be >=1). If set to 15, there is one different color every 15 minutes. Lowering the setting will increase color smoothness but will increase memory footprint ")]
		[Category("5. Time Color Settings")]
		[Gui.Design.DisplayNameAttribute("1. Color Resolution")]
		public int MinuteStep
		{
			get { return GHelper.minutestep; }
			set
			{
				GHelper.minutestep = value;
				GHelper.ComputeColorMapper();
			}
		}

		[Description("Starting Color Hue. Must be between 0-360 (see Wikipedia on color Hue")]
		[Category("5. Time Color Settings")]
		[Gui.Design.DisplayNameAttribute("2. Starting Hue")]
		public double StartHue
		{
			get { return GHelper.starthue; }
			set
			{
				GHelper.starthue = value;
				GHelper.ComputeColorMapper();
			}
		}

		[Description("Ending Color Hue. Must be between 0-360 (see Wikipedia on color Hue")]
		[Category("5. Time Color Settings")]
		[Gui.Design.DisplayNameAttribute("3. Ending Hue")]
		public double EndHue
		{
			get { return GHelper.endhue; }
			set
			{
				GHelper.endhue = value;
				GHelper.ComputeColorMapper();
			}
		}

		[Description("Use minute time series as data source instead of GomRecorder file")]
		[Category("6. Data")]
		[Gui.Design.DisplayNameAttribute("1. Data Source")]
		public Gom.MPDataSource DataSource
		{
			get { return dataSource; }
			set
			{
				dataSource = value;
				if (value != Gom.MPDataSource.File)
					WriteData = false;
			}
		}


		[Description("Apply premium correction")]
		[Category("6. Data")]
		[Gui.Design.DisplayNameAttribute("2. Premium correction")]
		public bool PremiumCorrection
		{
			get { return premiumCorrection; }
			set { premiumCorrection = value; }
		}

		[Description("Underlying instrument")]
		[Category("6. Data")]
		[Gui.Design.DisplayNameAttribute("3. Underlying Instr.")]
		public string UnderlyingInstr
		{
			get { return underlyingInstr; }
			set { underlyingInstr = value; }
		}

		[Description("Show Corrected Closes (mostly for debug purposes)")]
		[Category("6. Data")]
		[Gui.Design.DisplayNameAttribute("4. Show Corrected Closes")]
		public bool CorrectedCloses
		{
			get { return correctedCloses; }
			set { correctedCloses = value; }
		}

		[Browsable(false)]
		[Category("7. Custom Profile")]
		[Gui.Design.DisplayNameAttribute("1. Plot List")]
		public string CustomPlots
		{
			get { return customPlots; }
			set { customPlots = value; }
		}
		#endregion
	}

}

namespace Gom
{
	public enum MPColorMode
	{
		NoColor,
		TimeColor,
		Delta,
		Heat
	}

	public enum MPPlotMode
	{
		Daily,
		Bar,
		OnScreen,
		All,
		FromDate,
		None,
		LastNDays
	}

	public enum MPDataSource
	{
		File,
		MinuteData,
		TickData
	}

	public enum MPProfileData
	{
		Volume,
		Time,
		Tick
	}

	public enum MPAHMode
	{
		All,
		RTH,
		AH
	}

	public enum MPMsgStyle
	{
		Full,
		NameOnly,
		NumberOnly,
		None
	}

}

#region NinjaScript generated code. Neither change nor remove.
// This namespace holds all indicators and is required. Do not change it.
namespace NinjaTrader.Indicator
{
    public partial class Indicator : IndicatorBase
    {
        private GomMP[] cacheGomMP = null;

        private static GomMP checkGomMP = new GomMP();

        /// <summary>
        /// Gom Market Profile
        /// </summary>
        /// <returns></returns>
        public GomMP GomMP(int tickFilterMax, int tickFilterMin)
        {
            return GomMP(Input, tickFilterMax, tickFilterMin);
        }

        /// <summary>
        /// Gom Market Profile
        /// </summary>
        /// <returns></returns>
        public GomMP GomMP(Data.IDataSeries input, int tickFilterMax, int tickFilterMin)
        {
            if (cacheGomMP != null)
                for (int idx = 0; idx < cacheGomMP.Length; idx++)
                    if (cacheGomMP[idx].TickFilterMax == tickFilterMax && cacheGomMP[idx].TickFilterMin == tickFilterMin && cacheGomMP[idx].EqualsInput(input))
                        return cacheGomMP[idx];

            lock (checkGomMP)
            {
                checkGomMP.TickFilterMax = tickFilterMax;
                tickFilterMax = checkGomMP.TickFilterMax;
                checkGomMP.TickFilterMin = tickFilterMin;
                tickFilterMin = checkGomMP.TickFilterMin;

                if (cacheGomMP != null)
                    for (int idx = 0; idx < cacheGomMP.Length; idx++)
                        if (cacheGomMP[idx].TickFilterMax == tickFilterMax && cacheGomMP[idx].TickFilterMin == tickFilterMin && cacheGomMP[idx].EqualsInput(input))
                            return cacheGomMP[idx];

                GomMP indicator = new GomMP();
                indicator.BarsRequired = BarsRequired;
                indicator.CalculateOnBarClose = CalculateOnBarClose;
#if NT7
                indicator.ForceMaximumBarsLookBack256 = ForceMaximumBarsLookBack256;
                indicator.MaximumBarsLookBack = MaximumBarsLookBack;
#endif
                indicator.Input = input;
                indicator.TickFilterMax = tickFilterMax;
                indicator.TickFilterMin = tickFilterMin;
                Indicators.Add(indicator);
                indicator.SetUp();

                GomMP[] tmp = new GomMP[cacheGomMP == null ? 1 : cacheGomMP.Length + 1];
                if (cacheGomMP != null)
                    cacheGomMP.CopyTo(tmp, 0);
                tmp[tmp.Length - 1] = indicator;
                cacheGomMP = tmp;
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
        /// Gom Market Profile
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomMP GomMP(int tickFilterMax, int tickFilterMin)
        {
            return _indicator.GomMP(Input, tickFilterMax, tickFilterMin);
        }

        /// <summary>
        /// Gom Market Profile
        /// </summary>
        /// <returns></returns>
        public Indicator.GomMP GomMP(Data.IDataSeries input, int tickFilterMax, int tickFilterMin)
        {
            return _indicator.GomMP(input, tickFilterMax, tickFilterMin);
        }
    }
}

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    public partial class Strategy : StrategyBase
    {
        /// <summary>
        /// Gom Market Profile
        /// </summary>
        /// <returns></returns>
        [Gui.Design.WizardCondition("Indicator")]
        public Indicator.GomMP GomMP(int tickFilterMax, int tickFilterMin)
        {
            return _indicator.GomMP(Input, tickFilterMax, tickFilterMin);
        }

        /// <summary>
        /// Gom Market Profile
        /// </summary>
        /// <returns></returns>
        public Indicator.GomMP GomMP(Data.IDataSeries input, int tickFilterMax, int tickFilterMin)
        {
            if (InInitialize && input == null)
                throw new ArgumentException("You only can access an indicator with the default input/bar series from within the 'Initialize()' method");

            return _indicator.GomMP(input, tickFilterMax, tickFilterMin);
        }
    }
}
#endregion
