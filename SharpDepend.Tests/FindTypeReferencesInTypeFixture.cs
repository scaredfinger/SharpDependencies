using System;
using System.Collections.Generic;
using Mono.Cecil;
using Moq;
using NUnit.Framework;

namespace SharpDepend.Tests
{
	[TestFixture]
	public class FindTypeReferencesInTypeFixture : FindTypeReferencesFixtureBase
	{
		private static readonly IEnumerable<TypeReference> SampleDefinitionInterfaceImplementations =
			new[] { NewTypeReference(), NewTypeReference() };
		private static readonly TypeReference SampleDefinitionBase = NewTypeReference();
		private static readonly TypeDefinition SampleDefinition = NewTypeDefinition(PublicClass,
			SampleDefinitionBase, SampleDefinitionInterfaceImplementations);

		private static readonly IList<TypeReference> TypeReferencesInFields = 
			NewTypeReferencesListOfSize(2);

		private static readonly IList<TypeReference> TypeReferencesInMethods = 
			NewTypeReferencesListOfSize(4);

		private FindTypeReferencesInTypes _findTypeReferencesInTypes;

		private Mock<IFindTypeReferencesInMethods> _findInMethodsMock;
		private IFindTypeReferencesInMethods _findTypeReferencesInMethods;

		private Mock<IFindTypeReferencesInFields> _findInFieldsMock;
		private IFindTypeReferencesInFields _findTypeReferencesInFields;
		
		private Mock<IFindTypeReferencesInCustomAttributes> _findInCustomAttributesMock;
		private IFindTypeReferencesInCustomAttributes _findInCustomAttributes;
		private static readonly IList<TypeReference> CustomAttributeTypes = NewTypeReferencesListOfSize(4);

		[SetUp]
		public void SetUp()
		{
			SetupFindInMethods();
			SetupFindInFields();
			SetupFindInCustomAttributes();

			var findInMembers = new List<IFindTypeReferencesInMember>
    		{
    			_findTypeReferencesInFields, _findTypeReferencesInMethods
    		};

			_findTypeReferencesInTypes = new FindTypeReferencesInTypes(findInMembers, _findInCustomAttributes);
		}

		private void SetupFindInMethods()
		{
			_findInMethodsMock = new Mock<IFindTypeReferencesInMethods>();
			_findInMethodsMock.Setup(m => m.OfType(SampleDefinition))
				.Returns(TypeReferencesInMethods);

			_findTypeReferencesInMethods = _findInMethodsMock.Object;
		}

		private void SetupFindInFields()
		{
			_findInFieldsMock = new Mock<IFindTypeReferencesInFields>();
			_findInFieldsMock.Setup(m => m.OfType(SampleDefinition))
				.Returns(TypeReferencesInFields);

			_findTypeReferencesInFields = _findInFieldsMock.Object;
		}

		private void SetupFindInCustomAttributes()
		{
			_findInCustomAttributesMock = new Mock<IFindTypeReferencesInCustomAttributes>();
			_findInCustomAttributesMock.Setup(m => m.OfProvider(SampleDefinition))
				.Returns(CustomAttributeTypes);

			_findInCustomAttributes = _findInCustomAttributesMock.Object;
		}

		[Test]
		public void Can_create_instances()
		{
			Assert.That(_findTypeReferencesInTypes, Is.Not.Null);
		}

		[Test]
		public void Constructor_on_null_arg_throws_ArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new FindTypeReferencesInTypes(null, _findInCustomAttributes));
		}

		[Test]
		public void Constructor_on_null_findInCustomAttributes_throws_ArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new FindTypeReferencesInTypes(new IFindTypeReferencesInMember[0], null));
		}

		[Test]
		public void OfType_on_null_arg_throws_ArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				_findTypeReferencesInTypes.OfType(null));
		}

		[Test]
		public void OfType_returns_a_TypeReference_iterator()
		{
			var references = _findTypeReferencesInTypes.OfType(SampleDefinition);

			Assert.That(references, Is.InstanceOf<IEnumerable<TypeReference>>());
		}

		[Test]
		public void OfType_calls_findInFields_OfType()
		{
			Eager(() => 
				_findTypeReferencesInTypes.OfType(SampleDefinition));

			_findInFieldsMock.Verify(m => m.OfType(SampleDefinition));
		}

		[Test]
		public void OfType_result_contains_findInFiends_result()
		{
			var references = _findTypeReferencesInTypes.OfType(SampleDefinition);

			Assert.That(TypeReferencesInFields, Is.SubsetOf(references));
		}
		
		[Test]
		public void OfType_calls_findInMethods_OfType()
		{
			Eager(() =>
				_findTypeReferencesInTypes.OfType(SampleDefinition));

			_findInMethodsMock.Verify(m => m.OfType(SampleDefinition));
		}

		[Test]
		public void OfType_result_contains_findInMethods_result()
		{
			var references = _findTypeReferencesInTypes.OfType(SampleDefinition);

			Assert.That(TypeReferencesInMethods, Is.SubsetOf(references));
		}

		[Test]
		public void OfType_result_contains_base_class()
		{
			var references = _findTypeReferencesInTypes.OfType(SampleDefinition);

			Assert.That(references, Has.Some.EqualTo(SampleDefinitionBase));
		}

		[Test]
		public void OfType_result_contains_interface_implementations()
		{
			var references = _findTypeReferencesInTypes.OfType(SampleDefinition);

			Assert.That(SampleDefinitionInterfaceImplementations, Is.SubsetOf(references));
		}

		[Test]
		public void OfType_calls_findInCustomAttributes_OfProvider_with_type()
		{
			_findTypeReferencesInTypes.OfType(SampleDefinition);

			_findInCustomAttributesMock.Verify(m => m.OfProvider(SampleDefinition));
		}

		[Test]
		public void OfType_result_contains_custom_attribute_types()
		{
			var result = _findTypeReferencesInTypes.OfType(SampleDefinition);

			Assert.That(CustomAttributeTypes, Is.SubsetOf(result));
		}
	}
}
