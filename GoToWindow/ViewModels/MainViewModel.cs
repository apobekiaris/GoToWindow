﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using GoToWindow.Commands;
using GoToWindow.Extensibility;
using log4net;

namespace GoToWindow.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(GoToWindowContext).Assembly, "GoToWindow");

		public static MainViewModel Load(IEnumerable<IGoToWindowPlugin> plugins)
		{
			var list = new List<ISearchResult>();
			var disabledPlugins = Properties.Settings.Default.DisabledPlugins ?? new StringCollection();

			foreach (var plugin in plugins.Where(plugin => !disabledPlugins.Contains(plugin.Id)))
			{
				Stopwatch stopwatch = null;

				if (Log.IsDebugEnabled)
				{
					stopwatch = new Stopwatch();
					stopwatch.Start();
				}

				plugin.BuildList(list);

				if (stopwatch != null)
				{
					stopwatch.Stop();
					Log.Debug(string.Format("Plugin '{0}' took {1} to execute, now at {2} results.", plugin.Title, stopwatch.Elapsed, list.Count));
				}
			}

			var instance = new MainViewModel();
			var viewSource = new CollectionViewSource
			{
				Source = list.ToArray()
			};
			instance.Windows = viewSource;
			return instance;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public CollectionViewSource Windows { get; private set; }
		public ISearchResult SelectedWindowEntry { get; set; }

		private string _searchText;

		public string SearchText
		{
			get { return _searchText; }
			set
			{
				_searchText = value;
				OnPropertyChanged("SearchText");
			}
		}

		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public ICommand GoToWindowEntryShortcut { get; private set; }

		public event EventHandler Close;

		public MainViewModel()
		{
			var goToWindowEntryShortcutCommand = new GoToWindowEntryShortcutCommand(GetEntryAt);
			goToWindowEntryShortcutCommand.Executed += GoToWindowEntryShortcutCommand_Executed;
			GoToWindowEntryShortcut = goToWindowEntryShortcutCommand;
		}

		private ISearchResult GetEntryAt(int index)
		{
			var items = Windows.View.Cast<ISearchResult>().ToArray();

			if (index < items.Length)
				return items[index];

			return null;
		}

		void GoToWindowEntryShortcutCommand_Executed(object sender, EventArgs e)
		{
			if (Close != null)
				Close(this, new EventArgs());
		}

	}
}