using Microsoft.Owin;
using Microsoft.Owin.StaticFiles;

namespace SimulatorDialerDriver.WebApi
{
    public class SpaHostOptions
    {
        public FileServerOptions FileServerOptions { get; set; }
        public PathString EntryPath { get; set; }
        public PathString Route { get; set; }
    }
}