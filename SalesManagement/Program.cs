using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using SalesManagement.Model;

namespace SalesManagement
{
    class Program
    {
        static DirectoryInfo parentOfDataFolder = Directory.GetParent(Environment.CurrentDirectory).Parent;
        static SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder()
        {
            DataSource = @"(LocalDB)\MSSQLLocalDB",
            AttachDBFilename = $@"{parentOfDataFolder.FullName}\Data\SalesManagement.mdf",
            IntegratedSecurity = true,
            ConnectTimeout = 30,
            ApplicationName = "SalesManagement"

        };
        static void Main(string[] args)
        {

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                //InsertSomeDataIntoCommodity(conn);

                //InsertSomeDataIntoStore(conn);

                var alizmarket = conn.Query<Store>("SELECT * FROM Store AS S WHERE S.Name = 'alizMarket'").First();
                var commodities = conn.Query<Commodity>("SELECT * FROM Commodity AS C WHERE C.PRICE <= 500").ToList();

                //InsertSomeCommodityForStore(conn, alizmarket, commodities);

                var queryResult = conn.Query("SELECT S.Name AS SName, S.Address, C.Name AS CName, C.Price FROM Commodity AS C, Store AS S, StoreData AS SD where C.Id = SD.CommodityId AND S.Id = SD.StoreId ").ToList();
                queryResult.ForEach(q =>
                {
                    Console.WriteLine("{0} has {1}, Price: {2}", q.SName, q.CName, q.Price);
                });
            }

        }

        private static void InsertSomeCommodityForStore(SqlConnection conn, Store alizmarket, List<Commodity> commodities)
        {
            commodities.ForEach(c =>
            {
                conn.Execute("INSERT INTO dbo.[StoreData] (StoreId, CommodityId) VALUES (@StoreId, @CommodityId)",
                    new { StoreId = alizmarket.Id, CommodityId = c.Id });
            });
        }

        private static void InsertSomeDataIntoStore(SqlConnection conn)
        {
            conn.Execute("INSERT INTO dbo.[Store] (Name, Manager, Address) VALUES (@Name, @Manager, @Address)",
                new[]
                {
                        new Store(){ Name="alizMarket", Manager="Alireza", Address="fatemi"},
                        new Store(){ Name="haji", Manager="hajagha", Address="amirabad"},
                        new Store(){ Name="mahale", Manager="mamad", Address="gisha"}
                });
        }

        private static void InsertSomeDataIntoCommodity(SqlConnection conn)
        {
            conn.Execute("INSERT INTO dbo.[Commodity] (Name, Price) VALUES (@Name, @Price)",
                new[]
                {
                        new Commodity(){ Name="chips", Price=499},
                        new Commodity(){ Name="mast", Price=642},
                        new Commodity(){ Name="delester", Price=402},
                        new Commodity(){ Name="coca", Price=702},
                        new Commodity(){ Name="pepsi", Price=902},
                        new Commodity(){ Name="sprite", Price=302}
                });
        }
    }
}
