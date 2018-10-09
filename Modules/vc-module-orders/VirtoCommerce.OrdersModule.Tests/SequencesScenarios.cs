using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace VirtoCommerce.OrdersModule.Tests
{
    [Trait("Category", "CI")]
    public class SequencesScenarios //: FunctionalTestBase
    {
        public static Dictionary<string, string> GlobalNumbers = new Dictionary<string, string>();
        public static int RunCount = 0;

        /*
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            const string sql =
                @"IF OBJECT_ID('dbo.UniqueSequence', 'U') IS NULL
                    CREATE TABLE [dbo].[UniqueSequence]([Sequence] [nvarchar](255) NOT NULL,CONSTRAINT [PK_UniqueSequence] PRIMARY KEY CLUSTERED ([Sequence] ASC)
                    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON))";
            var repository = new CommerceRepositoryImpl("VirtoCommerce");
            repository.Database.ExecuteSqlCommand(sql);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            const string sql =
                @"IF OBJECT_ID('dbo.UniqueSequence', 'U') IS NOT NULL
                    DROP TABLE [dbo].[UniqueSequence]";
            var repository = new CommerceRepositoryImpl("VirtoCommerce");
            repository.Database.ExecuteSqlCommand(sql);

        }
        */

            /*
        [TestMethod]
        [DeploymentItem("connectionStrings.config")]
        [DeploymentItem("Configs/AppConfig.config", "Configs")]
        */

        //[Fact]
        //public void Can_run_sequences_performance()
        //{
        //    var repository = GetRepository();
        //    var sequence = new SequenceUniqueNumberGeneratorServiceImpl(() => repository);

        //    for (var i = 1; i < SequenceUniqueNumberGeneratorServiceImpl.SequenceReservationRange; i++)
        //    {
        //        var result = sequence.GenerateNumber("CO{0:yyMMdd}-{1:D5}");
        //        Debug.WriteLine(result);

        //        //This would fail if any duplicate generated
        //        Assert.False(GlobalNumbers.ContainsKey(result));
        //        GlobalNumbers.Add(result, result);

        //        /* don't need second checks here
        //        const string sql = "INSERT UniqueSequence VALUES(@p0);";

        //        //This would fail if any duplicate generated beause we use primary key
        //        var sqlResult = repository.Database.ExecuteSqlCommand(sql, result);
        //        Assert.Equal(1, sqlResult);
        //        */
        //    }
        //}

        //protected CommerceRepositoryImpl GetRepository()
        //{
        //    var repository = new CommerceRepositoryImpl(ConnectionString, new EntityPrimaryKeyGeneratorInterceptor(), new AuditableInterceptor(null));
        //    EnsureDatabaseInitialized(() => new CommerceRepositoryImpl(ConnectionString), () => Database.SetInitializer(new SetupDatabaseInitializer<CommerceRepositoryImpl, Configuration>()));

        //    /*
        //    const string sql =
        //        @"IF OBJECT_ID('dbo.UniqueSequence', 'U') IS NULL
        //            CREATE TABLE [dbo].[UniqueSequence]([Sequence] [nvarchar](255) NOT NULL,CONSTRAINT [PK_UniqueSequence] PRIMARY KEY CLUSTERED ([Sequence] ASC)
        //            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON))";

        //    repository.Database.ExecuteSqlCommand(sql);
        //    */

        //    return repository;
        //}

        //public override void Dispose()
        //{
        //    // Ensure LocalDb databases are deleted after use so that LocalDb doesn't throw if
        //    // the temp location in which they are stored is later cleaned.
        //    using (var context = new CommerceRepositoryImpl(ConnectionString))
        //    {
        //        context.Database.Delete();
        //    }
        //}
    }
}
