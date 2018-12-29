using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Exerciser
{
    class DbManager
    {
        const string dbPath = "moc2";
        SQLiteConnection db;
        public DbManager()
        {
            db = new SQLiteConnection(DependencyService.Get<IFileHelper>().GetLocalFilePath(dbPath));
            db.CreateTable<ExerciseType>();
            db.CreateTable<SDay>();
            db.CreateTable<Rep>();
        }


        ~DbManager()
        {
            db.Close();
        }


        public class Rep
        {
            [PrimaryKey, AutoIncrement, Column("_id")]
            public int Id { get; set; }
            [Indexed]
            public int ExerciseTypeId { get; set; }
            [Indexed]
            public int SDayId { get; set; }
            public int Repetition { get; set; }
        }

        public class SDay
        {
            [PrimaryKey, AutoIncrement, Column("_id")]
            public int Id { get; set; }
            [NotNull]
            public DateTime Day { get; set; }
            public double Weight { get; set; }
            public int Recognized { get; set; }
            public double Score { get; set; }
        }

        public class ExerciseType
        {
            [PrimaryKey, AutoIncrement, Column("_id")]
            public int Id { get; set; }
            [NotNull, Unique]
            public string Name { get; set; }
            public double Value { get; set; }
            public int Ordero { get; set; }
        }

        public ObservableCollection<Exercise> GetExercises(DateTime day)
        {
            var result = db.Query<ExerciseType>("Select * FROM ExerciseType ORDER BY Ordero DESC");
            ObservableCollection<Exercise> roc = new ObservableCollection<Exercise>();
            foreach (ExerciseType x in result)
            {

                roc.Add(new Exercise()
                {
                    Name = x.Name,
                    Repetitions = GetReps(x.Id, GetSDayId(day))
                });


            }
            return roc;
        }

        public int GetSDayId(DateTime day)
        {
            var result = db.Query<SDay>("SELECT _id FROM SDay WHERE Day=?", day);
            if (result.Count == 0)
            {
                SDay newDay = new SDay() { Day = day };
                db.Insert(newDay);
                return newDay.Id;
            }
            return result[0].Id;
        }

        public DateTime GetSDay(int SDayId)
        {
            return db.Query<SDay>("Select * FROM SDay WHERE _id=?", SDayId).First().Day;
        }

        public double GetReps(int ExerciseId, int SDayId)
        {
            var result = db.Query<Rep>("Select * FROM Rep WHERE ExerciseTypeId=? AND SDayId = ?", ExerciseId, SDayId);
            if (result.Count == 0)
            {
                Rep newRep = new Rep() { ExerciseTypeId = ExerciseId, SDayId = SDayId, Repetition = 0 };
                db.Insert(newRep);
                return newRep.Repetition;
            }
            else if (result[0].ExerciseTypeId == GetExerciseTypeIdByName("Weight")) return Math.Round((double)result[0].Repetition / 10, 1);
            return result[0].Repetition;
        }

        //hehehehehehehheh

        public int GetExerciseTypeIdByName(string name)
        {
            var result = db.Query<ExerciseType>("SELECT * FROM ExerciseType WHERE Name = ?", name);
            if (result.Count == 0) return 0;
            return result[0].Id;
        }

        public double GetValueByName(string name)
        {
            var result = db.Query<ExerciseType>("SELECT * FROM ExerciseType WHERE Name = ?", name);
            return result[0].Value;
        }

        public void SaveExercises(ObservableCollection<Exercise> list, DateTime day)
        {
            foreach (Exercise x in list)
            {
                SQLiteCommand Command;
                if (x.Name == "Weight") Command = db.CreateCommand("UPDATE Rep SET Repetition=? WHERE ExerciseTypeId=? AND SDayId=?", Math.Round(x.Repetitions * 10, 0), GetExerciseTypeIdByName(x.Name), GetSDayId(day));
                else Command = db.CreateCommand("UPDATE Rep SET Repetition=? WHERE ExerciseTypeId=? AND SDayId=?", x.Repetitions, GetExerciseTypeIdByName(x.Name), GetSDayId(day));
                Command.ExecuteNonQuery();
            }
        }

        public double GetScore(ObservableCollection<Exercise> collection)
        {
            double sum = 0;
            foreach (var x in collection)
            {
                sum += x.Repetitions * GetValueByName(x.Name);
            }
            return sum;
        }

        public void AddExerciseType(string name, double value)
        {
            int heh = 0;
            if (name == "Weight") heh = 2;
            if (name == "Calories" || name == "Kcal") heh = 1;
            var it = new ExerciseType()
            {
                Name = name,
                Value = value,
                Ordero = heh
            };
            db.Insert(it);
        }

        public void DeleteExerciseType(string name)
        {
            db.Delete(new ExerciseType { Id = GetExerciseTypeIdByName(name) });
        }

        public void EditExerciseType(string name, string newName, double newValue)
        {
            int heh = 0;
            if (newName == "Weight") heh = 2;
            if (newName == "Calories" || newName == "Kcal") heh = 1;
            db.CreateCommand("UPDATE ExerciseType SET Name=?, Value=?, Ordero=? WHERE _id=?", newName, newValue, heh, GetExerciseTypeIdByName(name)).ExecuteNonQuery();
        }

    }
}
