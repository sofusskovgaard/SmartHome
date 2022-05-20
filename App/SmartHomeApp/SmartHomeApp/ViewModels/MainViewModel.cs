using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microcharts;
using SmartHomeApp.Base;
using SmartHomeApp.Commands;
using SmartHomeApp.Dtos.DataApiService;
using SmartHomeApp.Infrastructure.TinyIoC;
using SmartHomeApp.Services.BlindsToggleService;
using SmartHomeApp.Services.DataApiService;
using Xamarin.Forms;

namespace SmartHomeApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDataApiService _dataApiService;
        private readonly IBlindsToggleService _blindsToggleService;
        
        public LineChart tChart { get; set; }
        public string tChartMin => tChart != null ? Math.Round(tChart.MinValue) + "°" : string.Empty;
        public string tChartMax => tChart != null ? Math.Round(tChart.MaxValue) + "°" : string.Empty; 
        
        public LineChart hChart { get; set; }

        public string hChartMin => hChart != null ? Math.Round(hChart.MinValue) + "%" : string.Empty;
        public string hChartMax => hChart != null ? Math.Round(hChart.MaxValue) + "%" : string.Empty;

        public RadialGaugeChart tLatestChart { get; set; }
        public string tLatestChartValue => tLatestChart != null ? Math.Round((Decimal)tLatestChart.Entries.First().Value) + "°" : string.Empty; 
        public RadialGaugeChart hLatestChart { get; set; }
        public string hLatestChartValue => hLatestChart != null ? Math.Round((Decimal)hLatestChart.Entries.First().Value) + "%" : string.Empty;
        
        public MainViewModel()
        {
            var container = TinyIoCContainer.Current;
            _dataApiService = container.Resolve<IDataApiService>();
            _blindsToggleService = container.Resolve<IBlindsToggleService>();
            
            MessagingCenter.Subscribe<MainViewModel, UpdatedChartCommand>(this, "update", (model, command) =>
            {
                model.GetType().GetProperty(command.Name).SetValue(model, command.Chart);
                model.OnPropertyChanged(command.Name);
                
                if (command.Chart is LineChart)
                {
                    model.OnPropertyChanged(command.Name + "Min");
                    model.OnPropertyChanged(command.Name + "Max");
                }
                if (command.Chart is RadialGaugeChart)
                    model.OnPropertyChanged(command.Name + "Value");
            });
        }

        private Command _toggleBlindsCommand;

        public ICommand ToggleBlindsCommand => _toggleBlindsCommand ??= new Command(() => _blindsToggleService.Toggle());

        private Command _refreshDataCommand;
        public ICommand RefreshDataCommand => _refreshDataCommand ??= new Command(() =>
        {
            _updateLineChart(nameof(tChart), MeasurementType.Temperature, LocationType.Home);
            _updateLineChart(nameof(hChart), MeasurementType.Humidity, LocationType.Home);
            _updateRadialChart(nameof(tLatestChart), MeasurementType.Temperature, LocationType.Home);
            _updateRadialChart(nameof(hLatestChart), MeasurementType.Humidity, LocationType.Home);
        });

        private async Task _updateLineChart(string chart, MeasurementType measurementType, LocationType locationType)
        {
            var result = new List<DataPointDto>(await _dataApiService.GetLastMinutes(180, measurementType, locationType));

            if (result.Any())
            {
                var entries = result.Select(x => new ChartEntry(x.Value)).ToList();
                
                MessagingCenter.Send(this, "update", new UpdatedChartCommand()
                {
                    Name = chart,
                    Chart = new LineChart()
                    {
                        Entries = entries,
                        MaxValue = entries.Max(x => x.Value) + .5f,
                        MinValue = entries.Min(x => x.Value) - .5f,
                        LineMode = LineMode.Straight,
                        LineSize = 2,
                        PointSize = 0,
                        IsAnimated = true,
                    }
                });
            }
        }
        
        private async Task _updateRadialChart(string chart, MeasurementType measurementType, LocationType locationType)
        {
            var result = await _dataApiService.GetLatest(14, measurementType, locationType);

            if (result != null)
            {

                MessagingCenter.Send(this, "update", new UpdatedChartCommand()
                {
                    Name = chart,
                    Chart = new RadialGaugeChart()
                    {
                        Entries = new []
                        {
                            new ChartEntry(result.Value)
                        },
                        MaxValue = 100f,
                        MinValue = 0f,
                        // LineSize = 2,
                        IsAnimated = true,
                    }
                });
            }
        }
    }
}