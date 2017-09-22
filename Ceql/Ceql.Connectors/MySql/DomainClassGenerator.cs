namespace Ceql.Connectors.MySql
{
    using Ceql.Connectors.MySql.Domain;
    using Ceql.Contracts;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DomainClassGenerator : AbstractComposer, ISchemaDomainGenerator
    {
        public void GenerateDomain(string schemaName, string nameSpace)
        {

            // create directory for schemaname
            Directory.CreateDirectory(schemaName);        
            
            
            // get all schema tables
            var schema = From<Tables>()
                .Where(t => t.TableSchema == schemaName)
                .Select(t => new
                {
                    t.TableSchema,
                    t.TableName
                })
                .ToList();

            var tableNames = schema.Select(t => t.TableName).ToList();
            
            // get all columns
            var tables = from columns in From<Columns>()
                 .Where(c => In(tableNames, c.TableName) && c.TableSchema == schemaName)
                 .Select(c => c)
                 .GroupBy(c => c.TableName)
                         join table in schema on columns.Key equals table.TableName
                         let x = new
                         {
                             table.TableName,
                             table.TableSchema,
                             Columns = columns.ToList()
                         }
                         select x;

            foreach (var table in tables)
            {

                // convert to type name
                var typeName = ToTypeName(table.TableName);
                using (var stream = File.Create(Path.Combine(schemaName,typeName + ".cs")))
                {
                    using (var file = new StreamWriter(stream))
                    {

                        OpenNamespace(file, nameSpace);
                        file.Write('\t');
                        file.WriteLine(@"using System;");
                        file.Write('\t');
                        file.WriteLine(@"using Ceql.Contracts;");
                        file.Write('\t');
                        file.WriteLine(@"using Ceql.Contracts.Attributes;");

                        file.WriteLine("");

                        if (!String.IsNullOrEmpty(table.TableSchema))
                        {
                            file.Write('\t');
                            file.WriteLine("[Schema(\"" + table.TableSchema.ToUpper() + "\")]");
                        }

                        file.Write('\t');
                        file.WriteLine("[Table(\"" + table.TableName.ToUpper() + "\")]");
                        file.Write('\t');
                        file.WriteLine(@"public class " + typeName + " : ITable");
                        file.Write('\t');
                        file.WriteLine("{");

                        foreach (var column in table.Columns)
                        {
                            file.Write('\t');
                            file.Write('\t');
                            file.WriteLine("[Field(\"" + column.ColumnName + "\")]");

                            // write autoincrement attribute
                            if (column.Extra.Contains("auto_increment"))
                            {
                                file.Write('\t');
                                file.Write('\t');
                                file.WriteLine("[AutoSequence]");
                            }

                            // add primary key attribute
                            if (column.ColumnKey.Contains("PRI"))
                            {
                                file.Write('\t');
                                file.Write('\t');
                                file.WriteLine("[PrimaryKey]");
                            }

                            var colType = ToType(column.DataType);
                            var nullable = "";
                            if (column.IsNullable == "YES" && colType != "string")
                            {
                                nullable = "?";
                            }

                            file.Write('\t');
                            file.Write('\t');
                            file.WriteLine("public " + colType + nullable + " " + ToColumnName(typeName, column.ColumnName) + " { get; set; }");
                        }

                        file.Write('\t');
                        file.WriteLine("}");
                        file.WriteLine("}");
                    }
                }
            }
        }


        private void OpenNamespace(StreamWriter writer, string ns)
        {
            writer.WriteLine(@"namespace " + ns);
            writer.WriteLine(@"{");
        }

        private string ToColumnName(string typeName, string original)
        {
            var newName = ToTypeName(original);
            if (newName == typeName)
            {
                newName += "Col";
            }
            return newName;
        }

        private string ToTypeName(string original)
        {
            var parts = original.ToLower().Split(new char[] { '_', '-' });
            var newName = "";

            foreach (var part in parts)
            {
                if (String.IsNullOrEmpty(part))
                {
                    continue;
                }
                newName += part.Substring(0, 1).ToUpper();

                if (part.Length > 1)
                {
                    newName += part.Substring(1);
                }
            }

            return newName;
        }

        private static Dictionary<string, string> TypeMap = new Dictionary<string, string>
        {
            {"timestamp","DateTime" },
            {"datetime","DateTime"},
            {"int", "int" },
            {"varchar","string" },
            {"float","float"},
            {"decimal","decimal"},
            {"text","string"}
        };

        private string ToType(string typeName)
        {
            if (TypeMap.ContainsKey(typeName))
            {
                return TypeMap[typeName];
            }

            throw new Exception("type " + typeName + " could not be mapped");
        }


    }
}
