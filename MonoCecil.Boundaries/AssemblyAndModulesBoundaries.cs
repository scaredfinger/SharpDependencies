using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;
using SampleAssembly;

namespace MonoCecil.Boundaries
{
	[TestFixture]
	public class AssemblyAndModulesBoundaries
	{
		private const string ExistingAssemblyFile = "SampleAssembly.dll";
		private const string NonExistingAssemblyFile = "NonExistingFile.dll";

		private static readonly Assembly ExistingAssembly = Assembly.LoadFrom(ExistingAssemblyFile);
		private static readonly IEnumerable<Module> ExistingAssemblyModules = ExistingAssembly.GetModules();
		private AssemblyDefinition _assemblyDefinition;
		private static readonly Type TypeOfSampleClass = typeof (SampleClass);

		[SetUp]
		public void SetUp()
		{
			_assemblyDefinition = AssemblyDefinition.ReadAssembly(ExistingAssemblyFile);
		}

		[Test]
		public void ReadAssembly_throws_exception_for_non_existing_file()
		{
			Assert.Throws<FileNotFoundException>(() =>
				AssemblyDefinition.ReadAssembly(NonExistingAssemblyFile));
		}

		[Test]
		public void ReadAssembly_does_not_throw_for_existing_file()
		{
			Assert.DoesNotThrow(() => 
				AssemblyDefinition.ReadAssembly(ExistingAssemblyFile));
		}
	
		[Test]
		public void Assembly_allow_access_to_modules()
		{
			var expectedModuleNames = ExistingAssemblyModules.Select(m => m.Name);
			
			var actualModuleNames = _assemblyDefinition.Modules.Select(x => x.Name);

			Assert.That(actualModuleNames, Is.EqualTo(expectedModuleNames));
		}

		[Test]
		public void Module_has_references_to_external_assemblies()
		{
			var expectedReferencedAssemblyNames = ExistingAssembly.GetReferencedAssemblies()
				.Select(m => m.Name);

			var actualReferencedAssemblyNames = AllModuleDefinitionsInSampleAssembly
				.SelectMany(x => x.AssemblyReferences).Distinct()
				.Select(x => x.Name);

			Assert.That(actualReferencedAssemblyNames, Is.EqualTo(expectedReferencedAssemblyNames));
		}

		private IEnumerable<ModuleDefinition> AllModuleDefinitionsInSampleAssembly
		{
			get { return _assemblyDefinition.Modules; }
		}

		[Test]
		[TestCase(typeof(DataSet))]
		[TestCase(typeof(ReadOnlyException))]
		[TestCase(typeof(DataTable))]
		public void Module_has_references_to_external_types(Type reference)
		{
			var actualReferencedTypes = AllTypeReferencesInSampleAssembly;

			Assert.That(actualReferencedTypes, Has.Some.Matches<TypeReference>(x =>
				x.FullName == reference.FullName));
		}

		private IEnumerable<TypeReference> AllTypeReferencesInSampleAssembly
		{
			get
			{
				return AllModuleDefinitionsInSampleAssembly
					.SelectMany(x => x.GetTypeReferences());
			}
		}

		[Test]
		public void Module_allow_access_to_private_types()
		{
			var types = AllTypeDefinitionsInSampleAssembly;

			Assert.That(types, Has.Some.Matches<TypeDefinition>(x =>
				x.FullName == "SampleAssembly.InternalClass"));
		}

		private IEnumerable<TypeDefinition> AllTypeDefinitionsInSampleAssembly
		{
			get
			{
				return AllModuleDefinitionsInSampleAssembly
					.SelectMany(x => x.Types);
			}
		}

		[Test]
		public void TypeDefinition_allow_access_to_private_inner_types()
		{
			var types = SampleClassDefinition.NestedTypes;

			Assert.That(types, Has.Some.Matches<TypeDefinition>(x =>
				x.Name == "PrivateInnerClass"));
		}

		private TypeDefinition SampleClassDefinition
		{
			get
			{
				return AllTypeDefinitionsInSampleAssembly
					.First(x => x.FullName == TypeOfSampleClass.FullName);
			}
		}

		[Test]
		public void MethodDefinion_allow_access_to_MSIL()
		{
			var fooMethod = FooMethodOfSampleClassDefinition;

			Assert.That(fooMethod.Body.Instructions, Is.Not.Empty);
		}

		private MethodDefinition FooMethodOfSampleClassDefinition
		{
			get
			{
				return SampleClassDefinition.Methods
					.First(x => x.Name == "Foo");
			}
		}

		[Test]
		public void MethodDefinion_allow_access_to_exceptionHandling_clauses()
		{
			var fooMethod = FooMethodOfSampleClassDefinition;

			var clauses = fooMethod.Body.ExceptionHandlers;
			Assert.That(clauses, Is.Not.Empty);
		}

		[Test]
		public void MethodDefinition_allow_access_to_catch_exceptionTypes()
		{
			var fooMethod = FooMethodOfSampleClassDefinition;

			var clause = fooMethod.Body.ExceptionHandlers.First();

			Assert.That(clause.CatchType.FullName, 
				Is.EqualTo("System.Data.ReadOnlyException"));
		}
	}
}
