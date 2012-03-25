using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Moq;
using NUnit.Framework;

namespace SharpDepend.Tests
{
	[TestFixture]
	public class FindTypeReferencesInMethodBodyFixture : FindTypeReferencesFixtureBase
	{
		private const int InstructionListSize = 3;

		private static readonly MethodDefinition SampleMethod =
			NewMethodDefinition(NewTypeReference(), NewTypeReferencesListOfSize(3));

		private static readonly IEnumerable<TypeReference> ExceptionTypes = 
			NewTypeReferencesListOfSize(4);

		private static readonly IList<TypeReference> TypeReferencesInCode =
			NewTypeReferencesListOfSize(InstructionListSize * 2);

		private static readonly IEnumerable<TypeReference> LocalVariableTypes = NewTypeReferencesListOfSize(4);

		private static readonly IList<Instruction> MethodCode = NewInstructionListOfSize(InstructionListSize);

		private static IList<Instruction> NewInstructionListOfSize(int n)
		{
			var result = new List<Instruction>();

			for (var i = 0; i < n; i ++ )
				result.Add(Instruction.Create(OpCodes.Nop));

			return result;
		}

		private FindTypeReferencesInMethodBody _findTypeReferences;
		
		private Mock<IFindTypeReferencesInExceptionSections> _findInExceptionSectionsMock;
		private IFindTypeReferencesInExceptionSections _findInExceptionSections;
		
		private Mock<IFindTypeReferencesInCode> _findInCode1Mock;
		private IFindTypeReferencesInCode _findInCode1;
		
		private Mock<IFindTypeReferencesInCode> _findInCode2Mock;
		private IFindTypeReferencesInCode _findInCode2;

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			MethodCode.ForEach(x => SampleMethod.Body.Instructions.Add(x));
			LocalVariableTypes.ForEach(x => SampleMethod.Body.Variables.Add(new VariableDefinition(x)));
		}

		[SetUp]
		public void SetUp()
		{
			SetupFindInExceptionSections();

			var findInCode = SetupFindInCode();

			_findTypeReferences = new FindTypeReferencesInMethodBody(_findInExceptionSections,
				findInCode);
		}

		private IEnumerable<IFindTypeReferencesInCode> SetupFindInCode()
		{
			_findInCode1Mock = new Mock<IFindTypeReferencesInCode>();

			MethodCode.ForEach((inst, i) =>
				_findInCode1Mock.Setup(m => m.OfInstruction(inst))
					.Returns(TypeReferencesInCode.Skip(i).Take(1)));

			_findInCode1 = _findInCode1Mock.Object;

			_findInCode2Mock = new Mock<IFindTypeReferencesInCode>();

			MethodCode.ForEach((inst, i) =>
				_findInCode2Mock.Setup(m => m.OfInstruction(inst))
					.Returns(TypeReferencesInCode.Skip(i + InstructionListSize).Take(1)));

			_findInCode2 = _findInCode2Mock.Object;

			return new[] { _findInCode1, _findInCode2 };
		}

		private void SetupFindInExceptionSections()
		{
			_findInExceptionSectionsMock = new Mock<IFindTypeReferencesInExceptionSections>();
			_findInExceptionSectionsMock.Setup(m => m.OfMethod(SampleMethod))
				.Returns(ExceptionTypes);

			_findInExceptionSections = _findInExceptionSectionsMock.Object;
		}

		[Test]
		public void Can_create_instances()
		{
			Assert.That(_findTypeReferences, Is.Not.Null);
		}

		[Test]
		public void OfMethod_throws_ArgumentNullException_on_null_method()
		{
			Assert.Throws<ArgumentNullException>(() =>
				_findTypeReferences.OfMethod(null));
		}

		[Test]
		public void OfMethod_returns_a_typeReference_iterator()
		{
			var result = _findTypeReferences.OfMethod(SampleMethod);

			Assert.That(result, Is.InstanceOf<IEnumerable<TypeReference>>());
		}

		[Test]
		public void OfMethod_calls_findInExceptionSections_ofMethod_with_sample_method()
		{
			_findTypeReferences.OfMethod(SampleMethod);

			_findInExceptionSectionsMock.Verify(m => m.OfMethod(SampleMethod));
		}

		[Test]
		public void OfMethod_result_contains_exception_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethod);

			Assert.That(ExceptionTypes, Is.SubsetOf(result));
		}

		[Test]
		public void OfMethod_calls_findInCode_of_instruction_for_findInCode_and_all_instructions()
		{
			Eager(() =>
				_findTypeReferences.OfMethod(SampleMethod));

			MethodCode.ForEach(instruction =>
				_findInCode1Mock.Verify(m => m.OfInstruction(instruction)));
			MethodCode.ForEach(instruction =>
				_findInCode2Mock.Verify(m => m.OfInstruction(instruction)));
		}

		[Test]
		public void OfMethod_result_contains_type_references_in_code()
		{
			var result = _findTypeReferences.OfMethod(SampleMethod);

			Assert.That(TypeReferencesInCode, Is.SubsetOf(result));
		}

		[Test]
		public void OfMethod_result_contains_local_variable_types()
		{
			var result = _findTypeReferences.OfMethod(SampleMethod);

			Assert.That(LocalVariableTypes, Is.SubsetOf(result));
		}
	}
}