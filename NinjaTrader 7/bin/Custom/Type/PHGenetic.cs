/*
 * PH Genetic Optimizer
 * Copyright (c) 2009, Piers Haken
 * 
 * v1.0 - 6/17/2009
 *	Initial Release
 *	
 * v1.01 - 6/20/2009
 *	Add Walkforward checkbox
 *	More suitable defaults
 * 
 * v1.02 - 6/21/2009
 *	BastTestFrom date hack for detecting walkforward
 *	
 * v1.03 - 6/21/2009
 *	added support for Enum parameters
 * 
 * v1.04 - 7/16/2009
 *	added support for bool parameters
 *	dump best DNA at end
 * 
 * v1.05 - 7/23/2009
 *	fix #combinations display
 * 
 * v1.06 - 7/26/2009
 *	use decimal for #combinations calculation
 * 
 * v1.07 - 9/1/2009
 *	don't create parameter definitions for non-primitive types
 * 
 * v1.08 - 1/25/2010
 *	NT7 support
 *	split common stuff into separate file	
 *
 * v1.09 - 1/25/2010
 *	fix 'abort'
 * 
 * v1.10 - 4/3/2010
 *	various fixes
 * 
 * v1.11 - 4/7/2010
 *	moved to separate namespace to avoid conflicts
 * 
 * Portions copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
 * 
 * Losely based on GAOptimizer by Pete_S
 *	http://www.ninjatrader-support2.com/vb/showpost.php?p=30291
 * 
*/


#region Using declarations
using System;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;
using System.Drawing;
using System.Reflection;
#endregion

namespace NinjaTrader.Strategy.PH
{
	#region Options Dialog
	public class GeneticOptionsDialog : OptionsDialog
	{
#if !NT7
		public GeneticOptionsDialog ()
		{
			InitializeComponent ();

			toolTip1.SetToolTip (nudMutationRate,
				"The proability that a parameter will be mutated during procreation.");
			toolTip1.SetToolTip (nudMutationStrength,
				"The amount that a parameter can change due to mutation,\nas a percentage of the parameter's possible range of values.");
			toolTip1.SetToolTip (nudFitnessGain,
				"Higher values increase the chance that better-performing\nindividuals will procreate.");
			toolTip1.SetToolTip (nudAliens,
				"The percentage of a new generation that is entirely randomly generated.");
			toolTip1.SetToolTip (nudStabilitySize,
				"When this fraction of all-time best-performing individuals remains unchanged between generations,\nthe population is considered stable and the algorithm is restarted with mostly random individuals.");
			toolTip1.SetToolTip (nudResetSize,
				"The fraction of best-performing individuals to keep when\nreseeding the population after stability is reached.");
		}
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudMutationRate;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudFitnessGain;
		private System.Windows.Forms.NumericUpDown nudAliens;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown nudMutationStrength;
		private System.Windows.Forms.NumericUpDown nudStabilitySize;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown nudResetSize;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ToolTip toolTip1;

		public double MutationRate
		{
			get { return (double) nudMutationRate.Value / 100.0; }
			//set { nudMutationRate.Value = (decimal) (value * 100); }
		}

		public double MutationStrength
		{
			get { return (double) nudMutationStrength.Value / 100.0; }
			//set { nudMutationStrength.Value = (decimal) (value * 100); }
		}

		public int FitnessGain
		{
			get { return (int) nudFitnessGain.Value; }
			//set { nudFitnessGain.Value = value; }
		}

		public double Aliens
		{
			get { return (double) nudAliens.Value / 100.0; }
			//set { nudAliens.Value = (decimal) (value * 100); }
		}

		public double StabilitySize
		{
			get { return (double) nudStabilitySize.Value / 100.0; }
			//set { nudStabilitySize.Value = (decimal) (value * 100); }
		}

