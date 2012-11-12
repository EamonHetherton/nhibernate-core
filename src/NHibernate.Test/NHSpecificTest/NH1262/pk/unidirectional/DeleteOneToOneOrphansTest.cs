using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1262.pk.unidirectional
{
	public class DeleteOneToOneOrphansTest : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1262.pk.unidirectional"; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = new Employee();
				s.Save(emp);
				var info = new EmployeeInfo(emp.id);
				emp.info = info;
				
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
			long empId;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var empInfoList = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				Employee emp = empList[0];
				Assert.NotNull(emp.info);
				
				var empAndInfoList = s.CreateQuery( "from Employee e, EmployeeInfo i where e.info = i" ).List();
				Assert.AreEqual(1, empAndInfoList.Count);

				var result = (object[])empAndInfoList[0];

				emp = result[0] as Employee;

				Assert.NotNull(result[1]);
				Assert.AreSame(emp.info, result[1]);

				empId = emp.id;
				emp.info = null;

				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var emp = s.Get<Employee>(empId);
				Assert.IsNull(emp.info);

				var empInfoList = s.CreateQuery( "from EmployeeInfo" ).List<EmployeeInfo>();
				Assert.AreEqual( 0, empInfoList.Count);

				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empList.Count);

				tx.Commit();
			}
		}
	}
}