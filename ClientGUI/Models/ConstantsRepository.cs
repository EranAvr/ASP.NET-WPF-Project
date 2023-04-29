using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI.Models
{
    public static class ConstantsRepository
    {
        public static readonly string clientConnectBaseAddress = "https://localhost:7252/";
        public static readonly string serverConnectionString = "mongodb://localhost:27017";
        public static readonly string serverDbName = "C_SHARP_FINAL";

        public static readonly string serverSessionCollName = "sessions";
        public static readonly string usersCollName = "users";
        public static readonly string serverExamsCollName = "exams";
        public static readonly string serverSolvedCollName = "solved_exams";
        public static readonly string serverStudentsGrades = "grades";
        public static readonly string serverQuestionsCollName = "questions";

        public static readonly string userTypeTeacher = "Teacher";
        public static readonly string userTypeStudent = "Student";
    }
}
