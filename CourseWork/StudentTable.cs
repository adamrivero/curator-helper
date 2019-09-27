using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    class StudentTable
    {
        public string ID { get; set; } // поля 
        public string name { get; set; }
        public string course { get; set; }
        public string faculty { get; set; }
        public string spec { get; set; }
        public string live { get; set; }
        public string number { get; set; }
        public string group_ { get; set; } // '_' используется для того что-бы отличать переменную от функции SQL 'GROUP'
        public string allhours { get; set; }
        public string losthours { get; set; }
        public string reason { get; set; }
        public string percent { get; set; }

        public StudentTable(string ID, string name, string course, string faculty, string spec, string live, string number, string group_) // конструктор с параметрами
        {
            this.ID = ID; // присваиваем конструктору значения полей
            this.name = name;
            this.course = course;
            this.faculty = faculty;
            this.spec = spec;
            this.live = live;
            this.number = number;
            this.group_ = group_;
        }
        public StudentTable(string ID, string name, string allhours, string losthours, string reason, string percent, string group_)
        {
            this.ID = ID;
            this.name = name;
            this.allhours = allhours;
            this.losthours = losthours;
            this.reason = reason;
            this.percent = percent;
            this.group_ = group_;
        }
    }
}
