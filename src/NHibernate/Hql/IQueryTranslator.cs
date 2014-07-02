using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Event;
using NHibernate.Type;

namespace NHibernate.Hql
{
	/// <summary>
	/// Defines the constract of an HQL->SQL translator.
	/// </summary>
	public interface IQueryTranslator
	{
		// Not ported: 
		// Error message constants moved to the implementation of Classic.QueryTranslator (C# can't have fields in interface)
		//public const string ErrorCannotFetchWithIterate = "fetch may not be used with scroll() or iterate()";
		//public const string ErrorNamedParameterDoesNotAppear = "Named parameter does not appear in Query: ";
		//public const string ErrorCannotDetermineType = "Could not determine type of: ";
		//public const string ErrorCannotFormatLiteral = "Could not format constant value to SQL literal: ";

		/// <summary>
		/// Compile a "normal" query. This method may be called multiple times. Subsequent invocations are no-ops.
		/// </summary>
		/// <param name="replacements">Defined query substitutions.</param>
		/// <param name="shallow">Does this represent a shallow (scalar or entity-id) select?</param>
		/// <exception cref="NHibernate.QueryException">There was a problem parsing the query string.</exception>
		/// <exception cref="NHibernate.MappingException">There was a problem querying defined mappings.</exception>
		void Compile(IDictionary<string, string> replacements, bool shallow);

		/// <summary>
		/// Perform a list operation given the underlying query definition.
		/// </summary>
		/// <param name="session">The session owning this query.</param>
		/// <param name="queryParameters">The query bind parameters.</param>
		/// <returns>The query list results.</returns>
		/// <exception cref="NHibernate.HibernateException"></exception>
		IList List(ISessionImplementor session, QueryParameters queryParameters);

		IEnumerable GetEnumerable(QueryParameters queryParameters, IEventSource session);

		// Not ported:
		//IScrollableResults scroll(QueryParameters queryParameters, ISessionImplementor session);

		/// <summary>
		/// Perform a bulk update/delete operation given the underlying query defintion.
		/// </summary>
		/// <param name="queryParameters">The query bind parameters.</param>
		/// <param name="session">The session owning this query.</param>
		/// <returns>The number of entities updated or deleted.</returns>
		/// <exception cref="NHibernate.HibernateException"></exception>
		int ExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session);

		/// <summary>
		/// The set of query spaces (table names) that the query referrs to.
		/// </summary>
		ISet<string> QuerySpaces { get; }

		// <summary>
		// The query identifier for this translator.  The query identifier is used in stats collection.
		// </summary>
		// Not ported:
		//string QueryIdentifier { get;}

		/// <summary>
		/// The SQL string generated by the translator.
		/// </summary>
		string SQLString { get; }

		IList<string> CollectSqlStrings { get; }

		/// <summary>
		/// The HQL string processed by the translator.
		/// </summary>
		string QueryString { get; }

		/// <summary>
		/// Returns the filters enabled for this query translator.
		/// </summary>
		/// <returns>Filters enabled for this query execution.</returns>
		IDictionary<string, IFilter> EnabledFilters { get; }

		/// <summary>
		/// Returns an array of Types represented in the query result.
		/// </summary>
		/// <returns>Query return types.</returns>
		IType[] ReturnTypes { get; }

		/// <summary>
		/// Returns an array of HQL aliases
		/// </summary>
		/// <returns>Returns an array of HQL aliases</returns>
		string[] ReturnAliases { get; }

		/// <summary>
		/// Returns the column names in the generated SQL.
		/// </summary>
		/// <returns>the column names in the generated SQL.</returns>
		string[][] GetColumnNames();

		// <summary>
		// Validate the scrollability of the translated query.
		// </summary>
		// <exception cref="NHibernate.HibernateException"></exception>
		// Not ported:
		//void validateScrollability();

		/// <summary>
		/// Does the translated query contain collection fetches?
		/// </summary>
		/// <returns>True if the query does contain collection fetched; false otherwise.</returns>
		bool ContainsCollectionFetches { get; }

		bool IsManipulationStatement { get; }

		Loader.Loader Loader { get; }

		IType[] ActualReturnTypes { get; }

        ParameterMetadata BuildParameterMetadata();
	}
}