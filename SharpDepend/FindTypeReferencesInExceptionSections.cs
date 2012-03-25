using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SharpDepend
{
	public class FindTypeReferencesInExceptionSections : IFindTypeReferencesInExceptionSections
	{
		public IEnumerable<TypeReference> OfMethod(MethodDefinition method)
		{
			if (method == null)
				throw new ArgumentNullException();

			return FromExceptionHandlersIn(method)
				.Where(HandlerIsCatchBlock)
				.Select(x => x.CatchType);
		}

		private static IEnumerable<ExceptionHandler> FromExceptionHandlersIn(MethodDefinition method)
		{
			return method.Body.ExceptionHandlers;
		}

		private static bool HandlerIsCatchBlock(ExceptionHandler x)
		{
			return x.HandlerType == ExceptionHandlerType.Catch;
		}
	}
}