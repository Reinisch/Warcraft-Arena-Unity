using System;
using System.Collections.Generic;
using System.Text;
using ElleRealTimeBaseDAO;
using ElleRealTimeBaseDAO.Interfaces;

namespace ElleRealTime.MySql
{
    public class Creatures : ElleRealTimeDbDAO, ICreatures
    {
        public string GetCreature()
        {
            return ElleRealTimeBaseDAO.Base.Creatures.GetBaseQueryCreatures();
        }
    }
}
