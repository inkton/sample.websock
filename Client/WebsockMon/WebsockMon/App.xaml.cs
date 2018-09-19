using System;
using System.IO;
using System.Resources;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.DeviceInfo;
using Newtonsoft.Json;
using Inkton.Nester;
using Inkton.Nester.Storage;
using Inkton.Nester.Cloud;
using Inkton.Nester.ViewModels;
using Inkton.Nester.Helpers;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WebsockMon
{
    public partial class App : Application, INesterControl
    {
        private const int ApiVersion = 1;
        private LogService _log;
        private NesterService _backend;

        public App()
        {
            InitializeComponent();

            /* 
             * Query objects can be cached locally. Specify the 
             * location of the cache here
             */
            StorageService cache = new StorageService(Path.Combine(
                    Path.GetTempPath(), "NesterCache"));
            cache.Clear();

            /* 
             * The DeviceIno identifies this device to the server
             * backend with a unique signature
             */ 
            string deviceSignature =
                JsonConvert.SerializeObject(CrossDeviceInfo.Current);

            _log = new LogService(Path.Combine(
                    Path.GetTempPath(), "NesterLog"));

            _backend = new NesterService(
                ApiVersion, deviceSignature, cache);

            /* 
             * testing the app locally on the desktop
             * change to http://websock.nestapp.yt/Monitor/
             * for testing the production app.
             *
             * Insert the docker allocated <portno> below.
             */
            _backend.Endpoint = "http://127.0.0.1:<portno>/Monitor/";

            MainPage = new MainPage();
        }

        public NesterService Backend
        {
            get { return _backend; }
        }

        public LogService Log
        {
            get { return _log; }
        }

        public ResourceManager GetResourceManager()
        {
            /*  Note: the default resource file may not
             *  have generated a "Resource.Designer.cs".
             *  If so remove and add a new resource file.
             *  The namespace to use appears in the 
             *  Resource.Designer.cs file ResourceManager
             *  property.
             */
            ResourceManager resmgr = new ResourceManager(
                  "WebsockMon.Resource",
                  typeof(App).GetTypeInfo().Assembly);
            return resmgr;
        }
    }
}
