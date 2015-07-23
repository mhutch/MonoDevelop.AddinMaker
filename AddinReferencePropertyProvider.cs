using MonoDevelop.Core;
using MonoDevelop.DesignerSupport;

namespace MonoDevelop.AddinMaker
{
	class AddinReferencePropertyProvider: IPropertyProvider
	{
		public object CreateProvider (object obj)
		{
			return new AddinReferenceDescriptor ((AddinReference)obj);
		}

		public bool SupportsObject (object obj)
		{
			return obj is AddinReference;
		}

		class AddinReferenceDescriptor: CustomDescriptor
		{
			readonly AddinReference addinReference;

			public AddinReferenceDescriptor (AddinReference addinReference)
			{
				this.addinReference = addinReference;
			}

			[LocalizedCategory ("Addin")]
			[LocalizedDisplayName ("ID")]
			[LocalizedDescription ("ID of the addin.")]
			public string Id {
				get { return addinReference.Include; }
			}

			[LocalizedCategory ("Addin")]
			[LocalizedDisplayName ("Required Version")]
			[LocalizedDescription ("Required version of the addin.")]
			public string Version {
				get { return addinReference.Version; }
				set { addinReference.Version = value; }
			}
		}
	}
}