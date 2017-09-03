using System.Collections.Generic;
namespace Ceql.Composition
{
    /// <summary>
    /// 
    /// </summary>
    public class WhereClause
    {
        public List<ExpressionAggregator> FilterExpression = new List<ExpressionAggregator>();
        public FromClause FromClause;
    }
}
