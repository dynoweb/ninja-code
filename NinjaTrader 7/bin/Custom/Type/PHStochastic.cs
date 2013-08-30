#region Using declarations
using System;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
#endregion

namespace NinjaTrader.Strategy.PH
{
	#region Options Dialog
	public class StochasticsOptionsDialog : OptionsDialog
	{
#if !NT7
		public StochasticsOptionsDialog ()
		{
			InitializeComponent ();

			toolTip1.SetToolTip (nudFitnessGain,
				"Higher values increase the chance that better-performing\nindividuals will procreate.");
		}
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudFitnessGain;
		private System.Windows.Forms.ToolTip toolTip1;

		public int FitnessGain
		{
			get { return (int) nudFitnessGain.Value; }
			//set { nudFitnessGain.Value = value; }
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose ();
			}
			base.Dispose (disposing);
		}


		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.components = new System.ComponentModel.Container ();
			this.label5 = new System.Windows.Forms.Label ();
			this.nudFitnessGain = new System.Windows.Forms.NumericUpDown ();
			this.toolTip1 = new System.Windows.Forms.ToolTip (this.components);
			((System.ComponentModel.ISupportInitialize) (this.nudFitnessGain)).BeginInit ();
			this.SuspendLayout ();
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label5.Location = new System.Drawing.Point (39, 69);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size (68, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "&Fitness Gain:";
			// 
			// nudFitnessGain
			// 
			this.nudFitnessGain.Location = new System.Drawing.Point (134, 67);
			this.nudFitnessGain.Name = "nudFitnessGain";
			this.nudFitnessGain.Size = new System.Drawing.Size (50, 20);
			this.nudFitnessGain.TabIndex = 11;
			this.nudFitnessGain.Value = new decimal (new int [] {
            30,
            0,
            0,
            0});
			// 
			// toolTip1
			// 
			this.toolTip1.IsBalloon = true;
			// 
			// StochasticsOptionsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size (508, 308);
			this.Controls.Add (this.label5);
			this.Controls.Add (this.nudFitnessGain);
			this.Name = "StochasticsOptionsDialog";
			this.Controls.SetChildIndex (this.nudFitnessGain, 0);
			this.Controls.SetChildIndex (this.label5, 0);
			((System.ComponentModel.ISupportInitialize) (this.nudFitnessGain)).EndInit ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}
		#endregion
#endif
	}
	#endregion

	[NinjaTrader.Gui.Design.DisplayName ("PH Stochastic")]
	public class PHStochastic : PHOptimizationMethod
	{
		static StochasticsOptionsDialog _form = null;
		static DateTime _dateStartLast = DateTime.MaxValue;

		int [] [] _rgParameterRunCounts;
		double [] [] _rgParameterRunTotals;
		double [] [] _rgParameterRunAverage;

		[Description ("The maximum gain that is used when seeking nearby imprevements.")]
		[Category ("Optimize")]
		[DisplayName ("Fitness Gain")]
		public int FitnessGainMax
		{
			get { return _fitnessGainMax; }
			set { _fitnessGainMax = Math.Max (0, value); }
		}
		int _fitnessGainMax = 20;

		[Description ("The percentage of each generation that focuses on improving a single parameter.")]
		[Category ("Optimize")]
		[DisplayName ("% Annealing")]
		public int PercentAnnealing
		{
			get { return _percentAnnealing; }
			set { _percentAnnealing = Math.Max (0, Math.Min (100, value)); }
		}
		int _percentAnnealing = 20;


		public PHStochastic ()
		{
		}

		public override void Optimize ()
		{
			bool fFirstRun = (_form == null);
			_rgParameterDefinitions = new ParameterDefinitions (this.Strategy, fFirstRun);
			if (fFirstRun)
				_form = new StochasticsOptionsDialog ();
			_form.InitializeParameters (_rgParameterDefinitions);

			DateTime dateStartLast = _dateStartLast;
			_dateStartLast = Strategy.BackTestFrom;

			if (!_form.WalkForward || Strategy.BackTestFrom <= dateStartLast)
			{
				if (_form.ShowDialog (Form.ActiveForm) != DialogResult.OK)
					return;
			}

#if !NT7
			_nPopulationSize = _form.PopulationSize;
			_cMaxGenerations = _form.MaxGenerations;
			_fitnessGainMax = _form.FitnessGain;
			_nPopulationSize = 10;//_form.PercentAnnealing;
			_nMinimumPerformce = _form.MinimumPerformance;
#endif

			int cParams = _rgParameterDefinitions.Count;

			_rgParameterRunCounts = new int [cParams] [];
			_rgParameterRunTotals = new double [cParams] [];
			_rgParameterRunAverage = new double [cParams] [];

			decimal cCombinationsActual = _rgParameterDefinitions.CountCombinations;
			long cCombinations = (long) Math.Min (cCombinationsActual, (decimal) long.MaxValue);

			for (int ipd = 0; ipd < _rgParameterDefinitions.Count; ipd++)
			{
				ParameterDefinition pd = _rgParameterDefinitions [ipd];

				int cValues = pd.CountValues;

				_rgParameterRunCounts [ipd] = new int [cValues];
				_rgParameterRunTotals [ipd] = new double [cValues];
				_rgParameterRunAverage [ipd] = new double [cValues];
			}

			Dictionary<ParameterSet, bool> setChildren = new Dictionary<ParameterSet, bool> ();
			List<ParameterSet> rgChildren = new List<ParameterSet> ();


			int fitnessGainMin = 1;
			int cAnnealing = _nPopulationSize * _percentAnnealing / 100;

			// create an initial generation of aliens
			int cIterations = 0;
			while (rgChildren.Count < _nPopulationSize)
			{
				ParameterSet child = CreateAlien ();
				if (setChildren.ContainsKey (child))
					continue;

				setChildren.Add (child, true);
				rgChildren.Add (child);
			}


			DateTime timeStart = DateTime.Now;
			ParameterSet bestChild = null;
			int iGeneration = 0;
			try
			{

				Strategy.Print (
					string.Format (
						"{0}: PH Stochastic Optimizer Start @ {1}, combinations: {2:N0}",
						Strategy.Instrument.FullName,
						timeStart,
						cCombinationsActual
					)
				);

				double diffMax = 0;
				double diffCurr = 0;
				int fitnessGain = fitnessGainMin;

				ParameterSet bestPrev = null;
				for (iGeneration = 0; iGeneration < _cMaxGenerations; iGeneration++)
				{
					// score the new generation
					ScoreGeneration (rgChildren);

					rgChildren.Sort ();

					if (bestChild == null)
						bestChild = rgChildren [0];
					else if (bestChild.Performance < rgChildren [0].Performance)
						bestChild = rgChildren [0];

					if (bestPrev != null)
					{
						if (double.IsNaN (bestPrev.Performance) || bestChild.Performance > bestPrev.Performance)
						{
							if (fitnessGain < FitnessGainMax)
								fitnessGain++;
						}
						else if (fitnessGain > fitnessGainMin)
							fitnessGain--;
					}
					bestPrev = bestChild;

					cIterations += rgChildren.Count;
					TimeSpan diff = DateTime.Now - timeStart;
					double iterPerSec = cIterations / diff.TotalSeconds;

					Strategy.Print (string.Format (
						"{0}: gen {1}/{2}, {3:N1} ips, max PERF: {4:N2}, gain={5}\t{6}",
						Strategy.Instrument.FullName,
						iGeneration + 1,
						_cMaxGenerations,
						iterPerSec,
						bestChild.Performance,
						fitnessGain,
						bestChild == null ? "" : _rgParameterDefinitions.ToString (bestChild)));

					if (bestChild != null && bestChild.Performance > this._nMinimumPerformce)
					{
						Strategy.Print (string.Format ("{0}: Minimum performance reached.", Strategy.Instrument.FullName));
						return;
					}

					if (iGeneration == _cMaxGenerations - 1)
						break;

					// initialize a new generation
					rgChildren.Clear ();


					int cCollisions = _nPopulationSize;
					while (cCollisions > 0 && rgChildren.Count < _nPopulationSize)
					{
						int iParam = _rand.Next (cParams);
						int cValues = _rgParameterDefinitions [iParam].CountValues;

						int [] rgParams = new int [cParams];
						for (int iParamFuzz = 0; iParamFuzz < cParams; iParamFuzz++)
							rgParams [iParamFuzz] = bestChild.GetParam (iParamFuzz);

						rgParams [iParam] = MutateParam (fitnessGain, cValues, rgParams [iParam]);

						ParameterSet child = new ParameterSet (rgParams);
						if (setChildren.ContainsKey (child))
						{
							cCollisions--;
						}
						else
						{
							setChildren.Add (child, true);
							rgChildren.Add (child);
						}
					}

					double loopFitnessGain = fitnessGain;
					if (rgChildren.Count < _nPopulationSize)
					{
						// pre-calculate the parameter value weights
						double [] [] rgrgWeights = new double [cParams] [];
						double [] rgTotalWeights = new double [cParams];

						for (int iParam = 0; iParam < cParams; iParam++)
						{
							int cValues = _rgParameterDefinitions [iParam].CountValues;
							rgrgWeights [iParam] = new double [cValues];

							double [] rgAverages = _rgParameterRunAverage [iParam];


							// get the range of PERF values
							double minPerformance = double.PositiveInfinity;
							double maxPerformance = double.NegativeInfinity;
							for (int i = 0; i < cValues; i++)
							{
								double performance = rgAverages [i];
								if (minPerformance > performance)
									minPerformance = performance;
								if (maxPerformance < performance)
									maxPerformance = performance;
							}

							// weight the procreation probabilities
							double totalWeight = 0;
							//if (maxPerformance != minPerformance)
							{
								for (int iValue = 0; iValue < cValues; iValue++)
								{
									double average = rgAverages [iValue];
									if (double.IsNaN (average) || double.IsInfinity (average))
										average = maxPerformance;

									double weight = Math.Pow ((average - minPerformance) / (maxPerformance - minPerformance), loopFitnessGain);
									if (double.IsNaN (weight))
										weight = 0.0;
									weight += 0.001;

									rgrgWeights [iParam] [iValue] = weight;
									totalWeight += weight;
								}
							}

							rgTotalWeights [iParam] = totalWeight;
						}

						cCollisions = _nPopulationSize;
						while (cCollisions > 0 && rgChildren.Count < _nPopulationSize && setChildren.Count < cCombinations)
						{
							// create a new set of genes
							int [] rgParams = new int [cParams];
							for (int iParam = 0; iParam < cParams; iParam++)
							{
								int cValues = _rgParameterDefinitions [iParam].CountValues;
								double [] rgWeights = rgrgWeights [iParam];

								double weightRandom = _rand.NextDouble () * rgTotalWeights [iParam];
								int iValueRandom = 0;
								while (iValueRandom < cValues && weightRandom > rgWeights [iValueRandom])
								{
									weightRandom -= rgWeights [iValueRandom];
									iValueRandom++;
								}

								rgParams [iParam] = iValueRandom;
							}


							// create the new child, and make sure it's unique
							ParameterSet child = new ParameterSet (rgParams);
							if (setChildren.ContainsKey (child))
							{
								if (loopFitnessGain > 1)
									loopFitnessGain--;

								cCollisions--;
							}
							else
							{
								setChildren.Add (child, true);
								rgChildren.Add (child);
							}
						}
					}
				}


			}
			catch (AbortOptimizationException)
			{
				Strategy.Print ("Optimization Aborted...");
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine (e);
				Strategy.Print (e.ToString ());
				//throw;
			}
			finally
			{
#if NT7
				WaitForIterationsCompleted (true);
#endif

				DateTime timeEnd = DateTime.Now;
				TimeSpan tsDuration = timeEnd - timeStart;

				if (iGeneration > 0)
				{
					Strategy.Print (string.Format (
						"{0}: PH Optimizer End @ {1}, duration: {2} ({3} per iteration)",
							Strategy.Instrument.FullName,
							timeEnd,
							tsDuration,
							TimeSpan.FromTicks (tsDuration.Ticks / iGeneration)
						)
					);
				}

				if (bestChild != null)
					Dump (bestChild);
			}
		}


		ParameterSet CreateAlien ()
		{
			int cParams = _rgParameterDefinitions.Count;
			int [] rgParams = new int [cParams];
			for (int iParam = 0; iParam < cParams; iParam++)
				rgParams [iParam] = _rand.Next (_rgParameterDefinitions [iParam].CountValues);

			return new ParameterSet (rgParams);
		}


		private int MutateParam (int mutationStrength, int cValues, int iValue)
		{
			double r = 0;
			for (int i = 0; i < mutationStrength; i++)
				r += _rand.NextDouble ();
			r -= mutationStrength / 2.0;

			int iValueNew = (int) Math.Round (iValue + r * cValues / mutationStrength);

			if (iValueNew < 0)
				iValueNew = 0;
			else if (iValueNew >= cValues)
				iValueNew = cValues - 1;

			return iValueNew;
		}


		void ScoreGeneration (IEnumerable<ParameterSet> rgChildren)
		{
			int cParams = _rgParameterDefinitions.Count;
			foreach (ParameterSet child in RunIterations (rgChildren))
			{
				// record the strategy's performance
				double performance = child.Performance;

				if (performance > double.NegativeInfinity)
				{
					for (int iParam = 0; iParam < cParams; iParam++)
					{
						int iValue = child.GetParam (iParam);

						_rgParameterRunCounts [iParam] [iValue]++;
						_rgParameterRunTotals [iParam] [iValue] += performance;
						_rgParameterRunAverage [iParam] [iValue] = _rgParameterRunTotals [iParam] [iValue] / _rgParameterRunCounts [iParam] [iValue];
					}
				}
			}
		}

#if NT7
		public override object Clone ()
		{
			PHStochastic clone = new PHStochastic ();
			clone.Parameters = (ParameterCollection) this.Parameters.Clone ();
			clone.MaxGenerations = this.MaxGenerations;
			clone.PopulationSize = this.PopulationSize;
			clone.FitnessGainMax = this.FitnessGainMax;
			clone.MinimumPerformance = this.MinimumPerformance;
			return clone;
		}

		public override bool RandomWalk { get { return true; } }
#endif
	}
}