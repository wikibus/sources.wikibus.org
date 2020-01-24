using DbUp.Cli;

namespace dbup
{
    class Program
    {
        static int Main(string[] args)
        {
            return new Upgrader(args).Run(SqlServerExtensions.SqlDatabase);
        }
    }
}
