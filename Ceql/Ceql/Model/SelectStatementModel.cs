namespace Ceql.Model
{
    using System.Collections.Generic;

    public class SelectStatementModel
    {

        public List<SelectAlias> SelectList;
        public string Sql;

        public List<FromAlias> AliasList;

        public bool IsAllSelect;
    }
}
