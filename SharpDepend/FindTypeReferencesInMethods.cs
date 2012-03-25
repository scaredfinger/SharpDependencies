using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace SharpDepend
{
	public class FindTypeReferencesInMethods : IFindTypeReferencesInMethods
	{
		private readonly IFindTypeReferencesInCustomAttributes _findInCustomAttributes;
		private readonly IFindTypeReferencesInMethodBody _findInMethodBody;

		public FindTypeReferencesInMethods(IFindTypeReferencesInCustomAttributes findInCustomAttributes, 
			IFindTypeReferencesInMethodBody findInMethodBody)
		{
			_findInCustomAttributes = findInCustomAttributes;
			_findInMethodBody = findInMethodBody;
		}

		public IEnumerable<TypeReference> OfType(TypeDefinition definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");

			return definition.Methods.SelectMany(m => OfMethod(m));
		}

		public IEnumerable<TypeReference> OfMethod(MethodDefinition method)
		{
			return MethodReturnType(method)
				.Concat(ParameterTypes(method))
				.Concat(MethodCustomAttributeTypes(method))
				.Concat(ReturnTypeCustomAttributesTypes(method))
				.Concat(ParameterCustomAttributeTypes(method))
				.Concat(TypesUsedInMethodBody(method));
		}

		private static IEnumerable<TypeReference> MethodReturnType(IMethodSignature method)
		{
			yield return method.ReturnType;
		}

		private static IEnumerable<TypeReference> ParameterTypes(IMethodSignature method)
		{
			return method.Parameters.Select(x => x.ParameterType);
		}

		private IEnumerable<TypeReference> MethodCustomAttributeTypes(ICustomAttributeProvider method)
		{
			return CustomAttributesOf(method);
		}

		private IEnumerable<TypeReference> CustomAttributesOf(ICustomAttributeProvider provider)
		{
			return _findInCustomAttributes.OfProvider(provider);
		}

		private IEnumerable<TypeReference> ReturnTypeCustomAttributesTypes(IMethodSignature method)
		{
			return CustomAttributesOf(method.MethodReturnType);
		}

		private IEnumerable<TypeReference> ParameterCustomAttributeTypes(IMethodSignature method)
		{
			return method.Parameters.SelectMany(x => CustomAttributesOf(x));
		}

		private IEnumerable<TypeReference> TypesUsedInMethodBody(MethodDefinition method)
		{
			return _findInMethodBody.OfMethod(method);
		}
	}
}