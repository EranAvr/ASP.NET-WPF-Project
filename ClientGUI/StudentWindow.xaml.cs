using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        private AuthResponse userAuthentication;
        private StudentExam? currentExam;
        private ExamBaseInfo? presentedInfo;
        public StudentWindow(AuthResponse auth)
        {
            InitializeComponent();
            userAuthentication = auth;
        }

        private void OnWindowLoaded(Object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(this.currentSessionAuth.ToString());
        }

        private void OnSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            
            
        }

        private void StartExamOnClick(Object sender, RoutedEventArgs e)
        {
            //studentContentControl.Content = new UserControlStudentInfo();
        }

        private async void buttonSearchOnClick(object sender, RoutedEventArgs e)
        {
            // Make validation:
            string examName = tbSearchExam.Text;
            if (examName == string.Empty)
            {
                MessageBox.Show("Exam Name field is empty");
                return;
            }
            // Search exam in server
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(ConstantsRepository.clientConnectBaseAddress)
            };
            HttpResponseMessage response = await httpClient.GetAsync($"api/Exams/Templates/Info/{examName}");
            string jsonBody = await response.Content.ReadAsStringAsync();
            if (jsonBody == String.Empty || jsonBody == null)
            {
                MessageBox.Show($"Exam Not Found!\nName: {examName}");
                return;
            }
            presentedInfo = JsonSerializer.Deserialize<ExamBaseInfo>(jsonBody);
            if (presentedInfo == null)
            {
                MessageBox.Show("Error while Parsing exam!");
                return;
            }
            
            tbPresentExamInfo.Text = presentedInfo.ToString();
        }
        private async void buttonSubmitOnClick(object sender, RoutedEventArgs e)
        {

            // Make validation:
            string sName = tbStudEnterName.Text;
            if (sName == string.Empty)
            {
                MessageBox.Show("Name field is empty");
                return;
            }
            string sId = tbStudEnterID.Text;
            if (sId == string.Empty)
            {
                MessageBox.Show("ID field is empty");
                return;
            }
            if (presentedInfo == null)
            {
                MessageBox.Show("Exam not chosen");
                return;
            }
            // Check for date and time-
            var currentTimeStamp = DateTime.Now;
            if (! (presentedInfo.Date <= currentTimeStamp && 
                currentTimeStamp < presentedInfo.Date.AddHours(presentedInfo.TimeDuration))) {
                MessageBox.Show("Exam is not running at the moment\nPlease try at another time\n" +
                    $"Current Time: {currentTimeStamp}\nExam\tFrom: {presentedInfo.Date}\n\tTo: {presentedInfo.Date.AddHours(presentedInfo.TimeDuration)}");
                return;
            }
            // Search exam in server
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(ConstantsRepository.clientConnectBaseAddress)
            };
            HttpResponseMessage response = await httpClient.GetAsync($"api/Exams/Solved/GetNew/{userAuthentication.userId}/{presentedInfo.Name}");
            string jsonBody = await response.Content.ReadAsStringAsync();
            if (jsonBody == String.Empty || jsonBody == null)
            {
                MessageBox.Show($"Exam Not Found!\nName: {presentedInfo.Name}");
                return;
            }
            currentExam = JsonSerializer.Deserialize<StudentExam>(jsonBody);
            if (currentExam != null)
            {
                MessageBox.Show("Good Luck! :)");
                RunExamWindow window = new RunExamWindow(currentExam);
                window.ShowDialog();
            }
            else {
                MessageBox.Show($"Error accord.\n{jsonBody}");
            }
        }
        private void buttonClearOnClick(object sender, RoutedEventArgs e)
        {
            // reset data:
            currentExam = null;
            // reset screen:
            tbStudEnterName.Text = string.Empty;
            tbStudEnterID.Text = string.Empty;
            tbSearchExam.Text = string.Empty;
            tbPresentExamInfo.Text = string.Empty;
        }
    }

}
