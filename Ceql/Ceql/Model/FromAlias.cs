using System.Collections.Generic;
using Ceql.Composition;
using System;
using Ceql.Model;


namespace Ceql.Model
{
    public class FromAlias
    {
        public FromClause FromClause;
        public Type TableType;
        public string Name;
        public int Index;
        
        //sub select intermediate
        public SelectStatementModel SubGeneratedSelect;

    }
}
