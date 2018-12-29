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
    public partial class AddPage : ContentPage
    {
        Entry NameEntry, ValueEntry;
        DbManager db;

        public AddPage()
        {
            InitializeComponent();
            db = new DbManager();
            NameEntry = this.FindByName<Entry>("nameEntry");
            ValueEntry = this.FindByName<Entry>("valueEntry");
        }

        public ICommand LolCommand
        {
            get
            {
                return new Command(() =>
                {
                    DisplayAlert("sdafas", "sdafsad", "sdafsad");
                });
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            //Navigation.NavigationStack.First<Page>().;
            Navigation.PopAsync();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            int c;
            if (NameEntry.Text != "" && ValueEntry.Text != "" && int.TryParse(ValueEntry.Text, out c))
            {
                db.AddExerciseType(NameEntry.Text, int.Parse(ValueEntry.Text));
                NameEntry.Text = "";
                ValueEntry.Text = "";
            }
            else
            {
                DisplayAlert("Error", "Invalid input", "Ok");
            }

        }

        private void nameEntry_Completed(object sender, EventArgs e)
        {
            ValueEntry.Focus();
        }

        private void valueEntry_Completed(object sender, EventArgs e)
        {
            Button_Clicked(this, null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NameEntry.Focus();
        }
    }
}