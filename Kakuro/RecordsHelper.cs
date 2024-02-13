using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.Settings;

namespace Kakuro
{
    public static class RecordsHelper
    {
        public const string RecordTableKey = "RecordTable";

        public static bool UpdateRecordTable(GameRecord newRecord)
        {
            bool recordTableUpdated = false;

            var recordTable = GetRecordTable();

            var sortedrecordTable = new ObservableCollection<GameRecord>(from gameRecord in recordTable
                                                                         orderby gameRecord.Difficulty, gameRecord.RecordTime descending
                                                                         select gameRecord);

            for (int i = 0; i < sortedrecordTable.Count - 1; i++)
            {
                var gameRecord = sortedrecordTable[i];
                if (gameRecord.Difficulty == newRecord.Difficulty && gameRecord.RecordTime > newRecord.RecordTime)
                {
                    sortedrecordTable.RemoveAt(i);
                    sortedrecordTable.Insert(i, newRecord);
                    recordTableUpdated = true;
                    break;
                }
            }

            return recordTableUpdated;
        }

        public static ObservableCollection<GameRecord> GetRecordTable()
        {
            ObservableCollection<GameRecord> recordTable;

            string serializedRecordTable = CrossSettings.Current.GetValueOrDefault(RecordTableKey, string.Empty);

            if (!string.IsNullOrEmpty(serializedRecordTable))
            {
                recordTable = JsonConvert.DeserializeObject<ObservableCollection<GameRecord>>(serializedRecordTable);
            }
            else
            {
                recordTable = new ObservableCollection<GameRecord>();
            }

            return recordTable;
        }

        public static void SaveRecordTable(ObservableCollection<GameRecord> recordTable)
        {
            var sortedRecordTable = new ObservableCollection<GameRecord>(from gameRecord in recordTable
                                                                         orderby gameRecord.Difficulty, gameRecord.RecordTime descending
                                                                         select gameRecord);
            CrossSettings.Current.AddOrUpdateValue(RecordTableKey, JsonConvert.SerializeObject(sortedRecordTable));
        }

        public class GameRecord
        {
            public GameRecord(GameDifficulty difficulty, TimeSpan recordTime)
            {
                Difficulty = difficulty;
                RecordTime = recordTime;
            }

            public GameDifficulty Difficulty { get; set; }
            public TimeSpan RecordTime { get; set; }
        }

        public enum GameDifficulty
        {
            easy,
            medium,
            hard
        }
    }
}