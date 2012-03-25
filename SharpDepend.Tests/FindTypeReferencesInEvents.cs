using System;
using System.Collections.Generic;
using Mono.Cecil;
using NUnit.Framework;

namespace SharpDepend.Tests
{
	[TestFixture]
	public class FindTypeReferencesInEventsFixture : FindTypeReferencesFixtureBase
	{
		private static readonly TypeDefinition SampleDefinition = NewTypeDefinition();

		private FindTypeReferencesInEvents _findTypeReferences;

		[SetUp]
		public void SetUp()
		{
			_findTypeReferences = new FindTypeReferencesInEvents();
		}

		[Test]
		public void Can_create_instances()
		{
			Assert.That(_findTypeReferences, Is.Not.Null);
		}

		[Test]
		public void OfType_throws_ArgumentNullException_on_null_definition()
		{
			Assert.Throws<ArgumentNullException>(() => 
				_findTypeReferences.OfType(null));
		}

		[Test]
		public void OfType_returns_a_TypeReference_iterator()
		{
			var result = _findTypeReferences.OfType(SampleDefinition);

			Assert.That(result, Is.InstanceOf<IEnumerable<TypeReference>>());
		}
	}
}
