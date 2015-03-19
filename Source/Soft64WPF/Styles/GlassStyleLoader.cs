/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace Soft64WPF.Styles
{
    public static class GlassStyleLoader
    {
        private static readonly Style s_GlassStyle;

        static GlassStyleLoader()
        {
            if (IsCompositionAvailable)
            {
                ResourceDictionary dictionary = new ResourceDictionary();
                dictionary.Source = new Uri(String.Format("/{0};component/Styles/GlassStyleDictionary.xaml", Assembly.GetEntryAssembly().GetName().Name), UriKind.RelativeOrAbsolute);
                s_GlassStyle = dictionary["GlassWindow"] as Style;
            }
        }

        public static void ApplyWindowGlassStyle(Window window)
        {
            if (IsCompositionAvailable)
            {
                window.Style = s_GlassStyle;
                window.StateChanged += WindowAction_StateChanged;
            }
        }

        public static void RemoveWindowGlassStyle(Window window)
        {
            if (!window.IsActive)
                return;

            if (IsCompositionAvailable)
            {
                window.Style = null;
                window.StateChanged -= WindowAction_StateChanged;
            }
        }

        private static void WindowAction_StateChanged(object sender, EventArgs e)
        {
            /* This hack helps fix the chrome in maxiumized soon */
            Window window = sender as Window;
            window.Dispatcher.Invoke(new Action(() =>
                {
                    window.Hide();
                    window.Show();
                }));
        }

        public static Boolean IsCompositionAvailable
        {
            get
            {
                Boolean enabledFlag = false;

                if (Environment.OSVersion.Version >= new Version(6, 0))
                {
                    /* If we are on vista or higher, make a call to dwm API to see if desktop composition is enabled */
                    DwmIsCompositionEnabled(out enabledFlag);
                }

                return enabledFlag;
            }
        }

        /* DLL function imports for Desktop Window Manager API */

        [DllImport("dwmapi.dll", CharSet = CharSet.None, CallingConvention = CallingConvention.Winapi)]
        private static extern UInt32 DwmIsCompositionEnabled(out Boolean isEnabled);
    }
}