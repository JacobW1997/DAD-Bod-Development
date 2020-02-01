using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBD_Swim_Tracker_0._2.Models.ViewModels
{
    public class AthleteDetailsViewModel
    {
        public AthleteDetailsViewModel(Athlete athlete)
        {
            //Athlete Info
            AthleteName = athlete.NAME;
            AthleteGender = athlete.GENDER;
            AthleteAge = athlete.AGE;

            //Race info
            AthleteRaceTimes = athlete.Results;
        }

        public string AthleteName { get; private set; }
        public string AthleteGender { get; private set; }
        public int AthleteAge { get; private set; }
        public ICollection<Result> AthleteRaceTimes { get; }
    }
}