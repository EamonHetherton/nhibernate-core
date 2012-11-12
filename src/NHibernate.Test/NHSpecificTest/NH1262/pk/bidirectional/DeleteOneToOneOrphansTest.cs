using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1262.pk.bidirectional
{
	public class DeleteOneToOneOrphansTest  : BugTestCase 
	{
		public override string BugNumber
		{
			get { return "NH1262.pk.bidirectional"; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = new Employee();
				emp.info = new EmployeeInfo(emp);

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
			long empId;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var empInfoList = s.CreateQuery( "from EmployeeInfo" ).List<EmployeeInfo>();
				Assert.AreEqual( 1, empInfoList.Count);
				
				var empList = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual( 1, empList.Count);
				
				var emp = empList[0];
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
	}
}
