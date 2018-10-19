﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	using System.Threading.Tasks;
	[TestFixture]
	public class DateFixtureAsync : FixtureBaseAsync
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.Dates.Mappings.Date.hbm.xml"}; }
		}

		[Test]
		public async Task SavingAndRetrievingTestAsync()
		{
			DateTime Now = DateTime.Now;
			return SavingAndRetrievingActionAsync(new AllDates {Sql_date = Now},
			                          entity => DateTimeAssert.AreEqual(entity.Sql_date, Now, true));
		}
	}
}