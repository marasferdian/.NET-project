using Caliburn.Micro;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using WpfApp1.Model;

namespace WpfApp1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            this.DataContext = this;
            InitializeTypeDropdownOptions();
            InitializeRegionDropdownOptions();
            InitializeStartYearDropdownOptions();
            this.TypeDropdownOptions = new BindableCollection<DropdownOption>(TypeOptionsList);
            this.RegionDropdownOptions = new BindableCollection<DropdownOption>(RegionOptionsList);
            this.StartYearDropdownOptions = new BindableCollection<DropdownOption>(StartYearOptionsList);
            this.Eclipses = new BindableCollection<Eclipse>(EclipseList);
            SetDefaults();
            InitializeComponent();
            DisableDownloadButtonOnInit();
            HideTableOnInit();
        }

        public bool isDownloadButtonEnabled = false;
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

        public BindableCollection<DropdownOption> StartYearDropdownOptions { get; set; }
        private List<DropdownOption> StartYearOptionsList = new List<DropdownOption>();

        private DropdownOption _selectedStartYear;

        public DropdownOption SelectedStartYear
        {
            get { return _selectedStartYear; }
            set
            {
                _selectedStartYear = value;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeTypeDropdownOptions()
        {
            this.TypeOptionsList.Add(new DropdownOption() { Label = "All Eclipses", Value = "" });
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
            this.RegionOptionsList.Add(new DropdownOption() { Label = "Worldwide", Value = "" });
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

        private void InitializeStartYearDropdownOptions()
        {
            this.StartYearOptionsList.Add(new DropdownOption() { Label = "Next 10 years", Value = "" });
            for (int startY=1900; startY< 2200; startY += 10)
            {
                String v = startY.ToString();
                String l = startY.ToString() + "-" + (startY + 9).ToString();
                this.StartYearOptionsList.Add(new DropdownOption() { Label = l, Value = v });
            }
        }

        private void SetDefaults()
        {
            this.SelectedType = TypeOptionsList[0];
            this.SelectedRegion = RegionOptionsList[0];
            this.SelectedStartYear = StartYearOptionsList[0];
        }

        private void DisableDownloadButtonOnInit()
        {
            downloadButton.IsEnabled = false;
        }
        private void HideTableOnInit()
        {
            eclipsesTable.Visibility= Visibility.Hidden;
        }
        private void getListButton_Click(object sender, RoutedEventArgs e)
        {
            List<Eclipse> output = DataAccess.BuildUrlAndGetEclipses(SelectedRegion.Value, SelectedStartYear.Value, SelectedType.Value);
            EclipseList = output;
            Eclipses.Clear();
            EclipseList.ForEach(ecl => Eclipses.Add(ecl));
            downloadButton.IsEnabled = true;
            eclipsesTable.Visibility = Visibility.Visible;
        }
        private string prepareString(string s)
        {
            return s.Replace(" ", "_").ToLower();
        }
        private string getFileName()
        {
            var trimmedRegion = prepareString(SelectedRegion.Label);
            var trimmedYear = prepareString(SelectedStartYear.Label);
            var trimmedType = prepareString(SelectedType.Label);
            string name = "eclipses-" + trimmedType + "-" + trimmedYear + "-" + trimmedRegion;
            return name;
        }
        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(SelectedType.Value + " " + SelectedRegion.Value);
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = getFileName();
            dlg.DefaultExt = ".txt";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {

                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DataAccess.DownloadContent(EclipseList, dlg.FileName);
                string filename = dlg.FileName;
            }


        }
    }
}
