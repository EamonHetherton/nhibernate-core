﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1547
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture, Explicit("Contains only performances benchmark")]
	public class FixtureAsync : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			var driverClass = ReflectHelper.ClassForName(configuration.GetProperty(Cfg.Environment.ConnectionDriver));
			DriverForSubstitutedCommand.DriverClass = driverClass;

			configuration.SetProperty(
				Cfg.Environment.ConnectionDriver,
				typeof(DriverForSubstitutedCommand).AssemblyQualifiedName);
		}

		protected override void DropSchema()
		{
			(Sfi.ConnectionProvider.Driver as DriverForSubstitutedCommand)?.CleanUp();
			base.DropSchema();
		}

		[Test]
		public async Task SimpleLinqPerfAsync()
		{
			await (BenchmarkAsync(
				"Simple LINQ",
				s =>
					s
						.Query<Entity>()
						.Where(e => e.Name == "Bob")));
		}

		[Test]
		public async Task LinqWithNonParameterizedConstantPerfAsync()
		{
			await (BenchmarkAsync(
				"Non parameterized constant",
				s =>
					s
						.Query<Entity>()
						.Where(e => e.Name == "Bob")
						.Select(e => new { e, c = 2 })));
		}

		[Test]
		public async Task LinqWithListParameterPerfAsync()
		{
			var names = new[] { "Bob", "Sally" };
			return BenchmarkAsync(
				"List parameter",
				s =>
					s
						.Query<Entity>()
						.Where(e => names.Contains(e.Name)));
		}

		private async Task BenchmarkAsync<T>(string test, Func<ISession, IQueryable<T>> queryFactory, CancellationToken cancellationToken = default(CancellationToken))
		{
			var driver = (DriverForSubstitutedCommand) Sfi.ConnectionProvider.Driver;
			var timings = new List<double>();
			var sw = new Stopwatch();

			var cache = (SoftLimitMRUCache)
				typeof(QueryPlanCache)
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				using (driver.SubstituteCommand())
				{
					var query = queryFactory(session);
					// Warm up.
					await (RunBenchmarkUnitAsync(cache, query));

					for (var j = 0; j < 1000; j++)
					{
						sw.Restart();
						await (RunBenchmarkUnitAsync(cache, query));
						sw.Stop();
						timings.Add(sw.Elapsed.TotalMilliseconds);
					}
				}

				await (tx.CommitAsync(cancellationToken));
			}

			var avg = timings.Average();
			Console.WriteLine(
				$"{test} average time: {avg}ms (s {Math.Sqrt(timings.Sum(t => Math.Pow(t - avg, 2)) / (timings.Count - 1))}ms)");
		}

		private static Task RunBenchmarkUnitAsync<T>(SoftLimitMRUCache cache, IQueryable<T> query)
		{
			try
			{
				// Do enough iterations for having a significant elapsed time in milliseconds.
				for (var i = 0; i < 20; i++)
				{
					// Always clear the query plan cache before running the query, otherwise the impact of 1547
					// change would be hidden by it. Simulates having many different queries run.
					cache.Clear();
					Assert.That(query.ToList, Throws.Nothing);
				}
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}

	public partial class DriverForSubstitutedCommand : IDriver
	{

		#region Firebird mess

		#endregion
		#region Pure forwarding

		#endregion

		private partial class SubstituteDbCommand : DbCommand
		{

			protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
			{
				return Task.FromResult<DbDataReader>(_substituteReader);
			}

			public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
			{
				return Task.FromResult<int>(0);
			}

			public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
			{
				return Task.FromResult<object>(null);
			}

			#region Pure forwarding

			#endregion
		}
	}
}
