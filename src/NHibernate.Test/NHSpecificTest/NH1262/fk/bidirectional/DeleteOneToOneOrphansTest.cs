using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1262.fk.bidirectional
{
	[TestFixture]
	public class DeleteOneToOneOrphansTest : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1262.fk.bidirectional"; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = new Employee();
				emp.Info = new EmployeeInfo(emp);

				s.Save(emp);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from EmployeeInfo");
				session.Delete("from Employee");
				tx.Commit();
			}
		}

		[Test]
		public void TestOrphanedWhileManaged()
		{
			long empId = 0;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var empInfoResults = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(1, empInfoResults.Count);

				var empResults = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empResults.Count);

				var emp = empResults[0];
				Assert.NotNull(emp);

				empId = emp.Id;
				emp.Info = null;
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var emp = s.Get<Employee>(empId);
				Assert.Null(emp.Info);

				var empInfoResults = s.CreateQuery("from EmployeeInfo").List<EmployeeInfo>();
				Assert.AreEqual(0, empInfoResults.Count);

				var empResults = s.CreateQuery("from Employee").List<Employee>();
				Assert.AreEqual(1, empResults.Count);

				t.Commit();
			}
		}

	}
}
