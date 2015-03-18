//
// AddinMSBuildGlobalPropertyProvider.cs
//
// Author:
//       Michael Hutchinson <m.j.hutchinson@gmail.com>
//
// Copyright (c) 2015 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Projects.Formats.MSBuild;
using Mono.Addins;

namespace MonoDevelop.AddinMaker
{
	#if MD_6

	[Extension ("/MonoDevelop/ProjectModel/MSBuildGlobalPropertyProviders")]
	class AddinMSBuildGlobalPropertyProvider : IMSBuildGlobalPropertyProvider
	{
		Dictionary<string, string> properties;

		#pragma warning disable 67
		public event EventHandler GlobalPropertiesChanged;
		#pragma warning restore 67

		public IDictionary<string, string> GetGlobalProperties ()
		{
			return properties ?? (properties = new Dictionary<string, string> {
				{ "MDAddinsDir" , UserProfile.Current.LocalInstallDir.Combine ("Addins") },
				{ "MDConfigDir" , UserProfile.Current.ConfigDir },
				{ "MDBinDir" , PropertyService.EntryAssemblyPath },
			});
		}
	}

	#endif
}

