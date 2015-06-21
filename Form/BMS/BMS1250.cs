/*
 * Author: Shahrooz Sabet
 * Date: 20150429
 * */
#region using
using Android.App;
using Android.OS;
using Android.Widget;
using NamaadMobile.Data;
using NamaadMobile.SharedElement;
using NamaadMobile.Util;
#endregion
namespace NamaadMobile
{
    [Activity(Label = "BMS1250")]
    public class BMS1250 : NamaadFormBase
    {
        #region Define
        public static NmdMobileDBAdapter dbHelper;
        // UI references.
        private LinearLayout bms1250MainLayout;
        private ListView bms1250ListView;
        #endregion
        #region Event
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ((SharedEnviroment)ApplicationContext).Logging = Prefs.getLogging(this);
            ((SharedEnviroment)ApplicationContext).TAG = "NamaadMobile.BMS1250";
            SetActionBarTitle(((SharedEnviroment)ApplicationContext).ActionName);
            SetContentView(Resource.Layout.bms1250);
            bms1250MainLayout = (LinearLayout)FindViewById(Resource.Id.bms1250MainLayout);
            bms1250ListView = (ListView)bms1250MainLayout.FindViewById(Resource.Id.bms1250ListView);
            BMSPublic bmsPublic = new BMSPublic();
            bmsPublic.AddEditableSensorToLayout(this, bms1250ListView);
            bmsPublic.AddSwitchDeviceToLayout(this, bms1250ListView);
            bmsPublic.AddResetToLayout(this, bms1250MainLayout);
        }
        #endregion
    }
}