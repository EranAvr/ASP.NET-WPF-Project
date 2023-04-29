using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientGUI.Models;
using Microsoft.Win32;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for EditTestMenu.xaml
    /// </summary>
    public partial class UserControlEditQuestion : UserControl
    {
        Exam currentExam;
        Question currentQuestion;
        //string examID;
        //string questionID;

        public UserControlEditQuestion(Exam e, Question q)
        {
            InitializeComponent();
            currentExam = e;
            currentQuestion = q;
            SetPresentQuestion(q);
        }

        private void SetPresentQuestion(Question q)
        {
            tbQtext.Text = q.QText;
            tbQpic.Text = q.QPic == null? "" : "Picture loaded. Double click to present..";
            tbQcorrect.Text = q.CorrectAnswer;
            if (q.Answers.Count > 0)
            {
                foreach (string ans in q.Answers)
                {
                    listBoxEditAnswers.Items.Add(ans);
                }
            }
            cbQrnd.IsChecked = q.Randomize;
        }

        // ListBox Events:
        private void AnswersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxEditAnswers.SelectedItem != null)
            {
                textBoxEditAnswer.Text = listBoxEditAnswers.SelectedItem.ToString();
            }

        }

        // Repository Buttons:
        // These methods handle connection with data
        // in repository.
        private void buttonSaveOnClick(object sender, RoutedEventArgs e)
        {
            currentQuestion.QText = tbQtext.Text;
            currentQuestion.CorrectAnswer = tbQcorrect.Text;
            // reinitialize the questions list
            // to prevent repeating answers.
            currentQuestion.Answers = new List<string>();
            foreach (string ans in listBoxEditAnswers.Items)
            {
                currentQuestion.Answers.Add(ans);
            }
            currentQuestion.Randomize = (bool)cbQrnd.IsChecked;
            // update current question's data in repository.
            // (currentQuest is a reference to exam in repo)
            currentExam.UpdateQuestion(currentQuestion);

            MessageBox.Show("Saved on temporary Repo");
        }

        // Question Edit Buttons:
        // These methods only handle GUI.
        private void buttonClearOnClick(object sender, RoutedEventArgs e)
        {
            // Clear current question's data on screen.
            tbQtext.Text = string.Empty;
            tbQpic.Text = string.Empty;
            tbQcorrect.Text = string.Empty;
            textBoxEditAnswer.Text = string.Empty;
            listBoxEditAnswers.Items.Clear();
            cbQrnd.IsChecked = false;

            MessageBox.Show("Cleared on temporary Repo");
        }
        private void buttonPictureOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                try
                {
                    currentQuestion.QPic = File.ReadAllBytes(openFileDialog.FileName);
                    tbQpic.Text = openFileDialog.FileName;
                }
                catch (Exception exception){
                    MessageBox.Show(exception.Message);
                    return;
                }
            }
        }
        private void buttonAddAnswerOnClick(object sender, RoutedEventArgs e) {
            string answer = textBoxEditAnswer.Text;
            // check whether given answer already exists in the listBox.
            foreach (string ans in listBoxEditAnswers.Items) {
                if (answer == ans) {
                    MessageBox.Show("Answer already exists!");
                    return;
                }
            }
            // add answer to listBox
            if (answer != string.Empty) {
                listBoxEditAnswers.Items.Add(answer);
            }
        }
        private void buttonSelectCorrectOnClick(object sender, RoutedEventArgs e)
        {
            if (listBoxEditAnswers.SelectedItem is string ans)
            {
                // if an answer is selected in listBox,
                // present it in a textBlock.
                tbQcorrect.Text = ans;
            }
        }
        private void buttonDeleteAnswerOnClick(object sender, RoutedEventArgs e) {
            int index = listBoxEditAnswers.SelectedIndex;
            // check if an answer is selected in listBox
            if (index >= 0)
            {
                // in case deleted answer is the correct answer-
                if (tbQcorrect.Text == listBoxEditAnswers.Items.GetItemAt(index)) {
                    tbQcorrect.Text = string.Empty;
                }
                // remove answer from screen-
                listBoxEditAnswers.Items.RemoveAt(index);
                textBoxEditAnswer.Text = string.Empty;
                
            }
        }
        
        private void buttonUpdateAnswerOnClick(object sender, RoutedEventArgs e) {
            int index = listBoxEditAnswers.SelectedIndex;
            string answer = textBoxEditAnswer.Text;
            // check if textBox contains a non-empty string, and
            // if an answer is selected in listBox
            if (index >= 0 && answer != string.Empty)
            {
                // in case deleted answer is the correct answer-
                if (tbQcorrect.Text == listBoxEditAnswers.Items.GetItemAt(index))
                {
                    tbQcorrect.Text = answer;
                }
                // update answer in listBox.
                listBoxEditAnswers.Items[index] = answer;

            }
        }
    }
}