		public double ResetSize
		{
			get { return (double) nudResetSize.Value / 100.0; }
			//set { nudResetSize.Value = (decimal) (value * 100); }
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
			this.label3 = new System.Windows.Forms.Label ();
			this.nudMutationRate = new System.Windows.Forms.NumericUpDown ();
			this.label4 = new System.Windows.Forms.Label ();
			this.nudFitnessGain = new System.Windows.Forms.NumericUpDown ();
			this.nudAliens = new System.Windows.Forms.NumericUpDown ();
			this.label6 = new System.Windows.Forms.Label ();
			this.label7 = new System.Windows.Forms.Label ();
			this.label8 = new System.Windows.Forms.Label ();
			this.label9 = new System.Windows.Forms.Label ();
			this.nudMutationStrength = new System.Windows.Forms.NumericUpDown ();
			this.nudStabilitySize = new System.Windows.Forms.NumericUpDown ();
			this.label10 = new System.Windows.Forms.Label ();
			this.label11 = new System.Windows.Forms.Label ();
			this.nudResetSize = new System.Windows.Forms.NumericUpDown ();
			this.label12 = new System.Windows.Forms.Label ();
			this.label13 = new System.Windows.Forms.Label ();
			this.toolTip1 = new System.Windows.Forms.ToolTip (this.components);
			((System.ComponentModel.ISupportInitialize) (this.nudMutationRate)).BeginInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudFitnessGain)).BeginInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudAliens)).BeginInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudMutationStrength)).BeginInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudStabilitySize)).BeginInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudResetSize)).BeginInit ();
			this.SuspendLayout ();
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label5.Location = new System.Drawing.Point (39, 121);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size (68, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "&Fitness Gain:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point (39, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size (77, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "&Mutation Rate:";
			// 
			// nudMutationRate
			// 
			this.nudMutationRate.Location = new System.Drawing.Point (134, 67);
			this.nudMutationRate.Name = "nudMutationRate";
			this.nudMutationRate.Size = new System.Drawing.Size (50, 20);
			this.nudMutationRate.TabIndex = 5;
			this.nudMutationRate.Value = new decimal (new int [] {
            20,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point (185, 69);
			this.label4.Margin = new System.Windows.Forms.Padding (0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size (15, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "%";
			// 
			// nudFitnessGain
			// 
			this.nudFitnessGain.Location = new System.Drawing.Point (134, 119);
			this.nudFitnessGain.Name = "nudFitnessGain";
			this.nudFitnessGain.Size = new System.Drawing.Size (50, 20);
			this.nudFitnessGain.TabIndex = 11;
			this.nudFitnessGain.Value = new decimal (new int [] {
            30,
            0,
            0,
            0});
			// 
			// nudAliens
			// 
			this.nudAliens.Location = new System.Drawing.Point (134, 145);
			this.nudAliens.Name = "nudAliens";
			this.nudAliens.Size = new System.Drawing.Size (50, 20);
			this.nudAliens.TabIndex = 13;
			this.nudAliens.Value = new decimal (new int [] {
            15,
            0,
            0,
            0});
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label6.Location = new System.Drawing.Point (39, 147);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size (38, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "&Aliens:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point (185, 147);
			this.label7.Margin = new System.Windows.Forms.Padding (0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size (15, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "%";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point (39, 95);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size (94, 13);
			this.label8.TabIndex = 7;
			this.label8.Text = "M&utation Strength:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point (187, 95);
			this.label9.Margin = new System.Windows.Forms.Padding (0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size (15, 13);
			this.label9.TabIndex = 9;
			this.label9.Text = "%";
			// 
			// nudMutationStrength
			// 
			this.nudMutationStrength.Location = new System.Drawing.Point (134, 93);
			this.nudMutationStrength.Name = "nudMutationStrength";
			this.nudMutationStrength.Size = new System.Drawing.Size (50, 20);
			this.nudMutationStrength.TabIndex = 8;
			this.nudMutationStrength.Value = new decimal (new int [] {
            20,
            0,
            0,
            0});
			// 
			// nudStabilitySize
			// 
			this.nudStabilitySize.DecimalPlaces = 1;
			this.nudStabilitySize.Increment = new decimal (new int [] {
            1,
            0,
            0,
            65536});
			this.nudStabilitySize.Location = new System.Drawing.Point (134, 171);
			this.nudStabilitySize.Name = "nudStabilitySize";
			this.nudStabilitySize.Size = new System.Drawing.Size (50, 20);
			this.nudStabilitySize.TabIndex = 16;
			this.nudStabilitySize.Value = new decimal (new int [] {
            4,
            0,
            0,
            0});
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point (185, 173);
			this.label10.Margin = new System.Windows.Forms.Padding (0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size (15, 13);
			this.label10.TabIndex = 17;
			this.label10.Text = "%";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point (39, 173);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size (69, 13);
			this.label11.TabIndex = 15;
			this.label11.Text = "S&tability Size:";
			// 
			// nudResetSize
			// 
			this.nudResetSize.DecimalPlaces = 1;
			this.nudResetSize.Increment = new decimal (new int [] {
            1,
            0,
            0,
            65536});
			this.nudResetSize.Location = new System.Drawing.Point (134, 197);
			this.nudResetSize.Name = "nudResetSize";
			this.nudResetSize.Size = new System.Drawing.Size (50, 20);
			this.nudResetSize.TabIndex = 19;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point (185, 199);
			this.label12.Margin = new System.Windows.Forms.Padding (0);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size (15, 13);
			this.label12.TabIndex = 20;
			this.label12.Text = "%";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point (39, 199);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size (61, 13);
			this.label13.TabIndex = 18;
			this.label13.Text = "&Reset Size:";
			// 
			// toolTip1
			// 
			this.toolTip1.IsBalloon = true;
			// 
			// GeneticOptionsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size (508, 308);
			this.Controls.Add (this.nudResetSize);
			this.Controls.Add (this.label12);
			this.Controls.Add (this.label13);
			this.Controls.Add (this.nudStabilitySize);
			this.Controls.Add (this.label10);
			this.Controls.Add (this.label11);
			this.Controls.Add (this.label6);
			this.Controls.Add (this.label5);
			this.Controls.Add (this.nudAliens);
			this.Controls.Add (this.nudFitnessGain);
			this.Controls.Add (this.nudMutationStrength);
			this.Controls.Add (this.nudMutationRate);
			this.Controls.Add (this.label9);
			this.Controls.Add (this.label7);
			this.Controls.Add (this.label8);
			this.Controls.Add (this.label4);
			this.Controls.Add (this.label3);
			this.Name = "GeneticOptionsDialog";
			this.Controls.SetChildIndex (this.label3, 0);
			this.Controls.SetChildIndex (this.label4, 0);
			this.Controls.SetChildIndex (this.label8, 0);
			this.Controls.SetChildIndex (this.label7, 0);
			this.Controls.SetChildIndex (this.label9, 0);
			this.Controls.SetChildIndex (this.nudMutationRate, 0);
			this.Controls.SetChildIndex (this.nudMutationStrength, 0);
			this.Controls.SetChildIndex (this.nudFitnessGain, 0);
			this.Controls.SetChildIndex (this.nudAliens, 0);
			this.Controls.SetChildIndex (this.label5, 0);
			this.Controls.SetChildIndex (this.label6, 0);
			this.Controls.SetChildIndex (this.label11, 0);
			this.Controls.SetChildIndex (this.label10, 0);
			this.Controls.SetChildIndex (this.nudStabilitySize, 0);
			this.Controls.SetChildIndex (this.label13, 0);
			this.Controls.SetChildIndex (this.label12, 0);
			this.Controls.SetChildIndex (this.nudResetSize, 0);
			((System.ComponentModel.ISupportInitialize) (this.nudMutationRate)).EndInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudFitnessGain)).EndInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudAliens)).EndInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudMutationStrength)).EndInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudStabilitySize)).EndInit ();
			((System.ComponentModel.ISupportInitialize) (this.nudResetSize)).EndInit ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}
		#endregion
#endif
	}
	#endregion

	[Gui.Design.DisplayName ("PH Genetic")]
	public class PHGenetic : PHOptimizationMethod
	{
		#region Variables

		static GeneticOptionsDialog _form = null;
		static DateTime _dateStartLast = DateTime.MaxValue;
		#endregion

		double _mutationRate = 20 / 100.0;
		double _mutationStrength = 20 / 100.0;
		double _fitnessGain = 20;
		double _percentAliens = 20 / 100.0;
		double _stabilitySize = 4 / 100.0;
		double _resetSize = 0 / 100.0;

#if NT7
		[Description ("The proability that a parameter will be mutated during procreation.")]
		[Category ("Optimize")]
		[DisplayName ("% Mutation Rate")]
		public double MutationRate
		{
			get { return _mutationRate * 100.0; }
			set { _mutationRate = Math.Max (0, Math.Min (1, value / 100.0)); }
		}

		[Description ("The amount that a parameter can change due to mutation, as a percentage of the parameter's possible range of values.")]
		[Category ("Optimize")]
		[DisplayName ("% Mutation Strength")]
		public double MutationStrength
		{
			get { return _mutationStrength * 100.0; }
			set { _mutationStrength = Math.Max (0, Math.Min (1, value / 100.0)); }
		}

		[Description ("Higher values increase the chance that better-performing individuals will procreate.")]
		[Category ("Optimize")]
		[DisplayName ("Fitness Gain")]
		public double FitnessGain
		{
			get { return _fitnessGain; }
			set { _fitnessGain = Math.Max (0, value); }
		}


		[Description ("The percentage of a new generation that is entirely randomly generated.")]
		[Category ("Optimize")]
		[DisplayName ("% Aliens")]
		public double PercentAliens
		{
			get { return _percentAliens * 100.0; }
			set { _percentAliens = Math.Max (0, Math.Min (1, value / 100.0)); }
		}


		[Description ("When this fraction of all-time best-performing individuals remains unchanged between generations, the population is considered stable and the algorithm is restarted with mostly random individuals.")]
		[Category ("Optimize")]
		[DisplayName ("% Stability Size")]
		public double StabilitySize
		{
			get { return _stabilitySize * 100.0; }
			set { _stabilitySize = Math.Max (0, Math.Min (1, value / 100.0)); }
		}

		[Description ("The fraction of best-performing individuals to keep when reseeding the population after stability is reached.")]
		[Category ("Optimize")]
		[DisplayName ("% Reset Size")]
		public double ResetSize
		{
			get { return _resetSize * 100.0; }
			set { _resetSize = Math.Max (0, Math.Min (1, value / 100.0)); }
		}

#endif

		public override void Optimize ()
		{
			DNA dnaBestEver = null;

			try
			{
				// initialize the parameter definition

				bool fFirstRun = (_form == null);
				_rgParameterDefinitions = new ParameterDefinitions (this.Strategy, fFirstRun);
				if (fFirstRun)
					_form = new GeneticOptionsDialog ();
				_form.InitializeParameters (_rgParameterDefinitions);

				DateTime dateStartLast = _dateStartLast;
				_dateStartLast = Strategy.BackTestFrom;

				if (!_form.WalkForward || Strategy.BackTestFrom <= dateStartLast)
				{
					if (_form.ShowDialog (Form.ActiveForm) != DialogResult.OK)
					{
#if NT7
						WaitForIterationsCompleted (true);
#endif
						return;
					}
				}

#if !NT7
				_nPopulationSize = _form.PopulationSize;
				_cMaxGenerations = _form.MaxGenerations;
				_mutationRate = _form.MutationRate;
				_mutationStrength = _form.MutationStrength;
				_fitnessGain = _form.FitnessGain;
				_percentAliens = _form.Aliens;
				_stabilitySize = _form.StabilitySize;
				_resetSize = _form.ResetSize;
				_nMinimumPerformce = _form.MinimumPerformance;
#endif

				int cAliens = (int) Math.Ceiling (_nPopulationSize * _percentAliens);
				int nStabilitySize = (int) Math.Ceiling (_nPopulationSize * _stabilitySize);
				int nResetSize = _nPopulationSize - (int) Math.Ceiling (_nPopulationSize * _resetSize);

				DateTime timeStart = DateTime.Now;

				int cParams = _rgParameterDefinitions.Count;

				decimal cCombinationsActual = _rgParameterDefinitions.CountCombinations;
				long cCombinations = (long) Math.Min (cCombinationsActual, (decimal) long.MaxValue);

				Strategy.Print (
					string.Format (
						"{0}: PH Genetic Optimizer Start @ {1}, combinations: {2:N0}",
						Strategy.Instrument.FullName,
						timeStart,
						cCombinationsActual
					)
				);


				List<DNA> rgChildren = new List<DNA> ();
				Dictionary<DNA, bool> setChildren = new Dictionary<DNA, bool> ();

				// create an initial generation of aliens
				int cIterations = 0;


				double stabilityScorePrev = double.NaN;

				List<DNA> rgParents = new List<DNA> ();
				int iGeneration;
				for (iGeneration = 1; iGeneration <= _cMaxGenerations; iGeneration++)
				{
					// fill the rest of this generation with aliens
					while (rgChildren.Count < _nPopulationSize && setChildren.Count < cCombinations)
					{
						DNA child = CreateAlien (0);
						if (setChildren.ContainsKey (child))
							continue;

						setChildren.Add (child, true);
						rgChildren.Add (child);
					}

					DNA dnaBestSinceReset = null;

					try
					{
						// score the new generation
						ScoreGeneration (rgChildren);

						//if (rgParents.Count > nPopulationSize)
						//	rgParents.RemoveRange (nPopulationSize, rgParents.Count - nPopulationSize);

						rgParents.AddRange (rgChildren);
						rgParents.Sort ();

						dnaBestSinceReset = rgParents [0];
					}
					finally
					{
						if (rgParents.Count > 0 &&
							dnaBestSinceReset != null &&
							(dnaBestEver == null || dnaBestEver.Performance < dnaBestSinceReset.Performance))
						{
							dnaBestEver = dnaBestSinceReset;
						}
					}
					
					double stabilityScore = 0.0;
					int cStabilityIndividuals = (int) Math.Min (rgParents.Count, nStabilitySize);
					if (cStabilityIndividuals < 1)
						cStabilityIndividuals = 1;

					for (int i = 0; i < cStabilityIndividuals; i++)
					{
						double scoreParent = rgParents [i].Performance;
						if (scoreParent > double.NegativeInfinity)
							stabilityScore += rgParents [i].Performance;
					}

					cIterations += rgChildren.Count;
					TimeSpan diff = DateTime.Now - timeStart;
					double iterPerSec = cIterations / diff.TotalSeconds;

					Strategy.Print (string.Format (
						"{0}: gen {1}/{2}, {3:N1} ips, max PERF: {4:N2}, stability PERF: {5:N2}\t{6}",
						Strategy.Instrument.FullName,
						iGeneration,
						_cMaxGenerations,
						iterPerSec,
						dnaBestSinceReset.Performance,
						stabilityScore / cStabilityIndividuals,
						rgParents.Count == 0 ? "" : _rgParameterDefinitions.ToString (dnaBestSinceReset)));


					if (dnaBestEver != null)
						Strategy.Print (dnaBestEver.Performance + ", " +  _nMinimumPerformce);
					if (dnaBestEver != null && dnaBestEver.Performance > _nMinimumPerformce)
					{
						Strategy.Print (string.Format ("{0}: Minimum performance reached.", Strategy.Instrument.FullName));
						return;
					}

					if (iGeneration == _cMaxGenerations)
						break;

					if (setChildren.Count >= cCombinations)
						break;

					if (stabilityScore == stabilityScorePrev)
					{
						//rgParents.RemoveRange (1, rgParents.Count - 1);
						rgParents.Clear ();
						rgChildren.Clear ();
						Strategy.Print (string.Format (
							"{0}: reset",
							Strategy.Instrument.FullName
							)
						);
						//stabilityScore = double.NaN;

						continue;
					}

					// get the range of PERF values
					double minPerformance = double.MaxValue;
					double maxPerformance = double.MinValue;
					for (int i = 0; i < rgParents.Count; i++)
					{
						double performance = rgParents [i].Performance;
						if (performance == double.NegativeInfinity)
							continue;
						if (minPerformance > performance)
							minPerformance = performance;
						if (maxPerformance < performance)
							maxPerformance = performance;
					}

					// weight the procreation probabilities
					if (maxPerformance != minPerformance)
					{
						for (int i = 0; i < rgParents.Count; i++)
						{
							DNA parent = rgParents [i];
							double performance = parent.Performance;
							if (performance == double.NegativeInfinity)
								parent.Weight = 0;
							else
							{
								double weight = (parent.Performance - minPerformance) / (maxPerformance - minPerformance);
#if true
								parent.Weight = Math.Pow (weight, _fitnessGain) + 0.001;
#else	
								parent.Weight = (Math.Pow (weight, fitnessGain / (iGeneration - parent.Generation)) + 0.001);
#endif
							}
						}
					}

					double totalWeight = 0;
					for (int i = 0; i < rgParents.Count; i++)
						totalWeight += rgParents [i].Weight;


					// initialize a new generation
					rgChildren.Clear ();

					// a certain percentage of the new population are aliens:
					int cAliensThisGeneration;
					if (stabilityScore == stabilityScorePrev)
						cAliensThisGeneration = nResetSize;
					else
						cAliensThisGeneration = cAliens;

					while (rgChildren.Count < cAliensThisGeneration && setChildren.Count < cCombinations)
					{
						DNA child = CreateAlien (iGeneration);
						if (setChildren.ContainsKey (child))
							continue;

						setChildren.Add (child, true);
						rgChildren.Add (child);
					}

					// fuzz each param individually
					for (int iParam = 0; iParam < cParams; iParam++)
					{
						int cValues = _rgParameterDefinitions [iParam].CountValues;

						for (int iFuzz = 0; iFuzz < 5; iFuzz++)
						{
							int [] rgParams = new int [cParams];
							for (int iParamFuzz = 0; iParamFuzz < cParams; iParamFuzz++)
								rgParams [iParamFuzz] = dnaBestSinceReset.GetParam (iParamFuzz);

							rgParams [iParam] = MutateParam (_mutationStrength, cValues, rgParams [iParam]);

							DNA child = new DNA (rgParams, dnaBestSinceReset.Generation + 1);
							if (!setChildren.ContainsKey (child))
							{
								setChildren.Add (child, true);
								rgChildren.Add (child);
							}
						}
					}

					while (rgChildren.Count < _nPopulationSize - cAliensThisGeneration && setChildren.Count < cCombinations)
					{
						if (UserAbort)
							throw new AbortOptimizationException ();

						// find parents rendomly according to weighted fitness
						double weightA = _rand.NextDouble () * totalWeight;
						int iParentA = 0;
						while (iParentA < rgParents.Count && weightA > rgParents [iParentA].Weight)
						{
							weightA -= rgParents [iParentA].Weight;
							iParentA++;
						}

						double weightB = _rand.NextDouble () * totalWeight;
						int iParentB = 0;
						while (iParentB < rgParents.Count && weightB > rgParents [iParentB].Weight)
						{
							weightB -= rgParents [iParentB].Weight;
							iParentB++;
						}

						// these are the parents:
						DNA parentA = rgParents [iParentA];
						DNA parentB = rgParents [iParentB];

						// create a new set of genes
						int [] rgParams = new int [cParams];
						for (int iParam = 0; iParam < cParams; iParam++)
						{
							// Take an attribute from parents
							DNA parent = (_rand.Next (100) < 50) ? parentA : parentB;
							rgParams [iParam] = parent.GetParam (iParam);

							// Should we mutate?
							int cValues = _rgParameterDefinitions [iParam].CountValues;
							if (cValues > 1 && _rand.NextDouble () < _mutationRate)
							{
								rgParams [iParam] = MutateParam (_mutationStrength, cValues, rgParams [iParam]);
							}
						}

						// create the new child, and make sure it's unique
						//int iGenerationChild = iGeneration;
						//int iGenerationChild = 1 + Math.Min (parentA.Generation, parentB.Generation);
						int iGenerationChild = (parentA.Generation + parentB.Generation) / 2;
						DNA child = new DNA (rgParams, iGenerationChild);
						if (!setChildren.ContainsKey (child))
						{
							setChildren.Add (child, true);
							rgChildren.Add (child);
						}
					}

					stabilityScorePrev = stabilityScore;
				}

				DateTime timeEnd = DateTime.Now;
				TimeSpan tsDuration = timeEnd - timeStart;

				Strategy.Print (string.Format (
					"{0}: PH Optimizer End @ {1}, duration: {2} ({3} per iteration)",
						Strategy.Instrument.FullName,
						timeEnd,
						tsDuration,
						TimeSpan.FromTicks (tsDuration.Ticks / iGeneration)
					)
				);

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
				if (dnaBestEver != null)
					Dump (dnaBestEver);
			}

#if NT7
			WaitForIterationsCompleted (true);
#endif
		}


		private int MutateParam (double mutationStrength, int cValues, int iValue)
		{
			int iValueMin = iValue - (int) (cValues * mutationStrength);
			int iValueMax = iValue + (int) (cValues * mutationStrength);
			int iValueNew = (_rand.Next (iValueMin, iValueMax) + _rand.Next (iValueMin, iValueMax)) / 2;

			if (iValueNew < 0)
				iValueNew = 0;
			else if (iValueNew >= cValues)
				iValueNew = cValues - 1;

			return iValueNew;
		}

		DNA ScoreGeneration (IEnumerable<DNA> rgChildren)
		{
			double maxPerformance = double.NegativeInfinity;
			DNA best = null;

			foreach (DNA child in RunIterations (rgChildren))
			{
				if (maxPerformance < child.Performance)
				{
					maxPerformance = child.Performance;
					best = child;
				}
			}

			return best;
		}

		DNA CreateAlien (int iGeneration)
		{
			int cParams = _rgParameterDefinitions.Count;
			int [] rgParams = new int [cParams];
			for (int iParam = 0; iParam < cParams; iParam++)
				rgParams [iParam] = _rand.Next (_rgParameterDefinitions [iParam].CountValues);

			return new DNA (rgParams, iGeneration);
		}

		class DNA : ParameterSet, IEquatable<DNA>, IComparable<DNA>
		{
			int _iGeneration;

			public DNA (int [] rgParams, int iGeneration)
				: base (rgParams)
			{
				_iGeneration = iGeneration;
			}

			public int Generation { get { return _iGeneration; } }

			#region IEquatable<DNA> Members

			public bool Equals (DNA other)
			{
				return base.Equals (other as ParameterSet);
			}

			#endregion

			#region IComparable<DNA> Members

			public int CompareTo (DNA other)
			{
				return base.CompareTo (other as ParameterSet);
			}

			#endregion
		}

#if NT7
		public override object Clone ()
		{
			PHGenetic clone = new PHGenetic ();
			clone.Parameters = (ParameterCollection) this.Parameters.Clone ();
			clone.MaxGenerations = this.MaxGenerations;
			clone.PopulationSize = this.PopulationSize;
			clone.MutationRate = this.MutationRate;
			clone.MutationStrength = this.MutationStrength;
			clone.FitnessGain = this.FitnessGain;
			clone.PercentAliens = this.PercentAliens;
			clone.StabilitySize = this.StabilitySize;
			clone.ResetSize = this.ResetSize;
			clone.MinimumPerformance = this.MinimumPerformance;

			return clone;
		}

		public override bool RandomWalk { get { return true; } }
#endif
	}
}

