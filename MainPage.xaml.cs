using System;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lesson_203
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        BMP280 BMP280;
        float temp;
        float pressure;
        float altitude;
        const float seaLevelPressure = 1013.25f;
        int reportnumber;
        DispatcherTimer dispatcherTimer;


        string resultxt;

        //A class which wraps the barometric sensor
        public MainPage()
        {
            this.InitializeComponent();

             dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);

            dispatcherTimer.Tick += tick;

            dispatcherTimer.Start();

       

        }

        private void tick(object sender, object e)
        {
            check();
           
        }

     

        //This method will be called by the application framework when the page is first loaded
        protected override async void OnNavigatedTo(NavigationEventArgs navArgs)
        {
            Debug.WriteLine("MainPage::OnNavigatedTo");

            MakePinWebAPICall();
            try
            {
                //Create a new object for our barometric sensor class
                BMP280 = new BMP280();
                //Initialize the sensor
                await BMP280.Initialize();

                //Create variables to store the sensor data: temperature, pressure and altitude. 
                //Initialize them to 0.
                temp = 0;
                pressure = 0;
                altitude = 0;


                //Create a constant for pressure at sea level. 
                //This is based on your local sea level pressure (Unit: Hectopascal)              
                //Read 10 samples of the data
                for (int i = 0; i < 10; i++)
                {
                    temp = await BMP280.ReadTemperature();
                    pressure = await BMP280.ReadPreasure();
                    altitude = await BMP280.ReadAltitude(seaLevelPressure);


                    //Write the values to your debug console                            
                    Debug.WriteLine("Temprature: " + temp.ToString() + " deg");
                    Debug.WriteLine("Pressure: " + pressure.ToString() + " Pa");
                    Debug.WriteLine("Altitude: " + altitude.ToString() + "m");

                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }




      


    }
       

           /// <summary>
        // This method will put your pin on the world map of makers using this lesson.
        // This uses imprecise location (for example, a location derived from your IP 
        // address with less precision such as at a city or postal code level). 
        // No personal information is stored.  It simply
        // collects the total count and other aggregate information.
        // http://www.microsoft.com/en-us/privacystatement/default.aspx
        // Comment out the line below to opt-out
        /// </summary>
        public void MakePinWebAPICall()
        {
            try
            {
                HttpClient client = new HttpClient();

                // Comment this line to opt out of the pin map.
                client.GetStringAsync("http://adafruitsample.azurewebsites.net/api?Lesson=203");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Web call failed: " + e.Message);
            }
        }
        private async void check()
        {

            if (reportnumber == null) reportnumber = 0;
            else
                reportnumber = reportnumber+1;

                temp = await BMP280.ReadTemperature();
                pressure = await BMP280.ReadPreasure();
                altitude = await BMP280.ReadAltitude(seaLevelPressure);

           
                 //Random rand = new Random();
                 //temp = (float)(24 + rand.NextDouble() * 4 - 2);
                 //pressure = (float)(9800 + rand.NextDouble() * 4 - 2);
                 //altitude = (float)(110 + rand.NextDouble() * 4 - 2);
         

            //Write the values to your debug console                            
            Debug.WriteLine("Temprature: " + temp.ToString() + " deg");
            Debug.WriteLine("Pressure: " + pressure.ToString() + " Pa");
            Debug.WriteLine("Altitude: " + altitude.ToString() + "m");
             resultxt = "Temprature: " + temp.ToString() + " deg \n" + "Pressure: " + pressure.ToString() + " Pa\n" + "Altitude: " + altitude.ToString() + "m" + "\n count: " + reportnumber.ToString();
           

            Task.Run(async () => { await AzureIoTHub.SendDeviceToCloudMessageAsync(reportnumber, temp, pressure); });

        }



        private void button_Click(object sender, RoutedEventArgs e)
        {
          
            check();
            result_txtBlock.Text = resultxt  ;

        }
    }
}
