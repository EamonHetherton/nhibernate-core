namespace NHibernate.Test.NHSpecificTest.NH1262.fk.reversed.bidirectional
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
		public virtual Employee EmployeeDetails { get; set; }

		public EmployeeInfo()
		{

		}

		public EmployeeInfo(Employee employee)
		{
			this.EmployeeDetails = employee;
		}
	}	
}
