﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace GoToWindow.Api.Tests
{
    [TestClass]
    public class WindowListFactoryTests
    {
        [TestMethod]
        public void CanGetAListOfActiveWindows_ContainingVisualStudio()
        {
            using (var givenAWindow = new GivenAWindow("GoToWindow.CanGetAListOfActiveWindows_ContainingVisualStudio"))
            {
                var windowsList = WindowsListFactory.Load();
                var windows = windowsList.Windows;

                AssertExists(windows, givenAWindow.Expected);
            }
        }

        private void AssertExists(IList<IWindowEntry> windows, IWindowEntry expected)
        {
            var containsExpected = windows.Count(window => expected.IsSameButHWnd(window)) >= 1;
            Assert.IsTrue(containsExpected, String.Format("Expected window {0}.\r\nWindows List:\r\n{1}", expected, String.Join("\r\n", windows)));
        }
    }
}