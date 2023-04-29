using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClientGUI.Models;
using Microsoft.Win32;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for TeacherWindow.xaml
    /// </summary>
    public partial class TeacherWindow : Window
    {
        public string baseAddress = "https://localhost:7252/";
        private IExamsRepository repo;
        private AuthResponse userAuthentication;
        private string currentExamID = string.Empty;
        private Exam currentExam = null;
        public TeacherWindow(AuthResponse auth)
        {
            InitializeComponent();
            repo = ExamsRepository.Instance;
            repo.Clear();
            listBoxServer.ItemsSource = repo.GetAllExams();
            userAuthentication = auth;
            // for content data-binding in UI
            this.DataContext = userAuthentication;
        }

        private void OnWindowLoaded(Object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(this.currentSessionAuth.ToString());
        }
        private void OnClosing(Object sender, CancelEventArgs e)
        {
            bool result = MessageBox.Show("Data that wasn't saved to the server,\nwill be lost.\nExit?", "Question",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No;
            if (result)
            {
                e.Cancel = true;
                return;
            }
        }

        private void OnClick(Object sender, RoutedEventArgs e)
        {
            // Adding new question to current exam.
            if (listBoxCurrExam.Items.Count > 0)
            {
                Question question = new Question();
                repo.GetExamByID(currentExamID).AddQuestion(question);
                listBoxCurrExam.Items.Add(question.QID);
                listBoxCurrExam.SelectedIndex = listBoxCurrExam.Items.Count - 1;
            }
        }

        private void LoadExamToScreen(Exam exam)
        {
            currentExam = exam;
            currentExamID = exam.examId;
            listBoxCurrExam.Items.Clear();
            // add first element, for exam info
            listBoxCurrExam.Items.Add("info");
            // add questions IDs to listBox
            foreach (Question q in exam.GetAllQuestions())
            {
                listBoxCurrExam.Items.Add(q.QID);
            }
            teacherContentControl.Content = new UserControlEditInfo(exam, userAuthentication);
        }

        private void ListBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (listBoxCurrExam.SelectedItem is string item)
            {
                if (item == "info")
                {
                    teacherContentControl.Content = new UserControlEditInfo(repo.GetExamByID(currentExamID), userAuthentication);
                }
                else
                {
                    teacherContentControl.Content =
                        new UserControlEditQuestion(repo.GetExamByID(currentExamID), repo.GetExamByID(currentExamID).GetQuestionByID(item));
                }
            }
        }



        // Current Exam Events:
        private void AddOnClick(Object sender, RoutedEventArgs e)
        {
            // Adding new question to current exam.
            if (listBoxCurrExam.Items.Count > 0)
            {
                Question question = new Question();
                repo.GetExamByID(currentExamID).AddQuestion(question);
                listBoxCurrExam.Items.Add(question.QID);
                listBoxCurrExam.SelectedIndex = listBoxCurrExam.Items.Count-1;
                MessageBox.Show("Repo Updated");
            }
            
        }
        private void DeleteOnClick(Object sender, RoutedEventArgs e)
        {
            // Delete question from current exam.
            if (listBoxCurrExam.SelectedItem != null)
            {
                string? item = listBoxCurrExam.SelectedItem.ToString();
                if (item != null && item != "info")
                {
                    repo.GetExamByID(currentExamID).DeleteQuestion(item);
                    listBoxCurrExam.Items.Remove(listBoxCurrExam.SelectedItem);
                    listBoxCurrExam.SelectedIndex = 0;
                    MessageBox.Show("Repo Updated");
                }
            }
        }



        // Server Events:
        private async void MyOnClick(Object sender, RoutedEventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            HttpResponseMessage response = await httpClient.GetAsync($"api/Exams/Templates/Of/{userAuthentication.userName}");
            string jsonResBody = await response.Content.ReadAsStringAsync();
            List<Exam>? examsFound = JsonSerializer.Deserialize<List<Exam>>(jsonResBody);

            if (examsFound == null || examsFound.Count == 0) {
                MessageBox.Show("No exams are found");
                return;
            }
            repo.ReloadExams(examsFound);
            listBoxServer.ItemsSource = examsFound;
            listBoxServer.Items.Refresh();
        }
        private async void AllOnClick(Object sender, RoutedEventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            HttpResponseMessage response = await httpClient.GetAsync("api/Exams/Templates/All");
            string jsonResBody = await response.Content.ReadAsStringAsync();
            List<Exam> examsFound = JsonSerializer.Deserialize<List<Exam>>(jsonResBody);
            
            if (examsFound == null || examsFound.Count == 0)
            {
                MessageBox.Show("No exams are found");
                return;
            }
            repo.ReloadExams(examsFound);
            listBoxServer.ItemsSource = repo.GetAllExams();
            listBoxServer.Items.Refresh();
        }
        private void SearchOnClick(Object sender, RoutedEventArgs e)
        {

        }
        private void ServerBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (listBoxServer.SelectedItem is Exam exam)
            {
                LoadExamToScreen(exam);
            }
        }



        // ScrollDown Menu Events:
        private void NewOnClick(Object sender, RoutedEventArgs e)
        {
            Exam exam = new Exam(new() { Author=userAuthentication.userName, AuthorID=userAuthentication.userId });
            LoadExamToScreen(exam);
            repo.AddNewExam(exam);
            listBoxServer.Items.Refresh();
            listBoxServer.SelectedIndex = listBoxServer.Items.Count - 1;

            MessageBox.Show("Exam added to Repo");
        }
        private async void SaveDBOnClick(Object sender, RoutedEventArgs e)
        {
            if (currentExam == null)
                return;

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            StringContent reqBody = new StringContent(JsonSerializer.Serialize(repo.GetExamByID(currentExamID)), 
                Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("api/Exams/Templates/Add", reqBody);
            //string jsonResBody = await response.Content.ReadAsStringAsync();
            string msgBody = await response.Content.ReadAsStringAsync();
            MessageBox.Show(msgBody);
        }
        private async void DeleteFromDBOnClick(Object sender, RoutedEventArgs e)
        {
            if (currentExam == null)
                return;
            // delete from server-
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAddress);
            HttpResponseMessage response = await httpClient.DeleteAsync($"api/Exams/Templates/Remove/{currentExamID}");
            // delete from repo-
            repo.DeleteExam(currentExamID);
            // update screen-
            if (listBoxServer.SelectedItem is Exam exam)
            {
                LoadExamToScreen(exam);
            }
            listBoxCurrExam.Items.Clear();
            teacherContentControl.Content = new UserControl();
            listBoxServer.Items.Refresh();
        }
        
        private void SaveFileOnClick(Object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true) {
                File.WriteAllText(saveFileDialog.FileName, JsonSerializer.Serialize(repo.GetExamByID(currentExamID)));
            }
        }
        private async void LoadFileOnClick(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                string fileName = openFileDialog.FileName;
                StreamReader reader = new StreamReader(fileName);
                string content = await reader.ReadToEndAsync();
                Exam? ex = JsonSerializer.Deserialize<Exam>(content);
                if (ex != null)
                {
                    repo.AddNewExam(ex);
                    listBoxServer.Items.Refresh();
                } 
            }
        }
    }
}
