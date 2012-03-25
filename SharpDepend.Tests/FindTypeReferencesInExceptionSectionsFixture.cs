using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace SharpDepend.Tests
{
	[TestFixture]
	public class FindTypeReferencesInExceptionSectionsFixture : FindTypeReferencesFixtureBase
	{
		private static readonly MethodDefinition SampleMethod = NewMethodDefinition();

		private FindTypeReferencesInExceptionSections _findTypeReferences;
		private static readonly IList<TypeReference> CatchBlockTypeReferences = NewTypeReferencesListOfSize(3);

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			CatchBlockTypeReferences.ForEach(AddCatchExceptionHandler);
		}

		private static void AddCatchExceptionHandler(TypeReference x)
		{
			SampleMethod.Body.ExceptionHandlers.Add(NewCatchExceptionHandler(x));
		}

		private static ExceptionHandler NewCatchExceptionHandler(TypeReference x)
		{
			return new ExceptionHandler(ExceptionHandlerType.Catch)
	       	{
	       		CatchType = x
	       	};
		}

		[SetUp]
		public void SetUp()
		{
			_findTypeReferences = new FindTypeReferencesInExceptionSections();
		}

		[Test]
		public void Can_create_instances()
		{
			Assert.That(_findTypeReferences, Is.Not.Null);
		}

		[Test]
		public void OfMethod_throws_NullAgumentException_on_null_method()
		{
			Assert.Throws<ArgumentNullException>(() => 
				_findTypeReferences.OfMethod(null));
		}

		[Test]
		public void OfMethod_returns_a_TypeReference_iterator()
		{
			var result = _findTypeReferences.OfMethod(SampleMethod);

			Assert.That(result, Is.InstanceOf<IEnumerable<TypeReference>>());
		}

		[Test]
		public void OfMethod_returns_exception_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethod);
			
			Assert.That(CatchBlockTypeReferences, Is.SubsetOf(result));
		}
	}
}