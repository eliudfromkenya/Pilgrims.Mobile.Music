using System;
using System.Collections.Generic;
using Pilgrims.Music.Models;

namespace Pilgrims.Music.Services.DateService
{
    public interface IDateService
    {
        WeekModel GetWeek(DateTime date);

        List<DayModel> GetDayList(DateTime firstDayInWeek, DateTime lastDayInWeek);
    }
}
