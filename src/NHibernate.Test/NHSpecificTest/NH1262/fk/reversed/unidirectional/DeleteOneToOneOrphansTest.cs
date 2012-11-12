using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1262.fk.reversed.unidirectional
{
	public class DeleteOneToOneOrphansTest : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1262.fk.reversed.unidirectional"; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = new Employee();
				emp.info = new EmployeeInfo();

				s.Save(emp);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from EmployeeInfo");
				s.Delete("from Employee");
				tx.Commit();
			}
		}

		[Test]
		public void testOrphanedWhileManaged()
		{
			long empId = 0;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				Employee emp = empList[0];
				Assert.NotNull(emp.info);

				empId = emp.id;
				emp.info = null;

				tx.Commit();

			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var emp = s.Get<Employee>(empId);
				Assert.IsNull(emp.info);
				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(0, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				tx.Commit();
			}

		}

		[Test]
		//@FailureExpected( jiraKey = "unknown" )
		public void testOrphanedWhileDetached()
		{
			long empId = 0;
			Employee emp;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				emp = empList[0];
				Assert.NotNull(emp.info);

				empId = emp.id;

				tx.Commit();

			}

			//only fails if the object is detached
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Lock(emp, LockMode.None);
				emp.info = null;
				//save using the new session (this used to work prior to 3.5.x)
				s.SaveOrUpdate(emp);
				tx.Commit();
			}


			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				emp = s.Get<Employee>(emp.id);
				Assert.IsNull(emp.info);

				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(0, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				tx.Commit();
			}
		}
	}
}