using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exerciser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPage : ContentPage
    {
        Entry NameEntry, ValueEntry;
        string oldName;
        DbManager db;

        public EditPage(string parameter)
        {
            InitializeComponent();
            oldName = parameter;
            db = new DbManager();
            NameEntry = this.FindByName<Entry>("nameEntry");
            ValueEntry = this.FindByName<Entry>("valueEntry");
            NameEntry.Text = parameter;
            ValueEntry.Text = db.GetValueByName(parameter).ToString();
        }


        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            double c;
            if (NameEntry.Text != "" && ValueEntry.Text != "" && double.TryParse(ValueEntry.Text, out c))
            {
                db.EditExerciseType(oldName, NameEntry.Text, double.Parse(ValueEntry.Text));
                NameEntry.Text = "";
                ValueEntry.Text = "";
                Navigation.PopAsync();
            }
            else
            {
                DisplayAlert("Error", "Invalid input", "Ok");
            }

        }
    }
}