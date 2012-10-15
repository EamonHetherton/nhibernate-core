﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	//TODO: Remove this visitor when https://www.re-motion.org/jira/browse/RM-5107 will be fixed
	public class NameUnNamedParameters : ExpressionTreeVisitor
	{
		public static Expression Visit(Expression expression)
		{
			var visitor = new NameUnNamedParameters();

			return visitor.VisitExpression(expression);
		}

		private readonly Dictionary<ParameterExpression, ParameterExpression> _renamedParameters = new Dictionary<ParameterExpression, ParameterExpression>();

		protected override Expression VisitParameterExpression(ParameterExpression expression)
		{
			if (string.IsNullOrEmpty(expression.Name))
			{
				ParameterExpression renamed;
				
				if (_renamedParameters.TryGetValue(expression, out renamed))
				{
					return renamed;
				}

				renamed = Expression.Parameter(expression.Type, Guid.NewGuid().ToString());

				_renamedParameters.Add(expression, renamed);

				return renamed;
			}

			return base.VisitParameterExpression(expression);
		}
	}
}