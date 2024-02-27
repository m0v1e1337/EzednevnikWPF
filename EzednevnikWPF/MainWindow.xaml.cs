using System.Windows.Controls;
using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;
using Calendar = System.Globalization.Calendar;

public class Note
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}

public class JsonHelper<T>
{
    public void Serialize(string fileName, T data)
    {
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(fileName, json);
    }

    public T Deserialize(string fileName)
    {
        string json = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<T>(json);
    }
}

public class Diary
{
    private List<Note> notes;
    private string fileName = "diary.json";
    private JsonHelper<List<Note>> jsonHelper = new JsonHelper<List<Note>>();

    public Diary()
    {
        if (File.Exists(fileName))
        {
            notes = jsonHelper.Deserialize(fileName);
        }
        else
        {
            notes = new List<Note>();
        }
    }

    public List<Note> GetNotesForDate(DateTime date)
    {
        return notes.Where(x => x.Date.Date == date.Date).ToList();
    }

    public void AddNote(Note note)
    {
        notes.Add(note);
        SaveChanges();
    }

    public void EditNote(Note note)
    {
        Note existingNote = notes.FirstOrDefault(x => x.Date == note.Date);
        if (existingNote != null)
        {
            existingNote.Title = note.Title;
            existingNote.Description = note.Description;
            SaveChanges();
        }
    }

    public void DeleteNote(Note note)
    {
        notes.Remove(note);
        SaveChanges();
    }

    private void SaveChanges()
    {
        jsonHelper.Serialize(fileName, notes);
    }
}

public partial class MainWindow : Window
{
    private Diary diary;

    public MainWindow()
    {
        InitializeComponent();

        diary = new Diary();
        Calendar.SelectedDate = DateTime.Today;
        LoadNotesForDate(DateTime.Today,
            dataGrid: DataGrid);
    }

    private void InitializeComponent()
    {
        throw new NotImplementedException();
        // Реализация этого метода должна быть сгенерирована средствами среды разработки при создании окна
    }

    private void LoadNotesForDate(DateTime date, DataGrid dataGrid)
    {
        var notes = diary.GetNotesForDate(date);
        dataGrid.ItemsSource = notes;
    }

    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Calendar.SelectedDate.HasValue)
        {
            LoadNotesForDate(
                Calendar.SelectedDate.Value,
                DataGrid);
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        NoteWindow noteWindow = new NoteWindow();
        if (noteWindow.ShowDialog() == true)
        {
            Note note = noteWindow.Note;
            diary.AddNote(note);

            LoadNotesForDate(note.Date, dataGrid);
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e, DataGrid dataGrid)
    {
        object selectedItem = dataGrid.SelectedItem;
        Note selectedNote = selectedItem as Note;
        if (selectedNote != null)
        {
 NoteWindow noteWindow = new NoteWindow(selectedNote);
            if (noteWindow.ShowDialog() == true)
            {
                Note note = noteWindow.Note;
                diary.EditNote(note);
                LoadNotesForDate(note.Date, dataGrid);
            }
        }
    }

    private DataGrid GetDataGrid(DataGrid dataGrid)
    {
        return dataGrid;
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e, DataGrid dataGrid)
    {
        if (dataGrid.SelectedItem is Note selectedNote)
        {
            diary.DeleteNote(selectedNote);
            LoadNotesForDate(selectedNote.Date, dataGrid: dataGrid);
        }
    }
}

public partial class NoteWindow : Window
{
    public Note Note { get; private set; }
    public object DescriptionTextBox { get; private set; }
    public object TitleTextBox { get; private set; }

    public NoteWindow()
    {
        InitializeComponent();
    }

    public NoteWindow(Note note)
    {
        InitializeComponent();
        Note = note;
        TitleTextBox.Text = note.Title;
        DescriptionTextBox.Text = note.Description;
    }

    private void InitializeComponent()
    {
        throw new NotImplementedException();
        // Реализация этого метода должна быть сгенерирована средствами среды разработки при создании окна
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (Note == null)
        {
            Note = new Note();
        }

        Note.Title = TitleTextBox.Text;
        Note.Description = DescriptionTextBox.Text;

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}