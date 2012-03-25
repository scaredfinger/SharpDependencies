using System;
using System.Collections.Generic;
using Mono.Cecil;
using Moq;
using NUnit.Framework;

namespace SharpDepend.Tests
{
	[TestFixture]
	public class FindTypeReferencesInMethodsFixture : FindTypeReferencesFixtureBase
	{
		private const int SampleMethodCustomAttributesCount = 3;
		private const int SampleMethodParametersCount = 4;
		private const int SampleMethodReturnTypeCustomAttributesCount = 3;

		private static readonly IEnumerable<TypeReference> SampleMethodCustomAttributeTypes =
			NewTypeReferencesListOfSize(SampleMethodCustomAttributesCount);

		private static readonly TypeReference SampleMethodReturnType = NewTypeReference();

		private static readonly IEnumerable<TypeReference> SampleMethodParameterTypes = 
			NewTypeReferencesListOfSize(SampleMethodParametersCount) ;

		private static readonly MethodDefinition SampleMethodDefinition = 
			NewSampleMethodDefinitionWithParametersOfType(SampleMethodParameterTypes);

		private static MethodDefinition NewSampleMethodDefinitionWithParametersOfType(
			IEnumerable<TypeReference> parameterTypes)
		{
			var result = new MethodDefinition(NewRandomString(), 
				MethodAttributes.Public, SampleMethodReturnType);

			parameterTypes.ForEach(type => 
				result.Parameters.Add(NewParameterOfType(type)));

			return result;
		}

		private static readonly TypeDefinition SampleDefinition = 
			NewTypeDefinitionWith(SampleMethodDefinition);

		private static TypeDefinition NewTypeDefinitionWith(MethodDefinition method)
		{
			var result = NewTypeDefinition();

			result.Methods.Add(method);

			return result;
		}

		private static readonly IEnumerable<TypeReference> MethodReturnCustomAttributeTypes = 
			NewTypeReferencesListOfSize(SampleMethodReturnTypeCustomAttributesCount);

		private static readonly IList<TypeReference> ParameterCustomAttributeTypes =
			NewTypeReferencesListOfSize(SampleMethodParametersCount * 2);

		private FindTypeReferencesInMethods _findTypeReferences;

		private Mock<IFindTypeReferencesInCustomAttributes> _findInCustomAttributesMock;
		private IFindTypeReferencesInCustomAttributes _findInCustomAttributes;
		
		private Mock<IFindTypeReferencesInMethodBody> _findInMethodBodiesMock;
		private IFindTypeReferencesInMethodBody _findInMethodBody;
		private static readonly IList<TypeReference> TypeReferenesInMethodBody = NewTypeReferencesListOfSize(4);

		[SetUp]
		public void SetUp()
		{
			SetupFindInCustomAttributes();

			_findInMethodBodiesMock = new Mock<IFindTypeReferencesInMethodBody>();
			_findInMethodBodiesMock.Setup(m => m.OfMethod(SampleMethodDefinition))
				.Returns(TypeReferenesInMethodBody);

			_findInMethodBody = _findInMethodBodiesMock.Object;

			_findTypeReferences = new FindTypeReferencesInMethods(_findInCustomAttributes, 
				_findInMethodBody);
		}

		private void SetupFindInCustomAttributes()
		{
			_findInCustomAttributesMock = new Mock<IFindTypeReferencesInCustomAttributes>();

			_findInCustomAttributesMock.Setup(m => m.OfProvider(SampleMethodDefinition))
				.Returns(SampleMethodCustomAttributeTypes);

			_findInCustomAttributesMock.Setup(m => m.OfProvider(SampleMethodDefinition.MethodReturnType))
				.Returns(MethodReturnCustomAttributeTypes);

			SetupParameterCustomAttributeTypes();

			_findInCustomAttributes = _findInCustomAttributesMock.Object;
		}

		private void SetupParameterCustomAttributeTypes()
		{
			SampleMethodDefinition.Parameters
				.ForEach((x, i) =>
					_findInCustomAttributesMock.Setup(m => m.OfProvider(x))
						.Returns(new[]
						{
							ParameterCustomAttributeTypes[2*i],
				         	ParameterCustomAttributeTypes[2*i + 1]
				        }));
		}

		[Test]
		public void Can_create_instances()
		{
			Assert.That(_findTypeReferences, Is.Not.Null);
		}

		[Test]
		public void OfType_on_null_argument_throws_argument_null_exception()
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

