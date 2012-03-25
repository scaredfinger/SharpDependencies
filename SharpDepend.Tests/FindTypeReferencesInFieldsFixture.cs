using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;

namespace SharpDepend.Tests
{
	[TestFixture]
	public class FindTypeReferencesInFieldsFixture : FindTypeReferencesFixtureBase
	{
		private static readonly TypeDefinition SampleDefinition = NewTypeDefinition();
		private static readonly IList<FieldDefinition> SampleDefinitionFields = CreateFields(4);

		private static readonly IEnumerable<TypeReference> SampleDefinitionFieldTypes =
			SampleDefinitionFields.Select(x => x.FieldType);

		private static IList<FieldDefinition> CreateFields(int  n)
		{
			var result = new List<FieldDefinition>();
			
			for(var i = 0; i < n; i ++)
			{
				var field = new FieldDefinition(NewRandomString(),
					FieldAttributes.Private, NewTypeReference());

				result.Add(field);
				SampleDefinition.Fields.Add(field);
			}

			return result;
		}

		private FindTypeReferencesInFields _findTypeReferencesInFields;

		[SetUp]
		public void SetUp()
		{
			_findTypeReferencesInFields = new FindTypeReferencesInFields();
		}

		[Test]
		public void Can_create_instances()
		{
			Assert.That(_findTypeReferencesInFields, Is.Not.Null);
		}

		[Test]
		public void OfType_on_null_arg_throws_ArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				_findTypeReferencesInFields.OfType(null));
		}

		[Test]
		public void OfType_returns_a_TypeReference_iterator()
		{
			var result = _findTypeReferencesInFields.OfType(SampleDefinition);

			Assert.That(result, Is.InstanceOf<IEnumerable<TypeReference>>());
		}

		[Test]
		public void OfType_returns_the_type_of_every_field()
		{
			var result = _findTypeReferencesInFields.OfType(SampleDefinition);

			Assert.That(result, Is.EquivalentTo(SampleDefinitionFieldTypes));
		}
	}
}