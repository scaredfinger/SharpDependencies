using System.Data;

namespace SampleAssembly
{
	public class SampleClass
	{
		public void Foo(DataSet data)
		{
			try
			{
				object s = data.Tables.Add("T");
				var t = (DataTable) s;
				var n = new DataTable();
			}
			catch (ReadOnlyException)
			{
				throw;
			}
		}

		public void Goo()
		{
			var t = new DataTable();
		}

		private class PrivateInnerClass
		{
		}
	}

	internal class InternalClass
	{

	}
}
