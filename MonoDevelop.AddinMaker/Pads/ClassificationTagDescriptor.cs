// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace MonoDevelop.AddinMaker.Pads
{
	class ClassificationTagDescriptor : DesignerSupport.CustomDescriptor
	{
		public ClassificationTagDescriptor (IClassificationTag classificationTag)
		{
			ClassificationType = classificationTag.ClassificationType;
		}

		[Category ("Classification")]
		[TypeConverter (typeof (ClassificationTypeConverter))]
		public IClassificationType ClassificationType { get; }
	}

	class ClassificationTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			return false;
		}

		public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof (string)) {
				return ((IClassificationType)value).Classification;
			}
			return base.ConvertTo (context, culture, value, destinationType);
		}

		public override bool GetPropertiesSupported (ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties (ITypeDescriptorContext context, object value, Attribute [] attributes)
		{
			var props = new List<PropertyDescriptor> ();
			int n = 0;
			foreach (var bt in ((IClassificationType)value).BaseTypes) {
				props.Add (new BaseTypePropertyDescriptor ($"Base{n++}", bt));
			}
			return new PropertyDescriptorCollection (props.ToArray (), true);
		}

		class BaseTypePropertyDescriptor : PropertyDescriptor
		{
			readonly IClassificationType value;

			public BaseTypePropertyDescriptor (string name, IClassificationType value)
				: base (name, new Attribute [] { new TypeConverterAttribute (typeof (ClassificationTypeConverter)) })
			{
				this.value = value;
			}

			public override Type ComponentType => PropertyType;
			public override bool IsReadOnly => true;
			public override Type PropertyType => typeof (IClassificationType);
			public override bool CanResetValue (object component) => false;
			public override object GetValue (object component) => value;
			public override void ResetValue (object component) => throw new NotSupportedException ();
			public override void SetValue (object component, object value) => throw new NotSupportedException ();
			public override bool ShouldSerializeValue (object component) => false;
		}
	}
}