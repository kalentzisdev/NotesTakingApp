using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace NotesTakingApp
{
    public partial class NoteTaker : Form
    {
        DataTable notes = new DataTable();
        bool editing = false;

        private string notesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "YourAppNameNotes");
        private const string NotesFilePath = "Notes.txt";


        public NoteTaker()
        {
            InitializeComponent();


            // Check if the directory exists, if not, create it
            if (!Directory.Exists(notesDirectory))
            {
                Directory.CreateDirectory(notesDirectory);
            }

            PopulateDataGridView();
        }


        private void NoteTaker_Load(object sender, EventArgs e)
        {

            notes.Columns.Add("Title");
            notes.Columns.Add("Notes");


            //add in the grid the data from the notes dataTable
            previousNotes.DataSource = notes;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                //removes the index we click from grid (previousNotes)
                notes.Rows[previousNotes.CurrentCell.RowIndex].Delete();
            }
            catch (Exception ex) { Console.WriteLine("Not a valid note!"); }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if(editing)
            {
                notes.Rows[previousNotes.CurrentCell.RowIndex]["Title"] = titleBox.Text;
                notes.Rows[previousNotes.CurrentCell.RowIndex]["Notes"] = noteBox.Text;
            }
            else
            {
                notes.Rows.Add(titleBox.Text, noteBox.Text);
            }

            //after click clear the boxes and editing to false
            titleBox.Text = "";
            noteBox.Text = "";
            editing = false;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            //clear boxes when we press newbutton
            titleBox.Text = "";
            noteBox.Text = "";
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            titleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            noteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();
            
            editing = true;
        }

        private void previousNotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //on doubleclick do the same with loadButton
            titleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            noteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();

            editing = true;
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {

            // Save the note to a file in the created directory
            string filePath = Path.Combine(notesDirectory, NotesFilePath);



            if (notes != null && notes.Rows.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    foreach(DataRow row in notes.Rows)
                    {
                        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                        writer.WriteLine($"{currentDate} - Title: {row["Title"]} - Note : {row["Notes"]}");
                        
                    }
                    
                }
            }
        }

        //because its private i will create a public method onStartApplication
        private void PopulateDataGridView()
        {
            string filePath = Path.Combine(notesDirectory, NotesFilePath);

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    
                    while ((line = reader.ReadLine()) != null || (line = reader.ReadLine()) != "")
                    {
                        //Split the line into two parts: "Title" and "Note"
                        string[] values = line.Split(new string[] { " - Title: ", " - Note : " }, StringSplitOptions.None);
                        

                        if (values != null && values.Length >= 2)
                        {
                            // Add a row to the DataTable
                            DataRow newRow = notes.NewRow();
                            newRow["Title"] = values[1]; // Assign the first part to the "Title" column
                            newRow["Notes"] = values[2]; // Assign the second part to the "Notes" column
                            notes.Rows.Add(newRow);
                        }

                    }
                }
            }
        }
    }
}
