using System;

public sealed class AccountHelper
{
    private static volatile AccountHelper instance;
    private static Object syncRootObject = new Object();

    public string UserName { get; set; }
    public string CompanyCode { get; set; }
    public string CompanyName { get; set; }

    public AccountHelper()
    {
        this.UserName = "Doğan BULUT";
        this.CompanyCode = "0001";
        this.CompanyName = "penera";
    }
    
    public static AccountHelper Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRootObject)
                {
                    if (instance == null)
                    {
                        instance = new AccountHelper();
                    }
                }
            }
            return instance;
        }
    }
}