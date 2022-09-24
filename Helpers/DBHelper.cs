using System;
using System.Collections.Generic;

public sealed class DBHelper
{
    private static volatile DBHelper instance;
    private static Object syncRootObject = new Object();

    public IDictionary<string,string> Tables { get; set; }
    public DBHelper()
    {
        Tables = new Dictionary<string,string>();
        Tables.Add("manufacturer",@"db\"+AccountHelper.Instance.CompanyCode+@"\maindata.db");
        Tables.Add("customer",@"db\"+AccountHelper.Instance.CompanyCode+@"\maindata.db");
        Tables.Add("order",@"db\"+AccountHelper.Instance.CompanyCode+@"\order.db");
    }
    
    public static DBHelper Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRootObject)
                {
                    if (instance == null)
                    {
                        instance = new DBHelper();
                    }
                }
            }
            return instance;
        }
    }
}