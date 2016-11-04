using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Kernel
{
    public class Sql
    {
        public static string GET_IDENTITY = "Select so.name Table_name,sc.name Identity_Column_name,ident_current(so.name) curr_value,ident_incr(so.name) incr_value,ident_seed(so.name) seed_value FROM sysobjects so Inner Join syscolumns sc ON so.id = sc.id and columnproperty(sc.id, sc.name, 'IsIdentity') = 1 AND so.xtype='U'";
    }
}
