using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace SharpDepend
{
	public interface IFindTypeReferencesInCode
	{
		IEnumerable<TypeReference> OfInstruction(Instruction instruction);
	}
}