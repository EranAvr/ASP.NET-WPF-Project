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
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for RunExamWindow.xaml
    /// </summary>
    public partial class RunExamWindow : Window
    {
        private bool cancelClosingAlert = false;
        private const string INFO_ITEM = "Info";
        StudentExam activeExam { get; }    // Reference to current active exam
        //List<(int, string)> PresentedExamInfo { get; set; }    // A list to keep track on questions (updated in 'OnLoad' method)
        List<Question> examPresentedList { get; set; }    // A list to keep track on questions (updated in 'OnLoad' method)

        public RunExamWindow(StudentExam exam)
        {
            InitializeComponent();
            activeExam = exam;
            //PresentedExamInfo = new List<(int, string)>();
            examPresentedList = new List<Question>();
            // making 'PresentedExamInfo' the source item of our listBox-
            listBoxSideInfo.ItemsSource = examPresentedList;
        }

        private void OnLoad(Object sender, RoutedEventArgs e)
        {
            // set some on-screen info:
            detailsExamName.Text = activeExam.BInfo.Name;

            // setup scrolling list:
            int listCounter = 0;
            examPresentedList.Add(new() { 
                QText = INFO_ITEM
            });
            foreach (Question q in activeExam.GetAllQuestions())
            {
                listCounter++;
                examPresentedList.Add(q);
            }
            if (activeExam.BInfo.Randomize) {
                for (int i = 1; i < examPresentedList.Count; ++i) {
                    int rndIdx = new Random().Next(1, examPresentedList.Count);
                    Question qHolder = examPresentedList[rndIdx];
                    examPresentedList[rndIdx] = examPresentedList[i];
                    examPresentedList[i] = qHolder;
                }
            }
            listBoxSideInfo.Items.Refresh();
            listBoxSideInfo.SelectedIndex = 0;

        }
        private void OnClosing(Object sender, CancelEventArgs e)
        {
            if (cancelClosingAlert) {
                return;
            }
            bool result = MessageBox.Show("Warning!\nBy closing all data will be lost.\nAre you sure?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No;
            if (result) {
                e.Cancel = true;
            }
        }
        private void ListBoxSideSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (listBoxSideInfo.SelectedItem is Question q)
            {
                if (q.QText == INFO_ITEM)
                {
                    runExamContentControl.Content = new UserControlPresentInfo(activeExam);
                }
                else
                {
                    listBoxSideInfo.Items.Refresh();
                    runExamContentControl.Content =
                        new UserControlPresentQuestion(q);
                }
            }
        }

        private bool IsSubmitValid() 
        {
            return true;
        }
        private async void SubmitExamOnClick(object sender, RoutedEventArgs e)
        {
            if (IsSubmitValid()) {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(ConstantsRepository.clientConnectBaseAddress);
                StringContent reqBody = new StringContent(JsonSerializer.Serialize(activeExam),
                    Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("api/Exams/Solved/Submit", reqBody);
                if (!response.IsSuccessStatusCode) {
                    MessageBox.Show($"Error accord!\n{response}");
                    return;
                }
                MessageBox.Show("Exam submitted successfully");
                cancelClosingAlert = true;
                Close();
            }
            else
            {
                MessageBox.Show("Error accord");
                return;
            }
        }
    }
}
