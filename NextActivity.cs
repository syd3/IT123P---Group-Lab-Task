using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace IT123P___Group_Lab_Task
{
    [Activity(Label = "NextActivity")]
    public class NextActivity : Activity
    {
        EditText editName, editSchool;
        Button btnAdd, btnSearch, btnUpdate, btnHome;
        RadioGroup gender;
        AutoCompleteTextView autoCompleteCountry;
        HttpWebResponse response;
        HttpWebRequest request;
        String name = "", school = "", country = "", selectedGender = "", res = "", login_name = "";
        int checkedItemId = 0;

        // The selected gender value is stored as an int in order to properly display the search results
        // If you want pure text, consider the changes specified below each corresponding line

        // Missing validation, so if unknown input is entered it returns a runtime error
        // Change IP to your own local IP

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.nextlayout);
            // Fetch the name of who logged in through Intent
            login_name = Intent.GetStringExtra("Name"); // Not Needed?? Not being used anywhere
            // ^ Can be used to display a welcome message?

            // Instantiate Widgets
            editName = FindViewById<EditText>(Resource.Id.editText1);
            editSchool = FindViewById<EditText>(Resource.Id.editText2);
            btnAdd = FindViewById<Button>(Resource.Id.button1);
            btnSearch = FindViewById<Button>(Resource.Id.button2);
            btnUpdate = FindViewById<Button>(Resource.Id.button3);
            btnHome = FindViewById<Button>(Resource.Id.button4);
            
            // Set RadioGroup
            gender = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            gender.CheckedChange += myRadioGroup_CheckedChange;
            
            // Set AutoComplete
            autoCompleteCountry = FindViewById<AutoCompleteTextView>(Resource.Id.autoCompleteTextView1);
            var country = new string[] { "Cambodia", "Indonesia", "Philippines", "Thailand", "Singapore" };
            ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, country);
            autoCompleteCountry.Adapter = adapter;

            btnAdd.Click += this.AddRecord;
            btnUpdate.Click += this.UpdateRecord;
            btnSearch.Click += this.SearchRecord;
            btnHome.Click += this.BackHome;
        }

        void myRadioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            checkedItemId = gender.CheckedRadioButtonId;
            RadioButton checkedRadioButton = FindViewById<RadioButton>(checkedItemId);
            selectedGender = checkedRadioButton.Text; // For converting the gender value into text, rather than int
            gender.Check(checkedItemId); // ??
        }

        public void AddRecord(object sender, EventArgs e)
        {
            name = editName.Text;
            school = editSchool.Text;
            country = autoCompleteCountry.Text;

            // Replace &gender= value to selectedGender instead if you want pure text data, rather than int
            request = (HttpWebRequest)WebRequest.Create("http://192.168.1.14/REST/add_record.php?name=" + name + "&school=" + school + "&country=" + country + "&gender=" + checkedItemId);
            response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            res = reader.ReadToEnd();
            Toast.MakeText(this, res, ToastLength.Long).Show();
        }

        public void UpdateRecord(object sender, EventArgs e)
        {
            name = editName.Text;
            school = editSchool.Text;
            country = autoCompleteCountry.Text;

            // Replace &gender= value to selectedGender instead if you want pure text data, rather than int
            request = (HttpWebRequest)WebRequest.Create("http://192.168.1.14/REST/update_record.php?name=" + name + "&school=" + school + "&country=" + country + "&gender=" + checkedItemId);
            response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            res = reader.ReadToEnd();
            Toast.MakeText(this, res, ToastLength.Long).Show();
        }

        public void SearchRecord(object sender, EventArgs e)
        {
            name = editName.Text;
            request = (HttpWebRequest)WebRequest.Create("http://192.168.1.14/REST/search_record.php?name=" + name);
            response = (HttpWebResponse)request.GetResponse();
            res = response.ProtocolVersion.ToString();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            var result = reader.ReadToEnd();
            // Parse the result to Json then get the root element
            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;

            var u1 = root[0]; // Implement loop if you plan to get more than 1 query result

            // Get the searched values one by one
            string searchedname = u1.GetProperty("name").ToString();
            string searchedschool = u1.GetProperty("school").ToString();
            string searchedcountry = u1.GetProperty("country").ToString();
            int searchedgender = Convert.ToInt32(u1.GetProperty("gender").ToString()); // Remove this if you want to display the data thru pure text, and add the line below
            // string searchedgender = u1.GetProperty("gender").ToString(); // For if displaying the data thru pure text

            // Set the data values to the widgets
            editName.Text = searchedname;
            editSchool.Text = searchedschool;
            autoCompleteCountry.Text = searchedcountry;
            gender.Check(searchedgender); // Remove if displaying data via pure text

            // Toast.MakeText(this, searchedgender, ToastLength.Long).Show(); // For debugging
        }

        public void BackHome(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
    }
}