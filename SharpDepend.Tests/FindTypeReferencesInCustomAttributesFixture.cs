using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;

namespace SharpDepend.Tests
{
	[TestFixture]
	public class FindTypeReferencesInCustomAttributesFixture : FindTypeReferencesFixtureBase
	{
		private FindTypeReferencesInCustomAttributes _findInCustomAttributes;

		private static readonly IList<CustomAttribute> CustomAttributes = NewCustomAttributeListOfSize(5);
		private static readonly IEnumerable<TypeReference> CustomAttributeTypes = CustomAttributes
			.Select(x => x.AttributeType);

		private ICustomAttributeProvider _attributeProvider;

		[SetUp]
		public void SetUp()
		{
			_attributeProvider = NewTypeDefinition();
			CustomAttributes.ForEach(x => _attributeProvider.CustomAttributes.Add(x));

			_findInCustomAttributes = new FindTypeReferencesInCustomAttributes();
		}

		[Test]
		public void Can_create_instances()
		{
			Assert.That(_findInCustomAttributes, Is.Not.Null);
		}

		[Test]
		public void OfProvider_throws_argument_null_exception_for_null_provider()
		{
			Assert.Throws<ArgumentNullException>(() => 
				_findInCustomAttributes.OfProvider(null));
		}

		[Test]
		public void OfProvider_return_attribute_types()
		{
			var result = _findInCustomAttributes.OfProvider(_attributeProvider);

			Assert.That(result, Is.EquivalentTo(CustomAttributeTypes));
		}
	}
}