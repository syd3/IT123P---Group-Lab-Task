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
        String name = "", school = "", country = "", selectedGender = "", res = "";
        int checkedItemId = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.nextlayout);

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
            selectedGender = checkedRadioButton.Text; // For converting the gender value into a string, rather than int
            gender.Check(checkedItemId);
        }

        public void AddRecord(object sender, EventArgs e)
        {
            name = editName.Text;
            school = editSchool.Text;
            country = autoCompleteCountry.Text;

            request = (HttpWebRequest)WebRequest.Create("http://192.168.1.14/REST/add_record.php?name=" + name + "&school=" + school + "&country=" + country + "&gender=" + selectedGender);
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

            request = (HttpWebRequest)WebRequest.Create("http://192.168.1.14/REST/update_record.php?name=" + name + "&school=" + school + "&country=" + country + "&gender=" + selectedGender);
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

            try {
                var u1 = root[0];

                // Get the searched values one by one
                string searchedname = u1.GetProperty("name").ToString();
                string searchedschool = u1.GetProperty("school").ToString();
                string searchedcountry = u1.GetProperty("country").ToString();
                string searchedgender = u1.GetProperty("gender").ToString();

                // Set the data values to the widgets
                editName.Text = searchedname;
                editSchool.Text = searchedschool;
                autoCompleteCountry.Text = searchedcountry;
                for (int i = 0; i < gender.ChildCount; i++) { // Iterate through radiobuttons to check for matching gender
                    
                    int id = gender.GetChildAt(i).Id;
                    if (FindViewById(id) is RadioButton) {
                        RadioButton rb = FindViewById<RadioButton>(id);
                        rb.Checked = false;
                        if (rb.Text == searchedgender) {
                            rb.Checked = true;
                            break;
                        }
                    }
                }
            } catch (IndexOutOfRangeException ex) {
                Toast.MakeText(this, "User not found", ToastLength.Long).Show();
            }
        }

        public void BackHome(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }
    }
}