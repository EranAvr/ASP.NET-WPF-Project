using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientGUI.Models
{
    internal class ExamsRepository : IExamsRepository
    {
        //private Dictionary<string, Exam> exams;
        private List<Exam> exams;
        
        // Make class Singleton:
        private static ExamsRepository _instance = null;
        private ExamsRepository() { 
            //exams = new Dictionary<string, Exam>();
            exams = new List<Exam>();
        }
        public static ExamsRepository Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new ExamsRepository();
                return _instance;
            }
        }
        public void Clear()
        {
            exams = new List<Exam>();
        }


        // Implement interface:
        
        // Handle memory operations-
        public void LoadExamsFromMemory()
        {
            throw new NotImplementedException();
        }

        public void SaveExamsToMemory()
        {
            throw new NotImplementedException();
        }

        public void ReloadExams(List<Exam> newExams)
        {
            exams = newExams;
        }

        // Handle CRUD operations-
        public void AddNewExam(Exam exam)
        {
            int index = exams.FindIndex(e => e.examId == exam.examId);
            if (index >= 0)
            {
                MessageBox.Show("Exam already exist.\nID: " + exam.examId);
            }
            else { 
                exams.Add(exam);
            }
                
            //MessageBox.Show("Exam added: " + exam.examId);
        }

        public void DeleteExam(string eId)
        {
            int index = exams.FindIndex(e => e.examId == eId);
            if (index >= 0)
            { 
                exams.RemoveAt(index);
                //MessageBox.Show("Exam deleted: " + eId);
            }
        }

        public List<Exam> GetAllExams()
        {
            return exams;
        }

        public Exam GetExamByID(string eId)
        {
            int index = exams.FindIndex(e => e.examId == eId);
            if (index >= 0)
            {
                return exams[index];
            }
            return null;
        }
        public Exam GetExamByName(string tname)
        {
            int index = exams.FindIndex(e => e.Info.Name == tname);
            if (index >= 0)
            {
                return exams[index];
            }
            return null;
        }

        public void UpdateExam(Exam exam)
        {
            int index = exams.FindIndex(e => e.examId == exam.examId);
            if (index >= 0)
            {
                exams[index].Info = exam.Info;
                exams[index].Questions = exam.Questions;
                //MessageBox.Show("Exam updated: " + exam.examId);
            }
        }
    }
}
