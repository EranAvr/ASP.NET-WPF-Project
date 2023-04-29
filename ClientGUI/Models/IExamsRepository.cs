using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI.Models
{
    internal interface IExamsRepository
    {
        void Clear();
        // Database operations
        void LoadExamsFromMemory();
        void SaveExamsToMemory();
        void ReloadExams(List<Exam> newExams);

        // CRUD operations
        void AddNewExam(Exam e);
        Exam GetExamByID(string tid);
        Exam GetExamByName(string tname);
        void UpdateExam(Exam e);
        void DeleteExam(string tid);
        List<Exam> GetAllExams();
    }
}
