using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace SharpDepend.Tests
{
	public class FindTypeReferencesFixtureBase
	{
		protected const TypeAttributes PublicClass = TypeAttributes.Public | TypeAttributes.Class;

		protected static TypeReference NewTypeReference()
		{
			var nameSpace = NewRandomString();
			var typeName = NewRandomString();

			return new TypeReference(nameSpace, typeName, null, null);
		}

		protected static string NewRandomString()
		{
			return Guid.NewGuid().ToString().Replace("-", "_");
		}

		protected static void Eager<TObject>(Func<IEnumerable<TObject>> func)
		{
			func().ToArray();
		}

		protected static ParameterDefinition NewParameterOfType(TypeReference x)
		{
			return new ParameterDefinition(NewRandomString(), ParameterAttributes.None, x);
		}

		protected static TypeDefinition NewTypeDefinition()
		{
			return NewTypeDefinition(PublicClass, NewTypeReference(), 
				new [] {NewTypeReference(), NewTypeReference()});
		}

		protected static TypeDefinition NewTypeDefinition(TypeAttributes attributes, 
			TypeReference baseType, IEnumerable<TypeReference> interfaceImplementations)
		{
			var result = new TypeDefinition(NewRandomString(), 
				NewRandomString(), attributes, baseType);

			foreach(var iff in interfaceImplementations)
				result.Interfaces.Add(iff);

			return result;
		}

		protected static IList<TypeReference> NewTypeReferencesListOfSize(int count)
		{
			var result = new List<TypeReference>();
			
			for (var i = 0; i < count; i ++ )
				result.Add(NewTypeReference());

			return result;
		}

		protected static MethodDefinition NewConstructor()
		{
			var owner = NewTypeDefinition();

			return NewConstructorFor(owner);
		}

		private static MethodDefinition NewConstructorFor(TypeDefinition owner)
		{
			var result = new MethodDefinition(".ctor", MethodAttributes.Public |
				MethodAttributes.HideBySig | MethodAttributes.SpecialName |
			    MethodAttributes.RTSpecialName, new TypeReference("System", "Void", null, null));

			owner.Methods.Add(result);

			return result;
		}

		protected static CustomAttribute NewCustomAttribute()
		{
			return new CustomAttribute(NewConstructor());
		}

		protected static IList<CustomAttribute> NewCustomAttributeListOfSize(int n)
		{
			var result = new List<CustomAttribute>();

			for (var i = 0; i < n; i ++ )
				result.Add(NewCustomAttribute());

			return result;
		}

		protected static MethodDefinition NewMethodDefinition(string name, MethodAttributes attributes, 
			TypeReference returnType, IEnumerable<TypeReference> parameterTypes)
		{
			var result = new MethodDefinition(name, attributes, returnType);

			parameterTypes.ForEach(type => 
			                       result.Parameters.Add(NewParameterOfType(type)));

			return result;
		}

		protected static MethodDefinition NewMethodDefinition(TypeReference returnType, 
			IEnumerable<TypeReference> parameterTypes)
		{
			return NewMethodDefinition(NewRandomString(), MethodAttributes.Public,
				returnType, parameterTypes);
		}

		protected static MethodDefinition NewMethodDefinition()
		{
			return NewMethodDefinition(NewTypeReference(), NewTypeReferencesListOfSize(1));
		}
	}
}