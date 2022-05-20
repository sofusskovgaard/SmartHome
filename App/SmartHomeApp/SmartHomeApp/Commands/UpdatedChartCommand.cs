using Microcharts;

namespace SmartHomeApp.Commands
{
    public class UpdatedChartCommand
    {
        public string Name { get; set; }
        public Chart Chart { get; set; }
    }
}