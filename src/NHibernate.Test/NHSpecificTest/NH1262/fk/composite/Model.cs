using System;

namespace NHibernate.Test.NHSpecificTest.NH1262.fk.composite
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
		public class Id
		{
			public virtual long companyId { get; set; }
			public virtual long personId { get; set; }

			public Id()
			{

			}

			public Id(long companyId, long personId)
			{
				this.companyId = companyId;
				this.personId = personId;
			}


			public override bool Equals(Object o)
			{
				if (this == o)
				{
					return true;
				}

				var t = this.GetType();
				var u = o.GetType();


				if (o == null || !t.IsAssignableFrom(u) || !u.IsAssignableFrom(t))
				{
					return false;
				}

				var id = o as Id;

				return companyId.Equals(id.companyId)
						&& personId.Equals(id.personId);

			}

			public override int GetHashCode()
			{
				return (31 * companyId.GetHashCode()) + personId.GetHashCode();
			}
		}

		public virtual Id id { get; set; }
		public virtual Employee employee { get; set; }

		public EmployeeInfo()
		{

		}

		public EmployeeInfo(long companyId, long personId)
		{
			this.id = new Id(companyId, personId);
		}

		public EmployeeInfo(Id id)
		{
			this.id = id;
		}
	}

	
}
