using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace SharpDepend
{
	public class FindTypeReferencesInTypes : IFindTypeReferences
	{
		private readonly IEnumerable<IFindTypeReferencesInMember> _findInMembers;
		private readonly IFindTypeReferencesInCustomAttributes _findInCustomAttributes;

		public FindTypeReferencesInTypes(IEnumerable<IFindTypeReferencesInMember> findInMembers, 
			IFindTypeReferencesInCustomAttributes findInCustomAttributes)
		{
			if (findInMembers == null)
				throw new ArgumentNullException("findInMembers");
			if (findInCustomAttributes == null)
				throw new ArgumentNullException("findInCustomAttributes");

			_findInMembers = findInMembers;
			_findInCustomAttributes = findInCustomAttributes;
		}

		public IEnumerable<TypeReference> OfType(TypeDefinition definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");

			return BaseType(definition)
				.Concat(InterfaceImplementations(definition))
				.Concat(FindTypeReferencesInMembers(definition))
				.Concat(FindTypeReferencesInCustomAttributes(definition));
		}

		private static IEnumerable<TypeReference> BaseType(TypeDefinition definition)
		{
			yield return definition.BaseType;
		}

		private static IEnumerable<TypeReference> InterfaceImplementations(TypeDefinition definition)
		{
			return definition.Interfaces;
		}

		private IEnumerable<TypeReference> FindTypeReferencesInMembers(TypeDefinition definition)
		{
			return _findInMembers
				.SelectMany(m => m.OfType(definition));
		}

		private IEnumerable<TypeReference> FindTypeReferencesInCustomAttributes(ICustomAttributeProvider definition)
		{
			return _findInCustomAttributes.OfProvider(definition);
		}
	}
}
