using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ceql.Composition;

namespace Ceql.Utils
{
    public static class CeqlUtils
    {

        public static List<FromClause> GetStatementList(FromClause from)
        {
            var list = new List<FromClause>();
            while (from != null)
            {
                list.Add(from);
                from = from.Parent;
            }
            /* 
             * because from clause hierarchy is in reverse order of the statement composition 
             * must invert the list to match from clause sequence to expression argument sequence 
             */
            list.Reverse();
            return list;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string source)
        {
            var chars = source.ToCharArray();
            var bytes = new byte[chars.Length*sizeof (char)];
            System.Buffer.BlockCopy(chars, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetCacheKey(string source)
        {
            var sha1 = SHA1.Create();
            //var hash = Convert.ToBase64String(sha1.ComputeHash(StringToBytes(source)));
            var hash = ToBase16String(sha1.ComputeHash(StringToBytes(source)));
            return hash;
        }

        private const string hexMap = "0123456789ABCDEF";
        public static string ToBase16String(byte[] bytes)
        {
            var result = "";
            foreach (var b in bytes)
            {
                
                result += hexMap[b%16].ToString() + hexMap[(b/16)%16].ToString();
            }
            return result;
        }



    }
}
