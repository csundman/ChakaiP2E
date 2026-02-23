using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

namespace ChakaiP2EFunctionsAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new HostBuilder()
                    .ConfigureFunctionsWorkerDefaults()
                    .Build();

                host.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
