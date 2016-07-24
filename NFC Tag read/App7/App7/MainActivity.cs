using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Nfc;
using Android.Nfc.Tech;
using System.Text;

namespace App7
{
    [Activity(Label = "App7", MainLauncher = true, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    [IntentFilter(new[] {NfcAdapter.ActionNdefDiscovered }, Categories = new[] {
    Intent.CategoryDefault,
    }, DataMimeType = "text/plain")]
    public class MainActivity : Activity
    {
        private TextView _textview;
        private NfcAdapter _nfcAdapter;
        private PendingIntent mPendingIntent;
        private IntentFilter ndefDetected;
        private IntentFilter[] intentF;

    protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            _textview = FindViewById<TextView>(Resource.Id.textview);
            Intent Myintent = new Intent(this, GetType());
            Myintent.SetFlags(ActivityFlags.SingleTop);
            mPendingIntent = PendingIntent.GetActivity(this, 0, Myintent, 0);
            ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
            try
            {
                ndefDetected.AddDataType("text/plain");
            }
            catch {};
            intentF = new IntentFilter[] {ndefDetected};

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            if (_nfcAdapter != null && _nfcAdapter.IsEnabled)
            {
                Toast.MakeText(this, "Nfc Found", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "Nfc Not Found", ToastLength.Long).Show();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            //_textview.Text = "OnPause";
            
            NfcManager manager = (NfcManager)GetSystemService(NfcService);
            NfcAdapter adapter = manager.DefaultAdapter;
            adapter.DisableForegroundDispatch(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            //_textview.Text = "OnResume";

            NfcManager manager = (NfcManager)GetSystemService(NfcService);
            manager.DefaultAdapter.EnableForegroundDispatch(this, mPendingIntent, intentF, null);
        }
        
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            //_textview.Text = "onNewIntent";

            var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

            if (tag != null)
            {
                IParcelable[] rawMsgs = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);

                if (rawMsgs != null)
                {
                    var msgs = new NdefMessage[rawMsgs.Length];

                    for (int i = 0; i < rawMsgs.Length; i++)
                    {
                        if (msgs == null)
                            break;
                        msgs[i] = (NdefMessage)rawMsgs[i];
                        _textview.Text = (Encoding.UTF8.GetString(msgs[i].GetRecords()[0].GetPayload())).Remove(0, 3);
                    }
                }
            }
        }
    }
}

