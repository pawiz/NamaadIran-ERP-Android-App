/*
 * Author: Shahrooz Sabet
 * Date: 20140628
 * */
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Java.Util;
using Mono.Data.Sqlite;

using NamaadDB.Adapter;
using NamaadDB.asqlitemanager;
using NamaadDB.Entity;
using NamaadDB.Util;
using NamaadDB.WebService;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NamaadDB
{
	/// <summary>
	/// TODO: Can use Time picker
	/// </summary>
	[Activity(Label = "@string/PishFactorForooshGah")]
	public class PishFactorForooshGah : ActionBaseForm
	{
		public static NmdDBAdapter dBHelper;
		/// <summary>
		/// The tag For Debugging purpose
		/// </summary>
		private const string TAG = "NamaadDB.PishFactorForooshGah";
		private const string _table = "WebGoodSalPrice";

		// UI references.
		private Button mCreateBtn;
		private Button mSaveBtn;
		private Button mBarCodeScannerBtn;
		private ListView listView;
		private TextView mTVLookUpCustomPriceTotal;
		private TextView mTVPosCustNo;
		private TextView mTVPosPriceType;
		private TextView mTVCurrentDate;
		private TextView mTV_Serial;
		private HorizontalScrollView hv;

		//private TextView mTVLookUpCustomQuantityTotal;
		//private ListView listViewQuantity;

		private IList<IParcelable> listItem = new List<IParcelable>();

		private LookUp_GoodSalePriceAdapter adapter;

		private int serialDprt;
		private int seq = 0;
		private bool _logging;

		//EventHandler
		private OnGlobalLayoutListener ngl;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			_logging = Prefs.getLogging(this);
			SetActionBarTitle(((SharedEnviroment)ApplicationContext).ActionName);
			SetContentView(Resource.Layout.pish_factor_forooshgah);
			mCreateBtn = (Button)FindViewById(Resource.Id.CreateBtn);
			mSaveBtn = (Button)FindViewById(Resource.Id.SaveBtn);
			mBarCodeScannerBtn = (Button)FindViewById(Resource.Id.BarCodeScannerBtn);
			mTVPosCustNo = ((TextView)FindViewById(Resource.Id.tVPosCustNo));
			mTVPosPriceType = ((TextView)FindViewById(Resource.Id.tVPosPriceType));
			mTVCurrentDate = ((TextView)FindViewById(Resource.Id.tVCurrentDate));
			mTV_Serial = ((TextView)FindViewById(Resource.Id.tV_Serial));
			hv = (HorizontalScrollView)FindViewById(Resource.Id.horizontalScrollView1);

			string strSQL;
			strSQL = "Select CrmDprt, PosCustNo, PosPriceType, CurrentDate From WebSalCommon";
			try
			{
				using (dBHelper = new NmdDBAdapter(this))
				{
					dBHelper.OpenOrCreateDatabase(((SharedEnviroment)ApplicationContext).DbNameClient);
					using (SqliteDataReader reader = dBHelper.ExecuteReader(strSQL))
						while (reader.Read())
						{
							mTVPosCustNo.Text = reader["PosCustNo"].ToString();
							mTVPosPriceType.Text = reader["PosPriceType"].ToString();
							//mTVCurrentDate_FactorForooshGah.Text = reader["CurrentDate"].ToString();
							serialDprt = int.Parse(reader["CrmDprt"].ToString());
						}
					strSQL = "SELECT Max(FormNo) As MaxFormNo FROM WebSalPerformaDtl";
					object obj = dBHelper.EXECUTESCALAR(strSQL);
					if (obj != DBNull.Value)
						mTV_Serial.Text = (int.Parse(obj.ToString()) + 1).ToString();

				}
			}
			catch (Exception e)
			{
				if (e.Message.Contains("SQLite error\r\nno such table:"))
					ExceptionHandler.toastMsg(this, GetString(Resource.String.error_reRefresh_pishFactor));
				else
					ExceptionHandler.toastMsg(this, e.Message);
				mCreateBtn.Enabled = false;
				mSaveBtn.Enabled = false;
				ExceptionHandler.logDActivity(e.ToString(), _logging, TAG);
			}
			mTVCurrentDate.Text = GetFormDate().ToString();
			mTVLookUpCustomPriceTotal = (TextView)FindViewById(Resource.Id.tVLookUpCustomPriceTotal);
			//mTVLookUpCustomQuantityTotal = (TextView)FindViewById(Resource.Id.tVLookUpCustomQuantityTotal);

			listView = (ListView)FindViewById(Resource.Id.lVFactorForooshGah);
			//var listItemCon = LastNonConfigurationInstance as IList<IParcelable>;
			//if (listItemCon != null)
			//{
			//	listItem = listItemCon;
			//}
			adapter = new LookUp_GoodSalePriceAdapter(this, listItem);// Using cursor is not feasible since we use Mono SQlite library.
			listView.Adapter = adapter;
			listView.TextFilterEnabled = true;
			// Tell the list view to show one checked/activated item at a time.
			listView.ChoiceMode = ChoiceMode.Single;

			RegisterForContextMenu(listView);

			mSaveBtn = (Button)FindViewById(Resource.Id.SaveBtn);
		}

		private void mCreateBtn_Click(object sender, EventArgs e)
		{
			/* Start new Activity that returns a result */

			Intent intent = new Intent(this, typeof(TableViewer));

			intent.PutExtra("Table", _table);
			intent.PutExtra("type", Types.TABLE);

			string tableDisplayName = GetString(Resource.String.WebGoodSalPrice);
			intent.PutExtra("TableDisplayName", tableDisplayName);
			IList<string> fieldNames = new List<string>();
			fieldNames.Add("Cost1");
			fieldNames.Add("PriceID");
			fieldNames.Add("PriceType");
			fieldNames.Add("Unit");
			fieldNames.Add("Price");
			fieldNames.Add("FarsiDesc");
			fieldNames.Add("ItemCode");
			IList<string> fieldDisplayNames = new List<string>();
			fieldDisplayNames.Add(GetString(Resource.String.tblLblCost1));
			fieldDisplayNames.Add(GetString(Resource.String.tblLblPriceID));
			fieldDisplayNames.Add(GetString(Resource.String.tblLblPriceType));
			fieldDisplayNames.Add(GetString(Resource.String.tblLblUnit));
			fieldDisplayNames.Add(GetString(Resource.String.tblLblPrice));
			fieldDisplayNames.Add(GetString(Resource.String.tblLblFarsiDesc));
			fieldDisplayNames.Add(GetString(Resource.String.tblLblItemCode));
			intent.PutStringArrayListExtra("FieldNames", fieldNames);
			intent.PutStringArrayListExtra("FieldDisplayNames", fieldDisplayNames);
			StartActivityForResult(intent, 0);
		}

		private void mSaveBtn_Click(object sender, EventArgs e)
		{
			Intent intent = new Intent(this, typeof(WSManager));

			string sql = "Select FarsiDesc,ItemCode From WebGoodSalPrice";
			intent.PutExtra("sql", sql);

			string tableDisplayName = GetString(Resource.String.WebGoodSalPrice);
			intent.PutExtra("TableDisplayName", tableDisplayName);
			IList<string> fieldNames = new List<string>();
			fieldNames.Add("FarsiDesc");
			fieldNames.Add("ItemCode");
			IList<string> fieldDisplayNames = new List<string>();
			fieldDisplayNames.Add(GetString(Resource.String.tblLblFarsiDesc));
			fieldDisplayNames.Add(GetString(Resource.String.tblLblItemCode));
			intent.PutStringArrayListExtra("FieldNames", fieldNames);
			intent.PutStringArrayListExtra("FieldDisplayNames", fieldDisplayNames);
			StartActivityForResult(intent, 0);
		}
		protected override void OnResume()
		{
			base.OnResume();
			mSaveBtn.Click += mSaveBtn_Click;
			mCreateBtn.Click += mCreateBtn_Click;
			mBarCodeScannerBtn.Click += mBarCodeScannerBtn_Click;
			ngl = new OnGlobalLayoutListener(delegate
			{
				hv.FullScroll(FocusSearchDirection.Right);
			}
			);
			hv.ViewTreeObserver.AddOnGlobalLayoutListener(ngl);
		}

		private void mBarCodeScannerBtn_Click(object sender, EventArgs e)
		{
			// start the scanner
			StartActivityForResult(typeof(ScanActivity), 1);
		}
		protected override void OnPause()
		{
			base.OnPause();
			mSaveBtn.Click -= mSaveBtn_Click;
			mCreateBtn.Click -= mCreateBtn_Click;
			mBarCodeScannerBtn.Click -= mBarCodeScannerBtn_Click;
			hv.ViewTreeObserver.RemoveGlobalOnLayoutListener(ngl);
			ngl.Dispose();
			ngl = null;
		}
		private void ToDB(WebGoodSalPrice res)
		{
			try
			{
				using (dBHelper = new NmdDBAdapter(this))
				{
					dBHelper.OpenOrCreateDatabase(((SharedEnviroment)ApplicationContext).DbNameClient);
					using (DataTable dt = dBHelper.GetDataTable(WebGoodSalPriceDTSB(res).ToString()))
					{
						dBHelper.FillToTable(dt, "WebSalPerformaDtl", null, null);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.toastMsg(this, e.Message);
				ExceptionHandler.logDActivity(e.ToString(), _logging, TAG);
			}
		}

		private static int GetFormDate()
		{
			PersianCalendar pc = new PersianCalendar();
			DateTime thisDate = DateTime.Now;
			int formDate = int.Parse(pc.GetYear(thisDate).ToString() + pc.GetMonth(thisDate).ToString() + pc.GetDayOfMonth(thisDate).ToString());
			return formDate;
		}
		public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo info)
		{
			base.OnCreateContextMenu(menu, v, info);
			menu.SetHeaderTitle(((WebGoodSalPrice)listItem.ElementAt(((Android.Widget.AdapterView.AdapterContextMenuInfo)info).Position)).FarsiDesc);
			menu.SetHeaderIcon(Android.Resource.Drawable.IcMenuDelete);
			//menu.Add(0, 0, 0, GetString(Resource.String.Update));
			menu.Add(0, 1, 0, GetString(Resource.String.Delete));
		}
		public override bool OnContextItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case 0:
					ContextItemClicked(item.TitleFormatted.ToString());
					break;
				case 1:
					DeleteItem(item);
					break;
			}
			return base.OnOptionsItemSelected(item);
		}

		private void DeleteItem(IMenuItem item)
		{
			Android.Widget.AdapterView.AdapterContextMenuInfo info = (Android.Widget.AdapterView.AdapterContextMenuInfo)item.MenuInfo;
			try
			{
				WebGoodSalPrice element = (WebGoodSalPrice)listItem.ElementAt(info.Position);
				mTVLookUpCustomPriceTotal.Text = (double.Parse(mTVLookUpCustomPriceTotal.Text) - element.Price * element.Quantity).ToString();
				listItem.RemoveAt(info.Position);
				using (dBHelper = new NmdDBAdapter(this))
				{
					dBHelper.OpenOrCreateDatabase(((SharedEnviroment)ApplicationContext).DbNameClient);
					dBHelper.ExecuteNonQuery("Delete From WebSalPerformaDtl Where FormNo=" + mTV_Serial.Text + " And Seq =" + (info.Position + 1) + " And SerialDprt=" + serialDprt + " And FormDate=" + mTVCurrentDate.Text);
				}
				//adapter.RemoveItem(element);
				//dBHelper.ExecuteNonQuery("Update WebSalPerformaDtl Set Seq=" + --seq + " Where FormNo=" + mTV_Serial_FactorForooshGah.Text + " And Seq =" + (info.Position + 2) + " And SerialDprt=" + serialDprt + " And FormDate=" + mTVCurrentDate_FactorForooshGah.Text);
			}
			catch (Exception e)
			{
				ExceptionHandler.toastMsg(this, e.Message);
				ExceptionHandler.logDActivity(e.ToString(), _logging, TAG);
			}
			adapter.NotifyDataSetChanged();
		}

		void ContextItemClicked(string item)
		{
			Console.WriteLine(item + " Option Menuitem Clicked");
			var t = Toast.MakeText(this, "Options Menu '" + item + "' Clicked", ToastLength.Short);
			t.Show();
		}
		//private void listViewItemSelected(object sender, AdapterView.ItemClickEventArgs e)
		//{
		//	// Make the newly clicked item the currently selected one.
		//	listView.SetItemChecked(e.Position, true);
		//}

		/// <summary>
		/// Called when the activity receives a results. Catch result
		/// </summary>
		/// <param name="requestCode">The integer request code originally
		/// supplied to startActivityForResult(), allowing you to identify
		/// who this result came from.</param>
		/// <param name="resultCode">The integer result code returned by 
		/// the child activity through its setResult().</param>
		/// <param name="data">An Intent, which can return result data to the caller 
		/// (various data can be attached to Intent "extras").</param>
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (resultCode == Result.Ok)
			{
				IParcelable[] tblFieldArr = null;
				try
				{
					if (requestCode == 0)
					{
						tblFieldArr = data.GetParcelableArrayExtra("TableField[]");
					}
					else if (requestCode == 1)
					{
						using (dBHelper = new NmdDBAdapter(this))
						{
							dBHelper.OpenOrCreateDatabase(((SharedEnviroment)ApplicationContext).DbNameClient);
							string barcode = data.GetStringExtra("barcode");
							long rowID = (long)dBHelper.EXECUTESCALAR("Select rowid From " + _table + " Where ItemCode Like '%" + barcode + "%'");
							tblFieldArr = dBHelper.getRecord(_table, rowID);
						}
					}
					QuantityDialogBuilder(WebGoodSalPriceBuilder(tblFieldArr));
				}
				catch (Exception e)
				{
					ExceptionHandler.toastMsg(this, e.Message);
					ExceptionHandler.logDActivity(e.ToString(), _logging, TAG);
				}
			}
		}

		private void QuantityDialogBuilder(WebGoodSalPrice webGoodSalePrice)
		{
			Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
			View line_editor = LayoutInflater.Inflate(Resource.Layout.line_editor, null);
			builder.SetView(line_editor);
			AlertDialog ad = builder.Create();
			ad.SetTitle(webGoodSalePrice.FarsiDesc);
			ad.SetIcon(Android.Resource.Drawable.IcDialogAlert);
			//ad.SetMessage("Alert message...");
			EditText etQuantity = (EditText)line_editor.FindViewById(Resource.Id.eTQuantity);
			TextView mTVUnitQuantityForm = ((TextView)line_editor.FindViewById(Resource.Id.tVUnitQuantityForm));
			mTVUnitQuantityForm.Text = webGoodSalePrice.Unit;
			double quantity = 1;
			ad.SetButton(GetString(Resource.String.OK), (s, e) =>
			{
				string str = etQuantity.Text.Trim();
				if (!string.IsNullOrWhiteSpace(str) && double.Parse(str) > 0)
					quantity = double.Parse(str);
				webGoodSalePrice.Quantity = quantity;
				AddItem(webGoodSalePrice);
			});
			ad.Show();
		}

		private WebGoodSalPrice WebGoodSalPriceBuilder(IParcelable[] tblFieldArr)
		{
			WebGoodSalPrice webGoodSalePrice = new WebGoodSalPrice();
			foreach (TableField field in tblFieldArr)
			{
				switch (field.getName())
				{
					case "Cost1":
						webGoodSalePrice.Cost1 = double.Parse(field.getValue());
						break;
					case "FarsiDesc":
						webGoodSalePrice.FarsiDesc = field.getValue();
						break;
					case "ItemCode":
						webGoodSalePrice.ItemCode = field.getValue();
						break;
					case "Price":
						webGoodSalePrice.Price = double.Parse(field.getValue());
						break;
					case "PriceID":
						webGoodSalePrice.PriceID = int.Parse(field.getValue());
						break;
					case "PriceType":
						webGoodSalePrice.PriceType = int.Parse(field.getValue());
						break;
					case "Unit":
						webGoodSalePrice.Unit = field.getValue();
						break;
				}
			}
			return webGoodSalePrice;
		}
		private void AddItem(WebGoodSalPrice webGoodSalePrice)
		{
			listItem.Add(webGoodSalePrice);
			//adapter.AddItem(res);
			adapter.NotifyDataSetChanged();
			//mTVLookUpCustomQuantityTotal.Text = (int.Parse(mTVLookUpCustomQuantityTotal.Text) + 1).ToString();
			mTVLookUpCustomPriceTotal.Text = (double.Parse(mTVLookUpCustomPriceTotal.Text) + webGoodSalePrice.Price * webGoodSalePrice.Quantity).ToString();
			ToDB(webGoodSalePrice);
		}
		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutParcelableArrayList("listItem", listItem);
			outState.PutString("mTVLookUpCustomPriceTotal", mTVLookUpCustomPriceTotal.Text);
			outState.PutString("formNo", mTV_Serial.Text);
			outState.PutInt("seq", seq);
			base.OnSaveInstanceState(outState);
		}
		protected override void OnRestoreInstanceState(Bundle savedState)
		{
			base.OnRestoreInstanceState(savedState);
			listItem = (IList<IParcelable>)savedState.GetParcelableArrayList("listItem"); ;
			adapter = new LookUp_GoodSalePriceAdapter(this, listItem);// Using cursor is not feasible since we use Mono SQlite library.
			listView.Adapter = adapter;
			mTVLookUpCustomPriceTotal.Text = savedState.GetString("mTVLookUpCustomPriceTotal");
			mTV_Serial.Text = savedState.GetString("formNo");
			seq = savedState.GetInt("seq");
		}
		//public override Java.Lang.Object OnRetainNonConfigurationInstance()
		//{
		//	base.OnRetainNonConfigurationInstance();
		//	return (Java.Lang.Object)listItem;//InValidCast Exception
		//}

		#region StringBuilder Implementation

		private StringBuilder WebGoodSalPriceDTSB(WebGoodSalPrice res)
		{
			int formDate = int.Parse(mTVCurrentDate.Text);
			StringBuilder sb = new StringBuilder();
			sb.Append("Select Cast(");
			sb.Append(serialDprt); sb.Append(" As int) As SerialDprt, Cast(");
			sb.Append(int.Parse(mTV_Serial.Text)); sb.Append(" As int) As FormNo, Cast(");
			sb.Append(formDate); sb.Append(" As int) As FormDate, Cast(");
			sb.Append(++seq); sb.Append(" As int) As Seq, Cast(");
			sb.Append("0 As int) As PreviousSeq,'");
			sb.Append(res.ItemCode); sb.Append("' As ItemCode, Cast(");
			sb.Append("0 As int) As BatchNo, Cast(");
			sb.Append("0 As int) As ServiceCode, Cast(");
			sb.Append(mTVPosCustNo.Text); sb.Append(" As int) As PersonNo, Cast(");
			sb.Append(res.PriceID); sb.Append(" As int) As PriceId, Cast(");
			sb.Append(res.PriceType); sb.Append(" As smallint) As PriceType, Cast(");
			sb.Append(res.Price); sb.Append(" As float) As Price, Cast(");
			sb.Append(res.Quantity); sb.Append(" As float) As Qty1, Cast(");
			sb.Append("1 As float) As Qty2, Cast(");
			sb.Append("0 As float) As UsedQty1, Cast(");
			sb.Append("0 As float) As UsedQty2, Cast(");
			sb.Append("0 As float) As IncrsDecrs1, Cast(");
			sb.Append("0 As float) As IncrsDecrs2, Cast(");
			sb.Append("0 As float) As IncrsDecrs3, Cast(");
			sb.Append("0 As float) As IncrsDecrs4, Cast(");
			sb.Append("0 As float) As UsedIncrsDecrs1, Cast(");
			sb.Append("0 As float) As UsedIncrsDecrs2, Cast(");
			sb.Append("0 As float) As UsedIncrsDecrs3, Cast(");
			sb.Append("0 As float) As UsedIncrsDecrs4, Cast(");
			sb.Append("0 As float) As HdrDiscount, Cast(");
			sb.Append("0 As float) As Discount, Cast(");
			sb.Append(res.Cost1); sb.Append(" As float) As Cost1, Cast(");
			sb.Append("0 As float) As Cost2, Cast(");
			sb.Append("0 As float) As Cost3, Cast(");
			sb.Append("0 As float) As Cost4, Cast(");
			sb.Append("0 As float) As Cost5, Cast(");
			sb.Append("0 As float) As Cost6, Cast(");
			sb.Append("0 As float) As Cost7, Cast(");
			sb.Append("0 As float) As Cost8, Cast(");
			sb.Append("0 As float) As Amount, Cast(");
			sb.Append("0 As float) As NetAmount, Cast(");
			sb.Append("0 As float) As CrdtAmount, Cast(");
			sb.Append("'' As varchar(250)) As Description, Cast(");
			sb.Append(formDate); sb.Append(" As int) As CreateDate, Cast(");
			sb.Append(((SharedEnviroment)ApplicationContext).UserCode); sb.Append(" As int) As CreateUser, Cast(");
			sb.Append(formDate); sb.Append(" As int) As ChangeDate, Cast(");
			sb.Append(((SharedEnviroment)ApplicationContext).UserCode); sb.Append(" As int) As ChangeUser, Cast(");
			sb.Append("1 As int) As TableDataVersion");

			return sb;
		}
		#endregion

		private partial class OnGlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
		{
			private Action on_global_layout;
			public OnGlobalLayoutListener(Action onGlobalLayout)
			{
				on_global_layout = onGlobalLayout;
			}
			public void OnGlobalLayout()
			{
				on_global_layout();
			}
		}
	}
}