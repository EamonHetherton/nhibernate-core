namespace NHibernate.Test.NHSpecificTest.NH1262.fk.reversed.unidirectional
{
	public class Employee
	{
		public virtual long Id { get; set; }
		public virtual EmployeeInfo Info { get; set; }

		public Employee()
		{

		}
	}

	public class EmployeeInfo
	{
		public virtual long Id { get; set; }

		public EmployeeInfo()
		{

		}

		public EmployeeInfo(long id)
		{
			this.Id = id;
		}
	}	
}
