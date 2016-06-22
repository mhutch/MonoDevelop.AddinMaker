using NUnit.Framework;
using System;
namespace MonoDevelop.AddinMaker.Tests
{
	[TestFixture]
	public class GlobalGroperties
	{
		[Test]
		public void GetProperties ()
		{
			var provider = new AddinMSBuildGlobalPropertyProvider ();
			var props = provider.GetGlobalProperties ();

			Assert.AreEqual (3, props.Count);
		}
	}
}