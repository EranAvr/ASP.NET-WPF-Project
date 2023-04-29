using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI
{
    public class ExamModel
    {
        private string testName;
        private int id;
        private DateTime dateOfCreation;
        private string author;
        private TimeOnly startingTime;
        private int duration;
        private bool randomOrder;

        public ExamModel(string testName, int id, string author,
            TimeOnly startingTime, int duration, bool randomOrder)
        {
            this.dateOfCreation = new DateTime();
        }

        public override string ToString()
        {
            return testName +" "+ dateOfCreation +" "+ id +" "+ author + " " +
                startingTime + " " + duration + " " + randomOrder;
        }
    }

    public record ExamQuestionModel
    {
        public int serialNum;
    }
}
