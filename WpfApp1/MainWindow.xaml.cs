using Caliburn.Micro;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

public class DropdownOption
{
    public String Label { get; set; }
    public String Value { get; set; }
}
public class Eclipse
{
    public String Type { get; set; }
    public String Date { get; set; }

    public Eclipse(String t, String d)
    {
        this.Type = t;
        this.Date = d;
    }
}
public class DataAccess
{
    private static HtmlDocument GetHtmlFromUrl(string url)
    {
        HttpClient httpClient = new HttpClient();
        var response = httpClient.GetAsync(url).Result;

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine($"Failed req {response.StatusCode}");
        }

        string responseMessage = response.Content.ReadAsStringAsync().Result;
        var doc = new HtmlDocument();
        doc.LoadHtml(responseMessage);
        return doc;
    }
    private static List<Eclipse> GetEclipses(string url)
    {
        var doc = GetHtmlFromUrl(url);
        var links = doc.DocumentNode.SelectNodes("//a[@class='ec-link']");
        List<Eclipse> eclipses = new List<Eclipse>();
        foreach (var link in links)
        {
            String href = link.GetAttributeValue("href", "");
            string[] splitted = href.Split('/');
            var type = splitted[2];
            var date = splitted[3];
            Eclipse ecl = new Eclipse(type, date);
            // var splitted_date = date.Split("-");
            eclipses.Add(ecl);

        }
        return eclipses;
    }

    public static List<Eclipse> BuildUrlAndGetEclipses(String region, String starty, String type)
    {
        var url = "http://www.timeanddate.com/eclipse/list.html?region=" + region + "&starty=" + starty + "&type=" + type;
        return GetEclipses(url);
    }
}
namespace WpfApp1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            this.DataContext = this;
            this.StartY = "2020";
            this.Eclipses = new BindableCollection<Eclipse>(EclipseList);
            InitializeTypeDropdownOptions();
            InitializeRegionDropdownOptions();
            this.TypeDropdownOptions = new BindableCollection<DropdownOption>(TypeOptionsList);
            this.RegionDropdownOptions = new BindableCollection<DropdownOption>(RegionOptionsList);
            InitializeComponent();
        }

        private string _startY;
        public string StartY
        {
            get { return _startY; }
            set
            {
                if (value != _startY)
                {
                    _startY = value;
                    PropertyChanged?.Invoke(this, new
                    PropertyChangedEventArgs(nameof(StartY)));
                }
            }
        }

        private List<Eclipse> _eclipsesList = new List<Eclipse>();
        public BindableCollection<Eclipse> Eclipses { get; set; }
        public List<Eclipse> EclipseList
        {
            get { return _eclipsesList; }
            set
            {
                if (value != _eclipsesList)
                {
                    _eclipsesList = value;
                    PropertyChanged?.Invoke(this, new
                    PropertyChangedEventArgs(nameof(EclipseList)));
                }
            }
        }
        public BindableCollection<DropdownOption> TypeDropdownOptions { get; set; }
        private List<DropdownOption> TypeOptionsList = new List<DropdownOption>();

        private DropdownOption _selectedType;

        public DropdownOption SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
            }
        }

        public BindableCollection<DropdownOption> RegionDropdownOptions { get; set; }
        private List<DropdownOption> RegionOptionsList = new List<DropdownOption>();

        private DropdownOption _selectedRegion;

        public DropdownOption SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                _selectedRegion = value;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeTypeDropdownOptions()
        {
            this.TypeOptionsList.Add(new DropdownOption() { Label= "Solar Eclipses", Value= "solar" });
            this.TypeOptionsList.Add(new DropdownOption() { Label = "Lunar Eclipses", Value = "lunar" });
            this.TypeOptionsList.Add(new DropdownOption() { Label = "Total Solar Eclipses", Value = "total-solar" });
            this.TypeOptionsList.Add(new DropdownOption() { Label = "Annular Solar Eclipses", Value = "annular-solar" });
            this.TypeOptionsList.Add(new DropdownOption() { Label = "Partial Solar Eclipses", Value = "partial-solar" });
            this.TypeOptionsList.Add(new DropdownOption() { Label = "Total Lunar Eclipses", Value = "total-lunar" });
            this.TypeOptionsList.Add(new DropdownOption() { Label = "Partial Lunar Eclipses", Value = "partial-lunar" });
            this.TypeOptionsList.Add(new DropdownOption() { Label = "Penumbral Lunar Eclipses", Value = "penumbral-lunar" });


        }

        private void InitializeRegionDropdownOptions()
        {
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Africa", Value = "africa" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Asia", Value = "asia" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Atlantic", Value = "atlantic" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Arctic", Value = "arctic" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Australia", Value = "australia" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Europe", Value = "europe" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Indian Ocean", Value = "indian-ocean" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "North America", Value = "north-america" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Pacific", Value = "pacific" });
            this.RegionOptionsList.Add(new DropdownOption() { Label = "South America", Value = "south-america" });


        }
        private void getListButton_Click(object sender, RoutedEventArgs e)
        {
            List<Eclipse> output = DataAccess.BuildUrlAndGetEclipses(SelectedRegion.Value, StartY, SelectedType.Value);
            EclipseList = output;
            Eclipses.Clear();
            EclipseList.ForEach(ecl => Eclipses.Add(ecl));
            
        }
        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(SelectedType.Value + " " + SelectedRegion.Value);
            
        }
    }
}
