using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ceql.Composition
{
    public class ExpressionAggregator
    {
        public EBooleanOperator? Operator;
        public Expression Expression;
        public List<FromClause> ExpressionBoundClauses;
    }
}
