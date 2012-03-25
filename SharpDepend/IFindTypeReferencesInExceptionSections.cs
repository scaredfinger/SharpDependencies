using System.Collections.Generic;
using Mono.Cecil;

namespace SharpDepend
{
	public interface IFindTypeReferencesInExceptionSections
	{
		IEnumerable<TypeReference> OfMethod(MethodDefinition method);
	}
}