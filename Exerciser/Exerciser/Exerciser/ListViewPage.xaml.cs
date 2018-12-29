using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Exerciser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        public ObservableCollection<Exercise> Exercises { get; set; }
        DateTime ADay;
        DbManager Db;
        ListView lv;
        Label dateLabel;


        public ListViewPage()
        {
            InitializeComponent();
            lv = this.FindByName<ListView>("Pagunia");
            Db = new DbManager();
            dateLabel = this.FindByName<Label>("label1");
            //scoreLabel = this.FindByName<Label>("scoreLabel");

            ADay = DateTime.Today;
            SetExercises(ADay);
            //dateLabel.Text = Eday.Day.Day.ToString() + "." + Eday.Day.Month.ToString() + "." + Eday.Day.Year.ToString();
            BindingContext = this;
        }


        async void Handle_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            var list = (ListView)sender;
            var item = list.SelectedItem as Exercise;
            string myinput = await App.InputBox(this.Navigation);
            if (myinput != null && myinput != "error")
            {
                await Task.Delay(10);

                if (Exercises.Where(Exercise => (Exercise.Name == item.Name && Exercise.Name != "Weight")).Count() != 0)
                    Exercises.Where(Exercise => (Exercise.Name == item.Name && Exercise.Name != "Weight")).First().Repetitions += int.Parse(myinput);
                else
                    Exercises.Where(Exercise => (Exercise.Name == item.Name && Exercise.Name == "Weight")).First().Repetitions += (double.Parse(myinput) / 10);


                Db.SaveExercises(Exercises, ADay);
                SetExercises(ADay);
            }

            if (myinput == "error")
            {
                await DisplayAlert("Error", "Invalid input", "Ok");
            }

            if (e.SelectedItem == null)
            {
                await DisplayAlert("Item Tapped", "An item was tafdgdsfpped.", "OK");
                return;
            }
            //await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }







        public ICommand MoreCommand
        {
            get
            {
                return new Command<string>((string parameter) =>
                {
                    Exercises.Where(x => x.Name == parameter).First().Repetitions++;
                    scoreLabel.Text = Math.Round(Db.GetScore(Exercises), 0).ToString();
                });
            }
        }

        public ICommand LessCommand
        {
            get
            {
                return new Command<string>((string parameter) =>
                {
                    if (Exercises.Where(x => (x.Name == parameter)).First().Repetitions-- <= 0) Exercises.Where(x => (x.Name == parameter)).First().Repetitions++;
                    scoreLabel.Text = Db.GetScore(Exercises).ToString();
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                //foreach (Exercise x in Exercises)
                //{

                //}
                return new Command(() =>
                {
                    Db.SaveExercises(Exercises, ADay);
                });
            }
        }

        public ICommand PreviousCommand
        {
            get
            {
                return new Command(() =>
                {
                    ADay = ADay.AddDays(-1);
                    SetExercises(ADay);
                });
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return new Command(() =>
                {
                    ADay = ADay.AddDays(1);
                    SetExercises(ADay);
                });
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new Command(() =>
                {
                    AddFunction();
                });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new Command<string>((string parameter) =>
                {
                    DeleteOptions(parameter);
                });
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return new Command<string>((string parameter) =>
                {
                    EditFunction(parameter);
                });
            }
        }

        public async void DeleteOptions(string parameter)
        {
            string x = await DisplayActionSheet("Attention", "Delete", "Cancel", String.Format("Are you sure to delete this exercise type:\n{0}?", parameter));
            if (x == "Delete")
            {
                Db.DeleteExerciseType(parameter);
                SetExercises(ADay);
            }
        }

        async public void AddFunction()
        {
            await Navigation.PushAsync(new AddPage());
        }

        async public void EditFunction(string parameter)
        {
            await Navigation.PushAsync(new EditPage(parameter));
        }

        public void SetExercises(DateTime date)
        {
            Exercises = Db.GetExercises(date);
            Exercises.Reverse();
            lv.ItemsSource = Exercises;
            dateLabel.Text = date.Day.ToString() + "." + date.Month.ToString() + "." + date.Year.ToString();
            scoreLabel.Text = Db.GetScore(Exercises).ToString();
        }



        protected override void OnAppearing()
        {
            SetExercises(ADay);
            base.OnAppearing();



            //System.Diagnostics.Debug.WriteLine("*****Here*****");
        }

    }
}