using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SharpDepend
{
	public class FindTypeReferencesInMethodBody : IFindTypeReferencesInMethodBody
	{
		private readonly IFindTypeReferencesInExceptionSections _findInExceptionSections;
		private readonly IEnumerable<IFindTypeReferencesInCode> _findInCode;

		public FindTypeReferencesInMethodBody(IFindTypeReferencesInExceptionSections findInExceptionSections, 
			IEnumerable<IFindTypeReferencesInCode> findInCode)
		{
			_findInExceptionSections = findInExceptionSections;
			_findInCode = findInCode;
		}

		public IEnumerable<TypeReference> OfMethod(MethodDefinition method)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			return TypeReferencesInExceptionSections(method)
				.Concat(LocalVariableTypes(method))
				.Concat(TypeReferencesInCode(method));
		}

		private IEnumerable<TypeReference> TypeReferencesInExceptionSections(MethodDefinition method)
		{
			return _findInExceptionSections.OfMethod(method);
		}

		private static IEnumerable<TypeReference> LocalVariableTypes(MethodDefinition method)
		{
			return method.Body.Variables.Select(x => x.VariableType);
		}

		private IEnumerable<TypeReference> TypeReferencesInCode(MethodDefinition method)
		{
			return method.Body.Instructions.SelectMany(x =>
				FindTypeReferencesInInstruction(x));
		}

		private IEnumerable<TypeReference> FindTypeReferencesInInstruction(Instruction instruction)
		{
			return _findInCode.SelectMany(y => y.OfInstruction(instruction));
		}
	}
}