		[Test]
		public void OfMethod_result_contains_method_return_type()
		{
			var result = _findTypeReferences.OfMethod(SampleMethodDefinition);

			Assert.That(result, Has.Some.EqualTo(SampleMethodReturnType));
		}

		[Test]
		public void OfType_result_contains_method_return_type()
		{
			var result = _findTypeReferences.OfType(SampleDefinition);

			Assert.That(result, Has.Some.EqualTo(SampleMethodReturnType));
		}

		[Test]
		public void OfMethod_result_contains_parameter_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethodDefinition);

			Assert.That(SampleMethodParameterTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfType_result_contains_parameter_types()
		{
			var result = _findTypeReferences.OfType(SampleDefinition);

			Assert.That(SampleMethodParameterTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfMethod_calls_findInCustomAttributes_OfProvider_method_definition()
		{
			_findTypeReferences.OfMethod(SampleMethodDefinition);

			_findInCustomAttributesMock.Verify(m => m.OfProvider(SampleMethodDefinition));
		}

		[Test]
		public void OfType_calls_findInCustomAttributes_OfProvider_method_definition()
		{
			Eager(() => 
				_findTypeReferences.OfType(SampleDefinition));

			_findInCustomAttributesMock.Verify(m => m.OfProvider(SampleMethodDefinition));
		}

		[Test]
		public void OfMethod_result_contains_method_custom_attribute_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethodDefinition);

			Assert.That(SampleMethodCustomAttributeTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfType_result_contains_method_custom_attribute_types()
		{
			var result = _findTypeReferences.OfType(SampleDefinition);

			Assert.That(SampleMethodCustomAttributeTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfMethod_calls_findInCustomAttributes_OfProvider_method_return_type()
		{
			_findTypeReferences.OfMethod(SampleMethodDefinition);

			_findInCustomAttributesMock.Verify(m => m.OfProvider(
				SampleMethodDefinition.MethodReturnType));
		}

		[Test]
		public void OfType_calls_findInCustomAttributes_OfProvider_method_return_type()
		{
			Eager(() =>
				_findTypeReferences.OfType(SampleDefinition));

			_findInCustomAttributesMock.Verify(m => m.OfProvider(
				SampleMethodDefinition.MethodReturnType));
		}

		[Test]
		public void OfMethod_result_contains_return_type_custom_attribute_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethodDefinition);

			Assert.That(MethodReturnCustomAttributeTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfType_result_contains_return_type_custom_attribute_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethodDefinition);

			Assert.That(MethodReturnCustomAttributeTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfMethod_calls_findInCustomAttributes_OfProvider_every_method_parameter()
		{
			Eager(() =>
			      _findTypeReferences.OfMethod(SampleMethodDefinition));

			SampleMethodDefinition.Parameters.ForEach(parameter => 
				_findInCustomAttributesMock.Verify(m => m.OfProvider(parameter)));
		}

		[Test]
		public void OfType_calls_findInCustomAttributes_OfProvider_every_method_parameter()
		{
			Eager(() =>
				_findTypeReferences.OfType(SampleDefinition));

			SampleMethodDefinition.Parameters.ForEach(parameter =>
				_findInCustomAttributesMock.Verify(m => m.OfProvider(parameter)));
		}

		[Test]
		public void OfMethod_result_contains_parameter_custom_attribute_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethodDefinition);

			Assert.That(ParameterCustomAttributeTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfType_result_contains_parameter_custom_attributes()
		{
			var result = _findTypeReferences.OfType(SampleDefinition);
			
			Assert.That(ParameterCustomAttributeTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfMethod_calls_findInMethodBodies_OfBody_with_method_body()
		{
			_findTypeReferences.OfMethod(SampleMethodDefinition);

			_findInMethodBodiesMock.Verify(m => m.OfMethod(SampleMethodDefinition));
		}

		[Test]
		public void OfType_calls_findInMethodBodies_OfBody_with_method_body()
		{
			Eager(() =>
				_findTypeReferences.OfType(SampleDefinition));

			_findInMethodBodiesMock.Verify(m => m.OfMethod(SampleMethodDefinition));
		}

		[Test]
		public void OfMethod_result_contains_type_references_in_method_body()
		{
			var result = _findTypeReferences.OfMethod(SampleMethodDefinition);

			Assert.That(TypeReferenesInMethodBody, Is.SubsetOf(result));
		}

		[Test]
		public void OfType_result_contains_type_references_in_method_body()
		{
			var result = _findTypeReferences.OfType(SampleDefinition);

			Assert.That(TypeReferenesInMethodBody, Is.SubsetOf(result));
		}
	}
}