﻿
namespace NHibernate.Test.NHSpecificTest.NH1262.fk.bidirectional
{
	public class Employee
	{
		public virtual long id { get; set; }
		public virtual EmployeeInfo info { get; set; }

		public Employee()
		{

		}
	}

	public class EmployeeInfo
	{
		public virtual long id { get; set; }
		public virtual Employee employee { get; set; }

		public EmployeeInfo()
		{

		}

		public EmployeeInfo(Employee emp)
		{
			employee = emp;
		}
	}
}
