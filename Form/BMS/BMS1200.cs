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
    [Activity(Label = "BMS1200")]
    public class BMS1200 : NamaadFormBase
    {
        #region Define
        public static NmdMobileDBAdapter dbHelper;
        // UI references.
        private LinearLayout bms1200MainLayout;
        private ListView bms1200ListView;
        #endregion
        #region Event
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ((SharedEnviroment)ApplicationContext).Logging = Prefs.getLogging(this);
            ((SharedEnviroment)ApplicationContext).TAG = "NamaadMobile.BMS1200";
            SetActionBarTitle(((SharedEnviroment)ApplicationContext).ActionName);
            SetContentView(Resource.Layout.bms1200);
            bms1200MainLayout = (LinearLayout)FindViewById(Resource.Id.bms1200MainLayout);
            bms1200ListView = (ListView)bms1200MainLayout.FindViewById(Resource.Id.bms1200ListView);
            BMSPublic bmsPublic = new BMSPublic();
            bmsPublic.AddEditableSensorToLayout(this, bms1200ListView);
            bmsPublic.AddSwitchDeviceToLayout(this, bms1200ListView);
            bmsPublic.AddResetToLayout(this, bms1200MainLayout);
        }
        #endregion
    }
}